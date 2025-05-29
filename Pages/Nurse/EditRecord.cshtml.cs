using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Pages.Nurse
{
    public class EditRecordModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EditRecordModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Appointment? Appointment { get; set; }

        [BindProperty]
        public VitalSignsInputModel VitalSigns { get; set; } = new();

        public class VitalSignsInputModel
        {
            public string? Temperature { get; set; }
            public string? BloodPressure { get; set; }
            public string? HeartRate { get; set; }
            public string? RespiratoryRate { get; set; }
            public string? SpO2 { get; set; }
            public string? Weight { get; set; }
            public string? Height { get; set; }
        }

        public class PatientRecordEditModel
        {
            public string Id { get; set; } = string.Empty;
            public string? Name { get; set; }
            public string? MedicalRecordNo { get; set; }
            public string? Room { get; set; }
            public string? Status { get; set; }
            public string? Allergies { get; set; }
            public string? Diagnosis { get; set; }
            public string? Alert { get; set; }
        }

        [BindProperty]
        public PatientRecordEditModel PatientRecord { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Appointment == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == Appointment.PatientId);

            if (patient == null)
            {
                return NotFound();
            }

            PatientRecord = new PatientRecordEditModel
            {
                Id = patient.UserId,
                Name = patient.Name,
                MedicalRecordNo = $"P{patient.UserId.GetHashCode():D6}",
                Room = patient.Room,
                Status = patient.Status,
                Allergies = patient.Allergies,
                Diagnosis = patient.Diagnosis,
                Alert = patient.Alert
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Appointment == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == Appointment.PatientId);

            if (patient == null)
            {
                return NotFound();
            }

            // Update patient record
            patient.Status = PatientRecord.Status;
            patient.Allergies = PatientRecord.Allergies;
           patient.Diagnosis = PatientRecord.Diagnosis;
            patient.Alert = PatientRecord.Alert;

            // Update vital signs with proper parsing
            // Update VitalSignsLegacy to VitalSigns and fix model reference
            var vitalSign = new Barangay.Models.VitalSign  // Fix model name
            {
                PatientId = patient.UserId,  // Use UserId instead of Id
                Temperature = decimal.TryParse(VitalSigns.Temperature, out var temp) ? temp : 0m,
                BloodPressure = VitalSigns.BloodPressure ?? string.Empty,
                HeartRate = int.TryParse(VitalSigns.HeartRate, out var hr) ? hr : 0,
                RespiratoryRate = int.TryParse(VitalSigns.RespiratoryRate, out var rr) ? rr : 0,
                SpO2 = int.TryParse(VitalSigns.SpO2, out var spo2) ? spo2 : 0,
                Weight = decimal.TryParse(VitalSigns.Weight, out var weight) ? weight : 0m,
                Height = decimal.TryParse(VitalSigns.Height, out var height) ? height : 0m,
                RecordedAt = DateTime.Now,
                Notes = Request.Form["Notes"].ToString() ?? string.Empty
            };
            
            // Remove duplicate SaveChangesAsync call
            _context.VitalSigns.Add(vitalSign);
            await _context.SaveChangesAsync();
            
            try
            {
                // Remove the duplicate SaveChangesAsync here
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(Appointment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}