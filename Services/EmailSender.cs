using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Barangay.Services
{
    // Custom email sender interface
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    // Custom email sender implementation
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;

        public EmailSender(string smtpHost, int smtpPort, string smtpUser, string smtpPassword)
        {
            _smtpHost = smtpHost ?? "smtp.gmail.com";
            _smtpPort = smtpPort > 0 ? smtpPort : 587;
            _smtpUser = smtpUser;
            _smtpPassword = smtpPassword;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Check if email configuration is valid
                if (string.IsNullOrEmpty(_smtpUser) || string.IsNullOrEmpty(_smtpPassword))
                {
                    Console.WriteLine("Email configuration is missing. Email not sent.");
                    return;
                }

                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(_smtpUser, _smtpPassword),
                    EnableSsl = true
                };
                
                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mail.To.Add(email);

                await client.SendMailAsync(mail);
                Console.WriteLine($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent application crashes
                Console.WriteLine($"Error sending email to {email}: {ex.Message}");
            }
        }
    }
} 