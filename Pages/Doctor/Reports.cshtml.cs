using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Barangay.Models;
using Barangay.Data;
using Barangay.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Barangay.Helpers;
using Microsoft.AspNetCore.Authorization;
using Barangay.Services;
using System.Globalization;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor,Head Doctor")]
    [Authorize(Policy = "DoctorReports")]
    public class ReportsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;
        
        // Simplified view model properties
        public List<string> MonthOptions { get; set; } = new List<string>();
        public string SelectedMonthLabel { get; set; } = string.Empty;
        public List<TopConditionRow> TopConditionStats { get; set; } = new List<TopConditionRow>();
        public List<string> TrendLabels { get; set; } = new List<string>();
        public List<int> TrendValues { get; set; } = new List<int>();
        
        // Selected month label (e.g., "August 2025") bound from query `month`
        [BindProperty(SupportsGet = true, Name = "month")]
        public string? SelectedMonthQuery { get; set; }
        
        public bool CanAccessReports { get; set; }

        public ReportsModel(
            IConfiguration configuration, 
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Policy already enforces access; keep flag for view condition
            CanAccessReports = await _permissionService.UserHasPermissionAsync(User, "Reports") ||
                               User.IsInRole("Admin") ||
                               User.IsInRole("Doctor") || User.IsInRole("Head Doctor");
            if (!CanAccessReports)
                return Forbid();

            // Build month options (last 12 months including current)
            var now = DateTime.Now;
            var culture = CultureInfo.GetCultureInfo("en-US");
            MonthOptions = Enumerable.Range(0, 12)
                .Select(i => now.AddMonths(-i).ToString("MMMM yyyy", culture))
                .ToList();

            // Parse selected month
            var fallbackLabel = MonthOptions.FirstOrDefault() ?? now.ToString("MMMM yyyy", culture);
            var label = string.IsNullOrWhiteSpace(SelectedMonthQuery) ? fallbackLabel : SelectedMonthQuery!;
            if (!DateTime.TryParseExact(label, "MMMM yyyy", culture, DateTimeStyles.None, out var selectedMonth))
            {
                selectedMonth = new DateTime(now.Year, now.Month, 1);
                label = selectedMonth.ToString("MMMM yyyy", culture);
            }
            SelectedMonthLabel = label;

            // Calculate month ranges
            var monthStart = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
            var nextMonth = monthStart.AddMonths(1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart;

            // Query: get medical records for selected month and decrypt diagnosis data
            var monthRecords = await _context.MedicalRecords
                .Where(m => m.Date >= monthStart && m.Date < nextMonth && !string.IsNullOrEmpty(m.Diagnosis))
                .ToListAsync();

            var lastMonthRecords = await _context.MedicalRecords
                .Where(m => m.Date >= lastMonthStart && m.Date < lastMonthEnd && !string.IsNullOrEmpty(m.Diagnosis))
                .ToListAsync();

            // Decrypt diagnosis data for current month
            foreach (var record in monthRecords)
            {
                record.DecryptSensitiveData(_encryptionService, User);
            }

            // Decrypt diagnosis data for last month
            foreach (var record in lastMonthRecords)
            {
                record.DecryptSensitiveData(_encryptionService, User);
            }

            // Group by decrypted diagnosis
            var monthGroups = monthRecords
                .GroupBy(m => m.Diagnosis)
                .Select(g => new { Condition = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var lastMonthGroups = lastMonthRecords
                .GroupBy(m => m.Diagnosis)
                .Select(g => new { Condition = g.Key, Count = g.Count() })
                .ToList();

            var lastMonthDict = lastMonthGroups.ToDictionary(x => x.Condition, x => x.Count);
            var totalThisMonth = monthGroups.Sum(x => x.Count);
            TopConditionStats = monthGroups.Select(x =>
            {
                lastMonthDict.TryGetValue(x.Condition, out var lastCount);
                var trend = x.Count == lastCount ? "flat" : (x.Count > lastCount ? "up" : "down");
                var percent = totalThisMonth > 0 ? (double)x.Count / totalThisMonth * 100.0 : 0.0;
                return new TopConditionRow
                {
                    Condition = x.Condition,
                    CasesThisMonth = x.Count,
                    PercentOfTotal = percent,
                    LastMonthCases = lastCount,
                    TrendDirection = trend
                };
            }).ToList();

            // Trend over past 6 months (including selected)
            TrendLabels.Clear();
            TrendValues.Clear();
            for (int i = 5; i >= 0; i--)
            {
                var month = monthStart.AddMonths(-i);
                var monthEnd = month.AddMonths(1);
                var labelItem = month.ToString("MMM yyyy", culture);
                
                // Get records for this month and decrypt to count properly
                var monthTrendRecords = await _context.MedicalRecords
                    .Where(m => m.Date >= month && m.Date < monthEnd && !string.IsNullOrEmpty(m.Diagnosis))
                    .ToListAsync();
                
                // Decrypt diagnosis data
                foreach (var record in monthTrendRecords)
                {
                    record.DecryptSensitiveData(_encryptionService, User);
                }
                
                var total = monthTrendRecords.Count;
                TrendLabels.Add(labelItem);
                TrendValues.Add(total);
            }

            return Page();
        }

        private async Task LoadOverallStatisticsAsync()
        {
            // Legacy method no longer used in simplified report
            await Task.CompletedTask;
        }

        private async Task LoadDetailedStatisticsAsync()
        {
            // Legacy method no longer used in simplified report
            await Task.CompletedTask;
        }

        private async Task LoadStaffStatisticsAsync()
        {
            // Legacy method no longer used in simplified report
            await Task.CompletedTask;
        }

        private async Task LoadPatientDemographicsAsync()
        {
            // Legacy method no longer used in simplified report
            await Task.CompletedTask;
        }

        private async Task LoadTopConditionsAsync()
        {
            // Legacy method no longer used in simplified report
            await Task.CompletedTask;
        }

        public class StatisticsInfo
        {
            public int TotalConsultations { get; set; }
            public int NewPatients { get; set; }
            public int TotalPrescriptions { get; set; }
            public int TotalAppointments { get; set; }
            public int TotalDoctors { get; set; }
            public int TotalNurses { get; set; }
            public int AverageConsultationTime { get; set; }
            public int SatisfactionRate { get; set; }
        }

        // Simplified table row for the redesigned report
        public class TopConditionRow
        {
            public string Condition { get; set; } = string.Empty;
            public int CasesThisMonth { get; set; }
            public double PercentOfTotal { get; set; }
            public int LastMonthCases { get; set; }
            public string TrendDirection { get; set; } = "flat"; // up | down | flat
        }

        public class DetailedStatInfo
        {
            public DateTime Date { get; set; }
            public int Consultations { get; set; }
            public int NewPatients { get; set; }
            public int Prescriptions { get; set; }
            public int Appointments { get; set; }
            public int AverageDuration { get; set; }
            public int SatisfactionRate { get; set; }
        }
        
        public class StaffStatistics
        {
            public string StaffId { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public int Consultations { get; set; }
            public int Prescriptions { get; set; }
            public int Appointments { get; set; }
        }
        
        public class TopCondition
        {
            public string Condition { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}