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
using System.Text.RegularExpressions;

namespace Barangay.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class AddStaffMemberModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AddStaffMemberModel> _logger;
        private readonly IDataEncryptionService _encryptionService;

        public AddStaffMemberModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AddStaffMemberModel> logger,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _encryptionService = encryptionService;
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

        public Dictionary<string, List<Permission>> CategorizedPermissions { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                // Initialize default values
                StaffMember = new StaffMember
                {
                    IsActive = true,
                    JoinDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                
                // Generate time slots for dropdown (30-minute intervals)
                for (int hour = 6; hour < 22; hour++)
                {
                    string period = hour < 12 ? "AM" : "PM";
                    int displayHour = hour <= 12 ? hour : hour - 12;
                    if (displayHour == 0) displayHour = 12;
                    
                    TimeSlots.Add($"{displayHour}:00 {period}");
                    TimeSlots.Add($"{displayHour}:30 {period}");
                }

                // Ensure essential simplified permissions exist (align with StaffPermissions page)
                await EnsureEssentialPermissionsAsync();

                // Load and categorize permissions
                var permissions = await _context.Permissions
                    .OrderBy(p => p.Category)
                    .ThenBy(p => p.Name)
                    .ToListAsync();

                if (!permissions.Any())
                {
                    await CreateDefaultPermissionsAsync();
                    permissions = await _context.Permissions
                        .OrderBy(p => p.Category)
                        .ThenBy(p => p.Name)
                        .ToListAsync();
                }

                // Group permissions by category (ensure Dashboard Access stays visible for all)
                var grouped = permissions
                    .GroupBy(p => string.IsNullOrEmpty(p.Category) ? "General" : p.Category)
                    .ToDictionary(g => g.Key, g => g.ToList());

                // Page categories - show actual pages available
                var standardCategories = new List<string>
                {
                    "Doctor Pages",
                    "Nurse Pages"
                };

                var ordered = new Dictionary<string, List<Permission>>();
                foreach (var cat in standardCategories)
                {
                    if (grouped.ContainsKey(cat))
                    {
                        ordered[cat] = grouped[cat];
                    }
                }
                foreach (var kv in grouped)
                {
                    if (!ordered.ContainsKey(kv.Key))
                    {
                        ordered[kv.Key] = kv.Value;
                    }
                }
                // Build simplified role categories by permission NAME so it works regardless of DB categories
                var byName = permissions
                    .GroupBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

                var nurseNames = new [] { "Appointments", "NurseDashboard", "PatientList", "PatientQueue", "VitalSigns" };
                var nurseList = new List<Permission>();
                foreach (var n in nurseNames)
                {
                    if (byName.TryGetValue(n, out var perm)) nurseList.Add(perm);
                }
                if (nurseList.Count > 0)
                {
                    ordered["Nurse"] = nurseList;
                }

                var doctorNames = new [] { "Consultation", "DoctorDashboard", "Reports" };
                var doctorList = new List<Permission>();
                foreach (var d in doctorNames)
                {
                    if (byName.TryGetValue(d, out var perm)) doctorList.Add(perm);
                }
                if (doctorList.Count > 0)
                {
                    ordered["Doctor"] = doctorList;
                }

                CategorizedPermissions = ordered;

                // Filter permissions based on selected role/position
                var role = (StaffMember.Role ?? string.Empty).Trim();
                var position = (StaffMember.Position ?? string.Empty).Trim();
                
                // Use position if available, otherwise fall back to role
                var selectedRole = !string.IsNullOrEmpty(position) ? position : role;
                
                if (!string.IsNullOrEmpty(selectedRole))
                {
                    var roleToCategories = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
                    {
                        // Doctor positions
                        ["Doctor"] = new HashSet<string>(new [] { "Doctor", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        ["Head Doctor"] = new HashSet<string>(new [] { "Doctor", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        
                        // Nurse positions  
                        ["Nurse"] = new HashSet<string>(new [] { "Nurse", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        ["Head Nurse"] = new HashSet<string>(new [] { "Nurse", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        
                        // Admin positions
                        ["Admin Staff"] = new HashSet<string>(new [] { "User Management", "Reports", "Reporting", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        ["Admin"] = new HashSet<string>(new [] { "User Management", "Reports", "Reporting", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        
                        // Other positions - show minimal permissions
                        ["Receptionist"] = new HashSet<string>(new [] { "Appointments", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase),
                        ["IT"] = new HashSet<string>(new [] { "User Management", "Dashboard Access" }, StringComparer.OrdinalIgnoreCase)
                    };

                    if (roleToCategories.TryGetValue(selectedRole, out var allowed))
                    {
                        var keep = new Dictionary<string, List<Permission>>(StringComparer.OrdinalIgnoreCase);
                        foreach (var kv in CategorizedPermissions)
                        {
                            if (allowed.Contains(kv.Key) || kv.Key.Equals("Dashboard Access", StringComparison.OrdinalIgnoreCase))
                                keep[kv.Key] = kv.Value;
                        }
                        CategorizedPermissions = keep;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync");
                ErrorMessage = "An error occurred while loading the page. Please try again.";
            }
        }

        // Mirror essential permission seeding used by StaffPermissions to guarantee simplified categories
        private async Task EnsureEssentialPermissionsAsync()
        {
            var mustHave = new List<Permission>
            {
                // Doctor Pages - Show actual pages available to doctors
                new Permission { Name = "DoctorDashboard", Description = "Access to Doctor Dashboard page", Category = "Doctor Pages" },
                new Permission { Name = "Consultation", Description = "Access to Consultation page", Category = "Doctor Pages" },
                new Permission { Name = "PatientRecords", Description = "Access to Patient Records page", Category = "Doctor Pages" },
                new Permission { Name = "PatientList", Description = "Access to Patient List page", Category = "Doctor Pages" },
                new Permission { Name = "Reports", Description = "Access to Reports page", Category = "Doctor Pages" },

                // Nurse Pages - Show actual pages available to nurses
                new Permission { Name = "NurseDashboard", Description = "Access to Nurse Dashboard page", Category = "Nurse Pages" },
                new Permission { Name = "PatientList", Description = "Access to Patient List page", Category = "Nurse Pages" },
                new Permission { Name = "Appointments", Description = "Access to Appointments page", Category = "Nurse Pages" },
                new Permission { Name = "VitalSigns", Description = "Access to Vital Signs page", Category = "Nurse Pages" },
                new Permission { Name = "PatientQueue", Description = "Access to Patient Queue page", Category = "Nurse Pages" }
            };

            var existingNames = await _context.Permissions.Select(p => p.Name).ToListAsync();
            var toInsert = mustHave.Where(p => !existingNames.Contains(p.Name)).ToList();
            if (toInsert.Count > 0)
            {
                await _context.Permissions.AddRangeAsync(toInsert);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateDefaultPermissionsAsync()
        {
            var defaultPermissions = new List<Permission>
            {
                // Dashboard permission (consolidated)
                new Permission {
                    Name = "Access Dashboard",
                    Description = "Can access all system dashboards",
                    Category = "Dashboard Access"
                },
                new Permission {
                    Name = "Manage Permissions",
                    Description = "Can manage user permissions",
                    Category = "Administration"
                },

                // Appointments permissions
                new Permission {
                    Name = "Create Appointments",
                    Description = "Can create new appointments",
                    Category = "Appointments"
                },
                new Permission {
                    Name = "View Appointments",
                    Description = "Can view appointment details",
                    Category = "Appointments"
                },

                // Doctor permissions
                new Permission {
                    Name = "Write Prescriptions",
                    Description = "Can write and update prescriptions",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Manage Consultations",
                    Description = "Can create and manage patient consultations",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "View Patient Details",
                    Description = "Can view detailed patient information",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Print Medical Records",
                    Description = "Can print patient medical records",
                    Category = "Medical Records"
                },

                // Nurse permissions
                new Permission {
                    Name = "Record Vital Signs",
                    Description = "Can record patient vital signs",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Manage Patient Queue",
                    Description = "Can manage the patient queue",
                    Category = "Patient Management"
                },
                new Permission {
                    Name = "View Patient History",
                    Description = "Can view patient medical history",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Create Medical Records",
                    Description = "Can create new medical records",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Edit Medical Records",
                    Description = "Can edit existing medical records",
                    Category = "Medical Records"
                },
                new Permission {
                    Name = "Manage Diagnoses",
                    Description = "Can create and manage diagnoses",
                    Category = "Medical Records"
                },

                // VitalSigns permissions (simplified: only Access Vital Signs)
                new Permission {
                    Name = "Access Vital Signs",
                    Description = "Can access the vital signs page",
                    Category = "Vital Signs"
                },
                new Permission {
                    Name = "Delete Vital Signs Data",
                    Description = "Can delete patient vital signs records",
                    Category = "Vital Signs"
                },

                // Prescriptions permissions
                new Permission {
                    Name = "Create Prescriptions",
                    Description = "Can create new prescriptions",
                    Category = "Prescriptions"
                },
                new Permission {
                    Name = "View Prescriptions",
                    Description = "Can view patient prescriptions",
                    Category = "Prescriptions"
                },
                new Permission {
                    Name = "Edit Prescriptions",
                    Description = "Can edit existing prescriptions",
                    Category = "Prescriptions"
                },
                new Permission {
                    Name = "Delete Prescriptions",
                    Description = "Can delete prescriptions",
                    Category = "Prescriptions"
                },

                // Records permissions
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
                    Name = "Delete Medical Records",
                    Description = "Can delete medical records",
                    Category = "Medical Records"
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
                },
                new Permission {
                    Name = "Approve Users",
                    Description = "Can approve user registrations",
                    Category = "User Management"
                },
                new Permission {
                    Name = "Delete Users",
                    Description = "Can delete users from the system",
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

                // Validate working days and hours
                if (string.IsNullOrEmpty(StaffMember.WorkingDays) || string.IsNullOrEmpty(StaffMember.WorkingHours))
                {
                    _logger.LogWarning("Working days or hours not provided");
                    ModelState.AddModelError(string.Empty, "Please select working days and hours.");
                    await OnGetAsync();
                    return Page();
                }

                // Additional server-side validation for name and phone format
                var name = (StaffMember.Name ?? string.Empty).Trim();
                var phone = (StaffMember.ContactNumber ?? string.Empty).Trim();

                // Allow letters (including diacritics), spaces, hyphen, apostrophe. No digits/symbols.
                var nameAllowedPattern = new Regex("^[A-Za-zÀ-ÖØ-öø-ÿ'\\-\\s]+$");
                if (!nameAllowedPattern.IsMatch(name))
                {
                    ModelState.AddModelError("StaffMember.Name", "Full name may only contain letters, spaces, hyphen (-), and apostrophe (').");
                }
                // Disallow 3 or more repeated identical letters
                if (Regex.IsMatch(name, "([A-Za-z])\\1{2,}"))
                {
                    ModelState.AddModelError("StaffMember.Name", "Full name cannot contain 3 or more repeated letters in a row.");
                }

                // Strict PH mobile format: +639XXXXXXXXX
                if (!Regex.IsMatch(phone, "^\\+639\\d{9}$"))
                {
                    ModelState.AddModelError("StaffMember.ContactNumber", "Contact number must be in the format +639XXXXXXXXX.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid: {Errors}", 
                        string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
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

                // Normalize and enforce permission selection when a role/position is chosen
                // If no permissions were selected, auto-grant essential permissions for the chosen role/position
                // Also ensure "Access Dashboard" is included
                var role = StaffMember.Role ?? string.Empty;
                var position = StaffMember.Position ?? string.Empty;
                
                // Use position if available, otherwise fall back to role
                var selectedRole = !string.IsNullOrEmpty(position) ? position : role;
                
                if (!string.IsNullOrWhiteSpace(selectedRole))
                {
                    // Normalize role to canonical casing to match [Authorize(Roles=...)] attributes
                    StaffMember.Role = NormalizeRoleName(selectedRole);

                    SelectedPermissions ??= new List<int>();

                    if (!SelectedPermissions.Any())
                    {
                        var essential = await GetEssentialPermissionsForRoleAsync(selectedRole);
                        if (essential != null && essential.Any())
                        {
                            SelectedPermissions = essential.Distinct().ToList();
                            _logger.LogInformation(
                                "No permissions selected; auto-granted {Count} essential permissions for role/position {Role}.",
                                SelectedPermissions.Count, selectedRole);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Please select at least one permission or use 'Grant Essential Permissions'.");
                            await OnGetAsync();
                            return Page();
                        }
                    }

                    // Ensure Access Dashboard is always granted if it exists in the DB
                    var accessDashboardId = await _context.Permissions
                        .Where(p => p.Name == "Access Dashboard")
                        .Select(p => p.Id)
                        .FirstOrDefaultAsync();
                    if (accessDashboardId != 0 && !SelectedPermissions.Contains(accessDashboardId))
                    {
                        SelectedPermissions.Add(accessDashboardId);
                        _logger.LogInformation("Added missing 'Access Dashboard' permission for role/position {Role}", selectedRole);
                    }

                    // Deduplicate
                    SelectedPermissions = SelectedPermissions.Distinct().ToList();
                }

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // Create the user account
                    var user = new ApplicationUser
                    {
                        UserName = StaffMember.Email,
                        Email = _encryptionService.Encrypt(StaffMember.Email), // Encrypt email
                        EmailConfirmed = true,
                        PhoneNumber = _encryptionService.Encrypt(StaffMember.ContactNumber), // Encrypt phone number
                        // Name fields will be populated below via FullName setter
                        IsActive = StaffMember.IsActive,
                        JoinDate = DateTime.Now,
                        Status = "Verified" // Set as verified since added by admin
                    };

                    // Populate FirstName, MiddleName, LastName, and Name from the provided full name
                    // Using the validated and trimmed 'name' variable
                    user.FullName = name;

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
                    
                    // Set default department value if not provided
                    if (string.IsNullOrEmpty(StaffMember.Department))
                    {
                        StaffMember.Department = "General";
                    }

                    _logger.LogInformation("Saving staff member details to database");
                    await _context.StaffMembers.AddAsync(StaffMember);
                    await _context.SaveChangesAsync();

                    // Save staff permissions
                    if (SelectedPermissions != null && SelectedPermissions.Any())
                    {
                        _logger.LogInformation("Saving {Count} permissions for staff member", SelectedPermissions.Count);
                        
                        // First, get the permission details to create claims
                        var selectedPermissionDetails = await _context.Permissions
                            .Where(p => SelectedPermissions.Contains(p.Id))
                            .ToListAsync();

                        // Create UserPermissions entries
                        var userPermissions = SelectedPermissions.Select(permissionId => new UserPermission
                        {
                            UserId = user.Id,
                            PermissionId = permissionId
                        }).ToList();

                        await _context.UserPermissions.AddRangeAsync(userPermissions);
                        
                        // Also create StaffPermissions entries
                        var staffPermissions = SelectedPermissions.Select(permissionId => new StaffPermission
                        {
                            StaffMemberId = StaffMember.Id,
                            PermissionId = permissionId,
                            GrantedAt = DateTime.UtcNow
                        }).ToList();
                        
                        await _context.StaffPermissions.AddRangeAsync(staffPermissions);

                        // Add permission claims
                        var claims = selectedPermissionDetails.Select(p => 
                            new System.Security.Claims.Claim("Permission", $"{p.Category}:{p.Name}")
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
                var role = NormalizeRoleName(StaffMember.Role);
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
                case "admin staff":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] { 
                            "Access Dashboard",
                            "Manage Permissions",
                            "Manage Users",
                            "Manage Medical Records",
                            "View Medical Records",
                            "Approve Users",
                            "Delete Users",
                            "Create Appointments",
                            "View Appointments"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "doctor":
                case "head doctor":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "DoctorDashboard", "Consultation", "PatientRecords", "PatientList", "Reports"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "nurse":
                case "head nurse":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "NurseDashboard", "PatientList", "Appointments", "VitalSigns", "PatientQueue"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "receptionist":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Dashboard",
                            "Appointments"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                case "it":
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Dashboard",
                            "Manage Users"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;

                default:
                    essentialPermissions.AddRange(permissions
                        .Where(p => new[] {
                            "Access Dashboard"
                        }.Contains(p.Name))
                        .Select(p => p.Id));
                    break;
            }

            return essentialPermissions;
        }

        private static string NormalizeRoleName(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return role;
            var r = role.Trim();
            if (string.Equals(r, "nurse", StringComparison.OrdinalIgnoreCase)) return "Nurse";
            if (string.Equals(r, "doctor", StringComparison.OrdinalIgnoreCase)) return "Doctor";
            if (string.Equals(r, "admin", StringComparison.OrdinalIgnoreCase)) return "Admin";
            if (string.Equals(r, "admin staff", StringComparison.OrdinalIgnoreCase) || string.Equals(r, "adminstaff", StringComparison.OrdinalIgnoreCase) || string.Equals(r, "staff", StringComparison.OrdinalIgnoreCase)) return "Admin Staff";
            // Fallback: capitalize first letter
            return char.ToUpper(r[0]) + r.Substring(1);
        }
    }
}