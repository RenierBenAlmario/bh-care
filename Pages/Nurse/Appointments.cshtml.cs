using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "Appointments")]
    public class AppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsModel(
            ApplicationDbContext context, 
            ILogger<AppointmentsModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public class AppointmentViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public DateTime AppointmentDate { get; set; }
            public TimeSpan AppointmentTime { get; set; }
            public string DoctorId { get; set; }
            public string DoctorName { get; set; }
            public AppointmentStatus Status { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
        }

        public List<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
        public List<AppointmentViewModel> TodayAppointments { get; set; } = new List<AppointmentViewModel>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Loading appointments for nurse dashboard");
                
                // Get all appointments with eager loading of Patient and Doctor
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => a.PatientName != "System Administrator" && a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

                _logger.LogInformation("Found {0} appointments in the database", appointments.Count);
                
                // Get today's date
                var today = DateTime.Today;
                
                // Convert to view models
                Appointments = appointments.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = string.IsNullOrEmpty(a.PatientName) ? 
                        (a.Patient != null ? a.Patient.FullName : "Unknown") : a.PatientName,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor?.FullName ?? "Not Assigned",
                    Status = a.Status,
                    Type = a.Type ?? "General",
                    Description = a.Description
                }).ToList();
                
                // Filter today's appointments (exclude Draft and Cancelled)
                TodayAppointments = Appointments
                    .Where(a => a.AppointmentDate.Date == today
                                && a.Status != AppointmentStatus.Draft
                                && a.Status != AppointmentStatus.Cancelled)
                    .OrderBy(a => a.AppointmentTime)
                    .ToList();
                
                _logger.LogInformation("Found {0} appointments for today", TodayAppointments.Count);

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments");
                StatusMessage = "Error loading appointments. Please try again later.";
                return Page();
            }
        }

    }
} 