using Barangay.Models;
using Barangay.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barangay.ViewComponents
{
    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly PermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SidebarMenuViewComponent(
            PermissionService permissionService,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string role)
        {
            if (!User.Identity.IsAuthenticated)
                return View("Default", new List<SidebarMenuItem>());

            var claimsPrincipal = User as ClaimsPrincipal;
            if (claimsPrincipal == null)
                return View("Default", new List<SidebarMenuItem>());
                
            var userId = _userManager.GetUserId(claimsPrincipal);
            if (string.IsNullOrEmpty(userId))
                return View("Default", new List<SidebarMenuItem>());

            // Get all permissions for the user
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);
            
            // Get current path to determine active item
            var currentPath = _httpContextAccessor.HttpContext?.Request.Path.Value?.ToLower() ?? "";

            // Define navigation items based on role and check against permissions
            var navItems = new List<SidebarMenuItem>();

            switch (role?.ToLower())
            {
                case "nurse":
                    // Dashboard is always accessible for a nurse role
                    navItems.Add(new SidebarMenuItem { 
                        Text = "Dashboard", 
                        Icon = "tachometer-alt", 
                        Url = "/Nurse/NurseDashboard", 
                        Permission = "Access Nurse Dashboard",
                        IsActive = currentPath.Contains("/nurse/nursedashboard")
                    });
                    
                    // Medical Records link
                    navItems.Add(new SidebarMenuItem { 
                        Text = "Medical Records", 
                        Icon = "file-medical", 
                        Url = "/Records/Index", 
                        Permission = "Manage Medical Records",
                        IsActive = currentPath.Contains("/records")
                    });
                    
                    if (userPermissions.Contains("ManageAppointments"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Appointments", 
                            Icon = "calendar-check", 
                            Url = "/Nurse/Appointments", 
                            Permission = "ManageAppointments",
                            IsActive = currentPath.Contains("/nurse/appointments")
                        });
                    
                    if (userPermissions.Contains("Record Vital Signs"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Vitals", 
                            Icon = "heartbeat", 
                            Url = "/Nurse/VitalSigns", 
                            Permission = "Record Vital Signs",
                            IsActive = currentPath.Contains("/nurse/vitalsigns")
                        });
                    
                    if (userPermissions.Contains("View Patient History"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Diagnosis", 
                            Icon = "stethoscope", 
                            Url = "/Nurse/Diagnosis", 
                            Permission = "View Patient History",
                            IsActive = currentPath.Contains("/nurse/diagnosis")
                        });
                    
                    if (userPermissions.Contains("Access Nurse Dashboard"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Queue", 
                            Icon = "list-ol", 
                            Url = "/Nurse/Queue", 
                            Permission = "Access Nurse Dashboard",
                            IsActive = currentPath.Contains("/nurse/queue")
                        });
                        
                    // Notifications section
                    if (userPermissions.Contains("View Notifications"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Notifications", 
                            Icon = "bell", 
                            Url = "/Nurse/Notifications", 
                            Permission = "View Notifications",
                            IsActive = currentPath.Contains("/nurse/notifications")
                        });
                        
                    // Settings section
                    if (userPermissions.Contains("Access Settings"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Settings", 
                            Icon = "cog", 
                            Url = "/Nurse/Settings", 
                            Permission = "Access Settings",
                            IsActive = currentPath.Contains("/nurse/settings")
                        });
                    break;

                case "doctor":
                    // Dashboard is always accessible for a doctor role
                    navItems.Add(new SidebarMenuItem { 
                        Text = "Dashboard", 
                        Icon = "tachometer-alt", 
                        Url = "/Doctor/DoctorDashboard", 
                        Permission = "Access Doctor Dashboard",
                        IsActive = currentPath.Contains("/doctor/doctordashboard")
                    });
                    
                    // Medical Records link
                    navItems.Add(new SidebarMenuItem { 
                        Text = "Medical Records", 
                        Icon = "file-medical", 
                        Url = "/Records/Index", 
                        Permission = "Manage Medical Records",
                        IsActive = currentPath.Contains("/records")
                    });
                    
                    if (userPermissions.Contains("Manage Appointments"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Appointments", 
                            Icon = "calendar-alt", 
                            Url = "/Doctor/Appointments", 
                            Permission = "Manage Appointments",
                            IsActive = currentPath.Contains("/doctor/appointments")
                        });
                    
                    if (userPermissions.Contains("View Patient History"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Patients", 
                            Icon = "user-injured", 
                            Url = "/Doctor/Patients", 
                            Permission = "View Patient History",
                            IsActive = currentPath.Contains("/doctor/patients")
                        });
                    
                    if (userPermissions.Contains("Create Prescriptions"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Prescriptions", 
                            Icon = "prescription-bottle-alt", 
                            Url = "/Doctor/Prescriptions", 
                            Permission = "Create Prescriptions",
                            IsActive = currentPath.Contains("/doctor/prescriptions")
                        });
                    
                    if (userPermissions.Contains("View Reports"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Reports", 
                            Icon = "chart-bar", 
                            Url = "/Doctor/Reports", 
                            Permission = "View Reports",
                            IsActive = currentPath.Contains("/doctor/reports")
                        });
                        
                    // Notifications section
                    if (userPermissions.Contains("View Notifications"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Notifications", 
                            Icon = "bell", 
                            Url = "/Doctor/Notifications", 
                            Permission = "View Notifications",
                            IsActive = currentPath.Contains("/doctor/notifications")
                        });
                        
                    // Settings section
                    if (userPermissions.Contains("Access Settings"))
                        navItems.Add(new SidebarMenuItem { 
                            Text = "Settings", 
                            Icon = "cog", 
                            Url = "/Doctor/Settings", 
                            Permission = "Access Settings",
                            IsActive = currentPath.Contains("/doctor/settings")
                        });
                    break;
            }

            return View("Default", navItems);
        }
    }

    public class SidebarMenuItem
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Permission { get; set; }
        public bool IsActive { get; set; }
    }
} 