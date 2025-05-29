using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse")]
    public class NotificationsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsModel(ApplicationDbContext context, ILogger<NotificationsModel> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            Notifications = new List<NotificationViewModel>();
        }

        public class NotificationViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Message { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRead { get; set; }
            public string Severity { get; set; } = "info";
            public string ActionUrl { get; set; }

            public string SeverityClass => Severity switch
            {
                "danger" => "danger",
                "warning" => "warning",
                "success" => "success",
                _ => "info"
            };

            public string SeverityText => Severity switch
            {
                "danger" => "Urgent",
                "warning" => "Important",
                "success" => "Success",
                _ => "Information"
            };
        }

        public List<NotificationViewModel> Notifications { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // In a real implementation, you would fetch notifications from the database
                // For now, we'll create sample notifications
                Notifications = new List<NotificationViewModel>
                {
                    new NotificationViewModel
                    {
                        Id = 1,
                        Title = "New Patient Check-in",
                        Message = "Patient John Doe has checked in for their appointment.",
                        CreatedAt = DateTime.Now.AddMinutes(-15),
                        IsRead = false,
                        Severity = "info",
                        ActionUrl = "/Nurse/Queue"
                    },
                    new NotificationViewModel
                    {
                        Id = 2,
                        Title = "Urgent: Medication Needed",
                        Message = "Room 3 patient requires immediate medication.",
                        CreatedAt = DateTime.Now.AddMinutes(-30),
                        IsRead = false,
                        Severity = "danger",
                        ActionUrl = "/Nurse/VitalSigns"
                    },
                    new NotificationViewModel
                    {
                        Id = 3,
                        Title = "Lab Results Ready",
                        Message = "Lab results for patient Sarah Smith are now available.",
                        CreatedAt = DateTime.Now.AddHours(-2),
                        IsRead = true,
                        Severity = "success",
                        ActionUrl = "/Records/Index"
                    },
                    new NotificationViewModel
                    {
                        Id = 4,
                        Title = "Appointment Reminder",
                        Message = "You have 5 appointments scheduled for tomorrow.",
                        CreatedAt = DateTime.Now.AddHours(-5),
                        IsRead = true,
                        Severity = "warning",
                        ActionUrl = "/Nurse/Appointments"
                    }
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostMarkAsReadAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostMarkAllAsReadAsync()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
} 