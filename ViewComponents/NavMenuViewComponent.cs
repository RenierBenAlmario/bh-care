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
    public class NavMenuViewComponent : ViewComponent
    {
        private readonly PermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NavMenuViewComponent(
            PermissionService permissionService,
            UserManager<ApplicationUser> userManager)
        {
            _permissionService = permissionService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string role)
        {
            if (!User.Identity.IsAuthenticated)
                return View("Default", new List<NavMenuItem>());

            // Cast User to ClaimsPrincipal
            var claimsPrincipal = User as ClaimsPrincipal;
            if (claimsPrincipal == null)
                return View("Default", new List<NavMenuItem>());
                
            var userId = _userManager.GetUserId(claimsPrincipal);
            if (string.IsNullOrEmpty(userId))
                return View("Default", new List<NavMenuItem>());

            // Get all permissions for the user
            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);

            // Define navigation items based on role and check against permissions
            var navItems = new List<NavMenuItem>();

            switch (role?.ToLower())
            {
                case "nurse":
                    if (userPermissions.Contains("Access Nurse Dashboard"))
                        navItems.Add(new NavMenuItem { Text = "Dashboard", Icon = "tachometer-alt", Url = "/Nurse/NurseDashboard", Permission = "Access Nurse Dashboard" });
                    
                    if (userPermissions.Contains("ManageAppointments"))
                        navItems.Add(new NavMenuItem { Text = "Appointments", Icon = "calendar-plus", Url = "/Nurse/NewAppointment", Permission = "ManageAppointments" });
                    
                    if (userPermissions.Contains("Record Vital Signs"))
                        navItems.Add(new NavMenuItem { Text = "Vitals", Icon = "heartbeat", Url = "/Nurse/VitalSigns", Permission = "Record Vital Signs" });
                    
                    if (userPermissions.Contains("Manage Medical Records"))
                        navItems.Add(new NavMenuItem { Text = "Records", Icon = "notes-medical", Url = "/Nurse/MedicalHistory", Permission = "Manage Medical Records" });
                    
                    if (userPermissions.Contains("View Patient History"))
                        navItems.Add(new NavMenuItem { Text = "Diagnosis", Icon = "stethoscope", Url = "/Nurse/DiagnoseDB", Permission = "View Patient History" });
                    
                    if (userPermissions.Contains("Access Nurse Dashboard"))
                        navItems.Add(new NavMenuItem { Text = "Queue", Icon = "list", Url = "/Nurse/PatientQueue", Permission = "Access Nurse Dashboard" });
                    break;

                case "doctor":
                    if (userPermissions.Contains("Access Doctor Dashboard"))
                        navItems.Add(new NavMenuItem { Text = "Dashboard", Icon = "tachometer-alt", Url = "/Doctor/DoctorDashboard", Permission = "Access Doctor Dashboard" });
                    
                    if (userPermissions.Contains("Manage Appointments"))
                        navItems.Add(new NavMenuItem { Text = "Appointments", Icon = "calendar-alt", Url = "/Doctor/Appointments", Permission = "Manage Appointments" });
                    
                    if (userPermissions.Contains("Manage Medical Records"))
                        navItems.Add(new NavMenuItem { Text = "Medical Records", Icon = "notes-medical", Url = "/Doctor/PatientRecords", Permission = "Manage Medical Records" });
                    
                    if (userPermissions.Contains("View Patient History"))
                        navItems.Add(new NavMenuItem { Text = "Patients", Icon = "user-injured", Url = "/Doctor/Patients", Permission = "View Patient History" });
                    
                    if (userPermissions.Contains("Create Prescriptions"))
                        navItems.Add(new NavMenuItem { Text = "Prescriptions", Icon = "prescription", Url = "/Doctor/Prescriptions", Permission = "Create Prescriptions" });
                    
                    if (userPermissions.Contains("View Reports"))
                        navItems.Add(new NavMenuItem { Text = "Reports", Icon = "chart-bar", Url = "/Doctor/Reports", Permission = "View Reports" });
                    break;

                case "admin":
                    if (userPermissions.Contains("Access Admin Dashboard"))
                        navItems.Add(new NavMenuItem { Text = "Dashboard", Icon = "tachometer-alt", Url = "/Admin/AdminDashboard", Permission = "Access Admin Dashboard" });
                    
                    if (userPermissions.Contains("Manage Users"))
                        navItems.Add(new NavMenuItem { Text = "User Management", Icon = "users", Url = "/Admin/UserManagement", Permission = "Manage Users" });
                    
                    if (userPermissions.Contains("Manage Permissions"))
                        navItems.Add(new NavMenuItem { Text = "Permissions", Icon = "key", Url = "/Admin/ManagePermissions", Permission = "Manage Permissions" });
                    
                    if (userPermissions.Contains("View Reports"))
                        navItems.Add(new NavMenuItem { Text = "Reports", Icon = "chart-line", Url = "/Admin/Reports", Permission = "View Reports" });
                    
                    if (userPermissions.Contains("Access Admin Dashboard"))
                        navItems.Add(new NavMenuItem { Text = "Settings", Icon = "cog", Url = "/Admin/Settings", Permission = "Access Admin Dashboard" });
                    break;
            }

            return View("Default", navItems);
        }
    }

    public class NavMenuItem
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Permission { get; set; }
        public bool IsActive { get; set; }
    }
} 