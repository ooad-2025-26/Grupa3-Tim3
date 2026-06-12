using System.Threading.Tasks;

namespace FitManager.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
