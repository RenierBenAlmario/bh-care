using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Barangay.Models;
using Barangay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.User
{
    [Authorize]
    public class AppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();
        public List<Appointment> PastAppointments { get; set; } = new List<Appointment>();
        public Dictionary<string, ApplicationUser> Doctors { get; set; } = new Dictionary<string, ApplicationUser>();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get the current date
            var today = DateTime.Now.Date;

            try
            {
                // Get all appointments for the current user
                var appointments = await _context.Appointments
                    .Where(a => a.PatientId == user.Id)
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

                // Ensure all appointments have valid times
                foreach (var appointment in appointments)
                {
                    // Ensure ReasonForVisit is not null
                    if (appointment.ReasonForVisit == null)
                    {
                        appointment.ReasonForVisit = "General Checkup";
                    }
                }

                // Get all doctor IDs from appointments
                var doctorIds = appointments
                    .Select(a => a.DoctorId)
                    .Distinct()
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                // Load all doctors in one query
                var doctors = await _userManager.Users
                    .Where(u => doctorIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);

                Doctors = doctors;

                // Split into upcoming and past appointments
                UpcomingAppointments = appointments
                    .Where(a => a.AppointmentDate >= today || 
                        (a.AppointmentDate == today && 
                            a.AppointmentTime >= DateTime.Now.TimeOfDay))
                    .ToList();

                PastAppointments = appointments
                    .Where(a => a.AppointmentDate < today || 
                        (a.AppointmentDate == today && 
                            a.AppointmentTime < DateTime.Now.TimeOfDay))
                    .ToList();
            }
            catch (InvalidCastException ex)
            {
                // Log the error
                Console.WriteLine($"Type conversion error: {ex.Message}");
                
                // Initialize empty lists to avoid null reference exceptions in the view
                UpcomingAppointments = new List<Appointment>();
                PastAppointments = new List<Appointment>();
            }
            catch (Exception ex)
            {
                // Log the general error
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                
                // Initialize empty lists to avoid null reference exceptions in the view
                UpcomingAppointments = new List<Appointment>();
                PastAppointments = new List<Appointment>();
            }

            return Page();
        }

        public string GetDoctorName(string doctorId)
        {
            if (string.IsNullOrEmpty(doctorId))
                return "Unknown Doctor";

            if (Doctors.TryGetValue(doctorId, out ApplicationUser doctor))
            {
                string fullName = "";
                if (!string.IsNullOrEmpty(doctor.FirstName) && !string.IsNullOrEmpty(doctor.LastName))
                {
                    fullName = $"Dr. {doctor.FirstName} {doctor.LastName}";
                }
                else
                {
                    fullName = doctor.UserName ?? doctor.Email ?? "Unknown Doctor";
                }
                return fullName;
            }

            return "Unknown Doctor";
        }

        public IActionResult OnGetBookNewAppointment()
        {
            return RedirectToPage("/BookAppointment");
        }

        public async Task<IActionResult> OnPostCancelAppointmentAsync(int appointmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.PatientId == user.Id);

            if (appointment == null)
            {
                TempData["Error"] = "Appointment not found.";
                return RedirectToPage();
            }

            // Only allow cancellation for future appointments
            if (appointment.AppointmentDate < DateTime.Now.Date || 
                (appointment.AppointmentDate == DateTime.Now.Date && appointment.AppointmentTime < DateTime.Now.TimeOfDay))
            {
                TempData["Error"] = "Cannot cancel past appointments.";
                return RedirectToPage();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Appointment cancelled successfully.";
            return RedirectToPage();
        }
    }
} 