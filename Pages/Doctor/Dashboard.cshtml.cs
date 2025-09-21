using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Helpers;

namespace Barangay.Pages.Doctor
{
    // Allow access either by Doctor role or explicit permission via policy
    [Authorize(Policy = "AccessDoctorDashboard")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalPatients { get; set; }
        public int ConsultedPatients { get; set; }
        public int PendingAppointments { get; set; }
        public int UrgentCases { get; set; }
        public List<Models.Appointment> TodaysAppointments { get; set; } = new List<Models.Appointment>();
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Get today's date
            var today = DateTime.Today;

            // Get all patients who have appointments with this doctor
            var allPatients = await _context.Appointments
                .Where(a => a.DoctorId == userId)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            // Count total unique patients
            TotalPatients = allPatients.Count;

            // Count consulted patients (appointments with Completed status)
            ConsultedPatients = await _context.Appointments
                .Where(a => a.DoctorId == userId && 
                           a.Status == AppointmentStatus.Completed)
                .Select(a => a.PatientId)
                .Distinct()
                .CountAsync();

            // Count pending appointments for today and future
            PendingAppointments = await _context.Appointments
                .CountAsync(a => a.DoctorId == userId && 
                                a.Status == AppointmentStatus.Pending &&
                                DateTimeHelper.IsDateGreaterThanOrEqual(a.AppointmentDate, DateTime.Today));

            // Count urgent cases
            UrgentCases = await _context.Appointments
                .CountAsync(a => a.DoctorId == userId && 
                                a.Status == AppointmentStatus.Pending && 
                                a.Type == "Urgent");

            // Get today's appointments for the queue
            TodaysAppointments = await _context.Appointments
                .Where(a => a.DoctorId == userId && 
                           a.AppointmentDate.Date == today.Date &&
                           (a.Status == AppointmentStatus.Pending || 
                            a.Status == AppointmentStatus.InProgress))
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();

            // Get recent notifications
            RecentNotifications = await _context.Notifications
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            return Page();
        }
    }
}