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
    [Authorize(Roles = "Admin")]
    public class StaffListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<StaffListModel> _logger;

        public StaffListModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StaffListModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public class StaffMemberViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Department { get; set; }
            public string Position { get; set; }
            public bool IsActive { get; set; }
            public DateTime JoinDate { get; set; }
            public Dictionary<string, List<string>> Permissions { get; set; } = new();
        }

        public List<StaffMemberViewModel> StaffMembers { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Fetching staff members list");

                // Get all staff members with their permissions
                var staffMembers = await _context.StaffMembers
                    .Include(s => s.User)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                foreach (var staff in staffMembers)
                {
                    var viewModel = new StaffMemberViewModel
                    {
                        Id = staff.Id,
                        Name = staff.Name,
                        Email = staff.Email,
                        Role = staff.Role,
                        Department = staff.Department ?? "N/A",
                        Position = staff.Position ?? "N/A",
                        IsActive = staff.IsActive,
                        JoinDate = staff.JoinDate
                    };

                    // Get user's permissions
                    if (staff.User != null)
                    {
                        // Get claims and user permissions
                        var claims = await _userManager.GetClaimsAsync(staff.User);
                        var userPermissions = await _context.UserPermissions
                            .Include(up => up.Permission)
                            .Where(up => up.UserId == staff.UserId)
                            .ToListAsync();

                        // Group permissions by category
                        var permissionsByCategory = new Dictionary<string, List<string>>();

                        // Add permissions from claims
                        foreach (var claim in claims.Where(c => c.Type == "Permission"))
                        {
                            var parts = claim.Value.Split(':');
                            if (parts.Length == 2)
                            {
                                var category = parts[0];
                                var permission = parts[1];

                                if (!permissionsByCategory.ContainsKey(category))
                                {
                                    permissionsByCategory[category] = new List<string>();
                                }
                                permissionsByCategory[category].Add(permission);
                            }
                        }

                        // Add permissions from UserPermissions table
                        foreach (var up in userPermissions)
                        {
                            var category = up.Permission.Category;
                            if (!permissionsByCategory.ContainsKey(category))
                            {
                                permissionsByCategory[category] = new List<string>();
                            }
                            if (!permissionsByCategory[category].Contains(up.Permission.Name))
                            {
                                permissionsByCategory[category].Add(up.Permission.Name);
                            }
                        }

                        viewModel.Permissions = permissionsByCategory;
                    }

                    StaffMembers.Add(viewModel);
                }

                _logger.LogInformation("Successfully fetched {Count} staff members", StaffMembers.Count);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching staff members list");
                TempData["ErrorMessage"] = "An error occurred while loading the staff list.";
                return RedirectToPage("/Admin/AdminDashboard");
            }
        }
    }
} 