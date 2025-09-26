using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System;
using System.Linq;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientList")]
    public class ImmunizationRequestsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<ImmunizationRequestsModel> _logger;
        private readonly IImmunizationReminderService _immunizationReminderService;
        private readonly IDataEncryptionService _encryptionService;

        public ImmunizationRequestsModel(
            EncryptedDbContext context,
            ILogger<ImmunizationRequestsModel> logger,
            IImmunizationReminderService immunizationReminderService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _immunizationReminderService = immunizationReminderService;
            _encryptionService = encryptionService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedBarangay { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; } = string.Empty;

        public List<ImmunizationShortcutForm> Requests { get; set; } = new();

        // Dropdown options for Tag Helpers
        public List<SelectListItem> BarangayOptions { get; set; } = new();
        public List<SelectListItem> StatusOptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Build dropdown options (Tag Helpers use Selected* model values to mark selection)
                BarangayOptions = new List<SelectListItem>
                {
                    new SelectListItem("All Barangays", ""),
                    new SelectListItem("Barangay 158", "Barangay 158"),
                    new SelectListItem("Barangay 159", "Barangay 159"),
                    new SelectListItem("Barangay 160", "Barangay 160"),
                    new SelectListItem("Barangay 161", "Barangay 161"),
                };

                StatusOptions = new List<SelectListItem>
                {
                    new SelectListItem("All Status", ""),
                    new SelectListItem("Pending", "Pending"),
                    new SelectListItem("Scheduled", "Scheduled"),
                    new SelectListItem("Completed", "Completed"),
                };

                var query = _context.ImmunizationShortcutForms.AsQueryable();

                // Order by most recent first and materialize the query first
                Requests = await query
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                // Decrypt all requests for authorized users BEFORE applying search filters
                foreach (var request in Requests)
                {
                    request.DecryptImmunizationShortcutData(_encryptionService, User);
                }

                // Apply search filters AFTER decryption
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    Requests = Requests.Where(r => r.ChildName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) || 
                                               r.MotherName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                                               r.FatherName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrWhiteSpace(SelectedBarangay))
                {
                    Requests = Requests.Where(r => r.Barangay == SelectedBarangay).ToList();
                }

                if (!string.IsNullOrWhiteSpace(SelectedStatus))
                {
                    Requests = Requests.Where(r => r.Status == SelectedStatus).ToList();
                }

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
