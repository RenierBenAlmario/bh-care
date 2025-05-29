using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Security.Claims;

namespace Barangay.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
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

                var prescription = new Prescription
                {
                    PatientId = request.PatientId,
                    DoctorId = doctorId,
                    Diagnosis = request.Diagnosis,
                    Duration = request.Duration,
                    Notes = request.Notes,
                    Status = PrescriptionStatus.Created,
                    CreatedAt = DateTime.UtcNow,
                    PrescriptionDate = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                // Add medicines
                foreach (var medicine in request.Medicines)
                {
                    var prescriptionMedicine = new PrescriptionMedicine
                    {
                        PrescriptionId = prescription.Id,
                        MedicationName = medicine.Name,
                        Dosage = medicine.Dosage,
                        Unit = medicine.Unit,
                        Frequency = medicine.Frequency
                    };
                    _context.PrescriptionMedicines.Add(prescriptionMedicine);
                }

                await _context.SaveChangesAsync();

                return Ok(new { success = true, prescriptionId = prescription.Id });
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
} 