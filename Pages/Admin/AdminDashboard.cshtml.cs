using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class AdminDashboardModel : PageModel
    {
        private readonly ILogger<AdminDashboardModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardModel(
            ILogger<AdminDashboardModel> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public int TotalDoctors { get; set; }
        public int TotalNurses { get; set; }
        public int TotalAppointments { get; set; }
        public int ActiveStaffCount { get; set; }
        public List<StaffOverviewModel> RecentStaff { get; set; }
        public DateTime CurrentPhilippineTime { get; set; }

        private DateTime GetPhilippineTime()
        {
            // Convert current UTC time to Philippine Standard Time (UTC+8)
            var utcNow = DateTime.UtcNow;
            TimeZoneInfo philippineZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, philippineZone);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Set current Philippine time
                CurrentPhilippineTime = GetPhilippineTime();

                // Get total counts
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                var nurses = await _userManager.GetUsersInRoleAsync("Nurse");
                
                TotalDoctors = doctors.Count;
                TotalNurses = nurses.Count;
                TotalAppointments = await _context.Appointments.CountAsync();
                
                // Calculate active staff count
                ActiveStaffCount = doctors.Count(d => d.IsActive) + nurses.Count(n => n.IsActive);

                // Get all staff members (doctors, nurses, admins)
                var staffRoles = new[] { "Doctor", "Nurse", "Admin", "Staff", "System Administrator" };
                RecentStaff = new List<StaffOverviewModel>();
                
                // Get users with staff roles
                foreach (var role in staffRoles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    foreach (var user in usersInRole)
                    {
                        // Skip if already added (user might have multiple roles)
                        if (RecentStaff.Any(s => s.Id == user.Id))
                            continue;
                            
                        RecentStaff.Add(new StaffOverviewModel
                        {
                            Id = user.Id,
                            Name = $"{user.FirstName} {user.LastName}",
                            Role = role,
                            Department = "General", // Since Department is not in ApplicationUser model
                            IsActive = user.IsActive,
                            LastActive = user.LastActive
                        });
                    }
                }
                
                // Sort by last active time
                RecentStaff = RecentStaff
                    .OrderByDescending(s => s.LastActive)
                    .Take(10)
                    .ToList();

                _logger.LogInformation($"Admin dashboard loaded successfully by user {User.Identity?.Name}");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin dashboard");
                TempData["ErrorMessage"] = "Error loading dashboard data. Please try again later.";
                return Page();
            }
        }
    }

    public class StaffOverviewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastActive { get; set; }
    }
}
        
