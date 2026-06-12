using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace FitManager.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpOptions _options;

        public SmtpEmailService(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_options.Host, _options.Port)
            {
                EnableSsl = _options.UseSsl,
                Credentials = new NetworkCredential(_options.Username, _options.Password)
            };

            var mail = new MailMessage(_options.From, to, subject, body)
            {
                IsBodyHtml = true
            };

            return client.SendMailAsync(mail);
        }
    }
}
