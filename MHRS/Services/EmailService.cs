using System.Net;
using System.Net.Mail;

namespace MHRS.Services
{
    public class EmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(
                        "bunyamin.cuni46@gmail.com",
                        "jcez aagv rils vsyt"
                    );

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("bunyamin.cuni46@gmail.com", "EHRS - Veteriner Randevu Sistemi"),
                        Subject = "Email Doğrulama Kodu",
                        Body = $@"
                            <html>
                            <body style='font-family: Arial, sans-serif;'>
                                <div style='max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                                    <h2 style='color: #333;'>Email Doğrulama</h2>
                                    <p>Merhaba,</p>
                                    <p>EHRS - Veteriner Randevu Sistemi'ne hoş geldiniz!</p>
                                    <p>Hesabınızı doğrulamak için aşağıdaki kodu kullanınız:</p>
                                    <div style='background: #f0f0f0; padding: 15px; text-align: center; margin: 20px 0; border-radius: 5px;'>
                                        <h1 style='color: #667eea; margin: 0;'>{otp}</h1>
                                    </div>
                                    <p style='color: #999; font-size: 12px;'>Bu kod 15 dakika geçerliliğine sahiptir.</p>
                                    <p style='color: #999; font-size: 12px;'>Eğer bu işlemi siz yapmadıysanız lütfen bu emaili göz ardı edin.</p>
                                    <hr style='border: none; border-top: 1px solid #ddd; margin: 20px 0;'>
                                    <p style='color: #999; font-size: 12px; text-align: center;'>© 2025 EHRS Veteriner Randevu Sistemi</p>
                                </div>
                            </body>
                            </html>
                        ",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation($"OTP emaili başarıyla gönderildi: {email}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email gönderme hatası: {email}");
                return false;
            }
        }
    }
}