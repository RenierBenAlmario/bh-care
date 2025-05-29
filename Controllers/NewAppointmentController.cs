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
using Barangay.Services;
using Barangay.Extensions;
using System.ComponentModel.DataAnnotations;
using Barangay.Helpers;
using Barangay.Models.Appointments;

namespace Barangay.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NewAppointmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NewAppointmentController> _logger;
        private readonly IEncryptionService _encryptionService;
        private readonly IAppointmentService _appointmentService;

        public NewAppointmentController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<NewAppointmentController> logger,
            IEncryptionService encryptionService,
            IAppointmentService appointmentService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _encryptionService = encryptionService;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            var appointments = await _appointmentService.GetAppointmentsForUserAsync(user.Id, isDoctor);
            
            return Ok(await appointments.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentDetails(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            if (appointment.PatientId != user.Id && !isDoctor)
            {
                return Unauthorized();
            }

            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                // Validate date and time
                if (string.IsNullOrEmpty(model.AppointmentDate) || string.IsNullOrEmpty(model.AppointmentTime))
                {
                    return BadRequest("Appointment date and time are required.");
                }

                var appointmentDate = DateTimeHelper.ParseDate(model.AppointmentDate);
                if (appointmentDate == DateTime.MinValue)
                {
                    return BadRequest("Invalid appointment date format.");
                }

                if (!TimeSpan.TryParse(model.AppointmentTime, out TimeSpan appointmentTime))
                {
                    return BadRequest("Invalid appointment time format.");
                }

                var appointment = new Appointment
                {
                    DoctorId = model.StaffId,
                    PatientId = user.Id,
                    PatientName = user.UserName,
                    ReasonForVisit = model.Description,
                    Status = AppointmentStatus.Pending,
                    AppointmentDate = appointmentDate,
                    AppointmentTime = appointmentTime,
                    AgeValue = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Check if the time slot is available
                var isAvailable = await _appointmentService.IsTimeSlotAvailableAsync(
                    model.StaffId,
                    appointmentDate,
                    appointmentTime);

                if (!isAvailable)
                {
                    return BadRequest("The selected time slot is not available. Please choose another time.");
                }

                var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment);
                if (createdAppointment != null)
                {
                    return Ok(new { message = "Appointment created successfully" });
                }

                return BadRequest("Failed to create appointment");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, "An error occurred while creating the appointment");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment appointment)
        {
            if (id != appointment.Id) return BadRequest();

            var existingAppointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (existingAppointment == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
            if (existingAppointment.PatientId != user.Id && !isDoctor)
            {
                return Unauthorized();
            }

            try
            {
                _context.Entry(existingAppointment).CurrentValues.SetValues(appointment);
                existingAppointment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Appointments.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
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

            return NoContent();
        }
    }
} 