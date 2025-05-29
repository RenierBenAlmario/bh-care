using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class ConsultationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConsultationModel> _logger;
        private readonly IConfiguration _configuration;

        public ConsultationModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ConsultationModel> logger,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public string? PatientId { get; set; }

        [BindProperty]
        public string? ChiefComplaint { get; set; }

        [BindProperty]
        public string? Diagnosis { get; set; }

        [BindProperty]
        public string? Treatment { get; set; }

        [BindProperty]
        public string? Notes { get; set; }

        public Patient? Patient { get; set; }
        public VitalSign? LatestVitalSigns { get; set; }
        public Barangay.Models.Appointment? Appointment { get; set; }
        
        [BindProperty]
        public int? AppointmentId { get; set; }
        
        [BindProperty]
        public List<PrescriptionMedicationViewModel> Medications { get; set; } = new();

        public List<MedicalRecord> MedicalRecords { get; set; } = new();
        
        // Flag to determine if all data is properly loaded
        public bool IsDataLoaded { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(string? patientId = null, int? id = null, bool startConsultation = false)
        {
            try
            {
                // Get the current authenticated doctor
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    _logger.LogWarning("No authenticated user found");
                    TempData["ErrorMessage"] = "Authentication error. Please log in again.";
                    return RedirectToPage("/Doctor/Appointments");
                }

                // Handle the case where patientId might be null
                if (string.IsNullOrEmpty(patientId) && id.HasValue)
                {
                    // Try to get the patientId from the appointment id
                    var appointment = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
                    if (appointment != null)
                    {
                        patientId = appointment.PatientId;
                    }
                }

                if (string.IsNullOrEmpty(patientId))
                {
                    TempData["ErrorMessage"] = "No patient selected. Please select a patient.";
                    return RedirectToPage("/Doctor/Appointments");
                }
                
                PatientId = patientId;
                AppointmentId = id;

                // Get patient information with null safety
                Patient = await _context.Patients
                    .Include(p => p.User)
                    .Include(p => p.VitalSigns)
                    .FirstOrDefaultAsync(p => p.UserId == patientId);
                    
                if (Patient == null)
                {
                    _logger.LogWarning("Patient not found with ID: {PatientId}", patientId);
                    TempData["ErrorMessage"] = "Patient not found.";
                    return RedirectToPage("/Doctor/Appointments");
                }

                // Get latest vital signs with null check
                LatestVitalSigns = Patient.VitalSigns?
                    .OrderByDescending(v => v.RecordedAt)
                    .FirstOrDefault();

                // Get appointment info if available
                if (id.HasValue)
                {
                    Appointment = await _context.Appointments
                        .Include(a => a.Doctor)
                        .FirstOrDefaultAsync(a => a.Id == id.Value);

                    if (Appointment == null)
                    {
                        _logger.LogWarning("Appointment not found with ID: {AppointmentId}", id);
                        TempData["ErrorMessage"] = "Appointment not found.";
                        return RedirectToPage("/Doctor/Appointments");
                    }

                    // Ensure proper TimeSpan format with safety checks
                    EnsureValidTimeSpan(Appointment);
                    
                    // Manually set the Patient property since we already have it
                    Appointment.PatientId = Patient.UserId;
                    Appointment.PatientName = Patient.FullName ?? string.Empty;
                    
                    // Validate that the current doctor has permission to view this appointment
                    if (Appointment.DoctorId != currentUser.Id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                    {
                        _logger.LogWarning("Doctor {DoctorId} attempted to access appointment {AppointmentId} assigned to doctor {AssignedDoctorId}", 
                            currentUser.Id, id, Appointment.DoctorId);
                        TempData["ErrorMessage"] = "You do not have permission to view this appointment.";
                        return RedirectToPage("/Doctor/Appointments");
                    }
                }
                else
                {
                    Appointment = await _context.Appointments
                        .Include(a => a.Doctor)
                        .Where(a => a.PatientId == patientId && 
                               (a.Status == AppointmentStatus.InProgress || a.Status == AppointmentStatus.Pending))
                        .OrderByDescending(a => a.AppointmentDate)
                        .FirstOrDefaultAsync();

                    if (Appointment == null)
                    {
                        // Create a temporary appointment object with default values to prevent null reference exceptions
                        Appointment = new Barangay.Models.Appointment
                        {
                            PatientId = Patient.UserId,
                            PatientName = Patient.FullName ?? string.Empty,
                            DoctorId = currentUser.Id, // Set the current doctor as the appointment's doctor
                            AppointmentDate = DateTime.Today,
                            AppointmentTime = TimeSpan.FromHours(9),
                            Status = AppointmentStatus.Pending,
                            Type = "Consultation"
                        };
                        
                        _logger.LogWarning("No active appointment found for patient: {PatientId}. Using default values.", patientId);
                        TempData["WarningMessage"] = "No active appointment found for this patient. Using default values.";
                    }
                    else
                    {
                        // Ensure proper TimeSpan format with safety checks
                        EnsureValidTimeSpan(Appointment);
                        
                        // Manually set the Patient property since we already have it
                        Appointment.PatientId = Patient.UserId;
                        Appointment.PatientName = Patient.FullName ?? string.Empty;
                        
                        // Validate that the current doctor has permission to view this appointment
                        if (Appointment.DoctorId != currentUser.Id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                        {
                            _logger.LogWarning("Doctor {DoctorId} attempted to access appointment for patient {PatientId} assigned to doctor {AssignedDoctorId}", 
                                currentUser.Id, patientId, Appointment.DoctorId);
                            TempData["ErrorMessage"] = "You do not have permission to view this appointment.";
                            return RedirectToPage("/Doctor/Appointments");
                        }
                    }
                }

                // Load medical records with null check
                if (!string.IsNullOrEmpty(Appointment.PatientId))
                {
                    MedicalRecords = await _context.MedicalRecords
                        .Include(r => r.Doctor)
                        .Include(r => r.Patient)
                        .Include(r => r.Medications) // Include related medications
                        .Where(r => r.PatientId == Appointment.PatientId)
                        .OrderByDescending(r => r.Date)
                        .ToListAsync();
                    
                    // If there are medical records, load medications from the most recent one
                    if (MedicalRecords.Any())
                    {
                        var latestMedicalRecord = MedicalRecords.FirstOrDefault();
                        if (latestMedicalRecord != null && !string.IsNullOrEmpty(latestMedicalRecord.Medications))
                        {
                            // Load medications from the latest medical record
                            try
                            {
                                var medicationsList = System.Text.Json.JsonSerializer.Deserialize<List<PrescriptionMedication>>(
                                    latestMedicalRecord.Medications
                                );
                                
                                if (medicationsList != null)
                                {
                                    Medications = medicationsList.Select(m => new PrescriptionMedicationViewModel
                                    {
                                        Id = m.Id,
                                        MedicationName = m.MedicationName,
                                        Dosage = m.Dosage,
                                        Instructions = m.Instructions
                                    }).ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error deserializing medications from medical record {Id}", latestMedicalRecord.Id);
                                Medications = new List<PrescriptionMedicationViewModel>();
                            }
                        }
                    }
                }

                // Start consultation if requested
                if (startConsultation && Appointment != null && Appointment.Status == AppointmentStatus.Pending)
                {
                    Appointment.Status = AppointmentStatus.InProgress;
                    Appointment.UpdatedAt = DateTime.Now;
                    _context.Appointments.Update(Appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Consultation started successfully!";
                }

                // If there's TempData for form values, restore them
                if (TempData["ChiefComplaint"] != null) ChiefComplaint = TempData["ChiefComplaint"].ToString();
                if (TempData["Diagnosis"] != null) Diagnosis = TempData["Diagnosis"].ToString();
                if (TempData["Treatment"] != null) Treatment = TempData["Treatment"].ToString();
                if (TempData["Notes"] != null) Notes = TempData["Notes"].ToString();
                if (TempData["Medications"] != null)
                {
                    try
                    {
                        Medications = System.Text.Json.JsonSerializer.Deserialize<List<PrescriptionMedicationViewModel>>(
                            TempData["Medications"]?.ToString() ?? "[]"
                        ) ?? new List<PrescriptionMedicationViewModel>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing medications from TempData");
                        Medications = new List<PrescriptionMedicationViewModel>();
                    }
                }
                
                // Set flag to indicate data is loaded successfully
                IsDataLoaded = true;
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultation data for patient {PatientId}", patientId);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading patient consultation data.";
                return RedirectToPage("/Doctor/Appointments");
            }
        }

        // Helper method to ensure TimeSpan is valid
        private void EnsureValidTimeSpan(Barangay.Models.Appointment appointment)
        {
            if (appointment == null) return;
            
            try
            {
                // If TimeSpan is default or invalid, set a default time
                if (appointment.AppointmentTime == default)
                {
                    appointment.AppointmentTime = TimeSpan.FromHours(9); // Default to 9 AM
                }
                
                // Ensure the TimeSpan is in a valid range (0-24 hours)
                if (appointment.AppointmentTime.TotalHours < 0 || appointment.AppointmentTime.TotalHours >= 24)
                {
                    appointment.AppointmentTime = TimeSpan.FromHours(9); // Default to 9 AM if out of range
                    _logger.LogWarning("Invalid appointment time corrected for appointment ID: {AppointmentId}", appointment.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing TimeSpan for appointment ID: {AppointmentId}", appointment.Id);
                appointment.AppointmentTime = TimeSpan.FromHours(9); // Default to 9 AM on error
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Get the current authenticated doctor
                var currentDoctor = await _userManager.GetUserAsync(User);
                if (currentDoctor == null)
                {
                    ModelState.AddModelError("", "Authentication error. Please log in again.");
                    return Page();
                }

                // Double-check doctor ID using claims as a fallback
                string doctorId = currentDoctor.Id;
                if (string.IsNullOrEmpty(doctorId))
                {
                    doctorId = GetCurrentDoctorId();
                    if (string.IsNullOrEmpty(doctorId))
                    {
                        ModelState.AddModelError("", "Unable to identify the current doctor. Please log in again.");
                        return Page();
                    }
                }

                // Validate the Dosage fields of all medications
                for (int i = 0; i < Medications.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Medications[i].MedicationName))
                    {
                        // Only validate medications that have a name entered
                        if (!string.IsNullOrEmpty(Medications[i].Dosage))
                        {
                            // Validate that the dosage is a valid number
                            if (!decimal.TryParse(Medications[i].Dosage, out _))
                            {
                                ModelState.AddModelError($"Medications[{i}].Dosage", "Dosage must be a valid number");
                            }
                        }
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Log validation errors for debugging
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogWarning("Model validation error: {ErrorMessage}", error.ErrorMessage);
                        }
                    }
                    return Page();
                }

                if (string.IsNullOrEmpty(PatientId))
                {
                    ModelState.AddModelError("", "Patient ID is required");
                    return Page();
                }

                if (string.IsNullOrEmpty(ChiefComplaint) || 
                    string.IsNullOrEmpty(Diagnosis) || 
                    string.IsNullOrEmpty(Treatment))
                {
                    ModelState.AddModelError("", "Please fill in all required fields");
                    return Page();
                }

                // Verify doctor exists in database
                var doctorId_string = doctorId.ToString();
                var doctorExists = await _context.Users.AnyAsync(u => u.Id.ToString() == doctorId_string);
                if (!doctorExists)
                {
                    _logger.LogError("Doctor with ID {DoctorId} does not exist in the database", doctorId);
                    ModelState.AddModelError("", "Doctor account not found in the system. Please contact an administrator.");
                    return Page();
                }

                // Verify patient exists in database
                string patientIdString = PatientId;
                var patientExists = await _context.Users.AnyAsync(u => u.Id.ToString() == patientIdString);
                if (!patientExists)
                {
                    _logger.LogError("Patient with ID {PatientId} does not exist in the database", PatientId);
                    ModelState.AddModelError("", "Patient not found in the system.");
                    return Page();
                }

                // Get appointment type for the record if available
                string consultationType = "Consultation";
                if (AppointmentId.HasValue)
                {
                    var appointment = await _context.Appointments.FindAsync(AppointmentId.Value);
                    if (appointment != null && !string.IsNullOrEmpty(appointment.Type))
                    {
                        consultationType = appointment.Type;
                    }
                }

                var medicalRecord = new MedicalRecord
                {
                    PatientId = PatientId,
                    DoctorId = doctorId, // Use the verified doctor ID
                    Date = DateTime.Now,
                    ChiefComplaint = ChiefComplaint,
                    Diagnosis = Diagnosis,
                    Treatment = Treatment,
                    Notes = Notes ?? string.Empty,
                    Type = consultationType, // Use the appointment type or default
                    Status = "Active"
                };

                // Begin transaction to ensure all operations succeed or fail together
                using var transaction = await _context.Database.BeginTransactionAsync();
                
                try
                {
                    _context.MedicalRecords.Add(medicalRecord);
                    await _context.SaveChangesAsync();
            
                    // Process medications (only add non-empty ones)
                    var validMedications = Medications
                        .Where(m => !string.IsNullOrEmpty(m.MedicationName))
                        .Select(m => new PrescriptionMedication
                        {
                            MedicalRecordId = medicalRecord.Id,
                            MedicationName = m.MedicationName ?? string.Empty,
                            Dosage = m.Dosage ?? string.Empty,
                            Instructions = m.Instructions ?? string.Empty
                        })
                        .ToList();
                    
                    if (validMedications.Any())
                    {
                        foreach (var medication in validMedications)
                        {
                            _context.PrescriptionMedications.Add(medication);
                        }
                        await _context.SaveChangesAsync();
                    }
            
                    if (AppointmentId.HasValue)
                    {
                        var appointment = await _context.Appointments.FindAsync(AppointmentId.Value);
                        if (appointment != null)
                        {
                            appointment.Status = AppointmentStatus.Completed;
                            appointment.UpdatedAt = DateTime.Now;
                            _context.Appointments.Update(appointment);
                            await _context.SaveChangesAsync();
                        }
                    }
                    
                    // Commit the transaction
                    await transaction.CommitAsync();
            
                    // Send email notification
                    var patient = await _context.Patients
                        .Include(p => p.User)
                        .FirstOrDefaultAsync(p => p.UserId == PatientId);
            
                    if (patient?.User?.Email != null)
                    {
                        await SendConsultationSummaryEmailAsync(
                            patient.User.Email,
                            patient.FullName ?? "Patient",
                            medicalRecord,
                            validMedications);
                    }
                }
                catch (Exception ex)
                {
                    // Roll back the transaction on error
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Transaction rolled back: Error saving medical record for patient: {PatientId}", PatientId);
                    throw; // Re-throw to be caught by the outer try-catch
                }

                TempData["SuccessMessage"] = "Medical record saved successfully";
                return RedirectToPage("/Doctor/Appointments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving medical record for patient: {PatientId}", PatientId);
                ModelState.AddModelError("", $"Error saving medical record: {ex.Message}");
                
                // Attempt to reload the page data to avoid "Unable to load consultation data" message
                IsDataLoaded = false;
                
                // Try to reload patient and appointment data
                if (!string.IsNullOrEmpty(PatientId))
                {
                    Patient = await _context.Patients
                        .Include(p => p.User)
                        .Include(p => p.VitalSigns)
                        .FirstOrDefaultAsync(p => p.UserId == PatientId);
                        
                    if (AppointmentId.HasValue)
                    {
                        Appointment = await _context.Appointments
                            .Include(a => a.Doctor)
                            .FirstOrDefaultAsync(a => a.Id == AppointmentId.Value);
                            
                        if (Appointment != null)
                        {
                            IsDataLoaded = true;
                        }
                    }
                }
                
                return Page();
            }
        }

        private async Task SendConsultationSummaryEmailAsync(
            string toEmail, 
            string patientName, 
            MedicalRecord record,
            List<PrescriptionMedication> medications)
        {
            try
            {
                var smtpSection = _configuration.GetSection("Smtp");
                var smtpHost = smtpSection["Host"] ?? "localhost";
                var smtpPort = int.TryParse(smtpSection["Port"], out int port) ? port : 25;
                var smtpUser = smtpSection["User"] ?? string.Empty;
                var smtpPassword = smtpSection["Password"] ?? string.Empty;
                var fromEmail = smtpSection["FromEmail"] ?? "noreply@barangayhealth.com";

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUser, smtpPassword),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = "Consultation Summary",
                    Body = GenerateEmailBody(patientName, record, medications),
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                _logger.LogInformation("Consultation summary email sent to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send consultation summary email to {Email}", toEmail);
                // Don't throw the exception - just log it and continue
            }
        }

        private string GenerateEmailBody(
            string patientName, 
            MedicalRecord record, 
            List<PrescriptionMedication> medications)
        {
            var medicationsList = medications.Any()
                ? string.Join("", medications.Select(m => $"<li>{m.MedicationName} - {m.Dosage} - {m.Instructions}</li>"))
                : "<li>No medications prescribed</li>";
                
            return $@"
                <h2>Consultation Summary for {patientName}</h2>
                <p><strong>Date:</strong> {record.Date:MMM dd, yyyy}</p>
                <p><strong>Chief Complaint:</strong> {record.ChiefComplaint}</p>
                <p><strong>Diagnosis:</strong> {record.Diagnosis}</p>
                <p><strong>Treatment:</strong> {record.Treatment}</p>
                <p><strong>Notes:</strong> {record.Notes}</p>
                
                <h3>Prescribed Medications:</h3>
                <ul>
                    {medicationsList}
                </ul>
            ";
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            if (Enum.TryParse<AppointmentStatus>(status, out var newStatus))
            {
                appointment.Status = newStatus;
                appointment.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteConsultationAsync(int id, string diagnosis, string prescription, string instructions)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            var medicalRecord = new MedicalRecord
            {
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                Date = DateTime.Now,
                Diagnosis = diagnosis,
                Prescription = prescription,
                Instructions = instructions,
                Status = AppointmentStatus.Completed.ToString()
            };

            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.Now;

            _context.MedicalRecords.Add(medicalRecord);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        // Helper method to get the current doctor's ID from claims
        private string GetCurrentDoctorId()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return string.Empty;
            }
            
            // Get user ID from the nameidentifier claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unable to get user ID from claims");
                return string.Empty;
            }
            
            // Verify the user is in the Doctor role
            var isDoctor = User.IsInRole("Doctor");
            if (!isDoctor)
            {
                _logger.LogWarning("User {UserId} is not in the Doctor role", userId);
                return string.Empty;
            }
            
            return userId;
        }

        public class PrescriptionMedicationViewModel
        {
            public int? Id { get; set; }
            
            [Display(Name = "Medicine Name")]
            public string? MedicationName { get; set; }
            
            [Display(Name = "Dosage")]
            [RegularExpression(@"^[0-9]*\.?[0-9]+$", ErrorMessage = "Dosage must be a valid number")]
            public string? Dosage { get; set; }
            
            [Display(Name = "Instructions")]
            public string? Instructions { get; set; }
        }
    }
}
