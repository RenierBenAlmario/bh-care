using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Barangay.Helpers;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Admin,Nurse")]
    public class NewAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewAppointmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public NewAppointmentModel(ApplicationDbContext context, ILogger<NewAppointmentModel> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public AppointmentInputModel Appointment { get; set; } = new AppointmentInputModel();

        public List<SelectListItem> PatientSelectList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> DoctorSelectList { get; set; } = new List<SelectListItem>();
        public string? SelectedPatientId { get; set; }

        public async Task OnGetAsync(string? patientId = null)
        {
            SelectedPatientId = patientId;
            
            // If a patient ID is provided, set it as the selected patient
            if (!string.IsNullOrEmpty(patientId))
            {
                Appointment.PatientId = patientId;
                
                // Get patient info to pre-fill
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == patientId);
                if (patient != null)
                {
                    TempData["Info"] = $"Scheduling appointment for {patient.FullName}";
                }
            }

            // Set default appointment date to tomorrow
            Appointment.AppointmentDate = DateTimeHelper.ToDateString(DateTime.Today.AddDays(1));
            
            await LoadPatientsAsync();
            await LoadDoctorsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            try
            {
                // Use the Patients table directly instead of joining with Users
                var patients = await _context.Patients
                    .OrderBy(p => p.FullName)
                    .ToListAsync();

                PatientSelectList = patients.Select(p => new SelectListItem
                {
                    Value = p.UserId,
                    Text = p.FullName
                }).ToList();
                
                _logger.LogInformation("Successfully loaded {Count} patients", patients.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patients");
                PatientSelectList = new List<SelectListItem>();
            }
        }

        private async Task LoadDoctorsAsync()
        {
            try
            {
                // Use AspNetUsers directly through UserManager instead of Users table
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                
                DoctorSelectList = doctors.Select(d => new SelectListItem
                {
                    Value = d.Id,
                    Text = d.FullName ?? d.UserName ?? d.Email
                }).OrderBy(d => d.Text).ToList();
                
                _logger.LogInformation("Successfully loaded {Count} doctors", doctors.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading doctors");
                DoctorSelectList = new List<SelectListItem>();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadPatientsAsync();
                await LoadDoctorsAsync();
                return Page();
            }

            try
            {
                // Get patient name for the appointment
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == Appointment.PatientId);

                if (patient == null)
                {
                    ModelState.AddModelError("Appointment.PatientId", "Selected patient not found.");
                    await LoadPatientsAsync();
                    await LoadDoctorsAsync();
                    return Page();
                }

                // Convert time string to TimeSpan
                if (!TimeSpan.TryParse(Appointment.AppointmentTimeString, out TimeSpan appointmentTime))
                {
                    ModelState.AddModelError("Appointment.AppointmentTimeString", "Invalid time format.");
                    await LoadPatientsAsync();
                    await LoadDoctorsAsync();
                    return Page();
                }

                // Parse appointment date
                var parsedDate = DateTimeHelper.ParseDate(Appointment.AppointmentDate);
                if (DateTimeHelper.IsDateEqual(parsedDate, DateTime.MinValue))
                {
                    ModelState.AddModelError("Appointment.AppointmentDate", "Invalid date format.");
                    await LoadPatientsAsync();
                    await LoadDoctorsAsync();
                    return Page();
                }

                // Create new appointment
                var newAppointment = new Barangay.Models.Appointment
                {
                    PatientId = Appointment.PatientId,
                    PatientName = patient.FullName ?? patient.Name ?? Appointment.PatientId,
                    DoctorId = Appointment.DoctorId,
                    AppointmentDate = parsedDate,
                    AppointmentTime = appointmentTime,
                    ReasonForVisit = Appointment.ReasonForVisit,
                    Description = Appointment.Description,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    // Add ApplicationUserId for the doctor
                    ApplicationUserId = Appointment.DoctorId
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("New appointment created: {AppointmentId}, Patient: {PatientName}, Date: {Date}",
                    newAppointment.Id, patient.FullName, newAppointment.AppointmentDate);

                TempData["SuccessMessage"] = "Appointment scheduled successfully!";
                return RedirectToPage("/Nurse/PatientQueue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling appointment");
                ModelState.AddModelError(string.Empty, $"Error scheduling appointment: {ex.Message}");
                await LoadPatientsAsync();
                await LoadDoctorsAsync();
                return Page();
            }
        }
    }

    public class AppointmentInputModel
    {
        [Required(ErrorMessage = "Patient is required")]
        public string PatientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doctor is required")]
        public string DoctorId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        public string AppointmentDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment time is required")]
        public string AppointmentTimeString { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reason for visit is required")]
        public string ReasonForVisit { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
} 