using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MotorStores.Infrastructure.Services
{
    public class EmailResult
    {
        public bool? status { get; set; }
        public int? otp { get; set; }
        public string? email { get; set; } = string.Empty;
        public string? username { get; set; } = string.Empty;
    }

    public class EmailService
    {
        private readonly IConfiguration _cfg;

        public EmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task<EmailResult> SendEmailAsync(
            string to,
            string subject,
            string body,
            int otp,
            string confirmedUserName)
        {
            var result = new EmailResult
            {
                otp = otp,
                email = to ?? string.Empty,
                username = confirmedUserName ?? string.Empty
            };

            // ✅ HARD GUARD — prevents Railway crash
            if (string.IsNullOrWhiteSpace(to))
            {
                result.status = false;
                return result;
            }

            try
            {
                var from = _cfg["Email:From"] ?? "no-reply@motorstores.com";
                var host = _cfg["Email:SmtpHost"] ?? "smtp.yourserver.com";
                var port = int.TryParse(_cfg["Email:SmtpPort"], out var p) ? p : 587;
                var user = _cfg["Email:User"];
                var pass = _cfg["Email:Password"];

                using var mail = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(to);

                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mail);

                result.status = true;
            }
            catch
            {
                // ❌ DO NOT rethrow
                // ❌ DO NOT call ex.ToString()
                // ✅ Just return failed result safely
                result.status = false;
            }

            return result;
        }
    }
}
