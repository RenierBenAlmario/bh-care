using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Threading.Tasks;
using System;

namespace Barangay.Pages.Doctor.Appointment
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Barangay.Models.Appointment Appointment { get; set; } = new Barangay.Models.Appointment();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
            if (Appointment == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == Appointment.Id);
            if (appointment == null)
            {
                return NotFound();
            }
            appointment.AppointmentDate = Appointment.AppointmentDate;
            appointment.AppointmentTime = Appointment.AppointmentTime;
            appointment.Description = Appointment.Description;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return RedirectToPage("/Doctor/Appointments");
        }
    }
} 