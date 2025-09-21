using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.Linq;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientList")]
    public class ImmunizationRequestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ImmunizationRequestsModel> _logger;
        private readonly IImmunizationReminderService _immunizationReminderService;

        public ImmunizationRequestsModel(
            ApplicationDbContext context,
            ILogger<ImmunizationRequestsModel> logger,
            IImmunizationReminderService immunizationReminderService)
        {
            _context = context;
            _logger = logger;
            _immunizationReminderService = immunizationReminderService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedBarangay { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; } = string.Empty;

        public List<ImmunizationShortcutForm> Requests { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var query = _context.ImmunizationShortcutForms.AsQueryable();

                // Apply search filters
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(r => r.ChildName.Contains(SearchTerm) || 
                                           r.MotherName.Contains(SearchTerm) ||
                                           r.FatherName.Contains(SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(SelectedBarangay))
                {
                    query = query.Where(r => r.Barangay == SelectedBarangay);
                }

                if (!string.IsNullOrWhiteSpace(SelectedStatus))
                {
                    query = query.Where(r => r.Status == SelectedStatus);
                }

                Requests = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading immunization requests");
                ModelState.AddModelError("", "An error occurred while loading immunization requests.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSendReminderAsync(string email, string motherName)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(motherName))
                {
                    return new JsonResult(new { success = false, message = "Email and mother name are required." });
                }

                var reminderMessage = $@"Dear {motherName},

This is a friendly reminder about your immunization schedule request.

IMMUNIZATION SERVICES:
- Available every Wednesday
- Time: 8:00 AM - 12:00 PM
- Location: Baesa Health Center

IMPORTANT REMINDERS:
- Please bring your child's birth certificate
- Bring any previous immunization records
- Arrive 15 minutes before your scheduled time
- If you need to reschedule, please contact us in advance

For any questions or to schedule an appointment, please contact Baesa Health Center.

Thank you for prioritizing your child's health.

Best regards,
Baesa Health Center Team";

                await _immunizationReminderService.SendImmunizationReminderAsync(email, motherName, reminderMessage);
                
                _logger.LogInformation("Individual immunization reminder sent successfully to {Email} for {MotherName}", email, motherName);
                
                return new JsonResult(new { success = true, message = "Reminder email sent successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send individual immunization reminder to {Email}", email);
                return new JsonResult(new { success = false, message = "Failed to send reminder email. Please try again." });
            }
        }
    }
}
