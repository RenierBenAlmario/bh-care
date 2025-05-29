using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DashboardModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public int TotalStaffMembers { get; set; }
        public Dictionary<string, int> StaffRoleDistribution { get; set; } = new();
        public Dictionary<DateTime, int> MonthlyActivityData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get staff roles
                var staffRoles = new[] { "Doctor", "Nurse", "Admin" };
                var roleIds = new List<string>();

                foreach (var roleName in staffRoles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        roleIds.Add(role.Id);
                    }
                }

                // Calculate total staff members
                TotalStaffMembers = await _context.UserRoles
                    .Where(ur => roleIds.Contains(ur.RoleId))
                    .Select(ur => ur.UserId)
                    .Distinct()
                    .CountAsync();

                // Calculate role distribution
                foreach (var roleName in staffRoles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var count = await _context.UserRoles
                            .CountAsync(ur => ur.RoleId == role.Id);
                        StaffRoleDistribution[roleName] = count;
                    }
                }

                // Calculate monthly activity data
                var sixMonthsAgo = DateTime.Now.AddMonths(-6);
                var monthlyData = await _userManager.Users
                    .Where(u => u.CreatedAt >= sixMonthsAgo)
                    .GroupBy(u => new DateTime(u.CreatedAt.Year, u.CreatedAt.Month, 1))
                    .Select(g => new { Month = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Month, x => x.Count);

                // Ensure we have entries for all months
                var now = DateTime.Now;
                for (int i = 5; i >= 0; i--)
                {
                    var month = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                    if (!monthlyData.ContainsKey(month))
                    {
                        monthlyData[month] = 0;
                    }
                }

                MonthlyActivityData = monthlyData;

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error and return to error page
                return RedirectToPage("/Error", new { message = "Error loading dashboard data: " + ex.Message });
            }
        }
    }
} 