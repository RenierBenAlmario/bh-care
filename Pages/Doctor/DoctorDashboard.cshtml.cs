using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Services;
using Microsoft.EntityFrameworkCore;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor,Head Doctor")]
    [Authorize(Policy = "DoctorDashboard")]
    public class DoctorDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public DoctorDashboardModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IPermissionService permissionService, IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        public string DoctorName { get; set; }
        public string DoctorEmail { get; set; }
        public string DoctorSpecialization { get; set; }
        public int TotalPatients { get; set; }
        public int Consulted { get; set; }
        public int Pending { get; set; }
        public int UrgentCases { get; set; }
        public List<NotificationItem> RecentNotifications { get; set; } = new();
        public List<Barangay.Models.Appointment> TodaysAppointments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid(); // User not found, or not logged in

            // Access enforced by policy

            DoctorName = user.FullName;
            DoctorEmail = user.Email;
            DoctorSpecialization = user.Specialization ?? "General Practitioner";

            var today = DateTime.Now.Date;

            var appointmentsQuery = _context.Appointments
                                          .Where(a => a.DoctorId == user.Id && a.AppointmentDate.Date == today);

            TotalPatients = await appointmentsQuery.CountAsync();
            Consulted = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Completed);
            Pending = await appointmentsQuery.CountAsync(a => a.Status == AppointmentStatus.Confirmed || a.Status == AppointmentStatus.InProgress);
            UrgentCases = 0; // Placeholder for urgent cases logic

            var appointments = await appointmentsQuery.Include(a => a.Patient).OrderBy(a => a.AppointmentTime).ToListAsync();
            
            // Decrypt patient data for each appointment
            foreach (var appointment in appointments)
            {
                if (appointment.Patient != null)
                {
                    appointment.Patient = appointment.Patient.DecryptSensitiveData(_encryptionService, User);
                }
                
                // Also decrypt the PatientName field if it's encrypted
                if (!string.IsNullOrEmpty(appointment.PatientName) && _encryptionService.IsEncrypted(appointment.PatientName))
                {
                    appointment.PatientName = appointment.PatientName.DecryptForUser(_encryptionService, User);
                }
            }
            
            TodaysAppointments = appointments;

            // Fetch real notifications from the database
            var notifications = await _context.Notifications
                                              .Where(n => n.RecipientId == user.Id && !n.IsRead)
                                              .OrderByDescending(n => n.CreatedAt)
                                              .Take(5)
                                              .ToListAsync();

            RecentNotifications = notifications.Select(n => new NotificationItem
            {
                Title = n.Title,
                Message = n.Message,
                CreatedAt = n.CreatedAt
            }).ToList();

            return Page();
        }
    }

    public class NotificationItem
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
