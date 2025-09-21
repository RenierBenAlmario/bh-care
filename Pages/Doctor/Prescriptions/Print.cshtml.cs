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
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.Doctor.Prescriptions
{
    [Authorize(Roles = "Doctor")]
    public class PrintModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public PrintModel(ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public class PrescriptionViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public int? PatientAge { get; set; }
            public string DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Diagnosis { get; set; }
            public int Duration { get; set; }
            public string Notes { get; set; }
            public PrescriptionStatus Status { get; set; }
            public DateTime PrescriptionDate { get; set; }
            public DateTime? ValidUntil { get; set; }
        }

        public PrescriptionViewModel Prescription { get; set; }
        public List<PrescriptionMedication> PrescriptionMedicines { get; set; } = new List<PrescriptionMedication>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.Patient)
                    .ThenInclude(p => p.User)
                .Include(p => p.Doctor)
                .Include(p => p.PrescriptionMedicines)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
            {
                return NotFound();
            }

            // Decrypt prescription data for display
            prescription.DecryptSensitiveData(_encryptionService, User);

            // Get patient's full name and age from User model
            var patientName = prescription.Patient?.User?.FullName ?? "Unknown Patient";
            int? patientAge = null;
            if (prescription.Patient?.User != null)
            {
                DateTime birthDate = DateTime.TryParse(prescription.Patient.User.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue;
                patientAge = CalculateAge(birthDate);
            }

            Prescription = new PrescriptionViewModel
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                PatientName = patientName,
                PatientAge = patientAge,
                DoctorId = prescription.DoctorId,
                DoctorName = prescription.Doctor?.FullName ?? "Unknown Doctor",
                Diagnosis = prescription.Diagnosis,
                Duration = prescription.Duration,
                Notes = prescription.Notes,
                Status = prescription.Status,
                PrescriptionDate = prescription.PrescriptionDate,
                ValidUntil = prescription.ValidUntil ?? prescription.PrescriptionDate.AddDays(prescription.Duration)
            };

            PrescriptionMedicines = prescription.PrescriptionMedicines.ToList();

            return Page();
        }

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}