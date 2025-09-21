using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientList")]
    public class CreateImmunizationRecordModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<CreateImmunizationRecordModel> _logger;
        private readonly IImmunizationReminderService _immunizationReminderService;

        public CreateImmunizationRecordModel(
            EncryptedDbContext context,
            ILogger<CreateImmunizationRecordModel> logger,
            IImmunizationReminderService immunizationReminderService)
        {
            _context = context;
            _logger = logger;
            _immunizationReminderService = immunizationReminderService;
        }

        [BindProperty]
        public ImmunizationRecord Record { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Set default values
            Record.HealthCenter = "Baesa Health Center";
            Record.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            Record.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            Record.CreatedBy = User.Identity?.Name ?? "Unknown";
            Record.UpdatedBy = User.Identity?.Name ?? "Unknown";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Set audit fields
                Record.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                Record.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                Record.CreatedBy = User.Identity?.Name ?? "Unknown";
                Record.UpdatedBy = User.Identity?.Name ?? "Unknown";

                _context.ImmunizationRecords.Add(Record);
                await _context.SaveChangesAsync();

                // Send email notification to parent
                if (!string.IsNullOrEmpty(Record.Email))
                {
                    await SendImmunizationRecordConfirmationEmailAsync();
                }

                TempData["SuccessMessage"] = $"Immunization record for {Record.ChildName} has been created successfully. Confirmation email sent to {Record.Email}.";
                _logger.LogInformation("Immunization record created for child {ChildName} by user {User}", 
                    Record.ChildName, User.Identity?.Name);

                return RedirectToPage("/Nurse/Immunization");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating immunization record for child {ChildName}", Record.ChildName);
                ModelState.AddModelError("", "An error occurred while saving the immunization record. Please try again.");
                return Page();
            }
        }

        private async Task SendImmunizationRecordConfirmationEmailAsync()
        {
            try
            {
                var confirmationMessage = $@"Dear {Record.MotherName},

Your child's immunization record has been successfully created at Baesa Health Center.

CHILD INFORMATION:
- Name: {Record.ChildName}
- Date of Birth: {Record.DateOfBirth:MMMM dd, yyyy}
- Sex: {Record.Sex}
- Family Number: {Record.FamilyNumber}

HEALTH CENTER DETAILS:
- Health Center: {Record.HealthCenter}
- Barangay: {Record.Barangay}
- Record Created: {Record.CreatedAt:MMMM dd, yyyy 'at' h:mm tt}

IMMUNIZATION SCHEDULE:
Your child's vaccination schedule has been recorded. Please ensure to bring your child for all scheduled vaccinations according to the official immunization schedule.

IMPORTANT REMINDERS:
- Keep this email as proof of your child's immunization record
- Bring this record for future visits
- Contact us if you have any questions about your child's vaccination schedule
- Immunization services are available every Wednesday from 8:00 AM to 12:00 PM

For any questions or concerns, please contact Baesa Health Center.

Best regards,
Baesa Health Center Team";

                await _immunizationReminderService.SendImmunizationReminderAsync(
                    Record.Email, 
                    Record.MotherName, 
                    confirmationMessage);

                _logger.LogInformation("Immunization record confirmation email sent to {Email} for child {ChildName}", 
                    Record.Email, Record.ChildName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send immunization record confirmation email to {Email}", Record.Email);
                // Don't fail the form submission if email fails
            }
        }
    }
}
