using Microsoft.AspNetCore.Mvc;
using Barangay.Models;
using Barangay.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Barangay.ViewComponents
{
    public class DynamicNavMenuViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DynamicNavMenuViewComponent> _logger;

        public DynamicNavMenuViewComponent(
            IPermissionService permissionService, 
            UserManager<ApplicationUser> userManager,
            ILogger<DynamicNavMenuViewComponent> logger)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _logger = logger;
        }

        public class NavMenuItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
            public string Icon { get; set; }
            public bool IsActive { get; set; }
            public List<string> RequiredPermissions { get; set; } = new List<string>();
        }

        public async Task<IViewComponentResult> InvokeAsync(string role)
        {
            var userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(new List<NavMenuItem>());
            }

            // Get current path to determine active menu item
            var currentPath = HttpContext.Request.Path.ToString().ToLower();
            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            
            // Debug permissions
            _logger.LogInformation($"User {userId} has {permissions.Count()} permissions: {string.Join(", ", permissions)}");
            
            // Define all possible menu items with their required permissions
            var allMenuItems = new List<NavMenuItem>();
            
            switch (role.ToLower())
            {
                case "nurse":
                    allMenuItems = new List<NavMenuItem>
                    {
                        new NavMenuItem {
                            Title = "Dashboard",
                            Url = "/Nurse/NurseDashboard",
                            Icon = "tachometer-alt",
                            IsActive = currentPath.Contains("/nurse/nursedashboard"),
                            RequiredPermissions = new List<string> { "Access Dashboard" }
                        },
                        new NavMenuItem {
                            Title = "Appointments",
                            Url = "/Nurse/Appointments",
                            Icon = "calendar-alt",
                            IsActive = currentPath.Contains("/nurse/appointments"),
                            RequiredPermissions = new List<string> { "Create Appointments", "View Appointments" }
                        },
                        new NavMenuItem {
                            Title = "Patient Queue",
                            Url = "/Nurse/PatientQueue",
                            Icon = "users",
                            IsActive = currentPath.Contains("/nurse/patientqueue"),
                            RequiredPermissions = new List<string> { "View Appointments", "View Patient Details" }
                        },
                        new NavMenuItem {
                            Title = "Vital Signs",
                            Url = "/Nurse/VitalSigns",
                            Icon = "heartbeat",
                            IsActive = currentPath.Contains("/nurse/vitalsigns"),
                            RequiredPermissions = new List<string> { "Access Vital Signs" }
                        },
                        new NavMenuItem {
                            Title = "Patient List",
                            Url = "/Nurse/PatientList",
                            Icon = "book-medical",
                            IsActive = currentPath.Contains("/nurse/patientlist"),
                            RequiredPermissions = new List<string> { "View Patient Details" }
                        }
                    };
                    break;
                
                case "doctor":
                    allMenuItems = new List<NavMenuItem>
                    {
                        new NavMenuItem {
                            Title = "Dashboard",
                            Url = "/Doctor/DoctorDashboard",
                            Icon = "tachometer-alt",
                            IsActive = currentPath.Contains("/doctor/doctordashboard"),
                            RequiredPermissions = new List<string> { "Access Dashboard" }
                        },
                        new NavMenuItem {
                            Title = "Consultation",
                            Url = "/Doctor/Consultation",
                            Icon = "stethoscope",
                            IsActive = currentPath.Contains("/doctor/consultation"),
                            RequiredPermissions = new List<string> { "Manage Consultations" }
                        },
                        new NavMenuItem {
                            Title = "Prescriptions",
                            Url = "/Doctor/Prescriptions",
                            Icon = "prescription",
                            IsActive = currentPath.Contains("/doctor/prescriptions"),
                            RequiredPermissions = new List<string> { "Create Prescriptions", "View Prescriptions", "Edit Prescriptions" }
                        },
                        new NavMenuItem {
                            Title = "Patient Results",
                            Url = "/Doctor/PatientList",
                            Icon = "clipboard-list",
                            IsActive = currentPath.Contains("/doctor/patientlist"),
                            RequiredPermissions = new List<string> { "View Patient Details", "View Medical Records" }
                        }
                    };
                    break;
            }

            // Filter menu items based on user permissions
            var userMenuItems = new List<NavMenuItem>();
            foreach (var menuItem in allMenuItems)
            {
                // Check if user has any of the required permissions
                var hasPermission = menuItem.RequiredPermissions.Any(p => permissions.Contains(p));
                _logger.LogInformation($"Menu item {menuItem.Title}: Required permissions: {string.Join(", ", menuItem.RequiredPermissions)}, HasPermission: {hasPermission}");
                
                if (hasPermission)
                {
                    userMenuItems.Add(menuItem);
                }
            }

            return View(userMenuItems);
        }
    }
} 