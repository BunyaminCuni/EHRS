using MHRS.Model;
using MHRS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace MHRS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppointmentDbContext _context;
        private readonly EmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        // Memory'de geçici OTP depolaması (üretim ortamında database kullan)
        private static Dictionary<string, (string otp, DateTime expiry)> _otpStore = new();

        public AuthController(AppointmentDbContext context, EmailService emailService, ILogger<AuthController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Kayıt için OTP gönder
        /// </summary>
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new { message = "Email adresi zorunludur" });
                }

                // 6 haneli OTP oluştur
                string otp = new Random().Next(100000, 999999).ToString();

                // OTP'yi memory'de depola (15 dakika geçerli)
                _otpStore[request.Email] = (otp, DateTime.UtcNow.AddMinutes(15));

                // Email gönder
                bool emailSent = await _emailService.SendOtpEmailAsync(request.Email, otp);

                if (!emailSent)
                {
                    return StatusCode(500, new { message = "Email gönderilemedi" });
                }

                _logger.LogInformation($"OTP gönderildi: {request.Email}");

                return Ok(new { message = "OTP emailinize gönderilmiştir. Lütfen kontrol ediniz." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTP gönderme hatası");
                return StatusCode(500, new { message = "Bir hata oluştu" });
            }
        }

        /// <summary>
        /// OTP doğrula ve kullanıcı kaydet
        /// </summary>
        [HttpPost("verify-otp")]
        public IActionResult VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Otp))
                {
                    return BadRequest(new { message = "Email ve OTP gereklidir" });
                }

                // OTP'yi kontrol et
                if (!_otpStore.ContainsKey(request.Email))
                {
                    return BadRequest(new { message = "OTP bulunamadı. Lütfen yeniden gönderin." });
                }

                var (storedOtp, expiry) = _otpStore[request.Email];

                // Geçerliliği kontrol et
                if (DateTime.UtcNow > expiry)
                {
                    _otpStore.Remove(request.Email);
                    return BadRequest(new { message = "OTP süresi dolmuştur. Lütfen yeniden gönderin." });
                }

                // OTP doğru mu kontrol et
                if (storedOtp != request.Otp)
                {
                    return BadRequest(new { message = "OTP hatalıdır" });
                }

                // OTP'yi sil
                _otpStore.Remove(request.Email);

                // Kullanıcıyı localStorage'a kaydet (frontend'de yapılır)
                _logger.LogInformation($"OTP doğrulandi: {request.Email}");

                return Ok(new
                {
                    message = "Email başarıyla doğrulandı!",
                    verified = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OTP doğrulama hatası");
                return StatusCode(500, new { message = "Bir hata oluştu" });
            }
        }

        /// <summary>
        /// OTP'yi test etmek için (geliştirme ortamında)
        /// </summary>
        [HttpGet("debug-otp/{email}")]
        public IActionResult GetDebugOtp(string email)
        {
            if (_otpStore.ContainsKey(email))
            {
                var (otp, _) = _otpStore[email];
                return Ok(new { email, otp, message = "⚠️ Sadece test için! Üretimde kaldır!" });
            }

            return NotFound(new { message = "OTP bulunamadı" });
        }

        // ============================================
        // YENİ EKLENEN ENDPOINT'LER
        // ============================================

        /// <summary>
        /// Yeni kullanıcı kaydı (veritabanına kaydet)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Telefon numarası kontrolü (unique)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Phone == request.Phone);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "Bu telefon numarası ile kayıtlı kullanıcı bulunmaktadır" });
                }

                // Email kontrolü (unique)
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingEmail != null)
                {
                    return BadRequest(new { message = "Bu email adresi ile kayıtlı kullanıcı bulunmaktadır" });
                }

                // Şifreyi hashle (SHA-256)
                string passwordHash = HashPassword(request.Password);

                // Yeni kullanıcı oluştur
                var user = new User
                {
                    UserName = request.UserName,
                    Phone = request.Phone,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    City = request.City ?? string.Empty,
                    Address = request.Address ?? string.Empty,
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Yeni kullanıcı kaydedildi: {user.UserName} (ID: {user.UserId})");

                return Ok(new
                {
                    message = "Kullanıcı başarıyla kaydedildi",
                    userId = user.UserId,
                    userName = user.UserName,
                    phone = user.Phone,
                    email = user.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı kayıt hatası");
                return StatusCode(500, new { message = "Kullanıcı kaydedilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcı girişi (telefon + şifre)
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Kullanıcıyı telefon numarasına göre bul
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Phone == request.Phone);

                if (user == null)
                {
                    return BadRequest(new { message = "Telefon numarası veya şifre hatalı" });
                }

                // Şifreyi hashle ve kontrol et
                string passwordHash = HashPassword(request.Password);

                if (user.PasswordHash != passwordHash)
                {
                    return BadRequest(new { message = "Telefon numarası veya şifre hatalı" });
                }

                _logger.LogInformation($"Kullanıcı giriş yaptı: {user.UserName} (ID: {user.UserId})");

                // Kullanıcı bilgilerini döndür (şifre hariç)
                return Ok(new UserResponseDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Phone = user.Phone,
                    Email = user.Email,
                    City = user.City,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı giriş hatası");
                return StatusCode(500, new { message = "Giriş yapılırken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcı bilgilerini getir (userId ile)
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }

                return Ok(new UserResponseDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Phone = user.Phone,
                    Email = user.Email,
                    City = user.City,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı bilgileri alınırken hata oluştu");
                return StatusCode(500, new { message = "Kullanıcı bilgileri alınırken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Telefon numarasına göre kullanıcı bilgisi getir
        /// </summary>
        [HttpGet("user/phone/{phone}")]
        public async Task<IActionResult> GetUserByPhone(string phone)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);

                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }

                return Ok(new UserResponseDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Phone = user.Phone,
                    Email = user.Email,
                    City = user.City,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı bilgileri alınırken hata oluştu");
                return StatusCode(500, new { message = "Kullanıcı bilgileri alınırken bir hata oluştu" });
            }
        }

        // ============================================
        // ADMIN - TÜM KULLANICILARI LİSTELE
        // ============================================
        /// <summary>
        /// Tüm kullanıcıları listele (Admin için)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        Phone = u.Phone,
                        Email = u.Email,
                        City = u.City,
                        CreatedAt = u.CreatedAt
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcılar listelen irken hata oluştu");
                return StatusCode(500, new { message = "Kullanıcılar listelenirken bir hata oluştu" });
            }
        }

        // ============================================
        // ADMIN - KULLANICI SİL
        // ============================================
        /// <summary>
        /// Kullanıcıyı sil (Admin için)
        /// </summary>
        [HttpDelete("user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }

                // Kullanıcının evcil hayvanlarını kontrol et
                var hasPets = await _context.Pets.AnyAsync(p => p.UserId == userId);

                if (hasPets)
                {
                    return BadRequest(new { message = "Bu kullanıcıya ait evcil hayvanlar olduğu için silinemez. Önce evcil hayvanları silin." });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kullanıcı silindi: {user.UserName} (ID: {userId})");

                return Ok(new { message = "Kullanıcı başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcı silinirken hata oluştu");
                return StatusCode(500, new { message = "Kullanıcı silinirken bir hata oluştu" });
            }
        }

        // ============================================
        // YARDIMCI FONKSİYON - ŞİFRE HASHLEME
        // ============================================
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }

    public class SendOtpRequest
    {
        public string Email { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}