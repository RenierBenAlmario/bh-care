using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class AppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Barangay.Models.Appointment> TodayAppointments { get; set; } = new();
        public List<Barangay.Models.Appointment> AllAppointments { get; set; } = new();
        public List<Barangay.Models.Appointment> UpcomingAppointments { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            var today = DateTime.Today;

            TodayAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == user.Id && 
                           a.AppointmentDate.Date == today.Date &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            UpcomingAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == user.Id && 
                           a.AppointmentDate.Date > today.Date &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            AllAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == user.Id &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int appointmentId, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
} 