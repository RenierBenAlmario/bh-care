using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class VitalSignsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<VitalSignsModel> _logger;
        private readonly string _connectionString;
        private readonly IDataEncryptionService _encryptionService;

        public VitalSignsModel(EncryptedDbContext context, ILogger<VitalSignsModel> logger, IConfiguration configuration, IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "Connection string not found");
            _encryptionService = encryptionService;
        }

        public class VitalSignViewModel
        {
            public int Id { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public DateTime RecordedAt { get; set; }
            public string? BloodPressure { get; set; }
            public string? HeartRate { get; set; }
            public string? Temperature { get; set; }
            public string? RespiratoryRate { get; set; }
            public string? SpO2 { get; set; }
            public string? Weight { get; set; }
            public string? Height { get; set; }
            public string? Notes { get; set; }
        }

        public List<VitalSignViewModel> VitalSignRecords { get; set; } = new();
        public List<Patient> Patients { get; set; } = new();
        public Patient? SelectedPatient { get; set; }
        public VitalSignViewModel? LatestVitalSigns { get; set; }

        public async Task OnGetAsync(string? patientId = null)
        {
            try
            {
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"Doctor ID: {doctorId}");
                
                // Get distinct patients from appointments
                Patients = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && !string.IsNullOrEmpty(a.PatientId))
                    .Select(a => new Patient 
                    {
                        UserId = a.PatientId,
                        Name = a.PatientName
                    })
                    .Distinct()
                    .ToListAsync();
                    
                _logger.LogInformation($"Found {Patients.Count} patients for dropdown");

                if (!string.IsNullOrEmpty(patientId))
                {
                    _logger.LogInformation($"Looking for patient with ID: {patientId}");
                    
                    SelectedPatient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.UserId == patientId);
                    
                    if (SelectedPatient != null)
                    {
                        _logger.LogInformation($"Found patient: {SelectedPatient.Name} with ID: {SelectedPatient.UserId}");
                        await LoadVitalSignsAsync(0); // The parameter won't be used
                        LatestVitalSigns = VitalSignRecords.FirstOrDefault();
                        
                        if (LatestVitalSigns == null)
                        {
                            _logger.LogInformation("No vital signs found for this patient");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Patient with ID {patientId} not found");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading vital signs: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, "Error loading vital signs. Please try again.");
            }
        }

        private async Task LoadVitalSignsAsync(int patientId)
        {
            VitalSignRecords = new List<VitalSignViewModel>();
            
            try
            {
                // First, let's check if there are any vital signs in the database at all
                var totalVitalSigns = await _context.VitalSigns.CountAsync();
                _logger.LogInformation($"Total vital signs in database: {totalVitalSigns}");
                
                // Now let's check if the patient ID is correct
                _logger.LogInformation($"Looking for vital signs with PatientId: '{SelectedPatient.UserId}'");
                
                // Let's use Entity Framework instead of raw SQL for better type safety
                var vitalSigns = await _context.VitalSigns
                    .Where(v => v.PatientId == SelectedPatient.UserId)
                    .OrderByDescending(v => v.RecordedAt)
                    .ToListAsync();
                    
                _logger.LogInformation($"Query executed for patient {SelectedPatient.UserId}, found {vitalSigns.Count} records");
                
                // If we didn't find any records, let's try to debug why
                if (vitalSigns.Count == 0)
                {
                    // Let's check if there are any vital signs with any patient ID
                    var anyVitalSigns = await _context.VitalSigns.FirstOrDefaultAsync();
                    if (anyVitalSigns != null)
                    {
                        _logger.LogInformation($"Found a vital sign with PatientId: '{anyVitalSigns.PatientId}'");
                    }
                }
                    
                foreach (var vs in vitalSigns)
                {
                    VitalSignRecords.Add(MapToViewModel(vs));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading vital signs for patient {SelectedPatient?.UserId}: {ex.Message}");
            }
        }

        private VitalSignViewModel MapToViewModel(VitalSign vs)
        {
            return new VitalSignViewModel
            {
                Id = vs.Id,
                PatientName = vs.Patient?.Name ?? "Unknown",
                RecordedAt = vs.RecordedAt,
                BloodPressure = vs.BloodPressure,
                HeartRate = vs.HeartRate?.ToString(),
                Temperature = vs.Temperature?.ToString(),
                RespiratoryRate = vs.RespiratoryRate?.ToString(),
                SpO2 = vs.SpO2?.ToString(),
                Weight = vs.Weight?.ToString(),
                Height = vs.Height?.ToString(),
                Notes = vs.Notes
            };
        }

        [BindProperty]
        public VitalSignViewModel NewVitalSign { get; set; } = new();

        public async Task<IActionResult> OnPostAddVitalSignAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadPatientsAsync();
                    return Page();
                }

                // Get the selected patient ID from the form
                var patientId = NewVitalSign.PatientName; // This will contain the selected patient ID
                if (string.IsNullOrEmpty(patientId))
                {
                    ModelState.AddModelError("NewVitalSign.PatientName", "The Patient field is required.");
                    await LoadPatientsAsync();
                    return Page();
                }

                // Get the patient details
                SelectedPatient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == patientId);

                if (SelectedPatient == null)
                {
                    ModelState.AddModelError("NewVitalSign.PatientName", "Invalid patient selected.");
                    await LoadPatientsAsync();
                    return Page();
                }

                // Create new vital sign record
                var vitalSign = new VitalSign
                {
                    PatientId = patientId,
                    // Store encrypted data in encrypted columns
                    EncryptedTemperature = !string.IsNullOrEmpty(NewVitalSign.Temperature) ? _encryptionService.Encrypt(NewVitalSign.Temperature) : null,
                    EncryptedBloodPressure = !string.IsNullOrEmpty(NewVitalSign.BloodPressure) ? _encryptionService.Encrypt(NewVitalSign.BloodPressure) : null,
                    EncryptedHeartRate = !string.IsNullOrEmpty(NewVitalSign.HeartRate) ? _encryptionService.Encrypt(NewVitalSign.HeartRate) : null,
                    EncryptedRespiratoryRate = !string.IsNullOrEmpty(NewVitalSign.RespiratoryRate) ? _encryptionService.Encrypt(NewVitalSign.RespiratoryRate) : null,
                    EncryptedSpO2 = !string.IsNullOrEmpty(NewVitalSign.SpO2) ? _encryptionService.Encrypt(NewVitalSign.SpO2) : null,
                    EncryptedWeight = !string.IsNullOrEmpty(NewVitalSign.Weight) ? _encryptionService.Encrypt(NewVitalSign.Weight) : null,
                    EncryptedHeight = !string.IsNullOrEmpty(NewVitalSign.Height) ? _encryptionService.Encrypt(NewVitalSign.Height) : null,
                    RecordedAt = DateTime.Now,
                    Notes = NewVitalSign.Notes ?? string.Empty
                };

                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                // Reload the page with success message
                TempData["SuccessMessage"] = "Vital signs recorded successfully!";
                return RedirectToPage(new { patientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vital signs: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error saving vital signs: {ex.Message}");
                await LoadPatientsAsync();
                return Page();
            }
        }

        private async Task LoadPatientsAsync()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Patients = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && !string.IsNullOrEmpty(a.PatientId))
                .Select(a => new Patient 
                {
                    UserId = a.PatientId,
                    Name = a.PatientName
                })
                .Distinct()
                .ToListAsync();
        }
    }
}