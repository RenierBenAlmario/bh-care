using System;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Barangay.Pages.Doctor.Appointment
{
    [Authorize(Roles = "Doctor")]
    public class StartModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StartModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == userId);

            if (appointment == null)
                return NotFound();

            if (appointment.Status != AppointmentStatus.Pending)
                return BadRequest("This appointment cannot be started.");

            // Update appointment status
            appointment.Status = AppointmentStatus.InProgress;
            await _context.SaveChangesAsync();

            // Create a notification for the patient
            var notification = new Notification
            {
                UserId = userId,
                RecipientId = appointment.PatientId,
                Type = "Appointment",
                Title = "Consultation Started",
                Message = $"Your consultation has started with Dr. {User.Identity?.Name}",
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = appointment.Id });
        }
    }
} 