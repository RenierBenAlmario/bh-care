using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Models;
using Barangay.Data;

namespace Barangay.Pages.Records
{
    [Authorize(Roles = "Admin,Doctor,Nurse,User")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public string BloodType { get; set; } = "O+";
        public double Height { get; set; } = 170;
        public double Weight { get; set; } = 70;
        public double BMI { get; set; } = 24.2;
        public DateTime LastCheckup { get; set; } = DateTime.Now.AddDays(-30);
        public bool IsNurse { get; set; }
        public bool IsDoctor { get; set; }
        public bool IsPatient { get; set; }
        public string CurrentUserId { get; set; }
        public string PatientName { get; set; }
        public List<Patient> Patients { get; set; } = new List<Patient>();

        public class VitalSign
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string BloodPressure { get; set; }
            public int HeartRate { get; set; }
            public double Temperature { get; set; }
            public int RespiratoryRate { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
        }

        public class MedicalHistoryRecord
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string Doctor { get; set; }
            public string Diagnosis { get; set; }
            public string Treatment { get; set; }
        }

        public class LabResult
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string TestName { get; set; }
            public string Result { get; set; }
            public string ReferenceRange { get; set; }
            public string Status { get; set; }
        }

        public List<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
        public List<MedicalHistoryRecord> MedicalHistory { get; set; } = new List<MedicalHistoryRecord>();
        public List<LabResult> LabResults { get; set; } = new List<LabResult>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login");
                }

                // Determine user role
                IsNurse = await _userManager.IsInRoleAsync(user, "Nurse");
                IsDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
                IsPatient = await _userManager.IsInRoleAsync(user, "User");
                CurrentUserId = user.Id;

                // If the user is a Patient, only show their own records
                if (IsPatient)
                {
                    // Get patient information
                    var patient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.UserId == CurrentUserId);

                    if (patient == null)
                    {
                        _logger.LogWarning($"Patient record not found for user ID {CurrentUserId}");
                        return Page();
                    }

                    PatientName = patient.FullName;

                    // Get vital signs for this patient
                    await LoadPatientVitalSigns(patient.UserId);

                    // Get medical history for this patient
                    await LoadPatientMedicalHistory(patient.UserId);

                    // Get lab results for this patient
                    await LoadPatientLabResults(patient.UserId);
                }
                // If the user is a Nurse or Doctor, show all patient records
                else if (IsNurse || IsDoctor)
                {
                    // Get all patients for dropdown
                    Patients = await _context.Patients
                        .OrderBy(p => p.FullName)
                        .ToListAsync();

                    // Load all vital signs
                    await LoadAllVitalSigns();

                    // Load all medical history
                    await LoadAllMedicalHistory();

                    // Load all lab results
                    await LoadAllLabResults();
                }
                else
                {
                    // Redirect users without proper roles
                    return RedirectToPage("/Account/Login");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading medical records");
                return Page();
            }
        }

        private async Task LoadPatientVitalSigns(string patientId)
        {
            var vitalSigns = await _context.VitalSigns
                .Where(v => v.PatientId == patientId)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();

            VitalSigns = vitalSigns.Select(v => new VitalSign
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = PatientName,
                Date = v.RecordedAt,
                BloodPressure = v.BloodPressure,
                HeartRate = v.HeartRate ?? 0,
                Temperature = (double)(v.Temperature ?? 0),
                RespiratoryRate = v.RespiratoryRate ?? 0,
                Weight = (double?)(v.Weight),
                Height = (double?)(v.Height)
            }).ToList();
        }

        private async Task LoadAllVitalSigns()
        {
            var vitalSigns = await _context.VitalSigns
                .Include(v => v.Patient)
                .OrderByDescending(v => v.RecordedAt)
                .ToListAsync();

            VitalSigns = vitalSigns.Select(v => new VitalSign
            {
                Id = v.Id,
                PatientId = v.PatientId,
                PatientName = v.Patient?.FullName ?? "Unknown",
                Date = v.RecordedAt,
                BloodPressure = v.BloodPressure,
                HeartRate = v.HeartRate ?? 0,
                Temperature = (double)(v.Temperature ?? 0),
                RespiratoryRate = v.RespiratoryRate ?? 0,
                Weight = (double?)(v.Weight),
                Height = (double?)(v.Height)
            }).ToList();
        }

        private async Task LoadPatientMedicalHistory(string patientId)
        {
            var medicalRecords = await _context.MedicalRecords
                .Where(m => m.PatientId == patientId)
                .Include(m => m.Doctor)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            MedicalHistory = medicalRecords.Select(m => new MedicalHistoryRecord
            {
                Id = m.Id,
                PatientId = m.PatientId,
                PatientName = PatientName,
                Date = m.Date,
                Doctor = m.Doctor?.FullName ?? "Unknown",
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment
            }).ToList();
        }

        private async Task LoadAllMedicalHistory()
        {
            var medicalRecords = await _context.MedicalRecords
                .Include(m => m.Patient)
                .Include(m => m.Doctor)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            MedicalHistory = medicalRecords.Select(m => new MedicalHistoryRecord
            {
                Id = m.Id,
                PatientId = m.PatientId,
                PatientName = m.Patient?.FullName ?? "Unknown",
                Date = m.Date,
                Doctor = m.Doctor?.FullName ?? "Unknown",
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment
            }).ToList();
        }

        private async Task LoadPatientLabResults(string patientId)
        {
            // For demo purposes, creating sample lab results
            // In a real implementation, you'd query a LabResults table
            LabResults = new List<LabResult>
            {
                new LabResult
                {
                    Id = 1,
                    PatientId = patientId,
                    PatientName = PatientName,
                    Date = DateTime.Now.AddDays(-30),
                    TestName = "Blood Glucose",
                    Result = "95 mg/dL",
                    ReferenceRange = "70-100 mg/dL (fasting)",
                    Status = "Normal"
                },
                new LabResult
                {
                    Id = 2,
                    PatientId = patientId,
                    PatientName = PatientName,
                    Date = DateTime.Now.AddDays(-30),
                    TestName = "Cholesterol",
                    Result = "180 mg/dL",
                    ReferenceRange = "<200 mg/dL",
                    Status = "Normal"
                }
            };
        }

        private async Task LoadAllLabResults()
        {
            // In a real implementation, you'd query a LabResults table
            // Here we're generating sample data for all patients
            LabResults = new List<LabResult>();

            foreach (var patient in Patients)
            {
                LabResults.Add(new LabResult
                {
                    Id = LabResults.Count + 1,
                    PatientId = patient.UserId,
                    PatientName = patient.FullName,
                    Date = DateTime.Now.AddDays(-30),
                    TestName = "Blood Glucose",
                    Result = "95 mg/dL",
                    ReferenceRange = "70-100 mg/dL (fasting)",
                    Status = "Normal"
                });

                LabResults.Add(new LabResult
                {
                    Id = LabResults.Count + 1,
                    PatientId = patient.UserId,
                    PatientName = patient.FullName,
                    Date = DateTime.Now.AddDays(-60),
                    TestName = "Cholesterol",
                    Result = "180 mg/dL",
                    ReferenceRange = "<200 mg/dL",
                    Status = "Normal"
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var record = await _context.MedicalRecords.FindAsync(id);
                if (record == null)
                {
                    return NotFound();
                }

                _context.MedicalRecords.Remove(record);
                await _context.SaveChangesAsync();
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical record");
                return RedirectToPage("./Error");
            }
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            try
            {
                var records = await _context.MedicalRecords
                    .Include(r => r.Patient)
                    .Include(r => r.Doctor)
                    .ToListAsync();

                // Export logic here
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting medical records");
                return RedirectToPage("./Error");
            }
        }
    }
} 