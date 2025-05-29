using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Doctor
{
    public class PrescriptionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public PrescriptionStatus? SelectedStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public IList<PrescriptionListItemViewModel> Prescriptions { get; set; }
        public IList<PrescriptionPatientViewModel> Patients { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get patients for the dropdown
            Patients = await _context.Patients
                .Include(p => p.User)
                .Select(p => new PrescriptionPatientViewModel
                {
                    Id = p.UserId,
                    FullName = p.User.FullName
                })
                .OrderBy(p => p.FullName)
                .ToListAsync();

            // Build query for prescriptions
            var query = _context.Prescriptions
                .Include(p => p.Patient)
                .ThenInclude(p => p.User)
                .Include(p => p.PrescriptionMedicines)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                query = query.Where(p =>
                    p.Patient.User.FullName.Contains(SearchQuery) ||
                    p.Diagnosis.Contains(SearchQuery));
            }

            if (SelectedStatus.HasValue)
            {
                query = query.Where(p => p.Status == SelectedStatus.Value);
            }

            // Calculate pagination
            var totalPrescriptions = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalPrescriptions / (double)PageSize);
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            // Get paginated results
            var prescriptions = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new PrescriptionListItemViewModel
                {
                    Id = p.Id,
                    PatientName = p.Patient.User.FullName,
                    CreatedAt = p.CreatedAt,
                    Status = p.Status.ToString(),
                    MedicineCount = p.PrescriptionMedicines.Count,
                    Duration = p.Duration
                })
                .ToListAsync();

            Prescriptions = prescriptions;

            return Page();
        }

        public string GetPageUrl(int pageNumber)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "currentPage", pageNumber.ToString() }
            };

            if (!string.IsNullOrEmpty(SearchQuery))
                queryParams.Add("searchQuery", SearchQuery);

            if (SelectedStatus.HasValue)
                queryParams.Add("selectedStatus", SelectedStatus.ToString());

            return Url.Page("./Prescriptions", queryParams);
        }
    }

    public class PrescriptionListItemViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int MedicineCount { get; set; }
        public int Duration { get; set; }
    }

    public class PrescriptionPatientViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
    }
} 