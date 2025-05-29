using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse")]
    public class DiagnosisModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DiagnosisModel> _logger;

        public DiagnosisModel(ApplicationDbContext context, ILogger<DiagnosisModel> logger)
        {
            _context = context;
            _logger = logger;
            DiagnosisRecords = new List<DiagnosisRecordViewModel>();
        }

        public class DiagnosisRecordViewModel
        {
            public int Id { get; set; }
            public string PatientId { get; set; }
            public string PatientName { get; set; }
            public DateTime Date { get; set; }
            public string DoctorName { get; set; }
            public string DiagnosisText { get; set; }
            public string Treatment { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
        }

        public List<DiagnosisRecordViewModel> DiagnosisRecords { get; set; }
        public string SearchTerm { get; set; }
        public string FilterOption { get; set; }

        public async Task<IActionResult> OnGetAsync(string search, string filter)
        {
            SearchTerm = search;
            FilterOption = filter ?? "all";

            try
            {
                // Start with a base query
                var query = _context.MedicalRecords
                    .Include(m => m.Patient)
                    .Include(m => m.Doctor)
                    .AsQueryable();

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    query = query.Where(m => 
                        m.Patient.FullName.Contains(SearchTerm) || 
                        m.Diagnosis.Contains(SearchTerm) ||
                        m.PatientId.Contains(SearchTerm));
                }

                // Apply category filter
                switch (FilterOption)
                {
                    case "recent":
                        query = query.Where(m => m.Date >= DateTime.Now.AddDays(-30));
                        break;
                    case "critical":
                        // Assuming there's a way to identify critical diagnoses
                        // This might need to be adjusted based on your data model
                        query = query.Where(m => m.Status == "Critical" || m.Diagnosis.Contains("Critical") || m.Diagnosis.Contains("Urgent"));
                        break;
                    case "pending":
                        query = query.Where(m => m.Status == "Pending" || m.Status == "In Progress");
                        break;
                    default:
                        // "all" - no additional filtering
                        break;
                }

                // Get the results and map to view model
                var records = await query
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();

                DiagnosisRecords = records.Select(m => new DiagnosisRecordViewModel
                {
                    Id = m.Id,
                    PatientId = m.PatientId,
                    PatientName = m.Patient?.FullName ?? "Unknown",
                    Date = m.Date,
                    DoctorName = m.Doctor?.FullName ?? "Unknown",
                    DiagnosisText = m.Diagnosis,
                    Treatment = m.Treatment,
                    Notes = m.Notes,
                    Status = m.Status ?? "Normal"
                }).ToList();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading diagnosis records");
                DiagnosisRecords = new List<DiagnosisRecordViewModel>();
                return Page();
            }
        }
    }
} 