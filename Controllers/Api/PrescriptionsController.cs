using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Security.Claims;
using Barangay.Services;

namespace Barangay.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPrescriptionPdfService _prescriptionPdfService;

        public PrescriptionsController(ApplicationDbContext context, IPrescriptionPdfService prescriptionPdfService)
        {
            _context = context;
            _prescriptionPdfService = prescriptionPdfService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreatePrescriptionRequest request)
        {
            try
            {
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { success = false, message = "Doctor not authenticated" });
                }

                var now = DateTime.UtcNow;
                var prescription = new Prescription
                {
                    PatientId = request.PatientId,
                    DoctorId = doctorId,
                    Diagnosis = request.Diagnosis,
                    Duration = request.Duration,
                    Notes = request.Notes,
                    Status = PrescriptionStatus.Created,
                    CreatedAt = now,
                    PrescriptionDate = now,
                    UpdatedAt = now,
                    // Persist the validity date so it's stored and available for printing
                    ValidUntil = now.AddDays(request.Duration)
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                // Add medicines
                if (request.Medicines != null && request.Medicines.Any())
                {
                    var medicationNames = request.Medicines.Select(m => m.Name.ToLower()).Distinct().ToList();
                    var existingMedications = await _context.Medications
                        .Where(m => medicationNames.Contains(m.Name.ToLower()))
                        .ToDictionaryAsync(m => m.Name.ToLower());

                    foreach (var medRequest in request.Medicines)
                    {
                        if (string.IsNullOrWhiteSpace(medRequest.Name)) continue;

                        if (!existingMedications.TryGetValue(medRequest.Name.ToLower(), out var medication))
                        {
                            medication = new Medication { Name = medRequest.Name };
                            _context.Medications.Add(medication);
                            // Defer SaveChanges to the end to get all new IDs in one go
                        }

                        var prescriptionMedication = new PrescriptionMedication
                        {
                            PrescriptionId = prescription.Id,
                            Medication = medication, // Assign navigation property
                            MedicationName = medication.Name, // Persist name for reporting/printing and to satisfy [Required]
                            Dosage = medRequest.Dosage.ToString(),
                            Unit = medRequest.Unit,
                            Frequency = medRequest.Frequency
                        };
                        _context.PrescriptionMedications.Add(prescriptionMedication);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, prescriptionId = prescription.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                        .ThenInclude(patient => patient.User)
                    .Include(p => p.Doctor)
                    .Include(p => p.PrescriptionMedicines)
                        .ThenInclude(pm => pm.Medication)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (prescription == null)
                {
                    return NotFound(new { success = false, message = "Prescription not found" });
                }

                var result = new
                {
                    success = true,
                    prescription = new
                    {
                        id = prescription.Id,
                        diagnosis = prescription.Diagnosis,
                        notes = prescription.Notes,
                        prescriptionDate = prescription.PrescriptionDate,
                        patient = prescription.Patient != null ? new
                        {
                            firstName = prescription.Patient.User?.FirstName,
                            lastName = prescription.Patient.User?.LastName,
                            birthDate = prescription.Patient.BirthDate
                        } : null,
                        doctor = prescription.Doctor != null ? new
                        {
                            firstName = prescription.Doctor.FirstName,
                            lastName = prescription.Doctor.LastName
                        } : null
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("generate-pdf")]
        public async Task<IActionResult> GeneratePrescriptionPdf([FromBody] GeneratePrescriptionPdfRequest request)
        {
            try
            {
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { success = false, message = "Doctor not authenticated" });
                }

                // Get doctor information
                var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Id == doctorId);
                var doctorName = doctor != null ? $"{doctor.FirstName} {doctor.LastName}" : "Unknown Doctor";

                // Get patient information
                var patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == request.PatientId);
                var patientName = patient?.User != null 
                    ? $"{patient.User.FirstName} {patient.User.LastName}"
                    : "Unknown Patient";

                // Create a temporary prescription object for PDF generation
                var prescription = new Prescription
                {
                    PatientId = request.PatientId,
                    DoctorId = doctorId,
                    Diagnosis = request.Diagnosis,
                    Notes = request.Notes,
                    Duration = request.Duration,
                    PrescriptionDate = DateTime.UtcNow,
                    ValidUntil = DateTime.UtcNow.AddDays(request.Duration),
                    Status = PrescriptionStatus.Created
                };

                var pdfBytes = await _prescriptionPdfService.GeneratePrescriptionPdfAsync(prescription, patientName, doctorName);

                var fileName = $"prescription_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            try
            {
                var prescription = await _context.Prescriptions
                    .Include(p => p.Patient)
                        .ThenInclude(patient => patient.User)
                    .Include(p => p.Doctor)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (prescription == null)
                {
                    return NotFound(new { success = false, message = "Prescription not found" });
                }

                var patientName = prescription.Patient?.User != null 
                    ? $"{prescription.Patient.User.FirstName} {prescription.Patient.User.LastName}"
                    : "Unknown Patient";

                var doctorName = prescription.Doctor != null 
                    ? $"{prescription.Doctor.FirstName} {prescription.Doctor.LastName}"
                    : "Unknown Doctor";

                var pdfBytes = await _prescriptionPdfService.GeneratePrescriptionPdfAsync(prescription, patientName, doctorName);

                var fileName = $"prescription_{prescription.Id}_{prescription.PrescriptionDate:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { success = false, message = "Doctor not authenticated" });
                }

                var prescription = await _context.Prescriptions
                    .FirstOrDefaultAsync(p => p.Id == id && p.DoctorId == doctorId);

                if (prescription == null)
                {
                    return NotFound(new { success = false, message = "Prescription not found" });
                }

                if (prescription.Status != PrescriptionStatus.Created && prescription.Status != PrescriptionStatus.Pending)
                {
                    return BadRequest(new { success = false, message = "Only created or pending prescriptions can be cancelled" });
                }

                prescription.Status = PrescriptionStatus.Cancelled;
                prescription.CancelledAt = DateTime.UtcNow;
                prescription.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    public class CreatePrescriptionRequest
    {
        public string PatientId { get; set; }
        public string Diagnosis { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
        public List<PrescriptionMedicineRequest> Medicines { get; set; }
    }

    public class PrescriptionMedicineRequest
    {
        public string Name { get; set; }
        public decimal Dosage { get; set; }
        public string Unit { get; set; }
        public string Frequency { get; set; }
    }

    public class GeneratePrescriptionPdfRequest
    {
        public string PatientId { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public int Duration { get; set; } = 7;
    }
} 