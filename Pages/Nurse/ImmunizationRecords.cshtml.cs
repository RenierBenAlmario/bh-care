using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientList")]
    public class ImmunizationRecordsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly IImmunizationReminderService _immunizationReminderService;
        private readonly ILogger<ImmunizationRecordsModel> _logger;

        public ImmunizationRecordsModel(
            EncryptedDbContext context,
            IImmunizationReminderService immunizationReminderService,
            ILogger<ImmunizationRecordsModel> logger)
        {
            _context = context;
            _immunizationReminderService = immunizationReminderService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SelectedBarangay { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string FamilyNumber { get; set; } = string.Empty;

        public IQueryable<ImmunizationRecord> Records { get; set; } = Enumerable.Empty<ImmunizationRecord>().AsQueryable();

        public async Task OnGetAsync()
        {
            var query = _context.ImmunizationRecords.AsQueryable();

            // Apply search filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(r => r.ChildName.Contains(SearchTerm) || 
                                        r.MotherName.Contains(SearchTerm) ||
                                        r.FatherName.Contains(SearchTerm));
            }

            if (!string.IsNullOrEmpty(SelectedBarangay))
            {
                query = query.Where(r => r.Barangay == SelectedBarangay);
            }

            if (!string.IsNullOrEmpty(FamilyNumber))
            {
                query = query.Where(r => r.FamilyNumber.Contains(FamilyNumber));
            }

            // Order by most recent first
            Records = query.OrderByDescending(r => r.CreatedAt);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var record = await _context.ImmunizationRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            _context.ImmunizationRecords.Remove(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Immunization record deleted successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateAsync(int id, string childName, string dateOfBirth, 
            string motherName, string fatherName, string sex, string address, string barangay, 
            string healthCenter, string email, string contactNumber,
            string bcgVaccineDate, string bcgVaccineRemarks,
            string hepatitisBVaccineDate, string hepatitisBVaccineRemarks,
            string pentavalent1Date, string pentavalent1Remarks,
            string pentavalent2Date, string pentavalent2Remarks,
            string pentavalent3Date, string pentavalent3Remarks,
            string opv1Date, string opv1Remarks,
            string opv2Date, string opv2Remarks,
            string opv3Date, string opv3Remarks,
            string ipv1Date, string ipv1Remarks,
            string ipv2Date, string ipv2Remarks,
            string pcv1Date, string pcv1Remarks,
            string pcv2Date, string pcv2Remarks,
            string pcv3Date, string pcv3Remarks,
            string mmr1Date, string mmr1Remarks,
            string mmr2Date, string mmr2Remarks)
        {
            var record = await _context.ImmunizationRecords.FindAsync(id);
            if (record == null)
            {
                return NotFound();
            }

            // Update basic information
            record.ChildName = childName;
            record.DateOfBirth = dateOfBirth;
            record.MotherName = motherName;
            record.FatherName = fatherName ?? string.Empty;
            record.Sex = sex;
            record.Address = address ?? string.Empty;
            record.Barangay = barangay;
            record.HealthCenter = healthCenter ?? string.Empty;
            record.Email = email ?? string.Empty;
            record.ContactNumber = contactNumber ?? string.Empty;

            // Update vaccine information
            record.BCGVaccineDate = bcgVaccineDate;
            record.BCGVaccineRemarks = bcgVaccineRemarks ?? string.Empty;
            record.HepatitisBVaccineDate = hepatitisBVaccineDate;
            record.HepatitisBVaccineRemarks = hepatitisBVaccineRemarks ?? string.Empty;
            
            // Pentavalent doses
            record.Pentavalent1Date = pentavalent1Date;
            record.Pentavalent1Remarks = pentavalent1Remarks ?? string.Empty;
            record.Pentavalent2Date = pentavalent2Date;
            record.Pentavalent2Remarks = pentavalent2Remarks ?? string.Empty;
            record.Pentavalent3Date = pentavalent3Date;
            record.Pentavalent3Remarks = pentavalent3Remarks ?? string.Empty;
            
            // OPV doses
            record.OPV1Date = opv1Date;
            record.OPV1Remarks = opv1Remarks ?? string.Empty;
            record.OPV2Date = opv2Date;
            record.OPV2Remarks = opv2Remarks ?? string.Empty;
            record.OPV3Date = opv3Date;
            record.OPV3Remarks = opv3Remarks ?? string.Empty;
            
            // IPV doses
            record.IPV1Date = ipv1Date;
            record.IPV1Remarks = ipv1Remarks ?? string.Empty;
            record.IPV2Date = ipv2Date;
            record.IPV2Remarks = ipv2Remarks ?? string.Empty;
            
            // PCV doses
            record.PCV1Date = pcv1Date;
            record.PCV1Remarks = pcv1Remarks ?? string.Empty;
            record.PCV2Date = pcv2Date;
            record.PCV2Remarks = pcv2Remarks ?? string.Empty;
            record.PCV3Date = pcv3Date;
            record.PCV3Remarks = pcv3Remarks ?? string.Empty;
            
            // MMR doses
            record.MMR1Date = mmr1Date;
            record.MMR1Remarks = mmr1Remarks ?? string.Empty;
            record.MMR2Date = mmr2Date;
            record.MMR2Remarks = mmr2Remarks ?? string.Empty;

            record.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            record.UpdatedBy = User.Identity?.Name ?? "Unknown";

            await _context.SaveChangesAsync();

            // Send email notification if email is provided
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    await _immunizationReminderService.SendVaccineUpdateNotificationAsync(email, childName, record);
                    TempData["SuccessMessage"] = $"Immunization record for {childName} updated successfully. Email notification sent to {email}.";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send vaccine update notification email to {Email}", email);
                    TempData["SuccessMessage"] = $"Immunization record for {childName} updated successfully. However, email notification could not be sent.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = $"Immunization record for {childName} updated successfully.";
            }

            return RedirectToPage();
        }
    }
}