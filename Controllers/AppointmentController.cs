using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Barangay.Extensions;
using Barangay.Services;
using Barangay.Helpers;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Hosting;
using Barangay.Models.Appointments;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AppointmentController> _logger;
        private readonly IAppointmentService _appointmentService;
        private readonly IWebHostEnvironment _env;

        public AppointmentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<AppointmentController> logger,
            IAppointmentService appointmentService,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _appointmentService = appointmentService;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => isDoctor ? a.DoctorId == user.Id : a.PatientId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appointmentDate = DateTimeHelper.ParseDate(model.AppointmentDate);
                if (appointmentDate == DateTime.MinValue)
                {
                    return BadRequest("Invalid appointment date format");
                }

                var appointmentTime = DateTimeHelper.ParseTime(model.AppointmentTime);
                if (appointmentTime == TimeSpan.Zero)
                {
                    return BadRequest("Invalid appointment time format");
                }

                // Get the current user ID correctly
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                var appointment = new Appointment
                {
                    DoctorId = model.DoctorId,
                    PatientId = userId,  // Use the actual user ID, not the name
                    PatientName = user.FullName,
                    AppointmentDate = appointmentDate,
                    AppointmentTime = appointmentTime,
                    Description = model.Description,
                    ReasonForVisit = model.Description ?? string.Empty,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Load doctor information to establish the relationship
                var doctor = await _userManager.FindByIdAsync(model.DoctorId);
                if (doctor == null)
                {
                    return BadRequest(new { error = "Doctor not found" });
                }

                // Get the Patient entity for the current user
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    // If patient doesn't exist, create a new Patient record
                    _logger.LogWarning($"Patient record not found for user {userId}. Creating a new patient record.");
                    
                    patient = new Patient
                    {
                        UserId = userId,
                        FullName = user.FullName ?? user.UserName ?? "Unknown",
                        Email = user.Email ?? string.Empty,
                        User = user
                    };
                    
                    _context.Patients.Add(patient);
                }

                // Set navigation properties
                appointment.Patient = patient;
                appointment.Doctor = doctor;

                // Add to context and save
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Appointment created successfully: ID={appointment.Id}, Doctor={doctor.Id}, Patient={user.Id}");

                return Ok(new
                {
                    id = appointment.Id,
                    date = appointment.GetFormattedDate(),
                    time = appointment.GetFormattedTime(),
                    status = appointment.Status.ToString(),
                    success = true,
                    message = "Appointment created successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while creating the appointment");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                return Ok(new
                {
                    id = appointment.Id,
                    doctorId = appointment.DoctorId,
                    patientId = appointment.PatientId,
                    date = appointment.GetFormattedDate(),
                    time = appointment.GetFormattedTime(),
                    description = appointment.Description,
                    status = appointment.Status.ToString()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment");
                return StatusCode(500, "An error occurred while retrieving the appointment");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentUpdateModel model)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            if (appointment.PatientId != user.Id && !isDoctor)
            {
                return Unauthorized();
            }

            try
            {
                if (!string.IsNullOrEmpty(model.AppointmentDate))
                {
                    appointment.AppointmentDate = DateTimeHelper.ParseDate(model.AppointmentDate);
                }

                if (!string.IsNullOrEmpty(model.AppointmentTime) && 
                    TimeSpan.TryParseExact(model.AppointmentTime, "HH:mm", CultureInfo.InvariantCulture, 
                    TimeSpanStyles.None, out TimeSpan time))
                {
                    appointment.AppointmentTime = time;
                }

                if (!string.IsNullOrEmpty(model.Description))
                {
                    appointment.Description = model.Description;
                }

                if (!string.IsNullOrEmpty(model.Status))
                {
                    if (Enum.TryParse<AppointmentStatus>(model.Status, out AppointmentStatus status))
                    {
                        appointment.Status = status;
                    }
                }

                appointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment");
                return StatusCode(500, "An error occurred while updating the appointment.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            if (appointment.PatientId != user.Id && !isDoctor)
            {
                return Unauthorized();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment deleted successfully" });
        }

        [HttpPost("book")]
        [Authorize]
        public async Task<IActionResult> BookAppointment([FromForm] AppointmentBookingViewModel model, IFormFile attachment = null)
        {
            try
            {
                // Get the logged in user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                // Get the doctor entity to establish proper relationship
                var doctor = await _userManager.FindByIdAsync(model.DoctorId);
                if (doctor == null)
                {
                    _logger.LogWarning($"Doctor not found with ID: {model.DoctorId}");
                    return BadRequest(new { error = "Doctor not found" });
                }

                // Parse date and time
                if (!DateTime.TryParse(model.Date, out DateTime appointmentDate))
                {
                    _logger.LogWarning($"Invalid date format: {model.Date}");
                    return BadRequest(new { error = "Invalid date format" });
                }

                // Handle different time formats (24-hour or 12-hour with AM/PM)
                TimeSpan appointmentTime;
                if (!TimeSpan.TryParse(model.Time, out appointmentTime))
                {
                    // The time might contain commas if multiple formats were provided
                    string timeToUse = model.Time.Contains(",") ? model.Time.Split(',')[0].Trim() : model.Time;
                    
                    // Try 12-hour format with AM/PM
                    string[] formats = { "h:mm tt", "hh:mm tt", "H:mm", "HH:mm" };
                    if (DateTime.TryParseExact(
                        timeToUse,
                        formats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime parsedTime))
                    {
                        appointmentTime = parsedTime.TimeOfDay;
                        _logger.LogInformation($"Successfully parsed time '{timeToUse}' to {appointmentTime}");
                    }
                    else
                    {
                        _logger.LogWarning($"Invalid time format: {model.Time}");
                        return BadRequest(new { error = $"Invalid time format: {model.Time}. Please use format like '2:00 PM' or '14:00'" });
                    }
                }

                // Check for conflicts
                var hasConflict = await _context.Appointments
                    .AnyAsync(a => a.DoctorId == model.DoctorId &&
                                 DateTimeHelper.AreDatesEqual(a.AppointmentDate, appointmentDate) &&
                                 a.AppointmentTime == appointmentTime &&
                                 a.Status != AppointmentStatus.Cancelled);

                if (hasConflict)
                {
                    _logger.LogWarning($"Time slot is no longer available: Doctor={model.DoctorId}, Date={appointmentDate}, Time={appointmentTime}");
                    return BadRequest(new { error = "Time slot is no longer available" });
                }

                // Create the appointment
                var appointment = new Appointment
                {
                    PatientId = userId,
                    PatientName = user.FullName ?? user.UserName ?? "Unknown", // Use username as fallback
                    DoctorId = model.DoctorId,
                    AppointmentDate = appointmentDate,
                    AppointmentTime = appointmentTime,
                    Description = model.Reason ?? string.Empty,
                    ReasonForVisit = model.Reason ?? string.Empty,
                    AgeValue = model.Age,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle file upload if present
                if (attachment != null && attachment.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "appointments");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachment.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }

                    appointment.AttachmentPath = $"/uploads/appointments/{uniqueFileName}";
                }

                // Get the Patient entity for the current user
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    // If patient doesn't exist, create a new Patient record
                    _logger.LogWarning($"Patient record not found for user {userId}. Creating a new patient record.");
                    
                    patient = new Patient
                    {
                        UserId = userId,
                        FullName = user.FullName ?? user.UserName ?? "Unknown",
                        Email = user.Email ?? string.Empty,
                        User = user
                    };
                    
                    _context.Patients.Add(patient);
                }

                // Set navigation properties
                appointment.Patient = patient;
                appointment.Doctor = doctor;

                // Save to database with explicit entity tracking
                _context.Entry(user).State = EntityState.Unchanged;
                _context.Entry(doctor).State = EntityState.Unchanged;
                _context.Appointments.Add(appointment);
                
                // Log action before saving
                _logger.LogInformation($"Attempting to save appointment for patient: {userId}, doctor: {model.DoctorId}");
                
                try {
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Save succeeded: Appointment ID: {appointment.Id}");
                }
                catch (DbUpdateException dbEx) {
                    _logger.LogError(dbEx, $"Database error saving appointment: {dbEx.InnerException?.Message ?? dbEx.Message}");
                    return StatusCode(500, new { error = "Database error occurred while saving the appointment." });
                }
                catch (Exception ex) {
                    _logger.LogError(ex, $"Error saving appointment to database: {ex.Message}");
                    throw; // Re-throw to be caught by outer try/catch
                }

                // Log success details
                _logger.LogInformation($"Appointment booked successfully: ID={appointment.Id}, " +
                    $"PatientId={appointment.PatientId}, " +
                    $"PatientName={appointment.PatientName}, " +
                    $"DoctorId={appointment.DoctorId}, " +
                    $"DateTime={appointment.AppointmentDate:yyyy-MM-dd} {appointment.GetFormattedTime()}");

                return Ok(new { 
                    success = true, 
                    message = "Appointment booked successfully",
                    appointmentId = appointment.Id,
                    doctorName = doctor.FullName,
                    patientName = user.FullName,
                    date = appointment.GetFormattedDate(),
                    time = appointment.GetFormattedTime()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment");
                return StatusCode(500, new { error = $"An error occurred while booking your appointment: {ex.Message}" });
            }
        }

        public class AppointmentBookingViewModel
        {
            public string DoctorId { get; set; }
            public string Date { get; set; }
            public string Time { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Reason { get; set; }
        }
    }
}
