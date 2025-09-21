using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class AppointmentTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AppointmentTestModel> _logger;

        public AppointmentTestModel(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<AppointmentTestModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public string Message { get; set; }
        public bool IsError { get; set; }
        public Barangay.Models.Appointment CreatedAppointment { get; set; }

        public void OnGet()
        {
            // Display page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var doctorUser = await _userManager.GetUserAsync(User);
                if (doctorUser == null)
                {
                    Message = "Unable to get current user";
                    IsError = true;
                    return Page();
                }
                
                // Get a patient for the test appointment
                var patient = await _context.Users
                    .Where(u => u.Id != doctorUser.Id)
                    .FirstOrDefaultAsync();
                
                if (patient == null)
                {
                    // Create a dummy patient
                    var patientId = Guid.NewGuid().ToString();
                    
                    // Create test appointment
                    var appointment = new Barangay.Models.Appointment
                    {
                        PatientId = patientId,
                        DoctorId = doctorUser.Id,
                        PatientName = "Test Patient",
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(10, 0, 0), // 10:00 AM
                        AppointmentTimeInput = "10:00 AM",
                        ReasonForVisit = "Test Appointment",
                        Status = AppointmentStatus.Pending,
                        Description = "Test appointment created through system",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Type = "Consultation",
                        AgeValue = 35
                    };
                    
                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();
                    
                    CreatedAppointment = appointment;
                    Message = $"Test appointment created successfully with ID: {appointment.Id}";
                    _logger.LogInformation("Test appointment created with ID {AppointmentId}", appointment.Id);
                }
                else
                {
                    // Create test appointment with actual patient
                    var appointment = new Barangay.Models.Appointment
                    {
                        PatientId = patient.Id,
                        DoctorId = doctorUser.Id,
                        PatientName = $"{patient.FirstName} {patient.LastName}",
                        AppointmentDate = DateTime.Today,
                        AppointmentTime = new TimeSpan(10, 0, 0), // 10:00 AM
                        AppointmentTimeInput = "10:00 AM",
                        ReasonForVisit = "Regular Checkup",
                        Status = AppointmentStatus.Pending,
                        Description = "Appointment created for testing",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Type = "Consultation",
                        AgeValue = 35
                    };
                    
                    _context.Appointments.Add(appointment);
                    await _context.SaveChangesAsync();
                    
                    CreatedAppointment = appointment;
                    Message = $"Test appointment created successfully with ID: {appointment.Id}";
                    _logger.LogInformation("Test appointment created with ID {AppointmentId}", appointment.Id);
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                Message = $"Error creating test appointment: {ex.Message}";
                IsError = true;
                _logger.LogError(ex, "Error creating test appointment");
                return Page();
            }
        }
    }
} 