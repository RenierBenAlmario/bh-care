using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.Doctor.Prescriptions
{
    [Authorize(Roles = "Doctor")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public class PrescriptionViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public string DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Diagnosis { get; set; }
            public int Duration { get; set; }
            public string Notes { get; set; }
            public PrescriptionStatus Status { get; set; }
            public DateTime PrescriptionDate { get; set; }
            public DateTime? CancelledAt { get; set; }
            public DateTime? ValidUntil { get; set; }
        }

        public PrescriptionViewModel Prescription { get; set; }
        public List<PrescriptionMedication> PrescriptionMedicines { get; set; } = new List<PrescriptionMedication>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            Prescription = new PrescriptionViewModel
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                PatientName = prescription.Patient?.FullName ?? "Unknown Patient",
                DoctorId = prescription.DoctorId,
                DoctorName = prescription.Doctor?.FullName ?? "Unknown Doctor",
                Diagnosis = prescription.Diagnosis,
                Duration = prescription.Duration,
                Notes = prescription.Notes,
                Status = prescription.Status,
                PrescriptionDate = prescription.PrescriptionDate,
                CancelledAt = prescription.CancelledAt,
                ValidUntil = prescription.ValidUntil ?? prescription.PrescriptionDate.AddDays(prescription.Duration)
            };

            PrescriptionMedicines = prescription.PrescriptionMedicines.ToList();

            return Page();
        }

        public string GetStatusBadgeColor(PrescriptionStatus status)
        {
            return status switch
            {
                PrescriptionStatus.Created => "primary",
                PrescriptionStatus.Filled => "success",
                PrescriptionStatus.Completed => "info",
                PrescriptionStatus.Cancelled => "danger",
                PrescriptionStatus.Pending => "warning",
                PrescriptionStatus.Dispensed => "secondary",
                _ => "secondary"
            };
        }
    }
} 