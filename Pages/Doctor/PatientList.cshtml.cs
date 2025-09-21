using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.Doctor
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Doctor,Head Doctor")]
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "DoctorPatientList")]
    public class PatientListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;
        private readonly List<string> _predefinedBarangays = new List<string>
        {
            "Barangay 158",
            "Barangay 159",
            "Barangay 160",
            "Barangay 161"
        };

        public PatientListModel(ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
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
            // Get list of unique barangays from database (use Barangay only)
            var databaseBarangays = await _context.Patients
                .Select(p => p.User.Barangay)
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
                .Where(p => p.UserId != "0e03f06e-ba88-46ed-b047-4974d8b8252a" && p.FullName != "System Administrator")
                .AsQueryable();

            // Exclude staff members (doctor, nurse, admin) from patient list
            var staffUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == _context.Roles.Where(r => r.Name == "Doctor").Select(r => r.Id).FirstOrDefault()
                    || ur.RoleId == _context.Roles.Where(r => r.Name == "Nurse").Select(r => r.Id).FirstOrDefault()
                    || ur.RoleId == _context.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefault())
                .Select(ur => ur.UserId)
                .Distinct()
                .ToListAsync();

            query = query.Where(p => !staffUserIds.Contains(p.UserId));

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
                query = query.Where(p => p.User.Barangay == SelectedBarangay);
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
                    Barangay = string.IsNullOrEmpty(p.User.Barangay) ? "Not specified" : p.User.Barangay,
                    Status = p.User.Status
                })
                .ToListAsync();

            // Decrypt patient data for authorized users
            foreach (var patient in patients)
            {
                // Get the full user object to decrypt
                var user = await _context.Users.FindAsync(patient.PatientId);
                if (user != null)
                {
                    // Decrypt user data
                    user.DecryptSensitiveData(_encryptionService, User);
                    
                    // Update the view model with decrypted data
                    patient.FullName = user.FullName;
                    patient.Email = user.Email;
                    patient.PhoneNumber = user.PhoneNumber;
                }
            }

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