using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Barangay.Services
{
    public class AppointmentReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AppointmentReminderBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(30); // Check every 30 minutes

        public AppointmentReminderBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<AppointmentReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Appointment Reminder Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var reminderService = scope.ServiceProvider.GetRequiredService<IAppointmentReminderService>();
                        await reminderService.ProcessScheduledRemindersAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing appointment reminders");
                }

                await Task.Delay(_period, stoppingToken);
            }

            _logger.LogInformation("Appointment Reminder Background Service stopped");
        }
    }
}
