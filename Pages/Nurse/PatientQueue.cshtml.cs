using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Barangay.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    [Authorize(Policy = "PatientQueue")]
    public class PatientQueueModel : PageModel
    {
        private readonly ILogger<PatientQueueModel> _logger;
        private readonly string _connectionString;  // Changed back to private
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientQueueModel(ILogger<PatientQueueModel> logger, IConfiguration configuration, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found");
            _context = context;
            _userManager = userManager;
            QueuedAppointments = new List<AppointmentViewModel>();
        }

        public class AppointmentViewModel
        {
            // Database properties
            public int Id { get; set; }
            public string PatientId { get; set; } = string.Empty;
            public string PatientName { get; set; } = string.Empty;
            public string AppointmentDate { get; set; } = string.Empty;
            public TimeSpan? AppointmentTime { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public int QueueNumber { get; set; }
            
            // JSON serialization properties with lowercase naming for JavaScript
            [JsonPropertyName("appointment_id")]
            public int appointment_id => Id;
            
            [JsonPropertyName("patient_id")]
            public string patient_id => PatientId;
            
            [JsonPropertyName("patient_name")]
            public string patient_name => PatientName;
            
            [JsonPropertyName("appointment_date")]
            public string appointment_date => AppointmentDate;
            
            [JsonPropertyName("appointment_time")]
            public string appointment_time => AppointmentTime?.ToString(@"hh\:mm") ?? "";
            
            [JsonPropertyName("doctor_name")]
            public string doctor_name => DoctorName;
            
            [JsonPropertyName("appointment_status")]
            public string appointment_status => Status;
            
            [JsonPropertyName("queue_number")]
            public int queue_number => QueueNumber;
            
            public int? patientAge { get; set; }
            
            // For vital signs display
            public string? BloodPressure { get; set; }
            public string? HeartRate { get; set; }
            public string? Temperature { get; set; }
            public string? Notes { get; set; }
            
            // JSON serialization properties for vital signs
            [JsonPropertyName("blood_pressure")]
            public string? bloodPressure => BloodPressure;
            
            [JsonPropertyName("heart_rate")]
            public string? heartRate => HeartRate;
            
            [JsonPropertyName("body_temperature")]
            public string? temperature => Temperature;
            
            [JsonPropertyName("patient_notes")]
            public string? notes => Notes;
            
            // For display in details modal
            [JsonPropertyName("prescription_text")]
            public string? prescription => null;
            
            [JsonPropertyName("instructions_text")]
            public string? instructions => null;
        }

        public List<AppointmentViewModel> QueuedAppointments { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                // Get all appointments
                var allAppointments = await _context.Appointments.ToListAsync();
                // For each unique patient, get their latest appointment for the correct display name
                var latestAppointments = allAppointments
                    .GroupBy(a => a.PatientId)
                    .Select(g => g.OrderByDescending(a => a.AppointmentDate).First())
                    .ToDictionary(a => a.PatientId ?? string.Empty, a => a.PatientName ?? string.Empty);

                // Add a date filter to the database query
                QueuedAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == DateTime.UtcNow.Date && (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.InProgress))
                    .OrderBy(a => a.AppointmentTime)
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        PatientId = a.PatientId,
                        PatientName = latestAppointments.ContainsKey(a.PatientId) ? latestAppointments[a.PatientId] : a.PatientName,
                        AppointmentDate = a.AppointmentDate.ToDateString(),
                        AppointmentTime = a.AppointmentTime,
                        DoctorName = a.DoctorId, // We'll update this with the actual name below
                        QueueNumber = 0, // We'll assign sequential numbers below
                        Status = a.Status.ToString()
                    })
                    .ToListAsync();

                // Assign sequential queue numbers
                for (int i = 0; i < QueuedAppointments.Count; i++)
                {
                    QueuedAppointments[i].QueueNumber = i + 1;
                }

                // Load doctor names
                var doctorIds = QueuedAppointments.Select(a => a.DoctorName).Distinct().ToList();
                var doctors = await _context.Users
                    .Where(u => doctorIds.Contains(u.Id.ToString()))
                    .ToListAsync();

                // Create a dictionary with proper casting to ApplicationUser
                var doctorsDictionary = doctors
                    .Select(u => {
                        // Check if u is ApplicationUser before casting
                        string fullName = "Unknown";
                        string userName = "Unknown";
                        
                        // Try to find the full user info using the ID
                        var appUser = _userManager.FindByIdAsync(u.Id.ToString()).Result;
                        if (appUser != null)
                        {
                            fullName = appUser.FullName ?? appUser.UserName ?? "Unknown";
                            userName = appUser.UserName ?? "Unknown";
                        }
                        // Otherwise use base User properties if available
                        else
                        {
                            // Try to access more generic property or just set a default
                            try
                            {
                                fullName = "Unknown User";
                                userName = "Unknown";
                            }
                            catch
                            {
                                // In case of any error, use default values
                                userName = "Unknown";
                                fullName = "Unknown";
                            }
                        }
                        
                        return new { 
                            Id = u.Id, 
                            Name = fullName
                        };
                    })
                    .ToDictionary(d => d.Id.ToString(), d => d.Name);

                // Update doctor names
                foreach (var appointment in QueuedAppointments)
                {
                    if (doctorsDictionary.TryGetValue(appointment.DoctorName, out var name))
                    {
                        appointment.DoctorName = name;
                    }
                }

                _logger.LogInformation($"Retrieved {QueuedAppointments.Count} appointments for today");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                ErrorMessage = "Error retrieving appointments. Please try again later.";
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                // Parse the status string to the AppointmentStatus enum
                if (Enum.TryParse<AppointmentStatus>(status, true, out var appointmentStatus))
                {
                    appointment.Status = appointmentStatus;
                    appointment.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Appointment status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Invalid status value: {status}";
                }
                
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment status: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Error updating appointment status. Please try again.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                // Set UpdatedAt before removing
                appointment.UpdatedAt = DateTime.Now;
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Appointment deleted successfully.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment {AppointmentId}", id);
                TempData["ErrorMessage"] = "Error deleting appointment. Please try again.";
                return RedirectToPage();
            }
        }
    }
}
