using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;

namespace Barangay.Pages.Admin
{
    public class UpdatePatientNameModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public UpdatePatientNameModel(ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public int AppointmentId { get; set; }

        [BindProperty]
        public string NewPatientName { get; set; } = string.Empty;

        [BindProperty]
        public string NewDependentName { get; set; } = string.Empty;

        public Appointment? CurrentAppointment { get; set; }
        public string CurrentPatientName { get; set; } = string.Empty;
        public string CurrentDependentName { get; set; } = string.Empty;
        public string StatusMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? appointmentId = null)
        {
            if (appointmentId.HasValue)
            {
                AppointmentId = appointmentId.Value;
                await LoadAppointmentData();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (AppointmentId <= 0 || string.IsNullOrEmpty(NewPatientName))
            {
                StatusMessage = "Error: Please provide a valid appointment ID and new patient name.";
                return Page();
            }

            try
            {
                // Get the appointment
                var appointment = await _context.Appointments.FindAsync(AppointmentId);
                if (appointment == null)
                {
                    StatusMessage = "Error: Appointment not found.";
                    return Page();
                }

                // Decrypt current names to show what we're changing from
                CurrentPatientName = !string.IsNullOrEmpty(appointment.PatientName) && _encryptionService.IsEncrypted(appointment.PatientName)
                    ? _encryptionService.DecryptForUser(appointment.PatientName, User)
                    : appointment.PatientName;

                CurrentDependentName = !string.IsNullOrEmpty(appointment.DependentFullName) && _encryptionService.IsEncrypted(appointment.DependentFullName)
                    ? _encryptionService.DecryptForUser(appointment.DependentFullName, User)
                    : appointment.DependentFullName ?? string.Empty;

                // Encrypt and update the patient name
                appointment.PatientName = _encryptionService.Encrypt(NewPatientName);

                // Update dependent name if provided
                if (!string.IsNullOrEmpty(NewDependentName))
                {
                    appointment.DependentFullName = _encryptionService.Encrypt(NewDependentName);
                }

                // Save changes
                await _context.SaveChangesAsync();

                StatusMessage = $"Success: Patient name updated from '{CurrentPatientName}' to '{NewPatientName}' for appointment {AppointmentId}.";
                
                // Reload data to show updated information
                await LoadAppointmentData();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: Failed to update patient name. {ex.Message}";
            }

            return Page();
        }

        private async Task LoadAppointmentData()
        {
            if (AppointmentId <= 0) return;

            CurrentAppointment = await _context.Appointments.FindAsync(AppointmentId);
            if (CurrentAppointment != null)
            {
                // Decrypt current names for display
                CurrentPatientName = !string.IsNullOrEmpty(CurrentAppointment.PatientName) && _encryptionService.IsEncrypted(CurrentAppointment.PatientName)
                    ? _encryptionService.DecryptForUser(CurrentAppointment.PatientName, User)
                    : CurrentAppointment.PatientName;

                CurrentDependentName = !string.IsNullOrEmpty(CurrentAppointment.DependentFullName) && _encryptionService.IsEncrypted(CurrentAppointment.DependentFullName)
                    ? _encryptionService.DecryptForUser(CurrentAppointment.DependentFullName, User)
                    : CurrentAppointment.DependentFullName ?? string.Empty;
            }
        }
    }
}
