using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "VitalSigns")]
    public class VitalSignsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<VitalSignsModel> _logger;
        private readonly IDataEncryptionService _encryptionService;

        public VitalSignsModel(EncryptedDbContext context, ILogger<VitalSignsModel> logger, IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        public class VitalSignViewModel
        {
            public int Id { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public string PatientId { get; set; } = string.Empty;
            public DateTime RecordedAt { get; set; } = DateTime.Now;
            public string? BloodPressure { get; set; }
            public string? HeartRate { get; set; }
            public string? Temperature { get; set; }
            public string? RespiratoryRate { get; set; }
            public string? SpO2 { get; set; }
            public string? Weight { get; set; }
            public string? Height { get; set; }
            public string? Notes { get; set; }
            public Patient Patient { get; set; } // Add this property
        }

        // New class for patient appointments
        public class PatientAppointmentViewModel
        {
            public int Id { get; set; }
            public DateTime AppointmentDate { get; set; }
            public TimeSpan AppointmentTime { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public AppointmentStatus Status { get; set; }
            public string Description { get; set; } = string.Empty;
        }
        
        // New class for today's appointments
        public class TodayAppointmentViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public TimeSpan AppointmentTime { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public AppointmentStatus Status { get; set; }
        }

        public class DoctorViewModel
        {
            public string Id { get; set; }
            public string FullName { get; set; }
        }

        [BindProperty]
        public VitalSignViewModel NewVitalSign { get; set; } = new();

        public List<VitalSignViewModel> VitalSignRecords { get; set; } = new();
        public List<SelectListItem> PatientSelectList { get; set; } = new List<SelectListItem>();
        public List<DoctorViewModel> Doctors { get; set; } = new();
        public string? SelectedPatientId { get; set; }
        public Patient? SelectedPatient { get; set; }
        public List<PatientAppointmentViewModel> PatientAppointments { get; set; } = new();
        public List<TodayAppointmentViewModel> TodayAppointments { get; set; } = new();
        public DateTime Today { get; set; } = DateTime.Today;
        public bool HasTodayAppointments => TodayAppointments.Any();

        // Additional info for Patient Information panel
        public string? SelectedPatientBarangay { get; set; }
        public int SelectedPatientAge { get; set; }
        public string FilledFormType { get; set; } = string.Empty; // "NCD Risk Assessment" or "HEEADSSS Assessment"
        public bool IsFormFilled { get; set; }

        public async Task<IActionResult> OnGetAsync(string patientId)
        {
            Today = DateTime.Today;
            
            // Load doctors for the dropdown
            var doctorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
            if (doctorRole != null)
            {
                var doctorUserIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == doctorRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();
                
                Doctors = await _context.Users
                    .Where(u => doctorUserIds.Contains(u.Id))
                    .Select(u => new DoctorViewModel { Id = u.Id, FullName = u.FullName })
                    .ToListAsync();
            }

            // Load today's appointments
            await LoadTodayAppointmentsAsync();
            
            // Load patients for the dropdown - prioritize patients with today's appointments
            await LoadPatientsWithTodayAppointmentsAsync();
            
            // If patientId is provided, set it as the selected patient
            if (!string.IsNullOrEmpty(patientId))
            {
                SelectedPatientId = patientId;
                SelectedPatient = await _context.Patients.FindAsync(patientId);
                // Set the selected patient in the new vital sign
                NewVitalSign.PatientId = patientId;
                // Load vital signs for this patient
                await LoadVitalSignsForPatientAsync(patientId);
                // Load appointments for this patient
                await LoadAppointmentsForPatientAsync(patientId);

                // Load Barangay from ApplicationUser and compute assessment form type
                if (SelectedPatient != null)
                {
                    // Decrypt patient data for display
                    SelectedPatient.DecryptSensitiveData(_encryptionService, User);
                    
                    var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == SelectedPatient.UserId);
                    if (appUser != null)
                    {
                        // Decrypt user data for display
                        appUser.DecryptSensitiveData(_encryptionService, User);
                        SelectedPatientBarangay = appUser?.Barangay;
                    }
                    SelectedPatientAge = SelectedPatient.Age;

                    if (SelectedPatientAge >= 20)
                    {
                        FilledFormType = "NCD Risk Assessment";
                        IsFormFilled = await _context.NCDRiskAssessments.AnyAsync(n => n.UserId == SelectedPatient.UserId);
                    }
                    else
                    {
                        FilledFormType = "HEEADSSS Assessment";
                        IsFormFilled = await _context.HEEADSSSAssessments.AnyAsync(h => h.UserId == SelectedPatient.UserId);
                    }
                }
            }
            else
            {
                // Load all vital signs if no specific patient is selected
                await LoadVitalSignsAsync();
            }
            
            return Page();
        }

        // New method to load today's appointments
        private async Task LoadTodayAppointmentsAsync()
        {
            var todayAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == Today)
                .OrderBy(a => a.AppointmentTime)
                .Include(a => a.Doctor)
                .Select(a => new TodayAppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.PatientName,
                    AppointmentTime = a.AppointmentTime,
                    DoctorName = a.Doctor != null ? a.Doctor.FullName : "Not assigned",
                    Type = a.Type ?? "General",
                    Status = a.Status
                })
                .ToListAsync();

            TodayAppointments = todayAppointments;
            
            _logger.LogInformation($"Loaded {todayAppointments.Count} appointments for today ({Today:yyyy-MM-dd})");
        }
        
        // New method to load patients with today's appointments for the dropdown
        private async Task LoadPatientsWithTodayAppointmentsAsync()
        {
            // Get patients with today's appointments
            var patientsWithTodayAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == Today &&
                       a.PatientName != "System Administrator" && 
                       a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                .Select(a => new { PatientId = a.PatientId, PatientName = a.PatientName })
                .Distinct()
                .ToListAsync();
                
            // If there are no appointments today, load patients as usual
            if (!patientsWithTodayAppointments.Any())
            {
                await LoadPatientsAsync();
                return;
            }
            
            // Set these as the patient select list
            PatientSelectList = patientsWithTodayAppointments
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = $"{p.PatientName} (Today's appointment)"
                })
                .ToList();
                
            // Also load other patients in case nurse needs to record vitals for someone else
            var otherPatients = await _context.Appointments
                .Where(a => a.AppointmentDate.Date != Today &&
                       a.PatientName != "System Administrator" && 
                       a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                .Select(a => new { PatientId = a.PatientId, PatientName = a.PatientName })
                .Distinct()
                .ToListAsync();
                
            // Add other patients to the select list
            PatientSelectList.AddRange(otherPatients
                .Where(op => !patientsWithTodayAppointments.Any(p => p.PatientId == op.PatientId))
                .Select(p => new SelectListItem
                {
                    Value = p.PatientId.ToString(),
                    Text = p.PatientName
                }));
                
            _logger.LogInformation($"Loaded {patientsWithTodayAppointments.Count} patients with today's appointments and {otherPatients.Count} other patients");
        }

        // New method to load patient appointments
        private async Task LoadAppointmentsForPatientAsync(string patientId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .Include(a => a.Doctor)
                .Select(a => new PatientAppointmentViewModel
                {
                    Id = a.Id,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    DoctorName = a.Doctor != null ? a.Doctor.FullName : "Not assigned",
                    Type = a.Type ?? "General",
                    Status = a.Status,
                    Description = a.Description
                })
                .Take(5) // Limit to latest 5 appointments
                .ToListAsync();

            PatientAppointments = appointments;
        }

        private async Task LoadPatientsAsync()
        {
            // Get all patients with appointments
            var patientsWithAppointments = await _context.Appointments
                .Where(a => a.PatientName != "System Administrator" && a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                .Select(a => new { PatientId = a.PatientId, PatientName = a.PatientName })
                .Distinct()
                .ToListAsync();

            // Get all patients who have had vital signs recorded
            var patientsWithVitalSigns = await _context.VitalSigns
                .Join(_context.Appointments.Where(a => a.PatientName != "System Administrator" && a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a"),
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
            var vitalSigns = await _context.VitalSigns
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();

            // Manually decrypt vital signs data
            foreach (var vitalSign in vitalSigns)
            {
                vitalSign.DecryptVitalSignData(_encryptionService, User);
            }

            var records = vitalSigns.Select(v => new VitalSignViewModel
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = patientName,
                RecordedAt = v.RecordedAt,
                BloodPressure = v.BloodPressure,
                HeartRate = v.HeartRate?.ToString(),
                Temperature = v.Temperature?.ToString(),
                RespiratoryRate = v.RespiratoryRate?.ToString(),
                SpO2 = v.SpO2?.ToString(),
                Weight = v.Weight?.ToString(),
                Height = v.Height?.ToString(),
                Notes = v.Notes
            }).ToList();

            VitalSignRecords = records;
        }

        private async Task LoadVitalSignsAsync()
        {
            // First get the latest appointment names for each patient
            var patientNames = await _context.Appointments
                .Where(a => a.PatientName != "System Administrator" && a.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                .GroupBy(a => a.PatientId)
                .Where(g => g.Key != null)
                .Select(g => new { PatientId = g.Key!, PatientName = g.OrderByDescending(a => a.AppointmentDate).First().PatientName })
                .ToDictionaryAsync(x => x.PatientId, x => x.PatientName);

            // Load vital signs data
            var vitalSigns = await _context.VitalSigns
                .Where(v => v.PatientId != "0e03f06e-ba88-46ed-b047-4974d8b8252a")
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();

            // Manually decrypt vital signs data
            foreach (var vitalSign in vitalSigns)
            {
                vitalSign.DecryptVitalSignData(_encryptionService, User);
            }

            var records = vitalSigns.Select(v => new VitalSignViewModel
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.PatientId, // Temporarily store ID here
                RecordedAt = v.RecordedAt,
                BloodPressure = v.BloodPressure,
                HeartRate = v.HeartRate?.ToString(),
                Temperature = v.Temperature?.ToString(),
                RespiratoryRate = v.RespiratoryRate?.ToString(),
                SpO2 = v.SpO2?.ToString(),
                Weight = v.Weight?.ToString(),
                Height = v.Height?.ToString(),
                Notes = v.Notes
            }).ToList();

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
                _logger.LogInformation("OnPostAddVitalSignAsync called with PatientId: {PatientId}", NewVitalSign.PatientId);
                
                if (string.IsNullOrEmpty(NewVitalSign.PatientId))
                {
                    ModelState.AddModelError(string.Empty, "The patientId field is required.");
                    await LoadTodayAppointmentsAsync();
                    await LoadPatientsWithTodayAppointmentsAsync();
                    return Page();
                }
                
                // Validate required fields
                if (string.IsNullOrEmpty(NewVitalSign.Temperature) || 
                    string.IsNullOrEmpty(NewVitalSign.RespiratoryRate) || 
                    string.IsNullOrEmpty(NewVitalSign.SpO2) || 
                    string.IsNullOrEmpty(NewVitalSign.Weight) || 
                    string.IsNullOrEmpty(NewVitalSign.Height))
                {
                    ModelState.AddModelError(string.Empty, "All vital sign measurements are required.");
                    await LoadTodayAppointmentsAsync();
                    await LoadPatientsWithTodayAppointmentsAsync();
                    return Page();
                }
                
                // Create new VitalSign entity
                var vitalSign = new VitalSign
                {
                    PatientId = NewVitalSign.PatientId,
                    Temperature = NewVitalSign.Temperature,
                    BloodPressure = NewVitalSign.BloodPressure,
                    HeartRate = NewVitalSign.HeartRate,
                    RespiratoryRate = NewVitalSign.RespiratoryRate,
                    SpO2 = NewVitalSign.SpO2,
                    Weight = NewVitalSign.Weight,
                    Height = NewVitalSign.Height,
                    RecordedAt = DateTime.Now,
                    Notes = NewVitalSign.Notes
                };

                // EncryptedDbContext will handle encryption automatically
                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully saved vital signs for patient {NewVitalSign.PatientId}");
                TempData["SuccessMessage"] = "Vital signs recorded successfully!";
                
                return RedirectToPage(new { patientId = NewVitalSign.PatientId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving vital signs: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error saving vital signs: {ex.Message}");
                await LoadTodayAppointmentsAsync();
                await LoadPatientsWithTodayAppointmentsAsync();
                
                if (!string.IsNullOrEmpty(NewVitalSign.PatientId))
                {
                    await LoadVitalSignsForPatientAsync(NewVitalSign.PatientId);
                    await LoadAppointmentsForPatientAsync(NewVitalSign.PatientId);
                    SelectedPatientId = NewVitalSign.PatientId;
                    SelectedPatient = await _context.Patients.FindAsync(NewVitalSign.PatientId);
                }
                else
                {
                await LoadVitalSignsAsync();
                }
                
                return Page();
            }
        }
    }
}