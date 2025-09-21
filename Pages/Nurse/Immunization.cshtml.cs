using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientList")]
    public class ImmunizationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImmunizationModel> _logger;
        private readonly IImmunizationReminderService _immunizationReminderService;

        public ImmunizationModel(
            ApplicationDbContext context, 
            ILogger<ImmunizationModel> logger,
            IImmunizationReminderService immunizationReminderService)
        {
            _context = context;
            _logger = logger;
            _immunizationReminderService = immunizationReminderService;
        }

        [BindProperty]
        public string EmailMessage { get; set; } = string.Empty;

        [BindProperty]
        public bool SendToAllPatients { get; set; } = true;

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostSendReminderAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EmailMessage))
                {
                    ErrorMessage = "Please enter an email message.";
                    return Page();
                }

                if (SendToAllPatients)
                {
                    // Get all patient users (exclude doctors, nurses, and admins)
                    var patientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Patient");
                    if (patientRole == null)
                    {
                        ErrorMessage = "Patient role not found.";
                        return Page();
                    }

                    var patientUserIds = await _context.UserRoles
                        .Where(ur => ur.RoleId == patientRole.Id)
                        .Select(ur => ur.UserId)
                        .ToListAsync();

                    var patients = await _context.Users
                        .Where(u => patientUserIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Email))
                        .Select(u => new { u.Email, u.FullName })
                        .ToListAsync();

                    int emailsSent = 0;
                    foreach (var patient in patients)
                    {
                        try
                        {
                            await _immunizationReminderService.SendImmunizationReminderAsync(
                                patient.Email, 
                                patient.FullName, 
                                EmailMessage);
                            emailsSent++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send immunization reminder to {Email}", patient.Email);
                        }
                    }

                    SuccessMessage = $"Immunization reminder sent successfully to {emailsSent} patients.";
                }
                else
                {
                    ErrorMessage = "Please select at least one recipient option.";
                    return Page();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending immunization reminders");
                ErrorMessage = "An error occurred while sending reminders. Please try again.";
                return Page();
            }
        }
    }
}
