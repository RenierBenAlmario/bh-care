using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class PrintMedicalRecordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrintMedicalRecordModel> _logger;

        public PrintMedicalRecordModel(ApplicationDbContext context, ILogger<PrintMedicalRecordModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Patient? Patient { get; set; }
        public PatientDetailsModel.MedicalRecordViewModel? Record { get; set; }
        public List<PatientDetailsModel.PrescriptionMedicationViewModel> Medications { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
            {
                _logger.LogWarning("Doctor ID not found");
                return Unauthorized();
            }

            try
            {
                // Get the medical record by ID
                var medicalRecord = await _context.MedicalRecords
                    .Include(r => r.Doctor)
                    .Include(r => r.Patient)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (medicalRecord == null)
                {
                    _logger.LogWarning("Medical record not found with ID: {RecordId}", id);
                    return Page();
                }

                // Get the patient information
                if (medicalRecord.Patient != null)
                {
                    Patient = await _context.Patients
                        .FirstOrDefaultAsync(p => p.UserId == medicalRecord.PatientId);
                }

                // Map medical record to view model
                Record = new PatientDetailsModel.MedicalRecordViewModel
                {
                    Id = medicalRecord.Id,
                    Date = medicalRecord.Date,
                    Type = medicalRecord.Type ?? "Consultation",
                    ChiefComplaint = medicalRecord.ChiefComplaint,
                    Diagnosis = medicalRecord.Diagnosis,
                    Treatment = medicalRecord.Treatment,
                    Notes = medicalRecord.Notes,
                    Status = medicalRecord.Status,
                    DoctorName = medicalRecord.Doctor?.UserName ?? "Unknown"
                };

                // Get any medications associated with this medical record
                var medications = await _context.PrescriptionMedications
                    .Where(m => m.MedicalRecordId == id)
                    .ToListAsync();

                Medications = medications.Select(m => new PatientDetailsModel.PrescriptionMedicationViewModel
                {
                    Id = m.Id,
                    MedicationName = m.MedicationName,
                    Dosage = m.Dosage,
                    Instructions = m.Instructions,
                    CreatedAt = medicalRecord.Date,
                    DoctorName = medicalRecord.Doctor?.UserName ?? "Unknown"
                }).ToList();

                _logger.LogInformation("Successfully loaded medical record {RecordId} for printing", id);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading medical record {RecordId} for printing", id);
                return Page();
            }
        }
    }
} 