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

        private DateTime GetPhilippineTime()
        {
            var utcNow = DateTime.UtcNow;
            TimeZoneInfo philippineZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            return TimeZoneInfo.ConvertTimeFromUtc(utcNow, philippineZone);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Initialize report data
                ReportData = new AdminReportsViewModel
                {
                    CurrentPhilippineTime = GetPhilippineTime(),
                    PatientRegistrations = new List<ReportData>(),
                    ConsultationsByType = new List<ReportData>(),
                    HealthIndexData = new List<ReportData>()
                };

                // Generate mock data
                GenerateReportData();

                // Load notifications
                await LoadNotificationsAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Reports page");
                TempData["ErrorMessage"] = "Failed to load reports. Please try again.";
                return Page();
            }
        }

        private void GenerateReportData()
        {
            var today = GetPhilippineTime().Date;
            var random = new Random(123); // Fixed seed for consistent mock data

            // Generate Patient Registrations data
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var count = random.Next(5, 25);
                ReportData.PatientRegistrations.Add(new ReportData
                {
                    Date = date,
                    Count = count,
                    Label = date.ToString("MMM dd")
                });
            }
            ReportData.TotalRegistrations = ReportData.PatientRegistrations.Sum(r => r.Count);

            // Generate Consultations by Type data
            var consultationTypes = new[] {
                ("General Checkup", 45),
                ("Vaccination", 30),
                ("Prenatal Care", 25),
                ("Dental", 20),
                ("Emergency", 15)
            };

            foreach (var (type, baseCount) in consultationTypes)
            {
                var variation = random.Next(-5, 6);
                ReportData.ConsultationsByType.Add(new ReportData
                {
                    Type = type,
                    Count = baseCount + variation,
                    Label = type,
                    Trend = $"{(variation >= 0 ? "+" : "")}{variation}%"
                });
            }
            ReportData.TotalConsultations = ReportData.ConsultationsByType.Sum(c => c.Count);

            // Generate Health Index data
            var baseIndex = 82.5m;
            for (int i = 29; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                var variation = (decimal)(random.NextDouble() * 2 - 1);
                var value = Math.Round(baseIndex + variation, 1);
                baseIndex = value; // Use previous value as new base

                ReportData.HealthIndexData.Add(new ReportData
                {
                    Date = date,
                    Value = value,
                    Label = date.ToString("MMM dd")
                });
            }

            ReportData.AverageHealthIndex = Math.Round(ReportData.HealthIndexData.Average(h => h.Value), 1);
            var trend = ReportData.HealthIndexData.Last().Value - ReportData.HealthIndexData.First().Value;
            ReportData.HealthIndexTrend = $"{(trend >= 0 ? "+" : "")}{trend:F1}%";
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