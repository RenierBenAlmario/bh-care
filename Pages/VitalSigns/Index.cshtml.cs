using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.VitalSigns
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
            VitalSignRecords = new List<VitalSignViewModel>();
            PatientSelectList = new List<SelectListItem>();
        }

        public class VitalSignViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public DateTime RecordedAt { get; set; }
            public string? BloodPressure { get; set; }
            public int? HeartRate { get; set; }
            public decimal? Temperature { get; set; }
            public int? RespiratoryRate { get; set; }
            public decimal? SpO2 { get; set; }
            public string? Notes { get; set; }
        }

        [BindProperty]
        public VitalSignViewModel NewVitalSign { get; set; } = new();
        public List<VitalSignViewModel> VitalSignRecords { get; set; }
        public List<SelectListItem> PatientSelectList { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                // Load patients for the dropdown
                var patients = await _context.Patients
                    .OrderBy(p => p.Name)
                    .Select(p => new SelectListItem
                    {
                        Value = p.UserId,
                        Text = p.Name
                    })
                    .ToListAsync();

                PatientSelectList = patients;
                _logger.LogInformation($"Loaded {patients.Count} patients for dropdown");

                // Load vital signs
                var vitalSigns = await _context.VitalSigns
                    .Include(v => v.Patient)
                    .OrderByDescending(v => v.RecordedAt)
                    .ToListAsync();

                VitalSignRecords = vitalSigns.Select(v => new VitalSignViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    PatientName = v.Patient?.Name ?? "Unknown",
                    RecordedAt = v.RecordedAt,
                    BloodPressure = v.BloodPressure,
                    HeartRate = v.HeartRate,
                    Temperature = v.Temperature,
                    RespiratoryRate = v.RespiratoryRate,
                    SpO2 = v.SpO2,
                    Notes = v.Notes
                }).ToList();

                _logger.LogInformation($"Loaded {VitalSignRecords.Count} vital sign records");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vital signs");
                ModelState.AddModelError(string.Empty, "Error loading vital signs. Please try again.");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var vitalSign = new VitalSign
                {
                    PatientId = NewVitalSign.PatientId,
                    Temperature = NewVitalSign.Temperature,
                    BloodPressure = NewVitalSign.BloodPressure,
                    HeartRate = NewVitalSign.HeartRate,
                    RespiratoryRate = NewVitalSign.RespiratoryRate,
                    SpO2 = NewVitalSign.SpO2.HasValue ? (int?)NewVitalSign.SpO2.Value : null,
                    RecordedAt = DateTime.Now,
                    Notes = NewVitalSign.Notes
                };

                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully saved vital signs for patient {NewVitalSign.PatientId}");
                TempData["SuccessMessage"] = "Vital signs recorded successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vital signs");
                ModelState.AddModelError(string.Empty, "Error saving vital signs. Please try again.");
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var vitalSign = await _context.VitalSigns.FindAsync(id);
                if (vitalSign == null)
                {
                    return NotFound();
                }

                _context.VitalSigns.Remove(vitalSign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully deleted vital sign record {id}");
                TempData["SuccessMessage"] = "Vital sign record deleted successfully!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vital sign record");
                TempData["ErrorMessage"] = "Error deleting vital sign record. Please try again.";
                return RedirectToPage();
            }
        }
    }
}
