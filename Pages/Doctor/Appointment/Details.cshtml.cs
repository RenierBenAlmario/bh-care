using System;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace Barangay.Pages.Doctor.Appointment
{
    [Authorize(Roles = "Doctor")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Appointment? Appointment { get; set; }
        public Patient? PatientDetails { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Load appointment without including Patient navigation
            Appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id && m.DoctorId == userId);

            if (Appointment == null)
            {
                return NotFound();
            }

            // Load patient details if PatientId exists
            if (!string.IsNullOrEmpty(Appointment.PatientId))
            {
                // First try to get the patient name from the Patients table
                var patientFromDb = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == Appointment.PatientId);

                if (patientFromDb != null)
                {
                    PatientDetails = patientFromDb;
                }
                else
                {
                    // If not found in Patients table, try to get from Appointments table
                    var patientName = await _context.Appointments
                        .Where(a => a.PatientId == Appointment.PatientId && !string.IsNullOrEmpty(a.PatientName))
                        .Select(a => a.PatientName)
                        .FirstOrDefaultAsync();

                    // Create a temporary Patient object with the name from appointments
                    PatientDetails = new Patient
                    {
                        UserId = Appointment.PatientId,
                        Name = !string.IsNullOrEmpty(patientName) ? patientName : "Unknown Patient",
                        Status = "Active"
                    };
                }
            }

            // Load medical records for the patient
            MedicalRecords = await _context.MedicalRecords
                .Where(m => m.PatientId == Appointment.PatientId)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            return Page();
        }
    }
} 