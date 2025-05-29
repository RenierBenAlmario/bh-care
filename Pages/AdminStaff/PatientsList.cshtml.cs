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
    public class PatientsListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PatientsListModel> _logger;

        public PatientsListModel(
            ApplicationDbContext context,
            ILogger<PatientsListModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [TempData]
        public string SuccessMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GenderFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AgeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int StartRecord { get; set; }
        public int EndRecord { get; set; }
        public int CurrentPage => PageNumber;

        public List<Patient> Patients { get; set; } = new List<Patient>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Initialize page parameters
                if (PageNumber < 1) PageNumber = 1;

                // Build query with filtering
                var query = _context.Patients.AsQueryable();

                // Apply search term
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    query = query.Where(p =>
                        p.FullName.Contains(SearchTerm) ||
                        p.ContactNumber.Contains(SearchTerm) ||
                        p.Email.Contains(SearchTerm));
                }

                // Apply gender filter
                if (!string.IsNullOrEmpty(GenderFilter))
                {
                    query = query.Where(p => p.Gender == GenderFilter);
                }

                // Apply age filter
                if (!string.IsNullOrEmpty(AgeFilter))
                {
                    // Calculate age range based on birth date
                    var today = DateTime.Today;
                    
                    switch (AgeFilter)
                    {
                        case "0-18":
                            var minDate1 = today.AddYears(-18);
                            query = query.Where(p => p.BirthDate >= minDate1);
                            break;
                        case "19-30":
                            var minDate2 = today.AddYears(-30);
                            var maxDate2 = today.AddYears(-19);
                            query = query.Where(p => p.BirthDate >= minDate2 && p.BirthDate <= maxDate2);
                            break;
                        case "31-45":
                            var minDate3 = today.AddYears(-45);
                            var maxDate3 = today.AddYears(-31);
                            query = query.Where(p => p.BirthDate >= minDate3 && p.BirthDate <= maxDate3);
                            break;
                        case "46-60":
                            var minDate4 = today.AddYears(-60);
                            var maxDate4 = today.AddYears(-46);
                            query = query.Where(p => p.BirthDate >= minDate4 && p.BirthDate <= maxDate4);
                            break;
                        case "61+":
                            var maxDate5 = today.AddYears(-61);
                            query = query.Where(p => p.BirthDate <= maxDate5);
                            break;
                    }
                }

                // Count total records
                TotalRecords = await query.CountAsync();

                // Calculate pagination
                TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
                StartRecord = (PageNumber - 1) * PageSize + 1;
                EndRecord = Math.Min(StartRecord + PageSize - 1, TotalRecords);

                // Apply ordering and pagination
                Patients = await query
                    .OrderBy(p => p.FullName)
                    .Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patients list");
                return RedirectToPage("/Error");
            }
        }
    }
} 