using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Barangay.Helpers;

namespace Barangay.Pages.Doctor
{
    [Authorize(Policy = "AccessDoctorDashboard")]
    public class DoctorDashboardModel : PageModel
    {
        private readonly ILogger<DoctorDashboardModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEncryptionService _encryptionService;

        public DoctorDashboardModel(
            ILogger<DoctorDashboardModel> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEncryptionService encryptionService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        public class DoctorPatientRecord
        {
            public string Name { get; set; } = string.Empty;
            public string RecordNo { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public DateTime AppointmentDate { get; set; }
            public AppointmentStatus Status { get; set; }
            public string Description { get; set; } = string.Empty;
            public int Age { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string LastVisit { get; set; } = string.Empty;
        }

        public int ConsultationCount { get; private set; }
        public int CriticalAlerts { get; private set; }
        public int TotalPatients { get; private set; }
        public int Consulted { get; private set; }
        public int Pending { get; private set; }
        public int UrgentCases { get; private set; }
        public Dictionary<string, int> ConsultationMetrics { get; private set; } = new();
        public List<DoctorPatientRecord> PatientRecords { get; private set; } = new();
        public List<DoctorPatientRecord> Alerts { get; private set; } = new();
        public List<DoctorPatientRecord> Queue { get; private set; } = new();
        public bool IsAvailable { get; private set; }
        public string WorkingDays { get; private set; } = "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday";
        public string WorkingHoursStart { get; private set; } = "05:00";
        public string WorkingHoursEnd { get; private set; } = "12:00";
        public int MaxDailyPatients { get; private set; } = 20;
        public string Status { get; private set; } = "Active";

        // Add this property to the class
        public StaffMember? Doctor { get; set; }
        
        // Fix the appointment list declarations - use the fully qualified name or an alias
        public List<Models.Appointment> UpcomingAppointments { get; set; } = new List<Models.Appointment>();
        public List<Models.Appointment> PastAppointments { get; set; } = new List<Models.Appointment>();

        public ApplicationUser CurrentDoctor { get; set; }
        public string DoctorName => CurrentDoctor?.FullName ?? CurrentDoctor?.Email ?? "Doctor";
        public string DoctorEmail => CurrentDoctor?.Email ?? "";
        public string DoctorSpecialization { get; set; } = "General Practitioner";
        
        public DoctorAvailability Availability { get; set; }
        public List<Notification> RecentNotifications { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                // Get doctor information
                CurrentDoctor = await _userManager.GetUserAsync(User);
                if (CurrentDoctor == null)
                    return NotFound();

                // Get availability settings
                Availability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(da => da.DoctorId == userId) ?? new DoctorAvailability
                    {
                        DoctorId = userId,
                        IsAvailable = true
                    };

                // Calculate dashboard metrics
                var today = DateTime.Today;
                
                // Get all unique patients
                TotalPatients = await _context.Appointments
                    .Where(a => a.DoctorId == userId)
                    .Select(a => a.PatientId)
                    .Distinct()
                    .CountAsync();

                // Get consulted (completed) appointments
                Consulted = await _context.Appointments
                    .CountAsync(a => a.DoctorId == userId && 
                                   a.Status == AppointmentStatus.Completed);

                // Get pending appointments
                Pending = await _context.Appointments
                    .CountAsync(a => a.DoctorId == userId && 
                                   a.Status == AppointmentStatus.Pending);

                // Get urgent cases
                UrgentCases = await _context.Appointments
                    .CountAsync(a => a.DoctorId == userId && 
                                   a.Status == AppointmentStatus.Pending && 
                                   a.Type == "Urgent");

                // Get recent notifications
                RecentNotifications = await _context.Notifications
                    .Where(n => n.RecipientId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctor dashboard");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostUpdateAvailabilityAsync(DoctorAvailability availability)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var existingAvailability = await _context.DoctorAvailabilities
                    .FirstOrDefaultAsync(da => da.DoctorId == userId);

                if (existingAvailability == null)
                {
                    availability.DoctorId = userId;
                    availability.LastUpdated = DateTime.Now;
                    _context.DoctorAvailabilities.Add(availability);
                }
                else
                {
                    existingAvailability.IsAvailable = availability.IsAvailable;
                    existingAvailability.Monday = availability.Monday;
                    existingAvailability.Tuesday = availability.Tuesday;
                    existingAvailability.Wednesday = availability.Wednesday;
                    existingAvailability.Thursday = availability.Thursday;
                    existingAvailability.Friday = availability.Friday;
                    existingAvailability.Saturday = availability.Saturday;
                    existingAvailability.Sunday = availability.Sunday;
                    existingAvailability.StartTime = availability.StartTime;
                    existingAvailability.EndTime = availability.EndTime;
                    existingAvailability.LastUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor availability");
                return RedirectToPage("/Error");
            }
        }
    }
}
