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

namespace Barangay.Pages.Doctor
{
    public class ReportsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public StatisticsInfo Statistics { get; set; } = new StatisticsInfo();
        public List<DetailedStatInfo> DetailedStats { get; set; } = new List<DetailedStatInfo>();
        public List<StaffStatistics> StaffStats { get; set; } = new List<StaffStatistics>();
        public Dictionary<string, int> PatientDemographics { get; set; } = new Dictionary<string, int>();
        public List<TopCondition> TopConditions { get; set; } = new List<TopCondition>();
        
        [BindProperty(SupportsGet = true)]
        public string ReportType { get; set; } = "all";
        
        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public ReportsModel(
            IConfiguration configuration, 
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task OnGetAsync()
        {
            // Set default dates if not provided
            StartDate ??= DateTime.Now.AddDays(-30);
            EndDate ??= DateTime.Now;

            await LoadOverallStatisticsAsync();
            await LoadDetailedStatisticsAsync();
            await LoadStaffStatisticsAsync();
            await LoadPatientDemographicsAsync();
            await LoadTopConditionsAsync();
        }

        private async Task LoadOverallStatisticsAsync()
        {
            try
            {
                Statistics.TotalConsultations = await _context.MedicalRecords
                    .Where(m => m.Date >= StartDate && m.Date <= EndDate)
                    .CountAsync();

                Statistics.TotalPrescriptions = await _context.Prescriptions
                    .Where(p => p.CreatedAt >= StartDate && p.CreatedAt <= EndDate)
                    .CountAsync();

                Statistics.NewPatients = await _context.Patients
                    .Where(p => p.CreatedAt >= StartDate && p.CreatedAt <= EndDate)
                    .CountAsync();

                Statistics.TotalAppointments = await _context.Appointments
                    .Where(a => DateTimeHelper.IsDateGreaterThanOrEqual(a.AppointmentDate, StartDate ?? DateTime.MinValue) && 
                               DateTimeHelper.IsDateLessThanOrEqual(a.AppointmentDate, EndDate ?? DateTime.MaxValue))
                    .CountAsync();

                // Get Duration values first, then process them in memory
                var durationRecords = await _context.MedicalRecords
                    .Where(m => m.Date >= StartDate && m.Date <= EndDate)
                    .Select(m => new { m.Duration, m.Date })
                    .ToListAsync();

                // Process the Duration values in memory
                var validDurations = durationRecords
                    .Where(r => int.TryParse(r.Duration?.ToString(), out int duration) && duration > 0)
                    .Select(r => r.Duration)
                    .ToList();

                var avgDuration = validDurations.Any()
                    ? validDurations.Average(d => Convert.ToDouble(d))
                    : 0;
                Statistics.AverageConsultationTime = (int)Math.Round(avgDuration);

                Statistics.TotalDoctors = (await _userManager.GetUsersInRoleAsync("Doctor")).Count;
                Statistics.TotalNurses = (await _userManager.GetUsersInRoleAsync("Nurse")).Count;

                var feedbacks = await _context.Feedbacks
                    .Where(f => f.CreatedAt >= StartDate && f.CreatedAt <= EndDate)
                    .ToListAsync();
                Statistics.SatisfactionRate = feedbacks.Any()
                    ? (int)Math.Round(feedbacks.Average(f => f.Rating) * 20) // Convert 1-5 scale to percentage
                    : 0;
            }
            catch (Exception ex)
            {
                // Log the error but continue with other data loading
                Console.WriteLine($"Error loading overall statistics: {ex.Message}");
            }
        }

        private async Task LoadDetailedStatisticsAsync()
        {
            try
            {
                var dates = Enumerable.Range(0, 7)
                    .Select(i => DateTime.Now.AddDays(-i).Date)
                    .ToList();

                foreach (var date in dates)
                {
                    var stat = new DetailedStatInfo { Date = date };

                    stat.Consultations = await _context.MedicalRecords
                        .CountAsync(m => m.Date.Date == date);

                    stat.NewPatients = await _context.Patients
                        .CountAsync(p => p.CreatedAt.Date == date);

                    stat.Prescriptions = await _context.Prescriptions
                        .CountAsync(p => p.CreatedAt.Date == date);

                    stat.Appointments = await _context.Appointments
                        .CountAsync(a => DateTimeHelper.ParseDate(a.AppointmentDate.ToString()).Date == date.Date);

                    // Get Duration values first, then process them in memory
                    var dateDurationRecords = await _context.MedicalRecords
                        .Where(m => m.Date.Date == date)
                        .Select(m => new { m.Duration, m.Date })
                        .ToListAsync();

                    // Process the Duration values in memory
                    var dateValidDurations = dateDurationRecords
                        .Where(r => int.TryParse(r.Duration?.ToString(), out int duration) && duration > 0)
                        .Select(r => r.Duration)
                        .ToList();

                    var avgDuration = dateValidDurations.Any()
                        ? dateValidDurations.Average(d => Convert.ToDouble(d))
                        : 0;
                    stat.AverageDuration = (int)Math.Round(avgDuration);

                    var feedbacks = await _context.Feedbacks
                        .Where(f => f.CreatedAt.Date == date)
                        .ToListAsync();
                    stat.SatisfactionRate = feedbacks.Any()
                        ? (int)Math.Round(feedbacks.Average(f => f.Rating) * 20)
                        : 0;

                    DetailedStats.Add(stat);
                }

                DetailedStats = DetailedStats.OrderBy(s => s.Date).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading detailed statistics: {ex.Message}");
            }
        }

        private async Task LoadStaffStatisticsAsync()
        {
            try
            {
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                foreach (var doctor in doctors)
                {
                    var stat = new StaffStatistics
                    {
                        StaffId = doctor.Id,
                        Name = doctor.FullName ?? doctor.UserName ?? "Unknown",
                        Role = "Doctor"
                    };

                    stat.Consultations = await _context.MedicalRecords
                        .CountAsync(m => m.DoctorId == doctor.Id && m.Date >= StartDate && m.Date <= EndDate);

                    stat.Prescriptions = await _context.Prescriptions
                        .CountAsync(p => p.DoctorId == doctor.Id && p.CreatedAt >= StartDate && p.CreatedAt <= EndDate);

                    stat.Appointments = await _context.Appointments
                        .CountAsync(a => a.DoctorId == doctor.Id && 
                                   DateTimeHelper.IsDateGreaterThanOrEqual(a.AppointmentDate, StartDate ?? DateTime.MinValue) && 
                                   DateTimeHelper.IsDateLessThanOrEqual(a.AppointmentDate, EndDate ?? DateTime.MaxValue));

                    StaffStats.Add(stat);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading staff statistics: {ex.Message}");
            }
        }

        private async Task LoadPatientDemographicsAsync()
        {
            try
            {
                var demographics = await _context.Patients
                    .Where(p => p.Gender != null)
                    .GroupBy(p => p.Gender)
                    .Select(g => new { gender = g.Key, count = g.Count() })
                    .ToDictionaryAsync(g => g.gender ?? "Other", g => g.count);

                // Ensure all gender categories exist
                foreach (var gender in new[] { "Male", "Female", "Other" })
                {
                    if (!demographics.ContainsKey(gender))
                    {
                        demographics[gender] = 0;
                    }
                }

                PatientDemographics = demographics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patient demographics: {ex.Message}");
                PatientDemographics = new Dictionary<string, int>
                {
                    { "Male", 0 },
                    { "Female", 0 },
                    { "Other", 0 }
                };
            }
        }

        private async Task LoadTopConditionsAsync()
        {
            try
            {
                TopConditions = await _context.MedicalRecords
                    .Where(m => m.Date >= StartDate && m.Date <= EndDate && !string.IsNullOrEmpty(m.Diagnosis))
                    .GroupBy(m => m.Diagnosis)
                    .Select(g => new TopCondition
                    {
                        Condition = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(c => c.Count)
                    .Take(10)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading top conditions: {ex.Message}");
            }
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