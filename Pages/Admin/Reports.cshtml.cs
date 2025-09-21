using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Barangay.Data;
using Barangay.Services;
using Barangay.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Barangay.Pages.Admin
{
        public class ReportData
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public decimal Value { get; set; }
        public string Trend { get; set; }
    }

    public class SicknessReportData
    {
        public string Sickness { get; set; }
        public int Count { get; set; }
    }

    public class AdminReportsViewModel
    {
        public DateTime CurrentPhilippineTime { get; set; }
        public List<ReportData> PatientRegistrations { get; set; }
        public List<ReportData> ConsultationsByType { get; set; }
        public List<ReportData> HealthIndexData { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalConsultations { get; set; }
        public decimal AverageHealthIndex { get; set; }
        public string HealthIndexTrend { get; set; }
        public List<SicknessReportData> MonthlySicknessSummary { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public List<int> AvailableYears { get; set; }
    }

    [Authorize(Roles = "Admin")]
    public class ReportsModel : AdminPageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsModel> _logger;

        public ReportsModel(
            ApplicationDbContext context,
            INotificationService notificationService,
            ILogger<ReportsModel> logger)
            : base(notificationService)
        {
            _context = context;
            _logger = logger;
        }

        public AdminReportsViewModel ReportData { get; private set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedYear { get; set; }

        [BindProperty(SupportsGet = true)]
        public int SelectedMonth { get; set; }

        private DateTime GetPhilippineTime()
        {
            var utcNow = DateTime.UtcNow;
            TimeZoneInfo philippineZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, philippineZone);
        }

        public async Task<IActionResult> OnGetAsync(int? year, int? month)
        {
            try
            {
                var philippineTime = GetPhilippineTime();
                SelectedYear = year ?? philippineTime.Year;
                SelectedMonth = month ?? philippineTime.Month;

                var availableYears = await _context.Appointments
                    .Select(a => a.AppointmentDate.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToListAsync();

                if (!availableYears.Any())
                {
                    availableYears.Add(philippineTime.Year);
                }

                var monthlySicknessSummary = await _context.Appointments
                    .Where(a => a.AppointmentDate.Year == SelectedYear && a.AppointmentDate.Month == SelectedMonth && !string.IsNullOrEmpty(a.ReasonForVisit))
                    .GroupBy(a => a.ReasonForVisit)
                    .Select(g => new SicknessReportData
                    {
                        Sickness = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(d => d.Count)
                    .ToListAsync();

                ReportData = new AdminReportsViewModel
                {
                    CurrentPhilippineTime = philippineTime,
                    PatientRegistrations = new List<ReportData>(),
                    ConsultationsByType = new List<ReportData>(),
                    HealthIndexData = new List<ReportData>(),
                    TotalRegistrations = 0,
                    TotalConsultations = 0,
                    AverageHealthIndex = 0,
                    HealthIndexTrend = "0%",
                    MonthlySicknessSummary = monthlySicknessSummary,
                    SelectedYear = SelectedYear,
                    SelectedMonth = SelectedMonth,
                    AvailableYears = availableYears
                };

                await LoadNotificationsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Reports page");
                TempData["ErrorMessage"] = "Failed to load reports. Please try again.";
                ReportData = new AdminReportsViewModel
                {
                    CurrentPhilippineTime = GetPhilippineTime(),
                    PatientRegistrations = new List<ReportData>(),
                    ConsultationsByType = new List<ReportData>(),
                    HealthIndexData = new List<ReportData>(),
                    MonthlySicknessSummary = new List<SicknessReportData>(),
                    AvailableYears = new List<int> { GetPhilippineTime().Year }
                };
                return Page();
            }
        }

        private void GenerateReportData()
        {
            // This method is kept for reference but not used anymore
            // In a real implementation, this would fetch actual data from the database
            _logger.LogInformation("Report data generation is disabled");
        }

        private new async Task LoadNotificationsAsync()
        {
            try
            {
                var notifications = await _notificationService.GetUnreadNotificationsAsync(User.Identity?.Name);
                ViewData["Notifications"] = notifications;
                ViewData["NotificationCount"] = notifications?.Count ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notifications");
                ViewData["Notifications"] = new List<Notification>();
                ViewData["NotificationCount"] = 0;
            }
        }
    }
} 