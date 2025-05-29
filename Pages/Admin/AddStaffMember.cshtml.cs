using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddStaffMemberModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AddStaffMemberModel> _logger;
        private readonly PermissionFixService _permissionFixService;

        public AddStaffMemberModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AddStaffMemberModel> logger,
            PermissionFixService permissionFixService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _permissionFixService = permissionFixService;
        }

        [BindProperty]
        public StaffMember StaffMember { get; set; } = new StaffMember();

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public List<int> SelectedPermissions { get; set; } = new List<int>();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;
        
        public List<string> DaysOfWeek { get; } = new List<string> 
        { 
            "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" 
        };
        
        public List<string> TimeSlots { get; } = new List<string>();

        public Dictionary<string, List<Permission>> PermissionsByCategory { get; set; } = new Dictionary<string, List<Permission>>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Initialize time slots (8:00 AM to 4:00 PM)
                for (int hour = 8; hour <= 16; hour++)
                {
                    string period = hour < 12 ? "AM" : "PM";
                    int displayHour = hour <= 12 ? hour : hour - 12;
                    if (displayHour == 0) displayHour = 12;
                    
                    TimeSlots.Add($"{displayHour}:00 {period}");
                    TimeSlots.Add($"{displayHour}:30 {period}");
                }

                // Ensure all necessary permissions exist in the database
                await _permissionFixService.EnsurePermissionsExistAsync();

                // Load permissions grouped by category
                var permissions = await _context.Permissions
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                PermissionsByCategory = permissions
                    .GroupBy(p => p.Category)
                    .ToDictionary(g => g.Key, g => g.ToList());

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading add staff member page");
                TempData["ErrorMessage"] = "Error loading the page. Please try again later.";
                return RedirectToPage("/Admin/AdminDashboard");
            }
        }

        private async Task CreateDefaultPermissionsAsync()
        {
            var defaultPermissions = new List<Permission>
            {
                // Administration permissions
                new Permission {
                    Name = "Access Admin Dashboard",
                    Description = "Can access the administration dashboard",
                    Category = "Administration"
                },
                new Permission {
                    Name = "Manage Permissions",
                    Description = "Can manage user permissions",
                    Category = "Administration"
                },

                // Appointments permissions
                new Permission {
                    Name = "Manage Appointments",
                    Description = "Can create and manage appointments",
                    Category = "Appointments"
                },
                new Permission {
                    Name = "View Appointments",
                    Description = "Can view appointment details",
                    Category = "Appointments"
                },
                new Permission {
                    Name = "Schedule Appointments",
                    Description = "Can schedule new appointments",
                    Category = "Appointments"
                },

                // Medical Records permissions
                new Permission {
                    Name = "Manage Medical Records",
                    Description = "Can create and edit medical records",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "View Medical Records",
                    Description = "Can view medical records",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Create Medical Records",
                    Description = "Can create new medical records",
                    Category = "Medical Records"
                },

                // Reports permissions
                new Permission {
                    Name = "Generate Reports",
                    Description = "Can generate system reports",
                    Category = "Reports"
                },
                new Permission {
                    Name = "View Reports",
                    Description = "Can view system reports",
                    Category = "Reports"
                },

                // User Management permissions
                new Permission {
                    Name = "Manage Users",
                    Description = "Can manage user accounts",
                    Category = "User Management"
                },
                new Permission {
                    Name = "View Users",
                    Description = "Can view user details",
                    Category = "User Management"
                }
            };

            await _context.Permissions.AddRangeAsync(defaultPermissions);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                _logger.LogInformation("Starting OnPostAsync for staff member creation with email: {Email}", StaffMember.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid");
                    await OnGetAsync();
                    return Page();
                }

                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(StaffMember.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Email already exists: {Email}", StaffMember.Email);
                    ModelState.AddModelError("StaffMember.Email", "This email is already registered.");
                    await OnGetAsync();
                    return Page();
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Create the user account
                    var user = new ApplicationUser
                    {
                        UserName = StaffMember.Email,
                        Email = StaffMember.Email,
                        EmailConfirmed = true,
                        PhoneNumber = StaffMember.ContactNumber,
                        Name = StaffMember.Name,
                        IsActive = StaffMember.IsActive,
                        JoinDate = DateTime.Now,
                        Status = "Verified" // Set as verified since added by admin
                    };

                    _logger.LogInformation("Creating user account for: {Email}", StaffMember.Email);
                    var result = await _userManager.CreateAsync(user, Password);
            
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Failed to create user account. Errors: {Errors}", 
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        await OnGetAsync();
                        return Page();
                    }

                    // Create or get the role
                    if (!await _roleManager.RoleExistsAsync(StaffMember.Role))
                    {
                        _logger.LogInformation("Creating new role: {Role}", StaffMember.Role);
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(StaffMember.Role));
                        if (!roleResult.Succeeded)
                        {
                            _logger.LogError("Failed to create role. Errors: {Errors}", 
                                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                            throw new Exception($"Failed to create role: {StaffMember.Role}");
                        }
                    }

                    // Assign role to user
                    _logger.LogInformation("Assigning role {Role} to user {Email}", StaffMember.Role, StaffMember.Email);
                    var roleAssignResult = await _userManager.AddToRoleAsync(user, StaffMember.Role);
                    if (!roleAssignResult.Succeeded)
                    {
                        _logger.LogError("Failed to assign role. Errors: {Errors}", 
                            string.Join(", ", roleAssignResult.Errors.Select(e => e.Description)));
                        throw new Exception($"Failed to assign role to user");
                    }

                    // Save staff member details
                    StaffMember.UserId = user.Id;
                    StaffMember.CreatedAt = DateTime.Now;
                    StaffMember.IsActive = true;

                    _logger.LogInformation("Saving staff member details to database");
                    await _context.StaffMembers.AddAsync(StaffMember);
                    await _context.SaveChangesAsync();

                    // Save staff permissions
                    if (SelectedPermissions != null && SelectedPermissions.Any())
                    {
                        _logger.LogInformation("Saving {Count} permissions for staff member", SelectedPermissions.Count);
                        
                        // Get the permission details to create claims
                        var selectedPermissionDetails = await _context.Permissions
                            .Where(p => SelectedPermissions.Contains(p.Id))
                            .ToListAsync();

                        // Create StaffPermissions entries
                        var staffPermissions = SelectedPermissions.Select(permissionId => new StaffPermission
                        {
                            StaffMemberId = StaffMember.Id,
                            PermissionId = permissionId,
                            GrantedAt = DateTime.UtcNow
                        }).ToList();

                        await _context.StaffPermissions.AddRangeAsync(staffPermissions);

                        // Add permission claims
                        var claims = selectedPermissionDetails.Select(p => 
                            new System.Security.Claims.Claim("Permission", p.Name)
                        ).ToList();

                        // Add role claim
                        claims.Add(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, StaffMember.Role));

                        var claimsResult = await _userManager.AddClaimsAsync(user, claims);
                        if (!claimsResult.Succeeded)
                        {
                            _logger.LogError("Failed to add permission claims. Errors: {Errors}", 
                                string.Join(", ", claimsResult.Errors.Select(e => e.Description)));
                            throw new Exception("Failed to add permission claims");
                        }

                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Successfully saved {Count} permissions and claims", SelectedPermissions.Count);
                    }
                    else
                    {
                        _logger.LogWarning("No permissions selected for staff member");
                    }

                    await transaction.CommitAsync();

                    _logger.LogInformation("Successfully created staff member with ID {StaffMemberId}", StaffMember.Id);
                    TempData["SuccessMessage"] = "Staff member created successfully.";
                    return RedirectToPage("/Admin/AdminDashboard");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during staff member creation transaction");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff member");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the staff member. Please try again.");
                await OnGetAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostGrantEssentialPermissionsAsync()
        {
            try
            {
                var role = StaffMember.Role;
                if (string.IsNullOrEmpty(role))
                {
                    return new JsonResult(new { success = false, message = "Please select a role first." });
                }

                var essentialPermissions = await GetEssentialPermissionsForRoleAsync(role);
                return new JsonResult(new { success = true, permissions = essentialPermissions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting essential permissions");
                return new JsonResult(new { success = false, message = "Error getting essential permissions." });
            }
        }

        private async Task<List<int>> GetEssentialPermissionsForRoleAsync(string role)
        {
            var permissions = await _context.Permissions.ToListAsync();
            var essentialPermissions = new List<int>();

            switch (role.ToLower())
            {
                case "admin":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] { 
                            "Access Admin Dashboard",
                            "Manage Permissions",
                            "Manage Users",
                            "Generate Reports",
                            "View Reports",
                            "Manage Medical Records",
                            "Manage Appointments"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "doctor":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Doctor Dashboard",
                            "Manage Medical Records",
                            "Create Medical Records",
                            "View Patient History",
                            "Create Prescriptions",
                            "View Prescriptions",
                            "Manage Appointments",
                            "View Reports"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "nurse":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Nurse Dashboard",
                            "Create Medical Records",
                            "View Patient History",
                            "Record Vital Signs",
                            "View Appointments",
                            "Create Appointments"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                default:
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Dashboard",
                            "View Appointments"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;
            }

            return essentialPermissions;
        }
    }
}