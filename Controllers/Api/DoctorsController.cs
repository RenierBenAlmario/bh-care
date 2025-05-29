using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Barangay.Extensions;
using Barangay.Helpers;

namespace Barangay.Controllers.Api
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DoctorsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorsController(ApplicationDbContext context, ILogger<DoctorsController> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableDoctors([FromQuery] string date)
        {
            var parsedDate = DateTimeHelper.ParseDate(date);
            if (parsedDate == DateTime.MinValue)
                return BadRequest(new { error = true, message = "Invalid date format" });

            _logger.LogInformation($"Fetching available doctors for {parsedDate:yyyy-MM-dd}");

            var doctorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
            if (doctorRole == null)
                return Ok(new { doctors = new List<object>() });

            var doctorUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == doctorRole.Id)
                .Select(ur => ur.UserId)
                .ToListAsync();

            var doctorsList = await _context.StaffMembers
                .Where(s => doctorUserIds.Contains(s.UserId) && s.IsActive && s.Role == "Doctor")
                .Select(s => new
                {
                    id = s.UserId,
                    name = s.Name,
                    specialization = s.Specialization,
                    workingHours = s.WorkingHours,
                    availableSlots = (s.MaxDailyPatients != 0 ? s.MaxDailyPatients : 8) - _context.Appointments
                        .Count(a => a.DoctorId == s.UserId && 
                               DateTimeHelper.IsDateEqual(a.AppointmentDate, parsedDate) &&
                               a.Status != AppointmentStatus.Cancelled)
                })
                .Where(d => d.availableSlots > 0)
                .ToListAsync();

            return Ok(new { doctors = doctorsList });
        }

        [HttpGet("{doctorId}/timeslots")]
        public async Task<IActionResult> GetAvailableTimeSlots(string doctorId, [FromQuery] string date)
        {
            var parsedDate = DateTimeHelper.ParseDate(date);
            if (parsedDate == DateTime.MinValue)
                return BadRequest(new { error = true, message = "Invalid date format" });

            var doctor = await _context.StaffMembers.FirstOrDefaultAsync(d => d.UserId == doctorId);
            if (doctor == null)
                return NotFound(new { error = true, message = "Doctor not found" });

            // Parse working hours
            string workingHours = doctor.WorkingHours ?? "9:00 AM - 5:00 PM";
            if (!TryParseWorkingHours(workingHours, out TimeSpan startTime, out TimeSpan endTime))
            {
                startTime = new TimeSpan(9, 0, 0);
                endTime = new TimeSpan(17, 0, 0);
            }

            // Get appointments first, then format the times
            var bookedAppointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           DateTimeHelper.IsDateEqual(a.AppointmentDate, parsedDate) &&
                           a.Status != AppointmentStatus.Cancelled)
                .Select(a => new { a.AppointmentTime })
                .ToListAsync();

            var bookedTimes = bookedAppointments
                .Select(a => a.AppointmentTime)
                .ToList();

            var availableSlots = GenerateTimeSlots(workingHours)
                .Select(t => TimeSpan.Parse(t))
                .Where(t => !bookedTimes.Contains(t))
                .Select(t => t.FormatTime())
                .ToList();

            return Ok(new { timeSlots = availableSlots });
        }

        private List<string> GenerateTimeSlots(string workingHours)
        {
            var timeSlots = new List<string>();
            if (!TryParseWorkingHours(workingHours, out TimeSpan startTime, out TimeSpan endTime))
            {
                startTime = new TimeSpan(9, 0, 0); // 9:00 AM
                endTime = new TimeSpan(17, 0, 0);  // 5:00 PM
            }

            var currentTime = startTime;
            var interval = TimeSpan.FromMinutes(30); // 30-minute intervals

            while (currentTime < endTime)
            {
                timeSlots.Add(currentTime.ToStandardFormat());
                currentTime = currentTime.Add(interval);
            }

            return timeSlots;
        }

        private bool TryParseWorkingHours(string workingHours, out TimeSpan startTime, out TimeSpan endTime)
        {
            startTime = TimeSpan.FromHours(9);
            endTime = TimeSpan.FromHours(17);

            if (string.IsNullOrWhiteSpace(workingHours))
                return false;

            var parts = workingHours.Split('-').Select(p => p.Trim()).ToArray();
            if (parts.Length != 2)
                return false;

            return TimeSpan.TryParse(parts[0], out startTime) && TimeSpan.TryParse(parts[1], out endTime);
        }
    }
}