using System.Collections.Generic;
using System.Threading.Tasks;
using Barangay.Models;
using Barangay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class AdminPageModel : PageModel
    {
        protected readonly INotificationService _notificationService;

        public AdminPageModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task LoadNotificationsAsync()
        {
            // Get unread notifications
            var notifications = await _notificationService.GetUnreadNotificationsAsync();
            ViewData["Notifications"] = notifications;
            
            // Get unread notification count
            var notificationCount = await _notificationService.GetUnreadNotificationCountAsync();
            ViewData["NotificationCount"] = notificationCount;
        }
    }
} 