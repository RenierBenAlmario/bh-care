using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Doctor
{
    public class PatientListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> _predefinedBarangays = new List<string>
        {
            "Barangay 158",
            "Barangay 159",
            "Barangay 160",
            "Barangay 161"
        };

        public PatientListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedBarangay { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<SelectListItem> BarangayList { get; set; }
        public IList<PatientViewModel> Patients { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get list of unique barangays from database
            var databaseBarangays = await _context.Patients
                .Select(p => p.User.Address)
                .Where(b => !string.IsNullOrEmpty(b))
                .Distinct()
                .ToListAsync();

            // Combine predefined barangays with database barangays
            var allBarangays = _predefinedBarangays
                .Union(databaseBarangays)
                .Distinct()
                .OrderBy(b => b)
                .ToList();

            BarangayList = allBarangays.Select(b => new SelectListItem { Value = b, Text = b }).ToList();

            // Build query
            var query = _context.Patients
                .Include(p => p.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                query = query.Where(p =>
                    p.User.FullName.Contains(SearchQuery) ||
                    p.User.Email.Contains(SearchQuery) ||
                    p.User.PhilHealthId.Contains(SearchQuery));
            }

            if (!string.IsNullOrEmpty(SelectedBarangay))
            {
                query = query.Where(p => p.User.Address == SelectedBarangay);
            }

            if (!string.IsNullOrEmpty(SelectedStatus))
            {
                query = query.Where(p => p.User.Status == SelectedStatus);
            }

            // Calculate pagination
            var totalPatients = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalPatients / (double)PageSize);
            CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

            // Get paginated results
            var patients = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(p => new PatientViewModel
                {
                    PatientId = p.UserId,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    PhoneNumber = p.User.PhoneNumber,
                    Barangay = p.User.Address,
                    Status = p.User.Status
                })
                .ToListAsync();

            Patients = patients;

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

            if (!string.IsNullOrEmpty(SelectedBarangay))
                queryParams.Add("selectedBarangay", SelectedBarangay);

            if (!string.IsNullOrEmpty(SelectedStatus))
                queryParams.Add("selectedStatus", SelectedStatus);

            return Url.Page("./PatientList", queryParams);
        }
    }

    public class PatientViewModel
    {
        public string PatientId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Barangay { get; set; }
        public string Status { get; set; }
    }
} 