using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Barangay.Models;
using Barangay.Data;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.User
{
    [Authorize]
    public class AppointmentConfirmationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AppointmentConfirmationModel> _logger;

        public AppointmentConfirmationModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<AppointmentConfirmationModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public Appointment Appointment { get; set; }
        public NCDRiskAssessment NCDRiskAssessment { get; set; }
        public HEEADSSSAssessment HEEADSSSAssessment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // If no specific appointment ID provided, get the most recent appointment
            if (id == null || id == 1) // 1 is a placeholder ID
            {
                // Get the most recent appointment for this user
                Appointment = await _context.Appointments
                    .Where(a => a.PatientId == user.Id || a.ApplicationUserId == user.Id)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();
                    
                if (Appointment == null)
                {
                    return NotFound("No appointments found for this user.");
                }
                
                id = Appointment.Id;
            }
            else
            {
                // Get the specific appointment
                Appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (Appointment == null)
                {
                    return NotFound("Appointment not found.");
                }

                // For security reasons, check if the appointment belongs to the current user
                if (Appointment.PatientId != user.Id && Appointment.ApplicationUserId != user.Id)
                {
                    _logger.LogWarning($"User {user.Id} attempted to access appointment {id} that belongs to another user.");
                    return Forbid();
                }
            }

            // Try to load associated NCD Risk Assessment
            NCDRiskAssessment = await _context.NCDRiskAssessments
                .FirstOrDefaultAsync(n => n.AppointmentId == id);

            // If no NCD Risk Assessment, try to load HEEADSSS Assessment
            if (NCDRiskAssessment == null)
            {
                HEEADSSSAssessment = await _context.HEEADSSSAssessments
                    .FirstOrDefaultAsync(h => h.AppointmentId == id);
            }

            return Page();
        }
    }
} 