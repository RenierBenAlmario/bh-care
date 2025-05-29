using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Barangay.Pages.User
{
    [Authorize]
    public class UserDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDashboardModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public List<Barangay.Models.Appointment> TodayAppointments { get; set; } = new();
        public List<Barangay.Models.Appointment> UpcomingAppointments { get; set; } = new();
        public List<Barangay.Models.Appointment> PastAppointments { get; set; } = new();
        public HealthReport LatestHealthReport { get; set; }
        public int CurrentQueueCount { get; set; }
        public int EstimatedWaitTime { get; set; }
        public ApplicationUser CurrentUser { get; set; }

        public bool IsDoctor { get; set; }
        public bool IsNurse { get; set; }
        public bool IsPatient { get; set; }

        [BindProperty]
        public Barangay.Models.Appointment Appointment { get; set; } = new();

        public List<ApplicationUser> AvailableDoctors { get; set; } = new();

        public UserDashboardModel(ApplicationDbContext context, ILogger<UserDashboardModel> logger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToPage("/Account/Login");

                CurrentUser = user;
                IsDoctor = await _userManager.IsInRoleAsync(user, "Doctor");
                IsNurse = await _userManager.IsInRoleAsync(user, "Nurse");
                IsPatient = await _userManager.IsInRoleAsync(user, "Patient");

                // Load latest health report
                LatestHealthReport = await _context.HealthReports
                    .Where(hr => hr.UserId == user.Id)
                    .OrderByDescending(hr => hr.CheckupDate)
                    .FirstOrDefaultAsync();

                // Get current queue info
                var today = DateTime.Today;
                CurrentQueueCount = await _context.Appointments
                    .CountAsync(a => a.Status == AppointmentStatus.Pending && 
                                   a.AppointmentDate.Date == today);
                EstimatedWaitTime = CurrentQueueCount * 15; // 15 minutes per patient

                // Load available doctors
                var doctorRole = await _userManager.GetUsersInRoleAsync("Doctor");
                AvailableDoctors = doctorRole.ToList();

                await LoadAppointments(user.Id, today);

                // Load all appointments for the nurse dashboard
                if (IsNurse)
                {
                    var allAppointments = await _context.Appointments
                        .Include(a => a.Doctor)
                        .Include(a => a.Patient)
                        .ToListAsync();

                    // Pass the appointments to the view
                    ViewData["AllAppointments"] = allAppointments;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user dashboard");
                ModelState.AddModelError(string.Empty, "Error loading dashboard data.");
                return Page();
            }
        }

        public async Task<IActionResult> OnGetDownloadReportAsync(int reportId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var report = await _context.HealthReports
                    .Include(hr => hr.Doctor)
                    .FirstOrDefaultAsync(hr => hr.Id == reportId && hr.UserId == user.Id);

                if (report == null)
                {
                    return NotFound();
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading health report");
                return RedirectToPage();
            }
        }

        public async Task<JsonResult> OnGetRefreshAppointmentsAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var today = DateTime.Today;
                await LoadAppointments(user.Id, today);

                return new JsonResult(new
                {
                    success = true,
                    today = TodayAppointments,
                    upcoming = UpcomingAppointments,
                    recent = PastAppointments.Take(5)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing appointments");
                return new JsonResult(new { success = false, message = "Error refreshing appointments" });
            }
        }

        private async Task LoadAppointments(string userId, DateTime today)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return;

                var query = _context.Appointments.AsQueryable();

                if (IsDoctor)
                    query = query.Where(a => a.DoctorId == userId);
                else
                    query = query.Where(a => a.PatientId == userId);

                var allAppointments = await query.ToListAsync();

                TodayAppointments = allAppointments
                    .Where(a => a.AppointmentDate.Date == today.Date)
                    .OrderBy(a => a.AppointmentTime)
                    .ToList();

                UpcomingAppointments = allAppointments
                    .Where(a => a.AppointmentDate.Date > today.Date)
                    .OrderBy(a => a.AppointmentDate)
                    .ToList();

                PastAppointments = allAppointments
                    .Where(a => a.AppointmentDate.Date < today.Date)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointments");
            }
        }

        public async Task<IActionResult> OnPostBookAppointmentAsync(IFormFile? attachmentFile)
        {
            try
            {
                _logger.LogInformation("Starting appointment booking process.");

                var doctorRole = await _userManager.GetUsersInRoleAsync("Doctor");
                AvailableDoctors = doctorRole.ToList();

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found.");
                    throw new InvalidOperationException("User not found");
                }

                Appointment.PatientId = user.Id;
                // Remove the duplicate declaration and fix patientName handling:
                var patientName = user.UserName ?? user.Email ?? "Unknown";
                if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                {
                    patientName = $"{user.FirstName} {user.LastName}".Trim();
                }
                if(string.IsNullOrEmpty(patientName)) 
                {
                    patientName = user.UserName ?? user.Email ?? "Unknown";
                }
                
                // Store the PatientName from the form before reinitializing the Appointment object
                var patientNameFromForm = Appointment.PatientName;
                
                // Get the user's full name to use as default if no name is provided
                string userName;
                if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                {
                    userName = !string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName) 
                        ? $"{user.FirstName} {user.LastName}".Trim() 
                        : user.UserName ?? user.Email ?? "Unknown";
                }
                else
                {
                    userName = user.UserName ?? user.Email ?? "Unknown";
                }
                
                // Use the form-provided name if available, otherwise use the user's full name
                var finalPatientName = !string.IsNullOrWhiteSpace(patientNameFromForm) ? patientNameFromForm : userName;
                
                // When you reinitialize the Appointment object, include the PatientName
                Appointment = new Barangay.Models.Appointment 
                { 
                    PatientId = user.Id,
                    DoctorId = Appointment.DoctorId ?? string.Empty,
                    AppointmentDate = Appointment.AppointmentDate,
                    AppointmentTime = Appointment.AppointmentTime,
                    Description = string.Empty,
                    ReasonForVisit = Appointment.ReasonForVisit ?? string.Empty,
                    Type = Appointment.Type ?? string.Empty,
                    PatientName = finalPatientName, // Use the determined name
                    Status = Models.AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Later when creating newAppointment, use the preserved PatientName
                var newAppointment = new Barangay.Models.Appointment
                {
                    DoctorId = Appointment.DoctorId,
                    PatientId = Appointment.PatientId,
                    PatientName = Appointment.PatientName, // This will now have the final name value
                    AppointmentDate = Appointment.AppointmentDate,
                    AppointmentTime = Appointment.AppointmentTime,
                    ReasonForVisit = Appointment.ReasonForVisit,
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    AttachmentPath = Appointment.AttachmentPath,
                    Description = Appointment.Description,
                    Prescription = Appointment.Prescription,
                    Instructions = Appointment.Instructions,
                    Type = Appointment.Type
                };

                if (string.IsNullOrEmpty(Appointment.DoctorId))
                {
                    _logger.LogWarning("DoctorId is not provided.");
                    ModelState.AddModelError("Appointment.DoctorId", "Doctor is required");
                    await LoadAppointments(user.Id, DateTime.Today);
                    return Page();
                }

                var doctor = await _userManager.FindByIdAsync(Appointment.DoctorId);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor not found in AspNetUsers.");
                    ModelState.AddModelError("Appointment.DoctorId", "Selected doctor not found");
                    await LoadAppointments(user.Id, DateTime.Today);
                    return Page();
                }

                string? appointmentTimeString = Request.Form["Appointment.AppointmentTime"].ToString();
                if (!string.IsNullOrEmpty(appointmentTimeString) && Appointment.AppointmentTime == default)
                {
                    string[] timeParts = appointmentTimeString.Split(':');
                    if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int hours) && int.TryParse(timeParts[1], out int minutes))
                    {
                        Appointment.AppointmentTime = new TimeSpan(hours, minutes, 0);
                    }
                }

                Appointment.Status = AppointmentStatus.Pending;
                Appointment.CreatedAt = DateTime.Now;
                Appointment.UpdatedAt = DateTime.Now;

                ModelState.Clear();

                if (attachmentFile != null && attachmentFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "appointments");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(attachmentFile.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachmentFile.CopyToAsync(fileStream);
                    }

                    Appointment.AttachmentPath = $"/uploads/appointments/{uniqueFileName}";
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Remove this duplicate declaration - we already have newAppointment defined above
                        // var newAppointment = new Barangay.Models.Appointment
                        // {
                        //     DoctorId = Appointment.DoctorId,
                        //     PatientId = Appointment.PatientId,
                        //     PatientName = Appointment.PatientName,
                        //     AppointmentDate = Appointment.AppointmentDate,
                        //     AppointmentTime = Appointment.AppointmentTime,
                        //     ReasonForVisit = Appointment.ReasonForVisit,
                        //     Status = AppointmentStatus.Pending,
                        //     CreatedAt = DateTime.Now,
                        //     UpdatedAt = DateTime.Now,
                        //     AttachmentPath = Appointment.AttachmentPath,
                        //     Description = Appointment.Description,
                        //     Prescription = Appointment.Prescription,
                        //     Instructions = Appointment.Instructions,
                        //     Type = Appointment.Type
                        // };
                        
                        // Use the newAppointment that was already created earlier
                        _context.Appointments.Add(newAppointment);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        
                        _logger.LogInformation($"Appointment booked successfully. ID: {newAppointment.Id}");
                        TempData["SuccessMessage"] = "Appointment booked successfully!";
                        return RedirectToPage();
                    }
                    catch (Exception efEx)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(efEx, "Error with EF insertion: {Message}", efEx.Message);

                        if (efEx.InnerException is SqlException sqlEx && sqlEx.Number == 547)
                        {
                            ModelState.AddModelError("Appointment.DoctorId", "Selected doctor is not valid. Please choose another doctor.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, $"Error booking appointment: {efEx.Message}");
                        }

                        await LoadAppointments(user.Id, DateTime.Today);
                        return Page();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, $"Error booking appointment: {ex.Message}");
                
                // Fix: Add null check for User before accessing FindFirstValue
                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
                
                await LoadAppointments(userId, DateTime.Today);
                return Page();
            }
        }

        public async Task<JsonResult> OnGetAvailableDoctorsAsync()
        {
            try
            {
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");

                _logger.LogInformation($"Found {doctors.Count} doctors in role");

                var availableDoctors = doctors.Select(d => new
                {
                    id = d.Id,
                    name = d.FullName ?? d.Email ?? d.UserName,
                    specialization = "General"
                }).ToList();

                _logger.LogInformation($"Returning {availableDoctors.Count} doctors");

                return new JsonResult(availableDoctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available doctors: {Message}", ex.Message);
                return new JsonResult(new List<object>());
            }
        }

        public async Task<JsonResult> OnGetAvailableTimeSlotsAsync(string doctorId, string date)
        {
            try
            {
                if (string.IsNullOrEmpty(doctorId) || string.IsNullOrEmpty(date))
                    return new JsonResult(new List<string>());

                if (!DateTime.TryParse(date, out DateTime appointmentDate))
                    return new JsonResult(new List<string>());

                var bookedTimes = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == appointmentDate.Date)
                    .Select(a => a.AppointmentTime)
                    .ToListAsync();

                var availableTimeSlots = new List<string>();
                for (int hour = 9; hour <= 17; hour++)
                {
                    for (int minute = 0; minute < 60; minute += 30)
                    {
                        var time = new TimeSpan(hour, minute, 0);
                        if (!bookedTimes.Contains(time))
                            availableTimeSlots.Add($"{hour:D2}:{minute:D2}");
                    }
                }

                return new JsonResult(availableTimeSlots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading available time slots");
                return new JsonResult(new List<string>());
            }
        }
    }
}