using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class PatientHistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientHistoryModel> _logger;

        public PatientHistoryModel(ApplicationDbContext context, ILogger<PatientHistoryModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class PatientHistoryViewModel
        {
            public int Id { get; set; }
            public string PatientName { get; set; }
            public DateTime VisitDate { get; set; }
            public string ConsultationType { get; set; }
            public string Diagnosis { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        public List<PatientHistoryViewModel> PatientHistory { get; set; } = new List<PatientHistoryViewModel>();

        public async Task OnGetAsync()
        {
            try
            {
                // Start with a base query - explicitly make it ordered
                IQueryable<Models.Appointment> query = _context.Appointments
                    .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .OrderByDescending(a => a.AppointmentDate); // This makes it IOrderedQueryable

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(a => 
                        (a.Patient.User.FirstName != null && a.Patient.User.FirstName.Contains(SearchTerm)) ||
                        (a.Patient.User.LastName != null && a.Patient.User.LastName.Contains(SearchTerm)) ||
                        (a.Patient.User.FullName != null && a.Patient.User.FullName.Contains(SearchTerm)));
                }

                var appointments = await query.Take(100).ToListAsync();

                PatientHistory = appointments.Select(a => new PatientHistoryViewModel
                {
                    Id = a.Id,
                    PatientName = a.Patient?.User?.FullName ?? "Unknown Patient",
                    VisitDate = a.AppointmentDate,
                    ConsultationType = a.Type ?? "General Consultation",
                    Diagnosis = a.Description ?? "No diagnosis recorded"
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient history");
                PatientHistory = new List<PatientHistoryViewModel>();
                TempData["ErrorMessage"] = "There was an error retrieving patient history. Please try again later.";
            }
        }
    }
} 