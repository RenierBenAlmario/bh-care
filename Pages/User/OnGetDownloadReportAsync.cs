using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Barangay.Pages.User
{
    public partial class UserDashboardModel
    {
        // This method intercepts the DownloadReport handler to provide a placeholder report if the table doesn't exist
        public async Task<IActionResult> OnGetDownloadReportAsync(int reportId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                try
                {
                    // First try to get the real report
                    var report = await _context.HealthReports
                        .Include(hr => hr.Doctor)
                        .FirstOrDefaultAsync(hr => hr.Id == reportId && hr.UserId == user.Id);

                    if (report != null)
                    {
                        var reportContent = $@"Health Report - {report.CheckupDate:MMMM dd, yyyy}
Blood Pressure: {report.BloodPressure}
Heart Rate: {report.HeartRate} BPM
Blood Sugar: {report.BloodSugar} mg/dL
Weight: {report.Weight} lbs
Temperature: {report.Temperature}°C
Physical Activity: {report.PhysicalActivity}
Notes: {report.Notes}
Doctor: {report.Doctor?.FirstName} {report.Doctor?.LastName}";

                        byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(reportContent);
                        string fileName = $"health_report_{report.CheckupDate:yyyy_MM_dd}.txt";
                        
                        return File(fileBytes, "text/plain", fileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to download real health report, using placeholder: {ex.Message}");
                }

                // If the real report can't be retrieved or doesn't exist, return a placeholder
                var placeholderContent = $@"Health Report - {DateTime.Now:MMMM dd, yyyy}
Note: This is a placeholder health report.
The health reports functionality is currently being set up.
Please contact your healthcare provider for actual health information.";

                byte[] placeholderBytes = System.Text.Encoding.UTF8.GetBytes(placeholderContent);
                string placeholderFileName = $"health_report_placeholder_{DateTime.Now:yyyy_MM_dd}.txt";
                
                return File(placeholderBytes, "text/plain", placeholderFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading health report");
                return RedirectToPage();
            }
        }
    }
} 