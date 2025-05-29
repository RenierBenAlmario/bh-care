using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Barangay.Pages.Nurse
{
    [Authorize(Policy = "AccessNurseDashboard")]
    public class NurseDashboardModel : PageModel
    {
        private readonly string _connectionString;
        private readonly PermissionService _permissionService;
        
        public NurseDashboardModel(IConfiguration configuration, PermissionService permissionService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found");
            _permissionService = permissionService;
            
            // Initialize properties with sample data (for May 27, 2025)
            ErrorMessage = string.Empty;
            TodaysTotalPatients = 5;
            InProgressCount = 2;
            WaitingCount = 1;
            CompletedTodayCount = 2;
            
            // Initialize collections
            Alerts = new List<AlertViewModel>();
            Queue = new List<QueueViewModel>();
            PatientRecords = new List<PatientRecordViewModel>();
        }
        
        public string ErrorMessage { get; set; }
        public int TodaysTotalPatients { get; set; }
        public int InProgressCount { get; set; }
        public int WaitingCount { get; set; }
        public int CompletedTodayCount { get; set; }
        public List<AlertViewModel> Alerts { get; set; }
        public List<QueueViewModel> Queue { get; set; }
        public List<PatientRecordViewModel> PatientRecords { get; set; }
        
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // The authorization is now handled by the [Authorize(Policy = "AccessNurseDashboard")] attribute
                // We don't need to check the permission again here

                // Check additional permissions for specific functionalities
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var canManageAppointments = await _permissionService.HasPermissionAsync(userId, "ManageAppointments");
                var canRecordVitalSigns = await _permissionService.HasPermissionAsync(userId, "Record Vital Signs");
                var canManageMedicalRecords = await _permissionService.HasPermissionAsync(userId, "Manage Medical Records");
                var canViewPatientHistory = await _permissionService.HasPermissionAsync(userId, "View Patient History");

                // Load dashboard data based on permissions
                // ... (rest of your existing dashboard loading logic)

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading dashboard: {ex.Message}";
                return Page();
            }
        }

        public class AlertViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
        
        public class QueueViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string WaitingTime { get; set; } = string.Empty;
            public string Priority { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

        public class PatientRecordViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; } = string.Empty;
            public string RecordNo { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
            public string Gender { get; set; } = string.Empty;
            public DateTime LastVisit { get; set; }
            public string Diagnosis { get; set; } = string.Empty;
        }
    }
}
