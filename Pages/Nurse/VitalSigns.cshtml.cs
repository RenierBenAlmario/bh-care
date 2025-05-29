using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse")]
    public class VitalSignsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VitalSignsModel> _logger;

        public VitalSignsModel(ApplicationDbContext context, ILogger<VitalSignsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class VitalSignViewModel
        {
            public int Id { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public string PatientId { get; set; } = string.Empty;
            public DateTime RecordedAt { get; set; } = DateTime.Now;
            public string? BloodPressure { get; set; }
            public int? HeartRate { get; set; }
            public decimal? Temperature { get; set; }
            public int? RespiratoryRate { get; set; }
            public int? SpO2 { get; set; }
            public decimal? Weight { get; set; }
            public decimal? Height { get; set; }
            public string? Notes { get; set; }
            public Patient Patient { get; set; } // Add this property
        }

        [BindProperty]
        public VitalSignViewModel NewVitalSign { get; set; } = new();

        public List<VitalSignViewModel> VitalSignRecords { get; set; } = new();
        public List<SelectListItem> PatientSelectList { get; set; } = new List<SelectListItem>();
        public string? SelectedPatientId { get; set; }
        public Patient? SelectedPatient { get; set; }

        public async Task<IActionResult> OnGetAsync(string patientId)
        {
            // Load all patients for the dropdown
            await LoadPatientsAsync();
            
            // If patientId is provided, set it as the selected patient
            if (!string.IsNullOrEmpty(patientId))
            {
                SelectedPatientId = patientId;
                SelectedPatient = await _context.Patients.FindAsync(patientId);
                // Set the selected patient in the new vital sign
                NewVitalSign.PatientId = patientId;
                // Load vital signs for this patient
                await LoadVitalSignsForPatientAsync(patientId);
            }
            else
            {
                // Load all vital signs if no specific patient is selected
                await LoadVitalSignsAsync();
            }
            
            return Page();
        }

        private async Task LoadPatientsAsync()
        {
            // Get all patients with appointments
            var patientsWithAppointments = await _context.Appointments
                .Select(a => new { PatientId = a.PatientId, PatientName = a.PatientName })
                .Distinct()
                .ToListAsync();

            // Get all patients who have had vital signs recorded
            var patientsWithVitalSigns = await _context.VitalSigns
                .Join(_context.Appointments,
                    v => v.PatientId,
                    a => a.PatientId,
                    (v, a) => new { PatientId = v.PatientId, PatientName = a.PatientName })
                .Distinct()
                .ToListAsync();

            _logger.LogInformation($"Found {patientsWithAppointments.Count} patients with appointments and {patientsWithVitalSigns.Count} patients with vital signs");

            // Combine both lists and remove duplicates
            var allPatients = patientsWithAppointments
                .Union(patientsWithVitalSigns, new PatientEqualityComparer())
                .Select(p => {
                    dynamic dp = p;
                    return new { PatientId = dp.PatientId, PatientName = dp.PatientName };
                })
                .ToList();

            _logger.LogInformation($"Total patients loaded for dropdown: {allPatients.Count}");

            PatientSelectList = allPatients
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = p.PatientName
                })
                .ToList();
        }

        private class PatientEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object? x, object? y)
            {
                if (x == null || y == null) return false;
                dynamic dx = x;
                dynamic dy = y;
                return dx.PatientId == dy.PatientId;
            }

            public int GetHashCode(object obj)
            {
                if (obj == null) throw new ArgumentNullException(nameof(obj));
                dynamic d = obj;
                return d.PatientId.GetHashCode();
            }
        }

        private async Task LoadVitalSignsForPatientAsync(string patientId)
        {
            // Get the patient name from the latest appointment
            var patientName = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => a.PatientName)
                .FirstOrDefaultAsync() ?? "Unknown";

            // Load vital signs for the specific patient
            var records = await _context.VitalSigns
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .Select(v => new VitalSignViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    PatientName = patientName,
                    RecordedAt = v.RecordedAt,
                    BloodPressure = v.BloodPressure,
                    HeartRate = v.HeartRate,
                    Temperature = v.Temperature,
                    RespiratoryRate = v.RespiratoryRate,
                    SpO2 = v.SpO2.HasValue ? (int?)Convert.ToInt32(v.SpO2.Value) : null,
                    Weight = v.Weight,
                    Height = v.Height,
                    Notes = v.Notes
                })
                .ToListAsync();

            VitalSignRecords = records;
        }

        private async Task LoadVitalSignsAsync()
        {
            // First get the latest appointment names for each patient
            var patientNames = await _context.Appointments
                .GroupBy(a => a.PatientId)
                .Where(g => g.Key != null)
                .Select(g => new { PatientId = g.Key!, PatientName = g.OrderByDescending(a => a.AppointmentDate).First().PatientName })
                .ToDictionaryAsync(x => x.PatientId, x => x.PatientName);

            // Then load vital signs and map the names
            var records = await _context.VitalSigns
                .OrderByDescending(v => v.RecordedAt)
                .Select(v => new VitalSignViewModel
                {
                    Id = v.Id,
                    PatientId = v.PatientId,
                    PatientName = v.PatientId, // Temporarily store ID here
                    RecordedAt = v.RecordedAt,
                    BloodPressure = v.BloodPressure,
                    HeartRate = v.HeartRate,
                    Temperature = v.Temperature,
                    RespiratoryRate = v.RespiratoryRate,
                    SpO2 = v.SpO2.HasValue ? (int?)Convert.ToInt32(v.SpO2.Value) : null,
                    Weight = v.Weight,
                    Height = v.Height,
                    Notes = v.Notes
                })
                .ToListAsync();

            // Map the correct patient names
            foreach (var record in records)
            {
                if (patientNames.TryGetValue(record.PatientId, out var name))
                {
                    record.PatientName = name;
                }
                else
                {
                    record.PatientName = "Unknown";
                }
            }

            VitalSignRecords = records;
        }

        public async Task<IActionResult> OnPostAddVitalSignAsync()
        {
            try
            {
                // Use raw SQL to insert vital signs
                var sql = @"
                    INSERT INTO VitalSigns (
                        PatientId, Temperature, BloodPressure, HeartRate, 
                        RespiratoryRate, SpO2, Weight, Height, RecordedAt, Notes
                    ) 
                    VALUES (
                        @PatientId, @Temperature, @BloodPressure, @HeartRate,
                        @RespiratoryRate, @SpO2, @Weight, @Height, @RecordedAt, @Notes
                    )";

                var parameters = new[]
                {
                    new SqlParameter("@PatientId", SqlDbType.NVarChar) { Value = NewVitalSign.PatientId },
                    new SqlParameter("@Temperature", SqlDbType.Decimal) { Value = NewVitalSign.Temperature.HasValue ? (object)NewVitalSign.Temperature : DBNull.Value },
                    new SqlParameter("@BloodPressure", SqlDbType.NVarChar) { Value = !string.IsNullOrEmpty(NewVitalSign.BloodPressure) ? (object)NewVitalSign.BloodPressure : DBNull.Value },
                    new SqlParameter("@HeartRate", SqlDbType.Int) { Value = NewVitalSign.HeartRate.HasValue ? (object)NewVitalSign.HeartRate : DBNull.Value },
                    new SqlParameter("@RespiratoryRate", SqlDbType.Int) { Value = NewVitalSign.RespiratoryRate.HasValue ? (object)NewVitalSign.RespiratoryRate : DBNull.Value },
                    new SqlParameter("@SpO2", SqlDbType.Int) { Value = NewVitalSign.SpO2.HasValue ? (object)NewVitalSign.SpO2 : DBNull.Value },
                    new SqlParameter("@Weight", SqlDbType.Decimal) { Value = NewVitalSign.Weight.HasValue ? (object)NewVitalSign.Weight : DBNull.Value },
                    new SqlParameter("@Height", SqlDbType.Decimal) { Value = NewVitalSign.Height.HasValue ? (object)NewVitalSign.Height : DBNull.Value },
                    new SqlParameter("@RecordedAt", SqlDbType.DateTime2) { Value = DateTime.Now },
                    new SqlParameter("@Notes", SqlDbType.NVarChar) { Value = !string.IsNullOrEmpty(NewVitalSign.Notes) ? (object)NewVitalSign.Notes : DBNull.Value }
                };

                await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                _logger.LogInformation($"Successfully saved vital signs for patient {NewVitalSign.PatientId}");
                TempData["SuccessMessage"] = "Vital signs recorded successfully!";
                
                await LoadPatientsAsync();
                await LoadVitalSignsAsync();

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vital signs: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error saving vital signs: {ex.Message}");
                await LoadPatientsAsync();
                await LoadVitalSignsAsync();
                return Page();
            }
        }
    }
}
