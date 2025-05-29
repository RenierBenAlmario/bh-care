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

namespace Barangay.Pages.AdminStaff
{
    [Authorize(Roles = "Admin Staff")]
    [Authorize(Policy = "AccessAdminDashboard")]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string SuccessMessage { get; set; }

        public int TodayAppointmentsCount { get; set; }
        public int TotalPatientsCount { get; set; }
        public int TotalMedicalRecordsCount { get; set; }
        public int PendingTasksCount { get; set; }
        public List<AppointmentViewModel> TodayAppointments { get; set; } = new List<AppointmentViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get today's date (server time)
                var today = DateTime.Now.Date;

                // Get today's appointments count
                TodayAppointmentsCount = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == today)
                    .CountAsync();

                // Get total patients count
                TotalPatientsCount = await _context.Patients.CountAsync();

                // Get total medical records count
                TotalMedicalRecordsCount = await _context.MedicalRecords.CountAsync();

                // Get pending tasks count (example: pending appointments)
                PendingTasksCount = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Pending && a.AppointmentDate.Date >= today)
                    .CountAsync();

                // Get today's appointments with related data
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => a.AppointmentDate.Date == today)
                    .OrderBy(a => a.AppointmentTime)
                    .Take(5) // Limit to 5 items for the dashboard
                    .ToListAsync();

                // Map to view model manually to avoid type conversion issues
                foreach (var appointment in appointments)
                {
                    TodayAppointments.Add(new AppointmentViewModel
                    {
                        Id = appointment.Id,
                        PatientName = appointment.Patient.FullName,
                        PatientId = int.Parse(appointment.PatientId),
                        DoctorName = appointment.Doctor.FullName,
                        DoctorId = appointment.DoctorId.ToString(),
                        AppointmentDate = appointment.AppointmentDate,
                        AppointmentTime = DateTime.Today.Add(appointment.AppointmentTime),
                        AppointmentType = appointment.Type,
                        Status = appointment.Status.ToString()
                    });
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToPage("/Error", new { message = "Error loading dashboard data: " + ex.Message });
            }
        }
    }

    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public int PatientId { get; set; }
        public string DoctorName { get; set; }
        public string DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string AppointmentType { get; set; }
        public string Status { get; set; }
    }
} 