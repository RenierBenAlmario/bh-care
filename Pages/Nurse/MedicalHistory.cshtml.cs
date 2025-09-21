using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Barangay.Extensions;
using Barangay.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace Barangay.Pages.Nurse
{
    [Authorize]
    public class MedicalHistoryModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<MedicalHistoryModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public MedicalHistoryModel(
            EncryptedDbContext context, 
            ILogger<MedicalHistoryModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        // Use Appointment model for patient information
        public List<Models.Appointment> Patients { get; set; } = new List<Models.Appointment>();
        public Models.Appointment? SelectedPatient { get; set; }
        public bool HasSelectedPatient { get; set; } = false;
        public List<Models.Appointment> PatientAppointments { get; set; } = new List<Models.Appointment>();
        public List<VitalSign> PatientVitalSigns { get; set; } = new List<VitalSign>();

        // Change this property to use the enum from Enums.cs
        public Barangay.Models.AppointmentStatus AppointmentStatus => Barangay.Models.AppointmentStatus.Pending;

        [BindProperty]
        public VitalSign InputVitalSign { get; set; } = new VitalSign();

        [BindProperty]
        public Models.MedicalHistory MedicalHistory { get; set; }

        [BindProperty]
        public string? Allergies { get; set; }

        [BindProperty]
        public string? CurrentMedications { get; set; }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public async Task<IActionResult> OnGetAsync(string? patientId = null, int? id = null)
        {
            try
            {
                // Check if user has permission to view patient history
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/Account/Login");
                }
                
                var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "View Patient History");
                
                if (!hasPermission)
                {
                    return RedirectToPage("/Account/AccessDenied");
                }

                if (id.HasValue)
                {
                    var appointment = await _context.Appointments
                        .FirstOrDefaultAsync(a => a.Id == id.Value);
                    if (appointment != null && !string.IsNullOrEmpty(appointment.PatientId))
                    {
                        patientId = appointment.PatientId;
                    }
                }

                // First get all appointments
                var appointments = await _context.Appointments.ToListAsync();
                var patients = await _context.Patients.ToListAsync();

                // Join and transform the data in memory
                var allAppointments = (from appointment in appointments
                                     join p in patients on appointment.PatientId equals p.UserId into patientGroup
                                     from patient in patientGroup.DefaultIfEmpty()
                                     select new Models.Appointment
                                     {
                                         Id = appointment.Id,
                                         PatientId = appointment.PatientId ?? string.Empty,
                                         PatientName = appointment.PatientName ?? "Unknown",
                                         Gender = patient?.Gender ?? string.Empty,
                                         ContactNumber = appointment.ContactNumber ?? string.Empty,
                                         DateOfBirth = patient?.BirthDate ?? default,
                                         Address = appointment.Address ?? string.Empty,
                                         EmergencyContact = appointment.EmergencyContact ?? string.Empty,
                                         EmergencyContactNumber = appointment.EmergencyContactNumber ?? string.Empty,
                                         Allergies = appointment.Allergies ?? string.Empty,
                                         MedicalHistory = appointment.MedicalHistory ?? string.Empty,
                                         CurrentMedications = appointment.CurrentMedications ?? string.Empty,
                                         AppointmentDate = appointment.AppointmentDate,
                                         AppointmentTime = SafeGetTimeSpan(appointment.AppointmentTime),
                                         AppointmentTimeInput = appointment.AppointmentTimeInput,
                                         DoctorId = appointment.DoctorId,
                                         ReasonForVisit = appointment.ReasonForVisit ?? string.Empty,
                                         Description = appointment.Description ?? string.Empty,
                                         Status = appointment.Status,
                                         AgeValue = (patient?.BirthDate ?? default) != default ? CalculateAge(patient.BirthDate) : 0,
                                         CreatedAt = appointment.CreatedAt,
                                         UpdatedAt = appointment.UpdatedAt,
                                         Type = appointment.Type ?? string.Empty,
                                         AttachmentPath = appointment.AttachmentPath ?? string.Empty,
                                         Prescription = appointment.Prescription ?? string.Empty,
                                         Instructions = appointment.Instructions ?? string.Empty
                                     }).ToList();

                Patients = allAppointments
                    .Where(a => !string.IsNullOrEmpty(a.PatientId))
                    .GroupBy(a => a.PatientId)
                    .Select(g => g.OrderByDescending(a => a.AppointmentDate).First())
                    .OrderBy(a => a.PatientName)
                    .ToList();

                if (!string.IsNullOrEmpty(patientId))
        {
            // Fetch the patient directly from the database to ensure the data is current.
            var patientFromDb = await _context.Patients.FindAsync(patientId);

            if (patientFromDb != null)
            {
                SelectedPatient = allAppointments
                    .FirstOrDefault(a => a.PatientId == patientId);

                if (SelectedPatient != null)
                {
                    // Replace projected data with actual DB data for consistency
                    SelectedPatient.MedicalHistory = patientFromDb.MedicalHistory;
                    SelectedPatient.Allergies = patientFromDb.Allergies;
                    SelectedPatient.CurrentMedications = patientFromDb.CurrentMedications;
                }

                HasSelectedPatient = true;
                PatientAppointments = allAppointments
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToList();

                try
                {
                    PatientVitalSigns = await GetVitalSignsForPatient(patientId);
                    _logger.LogInformation("Successfully retrieved {Count} vital signs for patient {PatientId}", PatientVitalSigns.Count, patientId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading vital signs for patient {PatientId}", patientId);
                    PatientVitalSigns = new List<VitalSign>();
                    TempData["ErrorMessage"] = "Error loading vital signs data. Please try again.";
                }
            }
            else
            {
                HasSelectedPatient = false;
            }
        }
                else
                {
                    HasSelectedPatient = false;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync");
                TempData["ErrorMessage"] = "An error occurred while loading the page. Please try again.";
                return Page();
            }
        }

        private async Task<List<VitalSign>> GetVitalSignsForPatient(string patientId)
        {
            var result = new List<VitalSign>();
            
            // Create a direct SQL connection to avoid EF Core type casting issues
            using (var connection = new SqlConnection(_context.Database.GetConnectionString()))
            {
                await connection.OpenAsync();
                
                var commandText = "SELECT Id, PatientId, Temperature, BloodPressure, HeartRate, RespiratoryRate, " +
                                 "SpO2, Weight, Height, RecordedAt, Notes FROM VitalSigns " +
                                 "WHERE PatientId = @PatientId ORDER BY RecordedAt DESC";
                
                using (var command = new SqlCommand(commandText, connection))
                {
                    // Add parameter to prevent SQL injection
                    command.Parameters.Add(new SqlParameter("@PatientId", patientId));
                    
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var vitalSign = new VitalSign
                            {
                                Id = reader.GetInt32(0),
                                PatientId = reader.GetString(1),
                                // Convert decimal values to strings
                                Temperature = reader.IsDBNull(2) ? null : reader.GetDecimal(2).ToString(),
                                BloodPressure = reader.IsDBNull(3) ? null : reader.GetString(3),
                                // Convert int values to strings
                                HeartRate = reader.IsDBNull(4) ? null : reader.GetInt32(4).ToString(),
                                RespiratoryRate = reader.IsDBNull(5) ? null : reader.GetInt32(5).ToString(),
                                // Convert decimal values to strings
                                SpO2 = reader.IsDBNull(6) ? null : reader.GetDecimal(6).ToString(),
                                Weight = reader.IsDBNull(7) ? null : reader.GetDecimal(7).ToString(),
                                Height = reader.IsDBNull(8) ? null : reader.GetDecimal(8).ToString(),
                                // DateTime is not nullable in the model
                                RecordedAt = reader.GetDateTime(9),
                                Notes = reader.IsDBNull(10) ? null : reader.GetString(10)
                            };
                            
                            result.Add(vitalSign);
                        }
                    }
                }
            }
            
            return result;
        }

        public async Task<IActionResult> OnPostSearchAsync(string searchTerm)
        {
            try
            {
                var patientId = Request.Query["patientId"].ToString();

                // First get all appointments
                var appointments = await _context.Appointments.ToListAsync();
                var patients = await _context.Patients.ToListAsync();

                // Join and transform the data in memory
                var allAppointments = appointments.Join(
                    patients,
                    a => a.PatientId,
                    p => p.UserId,
                    (appointment, patient) => new Models.Appointment
                    {
                        Id = appointment.Id,
                        PatientId = appointment.PatientId,
                        PatientName = appointment.PatientName,
                        Gender = patient.Gender,
                        ContactNumber = appointment.ContactNumber,
                        DateOfBirth = patient.BirthDate,
                        Address = appointment.Address,
                        EmergencyContact = appointment.EmergencyContact,
                        EmergencyContactNumber = appointment.EmergencyContactNumber,
                        Allergies = appointment.Allergies,
                        MedicalHistory = appointment.MedicalHistory,
                        CurrentMedications = appointment.CurrentMedications,
                        AppointmentDate = appointment.AppointmentDate,
                        AppointmentTime = SafeGetTimeSpan(appointment.AppointmentTime),
                        AppointmentTimeInput = appointment.AppointmentTimeInput,
                        DoctorId = appointment.DoctorId,
                        ReasonForVisit = appointment.ReasonForVisit,
                        Description = appointment.Description,
                        Status = appointment.Status,
                        AgeValue = patient.BirthDate != default ? CalculateAge(patient.BirthDate) : 0,
                        CreatedAt = appointment.CreatedAt,
                        UpdatedAt = appointment.UpdatedAt,
                        Type = appointment.Type,
                        AttachmentPath = appointment.AttachmentPath,
                        Prescription = appointment.Prescription,
                        Instructions = appointment.Instructions
                    }).ToList();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    allAppointments = allAppointments
                        .Where(a => a.PatientName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                               (a.ContactNumber != null && a.ContactNumber.Contains(searchTerm)))
                        .ToList();
                }

                Patients = allAppointments
                    .GroupBy(a => a.PatientId)
                    .Select(g => g.OrderByDescending(a => a.AppointmentDate).First())
                    .OrderBy(a => a.PatientName)
                    .ToList();

                if (!string.IsNullOrEmpty(patientId))
                {
                    SelectedPatient = allAppointments
                        .Where(a => a.PatientId == patientId)
                        .OrderByDescending(a => a.AppointmentDate)
                        .FirstOrDefault();

                    if (SelectedPatient != null)
                    {
                        HasSelectedPatient = true;
                        PatientAppointments = allAppointments
                            .Where(a => a.PatientId == patientId)
                            .OrderByDescending(a => a.AppointmentDate)
                            .ToList();
                        
                        try
                        {
                            // Use direct ADO.NET approach to avoid EF Core type casting issues
                            PatientVitalSigns = await GetVitalSignsForPatient(patientId);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error retrieving vital signs for patient {PatientId}", patientId);
                            PatientVitalSigns = new List<VitalSign>();
                            TempData["ErrorMessage"] = "Error loading vital signs data. Please try again.";
                        }
                    }
                    else
                    {
                        HasSelectedPatient = false;
                    }
                }
                else
                {
                    HasSelectedPatient = false;
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostSearchAsync");
                TempData["ErrorMessage"] = "An error occurred while searching. Please try again.";
                return Page();
            }
        }

        public async Task<IActionResult> OnPostUpdateMedicalHistoryAsync(string patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                TempData["ErrorMessage"] = "Patient ID is required.";
                return RedirectToPage();
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == patientId);
            if (patient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToPage(new { patientId });
            }

            // Find existing medical history or create a new one
            var medicalHistory = await _context.MedicalHistories.FirstOrDefaultAsync(m => m.PatientId == patientId);
            if (medicalHistory == null)
            {
                medicalHistory = new Models.MedicalHistory { PatientId = patientId };
                _context.MedicalHistories.Add(medicalHistory);
            }

            // Map form data to the medical history record
            MapMedicalHistoryFromForm(Request.Form);
            medicalHistory.ChiefComplaint = MedicalHistory.ChiefComplaint;
            medicalHistory.HistoryOfPresentIllness = MedicalHistory.HistoryOfPresentIllness;
            medicalHistory.PastMedicalHistory = MedicalHistory.PastMedicalHistory;
            medicalHistory.FamilyHistory = MedicalHistory.FamilyHistory;
            medicalHistory.PersonalSocialHistory = MedicalHistory.PersonalSocialHistory;
            medicalHistory.ReviewOfSystems = MedicalHistory.ReviewOfSystems;
            medicalHistory.PhysicalExamination = MedicalHistory.PhysicalExamination;

            // Also update allergies and current medications on the Patient record
            patient.Allergies = Allergies;
            patient.CurrentMedications = CurrentMedications;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Medical history updated successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical history for patient {PatientId}", patientId);
                TempData["ErrorMessage"] = "An error occurred while updating the medical history.";
            }

            return RedirectToPage(new { patientId });
        }

        public async Task<IActionResult> OnPostAddVitalSignsAsync(string patientId)
        {
            try
            {
                // Log relevant information to troubleshoot the issue
                _logger.LogInformation("Received request to add vital signs for patient ID: {PatientId}", patientId);
                _logger.LogInformation("Model binding state valid: {IsValid}", ModelState.IsValid);
                _logger.LogInformation("Vital sign input values: Temperature={Temperature}, HeartRate={HeartRate}, SpO2={SpO2}", 
                    InputVitalSign.Temperature, InputVitalSign.HeartRate, InputVitalSign.SpO2);
                
                if (string.IsNullOrEmpty(patientId))
                {
                    TempData["ErrorMessage"] = "No patient selected.";
                    _logger.LogWarning("Attempt to save vital signs without selecting a patient.");
                    return RedirectToPage();
                }

                // Initialize InputVitalSign.PatientId explicitly
                InputVitalSign.PatientId = patientId;

                // Check if patient exists in appointments
                var patientExists = await _context.Appointments
                    .AnyAsync(a => a.PatientId == patientId);
                    
                if (!patientExists)
                {
                    TempData["ErrorMessage"] = "Patient not found.";
                    _logger.LogWarning("Patient not found for ID: {PatientId}", patientId);
                    return RedirectToPage(new { patientId });
                }

                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning("Model error in {Key}: {Error}", state.Key, error.ErrorMessage);
                        }
                    }
                    TempData["ErrorMessage"] = "Please fill in all required vital sign fields.";
                    return RedirectToPage(new { patientId });
                }

                // Make sure Temperature is set (required field)
                if (string.IsNullOrEmpty(InputVitalSign.Temperature))
                {
                    TempData["ErrorMessage"] = "Temperature is required.";
                    return RedirectToPage(new { patientId });
                }

                // Create a new vital sign with explicit decimal conversion
                var newVitalSign = new VitalSign
                {
                    PatientId = patientId,
                    Temperature = InputVitalSign.Temperature,
                    BloodPressure = InputVitalSign.BloodPressure?.Trim(),
                    HeartRate = InputVitalSign.HeartRate,
                    RespiratoryRate = InputVitalSign.RespiratoryRate,
                    SpO2 = InputVitalSign.SpO2,
                    Weight = InputVitalSign.Weight,
                    Height = InputVitalSign.Height,
                    Notes = InputVitalSign.Notes?.Trim(),
                    RecordedAt = DateTime.Now
                };

                // Add debug logging for input values
                _logger.LogInformation("Saving vital sign: PatientId={PatientId}, Temperature={Temperature}, BloodPressure={BloodPressure}, HeartRate={HeartRate}", 
                    newVitalSign.PatientId, newVitalSign.Temperature, newVitalSign.BloodPressure, newVitalSign.HeartRate);

                // Check if we have an existing Patient record
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.UserId == patientId);
                    
                if (patient == null)
                {
                    // Get patient info from appointment
                    var appointment = await _context.Appointments
                        .Where(a => a.PatientId == patientId)
                        .OrderByDescending(a => a.AppointmentDate)
                        .FirstOrDefaultAsync();
                        
                    if (appointment != null)
                    {
                        // Create a new patient record based on appointment
                        patient = new Patient
                        {
                            UserId = patientId,
                            FullName = appointment.PatientName ?? "Unknown",
                            BirthDate = appointment.DateOfBirth ?? DateTime.MinValue,
                            Gender = appointment.Gender ?? string.Empty,
                            Address = appointment.Address ?? string.Empty,
                            ContactNumber = appointment.ContactNumber ?? string.Empty
                        };
                        
                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Created new patient record for PatientId: {PatientId}", patientId);
                    }
                }

                // Create a new vital sign record - EncryptedDbContext will handle encryption automatically
                var vitalSign = new VitalSign
                {
                    PatientId = patientId,
                    Temperature = InputVitalSign.Temperature,
                    BloodPressure = InputVitalSign.BloodPressure,
                    HeartRate = InputVitalSign.HeartRate,
                    RespiratoryRate = InputVitalSign.RespiratoryRate,
                    SpO2 = InputVitalSign.SpO2,
                    Weight = InputVitalSign.Weight,
                    Height = InputVitalSign.Height,
                    RecordedAt = InputVitalSign.RecordedAt,
                    Notes = InputVitalSign.Notes
                };

                // Add to context - EncryptedDbContext will handle encryption automatically
                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Vital sign saved successfully for {PatientId}", patientId);
                TempData["SuccessMessage"] = "Vital signs saved successfully!";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving vital signs for {PatientId}: {Message}", patientId, ex.InnerException?.Message);
                TempData["ErrorMessage"] = $"Database error: {ex.InnerException?.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving vital signs for {PatientId}: {Message}", patientId, ex.Message);
                TempData["ErrorMessage"] = "An unexpected error occurred while saving vital signs.";
            }
            
            // Reload the page with the correct patient
            return RedirectToPage(new { patientId });
        }

        // Helper class to compare appointments by PatientId
        private class AppointmentIdComparer : IEqualityComparer<Models.Appointment>
        {
            public bool Equals(Models.Appointment? x, Models.Appointment? y)
            {
                if (x == null || y == null) return false;
                return x.PatientId == y.PatientId;
            }
            
            public int GetHashCode(Models.Appointment obj)
            {
                if (obj == null) throw new ArgumentNullException(nameof(obj));
                return obj.PatientId?.GetHashCode() ?? 0;
            }
        }

        // Add a safe method to handle TimeSpan values that might cause formatting issues
        private TimeSpan SafeGetTimeSpan(TimeSpan input)
        {
            try
            {
                // Validate the TimeSpan by attempting to format it
                var test = input.ToString(@"hh\:mm");
                return input;
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "TimeSpan format error. Using default value instead.");
                return TimeSpan.Zero; // Return a safe default
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing TimeSpan. Using default value instead.");
                return TimeSpan.Zero; // Return a safe default
            }
        }

        private void MapMedicalHistoryFromForm(IFormCollection form)
        {
            MedicalHistory = new Models.MedicalHistory
            {
                ChiefComplaint = form["ChiefComplaint"].ToString() ?? string.Empty,
                HistoryOfPresentIllness = form["HistoryOfPresentIllness"].ToString() ?? string.Empty,
                PastMedicalHistory = form["PastMedicalHistory"].ToString() ?? string.Empty,
                FamilyHistory = form["FamilyHistory"].ToString() ?? string.Empty,
                PersonalSocialHistory = form["PersonalSocialHistory"].ToString() ?? string.Empty,
                ReviewOfSystems = form["ReviewOfSystems"].ToString() ?? string.Empty,
                PhysicalExamination = form["PhysicalExamination"].ToString() ?? string.Empty
            };
        }
    }
}
