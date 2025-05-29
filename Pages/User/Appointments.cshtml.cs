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

            return Page();
        }

        public IActionResult OnGetBookNewAppointment()
        {
            return RedirectToPage("/BookAppointment");
        }
    }
} 