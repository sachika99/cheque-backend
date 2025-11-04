using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace MotorStores.Infrastructure.Services
{
    public class EmailResult
    {
        public bool status { get; set; }
        public int otp { get; set; }
        public string email { get; set; }
        public string username { get; set; }
    }

    public class EmailService
    {
        private readonly IConfiguration _cfg;

        public EmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task<EmailResult> SendEmailAsync(string to, string subject, string body, int otp, string confirmedUserName)
        {
            var result = new EmailResult();

            try
            {
                var from = _cfg["Email:From"] ?? "no-reply@motorstores.com";
                var host = _cfg["Email:SmtpHost"] ?? "smtp.yourserver.com";
                var port = int.Parse(_cfg["Email:SmtpPort"] ?? "587");
                var user = _cfg["Email:User"];
                var pass = _cfg["Email:Password"];

                using var mail = new MailMessage(from, to, subject, body)
                {
                    IsBodyHtml = true
                };

                using var smtp = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(user, pass),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mail);

                result.status = true;
                result.otp = otp;
                result.email = to;
                result.username = confirmedUserName;
            }
            
            catch (Exception ex)
            {
                result.status = false;
                result.otp = otp;
                 result.email = to;
                result.username = confirmedUserName;
            }

            return result;
        }
    }
}
