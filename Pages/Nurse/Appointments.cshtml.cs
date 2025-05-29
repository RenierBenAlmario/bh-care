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
    [Authorize(Roles = "Nurse")]
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

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get all appointments
                var appointments = await _context.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

                // Get all doctors
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                var doctorDict = doctors.ToDictionary(d => d.Id, d => d.FullName ?? d.UserName ?? "Unknown");

                // Map to view model
                Appointments = appointments.Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.PatientName,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    DoctorId = a.DoctorId,
                    DoctorName = doctorDict.TryGetValue(a.DoctorId, out var name) ? name : "Unknown",
                    Status = a.Status,
                    Type = a.Type ?? "General",
                    Description = a.Description
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments");
                StatusMessage = "Error loading appointments. Please try again later.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                
                if (appointment == null)
                {
                    StatusMessage = "Appointment not found.";
                    return RedirectToPage();
                }

                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                
                StatusMessage = "Appointment deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment");
                StatusMessage = "Error deleting appointment. Please try again later.";
                return RedirectToPage();
            }
        }
    }
} 