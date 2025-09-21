using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Barangay.Pages.Admin
{
    [Authorize(Policy = "AccessAdminDashboard")]
    public class GrantEssentialPermissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GrantEssentialPermissionsModel> _logger;

        public GrantEssentialPermissionsModel(
            ApplicationDbContext context,
            ILogger<GrantEssentialPermissionsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public bool IsError { get; set; }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        public string StaffName { get; set; }
        public string StaffRole { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                IsError = true;
                StatusMessage = "No user specified.";
                return RedirectToPage("./ManagePermissions");
            }

            // Get staff member name and role
            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(s => s.UserId == userId);
            
            if (staffMember == null)
            {
                IsError = true;
                StatusMessage = "User not found.";
                return RedirectToPage("./ManagePermissions");
            }

            StaffName = staffMember.Name;
            StaffRole = staffMember.Role;
            UserId = userId;

            try
            {
                _logger.LogInformation("Granting essential permissions for {Role} user {UserId} ({Name})", 
                    StaffRole, UserId, StaffName);

                // Define role-specific essential permission names
                var essentialPermissionNames = new List<string>();
                
                // Common permissions for all staff - consolidated dashboard access
                essentialPermissionNames.Add("Access Dashboard");
                
                // Role-specific permissions
                switch (StaffRole?.ToLower())
                {
                    case "admin":
                        essentialPermissionNames.AddRange(new[] {
                            "Manage Users",
                            "View Reports",
                            "Approve Users",
                            "Manage Appointments",
                            "Manage Medical Records",
                            "Manage Permissions"
                        });
                        break;
                        
                    case "doctor":
                        essentialPermissionNames.AddRange(new[] {
                            "Manage Appointments",
                            "Manage Medical Records",
                            "View Patient History",
                            "Create Prescriptions",
                            "View Reports"
                        });
                        break;
                        
                    case "nurse":
                        essentialPermissionNames.AddRange(new[] {
                            "ManageAppointments",
                            "Record Vital Signs",
                            "View Patient History",
                            "Manage Medical Records"
                        });
                        break;
                        
                    case "admin staff":
                        essentialPermissionNames.AddRange(new[] {
                            "Manage Appointments",
                            "View Reports"
                        });
                        break;
                        
                    default:
                        // Basic permissions for unknown roles
                        essentialPermissionNames.AddRange(new[] {
                            "View Dashboard"
                        });
                        break;
                }
                
                _logger.LogInformation("Essential permissions for {Role}: {Permissions}", 
                    StaffRole, string.Join(", ", essentialPermissionNames));

                // Get essential permissions from the database
                var essentialPermissions = await _context.Permissions
                    .Where(p => essentialPermissionNames.Contains(p.Name))
                    .ToListAsync();
                
                if (essentialPermissions.Count < essentialPermissionNames.Count)
                {
                    _logger.LogWarning("Some essential permissions were not found in the database. " +
                        "Found {FoundCount} out of {RequestedCount}", 
                        essentialPermissions.Count, essentialPermissionNames.Count);
                    
                    // Log which permissions were not found
                    var foundPermissionNames = essentialPermissions.Select(p => p.Name).ToList();
                    var missingPermissionNames = essentialPermissionNames
                        .Where(name => !foundPermissionNames.Contains(name))
                        .ToList();
                    
                    _logger.LogWarning("Missing permissions: {MissingPermissions}", 
                        string.Join(", ", missingPermissionNames));
                }

                // Get user's existing permissions
                var existingPermissionIds = await _context.UserPermissions
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PermissionId)
                    .ToListAsync();

                // Add essential permissions that user doesn't already have
                var newPermissions = new List<UserPermission>();
                foreach (var permission in essentialPermissions)
                {
                    if (!existingPermissionIds.Contains(permission.Id))
                    {
                        newPermissions.Add(new UserPermission
                        {
                            UserId = userId,
                            PermissionId = permission.Id
                        });
                    }
                }

                if (newPermissions.Any())
                {
                    await _context.UserPermissions.AddRangeAsync(newPermissions);
                    await _context.SaveChangesAsync();
                    
                    var addedPermissionNames = essentialPermissions
                        .Where(p => newPermissions.Select(np => np.PermissionId).Contains(p.Id))
                        .Select(p => p.Name)
                        .ToList();
                    
                    _logger.LogInformation("Added {Count} essential permissions for user {UserId}: {Permissions}", 
                        newPermissions.Count, userId, string.Join(", ", addedPermissionNames));
                    
                    StatusMessage = $"Essential permissions have been granted to {StaffName}.";
                }
                else
                {
                    StatusMessage = $"{StaffName} already has all essential permissions.";
                    _logger.LogInformation("User {UserId} already has all essential permissions", userId);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting essential permissions to user {UserId}", userId);
                IsError = true;
                StatusMessage = "An error occurred while granting permissions: " + ex.Message;
                return Page();
            }
        }
    }
} 