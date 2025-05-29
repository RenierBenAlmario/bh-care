using System.Threading.Tasks;

namespace Barangay.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Implement email sending logic here
            await Task.CompletedTask;
        }
    }
} 