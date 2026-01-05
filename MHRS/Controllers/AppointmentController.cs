using MHRS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MHRS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentDbContext _context;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(AppointmentDbContext context, ILogger<AppointmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Tüm şehirleri getirir
        /// </summary>
        [HttpGet("cities")]
        public async Task<ActionResult<IEnumerable<City>>> GetAllCities()
        {
            try
            {
                var cities = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return Ok(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şehirler getirilirken hata oluştu");
                return StatusCode(500, "Şehirler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen şehirdeki hastaneleri getirir
        /// </summary>
        [HttpGet("hospitals/{cityId}")]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitalsByCity(int cityId)
        {
            try
            {
                var hospitals = await _context.Hospitals
                    .Where(h => h.CityId == cityId)
                    .OrderBy(h => h.HospitalName)
                    .ToListAsync();

                if (!hospitals.Any())
                {
                    return NotFound(new { message = "Bu şehirde hastane bulunamadı" });
                }

                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastaneler getirilirken hata oluştu: {CityId}", cityId);
                return StatusCode(500, "Hastaneler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen şehirdeki ilçeleri getirir
        /// </summary>
        [HttpGet("districts/{cityId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetDistrictsByCity(int cityId)
        {
            try
            {
                var districts = await _context.Hospitals
                    .Where(h => h.CityId == cityId && !string.IsNullOrEmpty(h.DistrictName))
                    .Select(h => h.DistrictName)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToListAsync();

                if (!districts.Any())
                {
                    return NotFound(new { message = "Bu şehirde ilçe bulunamadı" });
                }

                return Ok(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İlçeler getirilirken hata oluştu: {CityId}", cityId);
                return StatusCode(500, "İlçeler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen şehir ve ilçedeki hastaneleri getirir
        /// </summary>
        [HttpGet("hospitals/{cityId}/{districtName}")]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetHospitalsByCityAndDistrict(int cityId, string districtName)
        {
            try
            {
                var hospitals = await _context.Hospitals
                    .Where(h => h.CityId == cityId && h.DistrictName == districtName)
                    .OrderBy(h => h.HospitalName)
                    .ToListAsync();

                if (!hospitals.Any())
                {
                    return NotFound(new { message = "Bu ilçede hastane bulunamadı" });
                }

                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastaneler getirilirken hata oluştu: CityId={CityId}, District={District}", cityId, districtName);
                return StatusCode(500, "Hastaneler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen hastanedeki doktorları getirir
        /// </summary>
        [HttpGet("doctors/{hospitalId}")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctorsByHospital(int hospitalId)
        {
            try
            {
                var doctors = await _context.Doctors
                    .Where(d => d.HospitalId == hospitalId)
                    .OrderBy(d => d.DoctorName)
                    .ToListAsync();

                if (!doctors.Any())
                {
                    return NotFound(new { message = "Bu hastanede doktor bulunamadı" });
                }

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doktorlar getirilirken hata oluştu: {HospitalId}", hospitalId);
                return StatusCode(500, "Doktorlar getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen doktor, tarih için müsait saatleri getirir
        /// </summary>
        [HttpGet("available-times/{doctorId}/{date}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableTimes(int doctorId, string date)
        {
            try
            {
                if (!DateTime.TryParse(date, out DateTime selectedDate))
                {
                    return BadRequest(new { message = "Geçersiz tarih formatı" });
                }

                selectedDate = selectedDate.Date;

                var existingAppointments = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId &&
                                a.AppointmentDate.Date == selectedDate &&
                                a.IsDone == false)
                    .Select(a => a.AppointmentDate)
                    .ToListAsync();

                _logger.LogInformation($"📅 {selectedDate.ToShortDateString()} tarihinde Doktor {doctorId} için {existingAppointments.Count} randevu bulundu");

                // Randevuları saat:dakika formatına çevir (HH:mm)
                var bookedTimeSlots = existingAppointments
                    .Select(a => $"{a.Hour:D2}:{a.Minute:D2}")
                    .ToHashSet();

                _logger.LogInformation($"🔴 Dolu saatler: {string.Join(", ", bookedTimeSlots)}");

                var allTimeSlots = new List<string>();
                for (int hour = 9; hour < 17; hour++)
                {
                    allTimeSlots.Add($"{hour:D2}:00");
                    allTimeSlots.Add($"{hour:D2}:30");
                }

                var availableTimeSlots = allTimeSlots
                    .Where(timeSlot => !bookedTimeSlots.Contains(timeSlot))
                    .ToList();

                _logger.LogInformation($"✅ Müsait saatler: {string.Join(", ", availableTimeSlots)}");

                return Ok(availableTimeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müsait saatler getirilirken hata oluştu");
                return StatusCode(500, "Müsait saatler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Kullanıcının evcil hayvanlarını getirir
        /// </summary>
        [HttpGet("pets/user/{userId}")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByUser(int userId)
        {
            try
            {
                var pets = await _context.Pets
                    .Where(p => p.UserId == userId)
                    .OrderBy(p => p.PetName)
                    .ToListAsync();

                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Evcil hayvanlar getirilirken hata oluştu");
                return StatusCode(500, "Evcil hayvanlar getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Yeni evcil hayvan ekler
        /// </summary>
        [HttpPost("pet")]
        public async Task<IActionResult> CreatePet([FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // User kontrolü
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
                if (user == null)
                {
                    return NotFound(new { message = "Kullanıcı bulunamadı" });
                }

                var pet = new Pet
                {
                    PetName = request.PetName,
                    PetType = request.PetType,
                    Age = request.Age,
                    Gender = request.Gender,
                    Breed = request.Breed,
                    Notes = request.Notes,
                    UserId = request.UserId,
                    CreatedAt = DateTime.Now
                };

                _context.Pets.Add(pet);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Yeni evcil hayvan eklendi: {pet.PetName} (ID: {pet.PetId})");

                return CreatedAtAction(
                    nameof(GetPetsByUser),
                    new { userId = pet.UserId },
                    new
                    {
                        message = "Evcil hayvan başarıyla eklendi",
                        petId = pet.PetId,
                        petName = pet.PetName
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Evcil hayvan ekleme hatası");
                return StatusCode(500, new { message = "Evcil hayvan eklenirken bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Evcil hayvanı günceller
        /// </summary>
        [HttpPut("pet/{petId}")]
        public async Task<IActionResult> UpdatePet(int petId, [FromBody] CreatePetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId);

                if (pet == null)
                {
                    return NotFound(new { message = "Evcil hayvan bulunamadı" });
                }

                pet.PetName = request.PetName;
                pet.PetType = request.PetType;
                pet.Age = request.Age;
                pet.Gender = request.Gender;
                pet.Breed = request.Breed;
                pet.Notes = request.Notes;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Evcil hayvan güncellendi: {pet.PetName} (ID: {petId})");

                return Ok(new { message = "Evcil hayvan başarıyla güncellendi", petId = pet.PetId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Evcil hayvan güncelleme hatası");
                return StatusCode(500, new { message = "Evcil hayvan güncellenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Evcil hayvanı siler
        /// </summary>
        [HttpDelete("pet/{petId}")]
        public async Task<IActionResult> DeletePet(int petId)
        {
            try
            {
                var pet = await _context.Pets.FirstOrDefaultAsync(p => p.PetId == petId);

                if (pet == null)
                {
                    return NotFound(new { message = "Evcil hayvan bulunamadı" });
                }

                // Bu hayvana ait aktif randevu var mı kontrol et
                var hasActiveAppointments = await _context.Appointments
                    .AnyAsync(a => a.PetId == petId && a.IsDone == false);

                if (hasActiveAppointments)
                {
                    return BadRequest(new { message = "Bu hayvana ait aktif randevular olduğu için silinemez" });
                }

                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Evcil hayvan silindi: {pet.PetName} (ID: {petId})");

                return Ok(new { message = "Evcil hayvan başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Evcil hayvan silme hatası");
                return StatusCode(500, new { message = "Evcil hayvan silinirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Kullanıcının tamamlanmış randevuları
        /// DEĞİŞTİ: ownerPhone -> userId, User tablosu ile JOIN
        /// </summary>
        [HttpGet("done/user/{userId}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetDoneAppointments(int userId)
        {
            try
            {
                var now = DateTime.Now;

                var appointments = await (from appointment in _context.Appointments
                                          where appointment.IsDone == true || (appointment.IsDone == false && appointment.AppointmentDate < now)
                                          join pet in _context.Pets
                                          on appointment.PetId equals pet.PetId
                                          where pet.UserId == userId
                                          join user in _context.Users
                                          on pet.UserId equals user.UserId
                                          join doctor in _context.Doctors
                                          on appointment.DoctorId equals doctor.DoctorId into doctorGroup
                                          from doctor in doctorGroup.DefaultIfEmpty()
                                          orderby appointment.AppointmentDate descending
                                          select new AppointmentResponseDto
                                          {
                                              PetName = pet.PetName,
                                              UserPhone = user.Phone,
                                              AppointmentId = appointment.AppointmentId,
                                              AppointmentDate = appointment.AppointmentDate,
                                              IsDone = appointment.IsDone,
                                              DoctorName = doctor != null ? doctor.DoctorName : null
                                          }).ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tamamlanmış randevular getirilirken hata oluştu");
                return StatusCode(500, "Randevular getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Kullanıcının bekleyen randevuları
        /// DEĞİŞTİ: ownerPhone -> userId, User tablosu ile JOIN
        /// </summary>
        [HttpGet("pending/user/{userId}")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetPendingAppointments(int userId)
        {
            try
            {
                var now = DateTime.Now;

                var appointments = await (from appointment in _context.Appointments
                                          where appointment.IsDone == false && appointment.AppointmentDate >= now
                                          join pet in _context.Pets
                                          on appointment.PetId equals pet.PetId
                                          where pet.UserId == userId
                                          join user in _context.Users
                                          on pet.UserId equals user.UserId
                                          join doctor in _context.Doctors
                                          on appointment.DoctorId equals doctor.DoctorId into doctorGroup
                                          from doctor in doctorGroup.DefaultIfEmpty()
                                          orderby appointment.AppointmentDate
                                          select new AppointmentResponseDto
                                          {
                                              PetName = pet.PetName,
                                              UserPhone = user.Phone,
                                              AppointmentId = appointment.AppointmentId,
                                              AppointmentDate = appointment.AppointmentDate,
                                              IsDone = appointment.IsDone,
                                              DoctorName = doctor != null ? doctor.DoctorName : null
                                          }).ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bekleyen randevular getirilirken hata oluştu");
                return StatusCode(500, "Randevular getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Tüm randevuları getirir (ADMIN İÇİN)
        /// DEĞİŞTİ: User tablosu ile JOIN
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments()
        {
            try
            {
                var appointments = await (from appointment in _context.Appointments
                                          join pet in _context.Pets
                                          on appointment.PetId equals pet.PetId
                                          join user in _context.Users
                                          on pet.UserId equals user.UserId
                                          join doctor in _context.Doctors
                                          on appointment.DoctorId equals doctor.DoctorId into doctorGroup
                                          from doctor in doctorGroup.DefaultIfEmpty()
                                          orderby appointment.AppointmentDate descending
                                          select new AppointmentResponseDto
                                          {
                                              PetName = pet.PetName,
                                              UserPhone = user.Phone,
                                              AppointmentId = appointment.AppointmentId,
                                              AppointmentDate = appointment.AppointmentDate,
                                              IsDone = appointment.IsDone,
                                              DoctorName = doctor != null ? doctor.DoctorName : null
                                          }).ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm randevular getirilirken hata oluştu");
                return StatusCode(500, "Randevular getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Randevu durumunu tamamlandı olarak işaretler
        /// </summary>
        [HttpPatch("{appointmentId}/complete")]
        public async Task<IActionResult> CompleteAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(m => m.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return NotFound(new { message = "Randevu bulunamadı" });
                }

                if (appointment.IsDone)
                {
                    return BadRequest(new { message = "Randevu zaten tamamlanmış" });
                }

                appointment.IsDone = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Randevu durumu başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu durumu güncellenirken hata oluştu: {AppointmentId}", appointmentId);
                return StatusCode(500, "Randevu güncellenirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Yeni randevu oluşturur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] NewAppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Pet kontrolü
                var pet = await _context.Pets
                    .FirstOrDefaultAsync(p => p.PetId == request.PetId);

                if (pet == null)
                {
                    return NotFound(new { message = "Evcil hayvan bulunamadı" });
                }

                // Hastane kontrolü
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == request.HospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                // Şehir kontrolü
                if (hospital.CityId != request.CityId)
                {
                    return BadRequest(new { message = "Seçilen hastane seçilen şehirde değil" });
                }

                // Doktor kontrolü
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.DoctorId == request.DoctorId && d.HospitalId == request.HospitalId);

                if (doctor == null)
                {
                    return NotFound(new { message = "Doktor bulunamadı veya bu hastaneye ait değil" });
                }

                // Aynı doktor ve saatte başka randevu var mı kontrol et
                var existingAppointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.DoctorId == request.DoctorId &&
                                            a.AppointmentDate == request.AppointmentDate &&
                                            a.IsDone == false);

                if (existingAppointment != null)
                {
                    return BadRequest(new { message = "Bu doktorun seçilen saatte başka randevusu var. Lütfen farklı bir saat seçin." });
                }

                // Tarihi Unspecified olarak işaretle (timezone dönüşümü olmasın)
                var appointmentDateTime = DateTime.SpecifyKind(request.AppointmentDate, DateTimeKind.Unspecified);

                _logger.LogInformation($"🕐 Randevu kaydediliyor: {appointmentDateTime:yyyy-MM-dd HH:mm:ss}");

                // Yeni randevu oluştur
                var appointment = new Appointment
                {
                    PetId = request.PetId,
                    HospitalId = request.HospitalId,
                    DoctorId = request.DoctorId,
                    AppointmentDate = appointmentDateTime,
                    IsDone = false
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"✅ Randevu kaydedildi: ID={appointment.AppointmentId}, Tarih={appointment.AppointmentDate:yyyy-MM-dd HH:mm:ss}");

                return CreatedAtAction(
                    nameof(GetAllAppointments),
                    new { id = appointment.AppointmentId },
                    new
                    {
                        message = "Randevu başarıyla oluşturuldu",
                        appointmentId = appointment.AppointmentId,
                        petId = request.PetId,
                        hospitalId = request.HospitalId,
                        doctorId = request.DoctorId
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu oluşturulurken hata oluştu");
                return StatusCode(500, "Randevu oluşturulurken bir hata oluştu");
            }
        }

        /// <summary>
        /// Randevuyu siler
        /// </summary>
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            try
            {
                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

                if (appointment == null)
                {
                    return NotFound(new { message = "Randevu bulunamadı" });
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Randevu başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu silinirken hata oluştu: {AppointmentId}", appointmentId);
                return StatusCode(500, "Randevu silinirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Yeni hastane oluşturur
        /// </summary>
        [HttpPost("hospital")]
        public async Task<IActionResult> CreateHospital([FromBody] CreateHospitalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var city = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == request.CityId);

                if (city == null)
                {
                    return NotFound(new { message = "Şehir bulunamadı" });
                }

                var existingHospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalName.ToLower() == request.HospitalName.ToLower());

                if (existingHospital != null)
                {
                    return BadRequest(new { message = "Bu adlı hastane zaten mevcut" });
                }

                var hospital = new Hospital
                {
                    HospitalName = request.HospitalName,
                    CityId = request.CityId,
                    Phone = request.Phone,
                    Address = request.Address,
                    Description = request.Description,
                    DistrictName = request.DistrictName
                };

                _context.Hospitals.Add(hospital);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Yeni hastane eklendi: {hospital.HospitalName} (ID: {hospital.HospitalId})");

                return CreatedAtAction(
                    nameof(GetHospitalsByCity),
                    new { cityId = hospital.CityId },
                    new
                    {
                        message = "Hastane başarıyla eklendi",
                        hospitalId = hospital.HospitalId,
                        hospitalName = hospital.HospitalName
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastane ekleme hatası");
                return StatusCode(500, new { message = "Hastane eklenirken bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Hastaneyi günceller
        /// </summary>
        [HttpPut("hospital/{hospitalId}")]
        public async Task<IActionResult> UpdateHospital(int hospitalId, [FromBody] CreateHospitalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == hospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                var city = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == request.CityId);

                if (city == null)
                {
                    return NotFound(new { message = "Şehir bulunamadı" });
                }

                hospital.HospitalName = request.HospitalName;
                hospital.CityId = request.CityId;
                hospital.Phone = request.Phone;
                hospital.Address = request.Address;
                hospital.Description = request.Description;
                hospital.DistrictName = request.DistrictName;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Hastane güncellendi: {hospital.HospitalName} (ID: {hospitalId})");

                return Ok(new { message = "Hastane başarıyla güncellendi", hospitalId = hospital.HospitalId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastane güncelleme hatası");
                return StatusCode(500, new { message = "Hastane güncellenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Hastaneyi siler
        /// </summary>
        [HttpDelete("hospital/{hospitalId}")]
        public async Task<IActionResult> DeleteHospital(int hospitalId)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == hospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                var hasAppointments = await _context.Appointments
                    .AnyAsync(a => a.HospitalId == hospitalId);

                if (hasAppointments)
                {
                    return BadRequest(new { message = "Bu hastaneye ait randevular olduğu için silinemez" });
                }

                _context.Hospitals.Remove(hospital);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Hastane silindi: {hospital.HospitalName} (ID: {hospitalId})");

                return Ok(new { message = "Hastane başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastane silme hatası");
                return StatusCode(500, new { message = "Hastane silinirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Yeni doktor oluşturur
        /// </summary>
        [HttpPost("doctor")]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == request.HospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                var doctor = new Doctor
                {
                    DoctorName = request.DoctorName,
                    Phone = request.Phone,
                    HospitalId = request.HospitalId
                };

                _context.Doctors.Add(doctor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Yeni doktor eklendi: {doctor.DoctorName} (ID: {doctor.DoctorId})");

                return CreatedAtAction(
                    nameof(GetDoctorsByHospital),
                    new { hospitalId = doctor.HospitalId },
                    new
                    {
                        message = "Doktor başarıyla eklendi",
                        doctorId = doctor.DoctorId,
                        doctorName = doctor.DoctorName
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doktor ekleme hatası");
                return StatusCode(500, new { message = "Doktor eklenirken bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Doktoru günceller
        /// </summary>
        [HttpPut("doctor/{doctorId}")]
        public async Task<IActionResult> UpdateDoctor(int doctorId, [FromBody] CreateDoctorRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { message = "Doktor bulunamadı" });
                }

                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == request.HospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                doctor.DoctorName = request.DoctorName;
                doctor.Phone = request.Phone;
                doctor.HospitalId = request.HospitalId;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Doktor güncellendi: {doctor.DoctorName} (ID: {doctorId})");

                return Ok(new { message = "Doktor başarıyla güncellendi", doctorId = doctor.DoctorId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doktor güncelleme hatası");
                return StatusCode(500, new { message = "Doktor güncellenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Doktoru siler
        /// </summary>
        [HttpDelete("doctor/{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            try
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { message = "Doktor bulunamadı" });
                }

                var hasActiveAppointments = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == doctorId && a.IsDone == false);

                if (hasActiveAppointments)
                {
                    return BadRequest(new { message = "Bu doktora ait aktif randevular olduğu için silinemez" });
                }

                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Doktor silindi: {doctor.DoctorName} (ID: {doctorId})");

                return Ok(new { message = "Doktor başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doktor silme hatası");
                return StatusCode(500, new { message = "Doktor silinirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Tüm doktorları getirir (ADMIN İÇİN)
        /// </summary>
        [HttpGet("doctors")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllDoctors()
        {
            try
            {
                var doctors = await (from doctor in _context.Doctors
                                     join hospital in _context.Hospitals
                                     on doctor.HospitalId equals hospital.HospitalId
                                     join city in _context.Cities
                                     on hospital.CityId equals city.CityId
                                     orderby doctor.DoctorName
                                     select new
                                     {
                                         DoctorId = doctor.DoctorId,
                                         DoctorName = doctor.DoctorName,
                                         Phone = doctor.Phone,
                                         HospitalId = doctor.HospitalId,
                                         HospitalName = hospital.HospitalName,
                                         CityName = city.CityName
                                     }).ToListAsync();

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm doktorlar getirilirken hata oluştu");
                return StatusCode(500, "Doktorlar getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Tüm evcil hayvanları getirir (ADMIN İÇİN)
        /// </summary>
        [HttpGet("pets")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetAllPets()
        {
            try
            {
                var pets = await _context.Pets
                    .OrderBy(p => p.PetName)
                    .ToListAsync();

                return Ok(pets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm evcil hayvanlar getirilirken hata oluştu");
                return StatusCode(500, "Evcil hayvanlar getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Belirtilen şehirdeki nöbetçi hastaneleri getirir
        /// </summary>
        [HttpGet("on-duty-hospitals/{cityId}")]
        public async Task<ActionResult<IEnumerable<Hospital>>> GetOnDutyHospitals(int cityId)
        {
            try
            {
                var hospitals = await _context.Hospitals
                    .Where(h => h.CityId == cityId && h.IsOnDuty == true)
                    .OrderBy(h => h.HospitalName)
                    .ToListAsync();

                return Ok(hospitals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nöbetçi hastaneler getirilirken hata oluştu: {CityId}", cityId);
                return StatusCode(500, "Nöbetçi hastaneler getirilirken bir hata oluştu");
            }
        }

        /// <summary>
        /// Hastanenin nöbet durumunu değiştirir (ADMIN İÇİN)
        /// </summary>
        [HttpPatch("hospital/{hospitalId}/toggle-duty")]
        public async Task<IActionResult> ToggleHospitalDuty(int hospitalId)
        {
            try
            {
                var hospital = await _context.Hospitals
                    .FirstOrDefaultAsync(h => h.HospitalId == hospitalId);

                if (hospital == null)
                {
                    return NotFound(new { message = "Hastane bulunamadı" });
                }

                hospital.IsOnDuty = !hospital.IsOnDuty;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Hastane nöbet durumu değişti: {hospital.HospitalName} - Nöbetçi: {hospital.IsOnDuty}");

                return Ok(new
                {
                    message = hospital.IsOnDuty ? "Hastane nöbete açıldı" : "Hastane nöbetten çıkarıldı",
                    isOnDuty = hospital.IsOnDuty
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hastane nöbet durumu güncellenirken hata oluştu");
                return StatusCode(500, new { message = "Nöbet durumu güncellenirken bir hata oluştu" });
            }
        }

        // ============================================
        // YENİ EKLENEN ENDPOINT - KONUM DEĞİŞTİRME
        // ============================================

        /// <summary>
        /// Kullanıcının şehrini günceller (Konum değiştir özelliği için)
        /// </summary>
        [HttpPut("update-city/{userId}")]
        public async Task<IActionResult> UpdateUserCity(int userId, [FromBody] UpdateCityRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Geçersiz veri" });
                }

                var user = await _context.Users
                    .Include(u => u.City)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                var city = await _context.Cities.FindAsync(request.CityId);
                if (city == null)
                {
                    return NotFound(new { success = false, message = "Şehir bulunamadı" });
                }

                user.CityId = request.CityId;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Kullanıcı şehri güncellendi: UserId={userId}, NewCity={city.CityName}");

                return Ok(new
                {
                    success = true,
                    message = "Konum başarıyla güncellendi",
                    cityId = city.CityId,
                    cityName = city.CityName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şehir güncelleme hatası");
                return StatusCode(500, new { success = false, message = "Konum güncellenirken bir hata oluştu" });
            }
        }
    }

    public class CreateHospitalRequest
    {
        [Required(ErrorMessage = "Hastane adı zorunludur")]
        [MaxLength(150)]
        public string HospitalName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir zorunludur")]
        public int CityId { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur")]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres zorunludur")]
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string DistrictName { get; set; } = string.Empty;
    }

    // YENİ: Şehir güncelleme request modeli
    public class UpdateCityRequest
    {
        [Required(ErrorMessage = "Şehir ID'si zorunludur")]
        public int CityId { get; set; }
    }
}