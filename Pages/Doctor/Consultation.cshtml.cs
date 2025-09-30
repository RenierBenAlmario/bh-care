using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Helpers;
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
using Barangay.Attributes;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor,Head Doctor")]
    [Authorize(Policy = "Consultation")]
    public class ConsultationModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConsultationModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConsultationPdfService _consultationPdfService;
        private readonly IDataEncryptionService _encryptionService;
        private readonly IAppointmentReminderService _appointmentReminderService;

        public ConsultationModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ConsultationModel> logger,
            IConfiguration configuration,
            IConsultationPdfService consultationPdfService,
            IDataEncryptionService encryptionService,
            IAppointmentReminderService appointmentReminderService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _consultationPdfService = consultationPdfService;
            _encryptionService = encryptionService;
            _appointmentReminderService = appointmentReminderService;
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

        [BindProperty]
        public string? FollowUpReason { get; set; }

        [BindProperty]
        public DateTime? FollowUpDate { get; set; }

        [BindProperty]
        public TimeSpan? FollowUpTime { get; set; }

        [BindProperty]
        public string? Prescribe { get; set; }

        public Patient? Patient { get; set; }
        public VitalSign? LatestVitalSigns { get; set; }
        public Barangay.Models.Appointment? Appointment { get; set; }
        public HEEADSSSAssessment? HEEADSSSAssessment { get; set; }
        public NCDRiskAssessment? NCDRiskAssessment { get; set; }
        public AdolescentHealthInfo? AdolescentHealthInfo { get; set; }
        
        [BindProperty]
        public int? AppointmentId { get; set; }
        
        [BindProperty]
        public List<PrescriptionMedicationViewModel> Medications { get; set; } = new();

        public List<MedicalRecord> MedicalRecords { get; set; } = new();
        
        // Flag to determine if all data is properly loaded
        public bool IsDataLoaded { get; set; } = false;

        public List<Barangay.Models.Appointment> ConsultationQueue { get; set; } = new();

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

                _logger.LogInformation("Current user: {UserId}, Email: {Email}", currentUser.Id, currentUser.Email);

                if (!id.HasValue)
                {
                    // If no appointment ID is provided, show the list of patients waiting for consultation.
                    var doctorId = await GetCurrentDoctorIdWithFallbackAsync();
                    if (string.IsNullOrEmpty(doctorId))
                    {
                        _logger.LogWarning("Could not identify the current doctor. User ID: {UserId}", currentUser.Id);
                        TempData["ErrorMessage"] = "Could not identify the current doctor. Please ensure you are logged in as a doctor.";
                        return RedirectToPage("/Index");
                    }

                    _logger.LogInformation("Loading consultation queue for doctor: {DoctorId}", doctorId);
                    _logger.LogInformation("Today's date: {Today}", DateTimeHelper.Today);

                    // Filter: today and upcoming only, exclude draft/cancelled/completed, and only for current doctor
                    var today = DateTimeHelper.Today;
                    var validStatuses = new[] { AppointmentStatus.Pending, AppointmentStatus.Confirmed, AppointmentStatus.InProgress };

                    _logger.LogInformation("Looking for appointments for doctor {DoctorId} with Status IN: {Statuses} starting from {Today}", doctorId, string.Join(", ", validStatuses.Select(s => (int)s)), today);

                    ConsultationQueue = await _context.Appointments
                        .Include(a => a.Patient)
                            .ThenInclude(p => p.User)
                        .Where(a => a.DoctorId == doctorId
                                    && a.AppointmentDate.Date >= today
                                    && validStatuses.Contains(a.Status))
                        .OrderBy(a => a.AppointmentDate)
                        .ThenBy(a => a.AppointmentTime)
                        .ToListAsync();

                    _logger.LogInformation("Found {Count} appointments for today and upcoming", ConsultationQueue.Count);

                    // Decrypt patient names for all appointments
                    foreach (var appointment in ConsultationQueue)
                    {
                        if (appointment.Patient?.User != null)
                        {
                            appointment.Patient.User.DecryptSensitiveData(_encryptionService, User);
                        }
                        
                        // Decrypt the actual patient name from the appointment
                        if (!string.IsNullOrEmpty(appointment.PatientName) && _encryptionService.IsEncrypted(appointment.PatientName))
                        {
                            appointment.PatientName = _encryptionService.DecryptForUser(appointment.PatientName, User);
                        }
                        
                        // Decrypt dependent name if applicable
                        if (!string.IsNullOrEmpty(appointment.DependentFullName) && _encryptionService.IsEncrypted(appointment.DependentFullName))
                        {
                            appointment.DependentFullName = _encryptionService.DecryptForUser(appointment.DependentFullName, User);
                        }
                    }

                    // Log each appointment found for debugging
                    foreach (var appointment in ConsultationQueue)
                    {
                        _logger.LogInformation("Appointment: {Id}, Patient: {Patient}, Status: {Status}, Time: {Time}", 
                            appointment.Id, 
                            appointment.PatientName ?? "Unknown", 
                            appointment.Status, 
                            appointment.AppointmentTime);
                    }

                    // If no appointments found, provide some context for today/upcoming
                    if (ConsultationQueue.Count == 0)
                    {
                        var totalAppointmentsToday = await _context.Appointments
                            .Where(a => a.AppointmentDate.Date == today)
                            .CountAsync();

                        var appointmentsForOtherDoctors = await _context.Appointments
                            .Where(a => a.AppointmentDate.Date >= today && a.DoctorId != doctorId)
                            .CountAsync();

                        _logger.LogInformation("No appointments found for doctor {DoctorId}. Total appointments today: {TotalToday}, Appointments for other doctors today or upcoming: {OtherDoctors}", 
                            doctorId, totalAppointmentsToday, appointmentsForOtherDoctors);

                        if (totalAppointmentsToday > 0)
                        {
                            TempData["InfoMessage"] = $"No appointments found for you today. Today: {totalAppointmentsToday}, Assigned to other doctors (today or upcoming): {appointmentsForOtherDoctors}.";
                        }
                        else
                        {
                            TempData["InfoMessage"] = "No appointments scheduled for today.";
                        }
                    }

                    // Set IsDataLoaded to false when showing consultation queue (no specific appointment selected)
                    IsDataLoaded = false;
        
                    return Page();
                }

                // If an appointment ID is provided, load all data for the consultation form.
                AppointmentId = id.Value;
                _logger.LogInformation("Loading consultation data for appointment ID: {AppointmentId}", AppointmentId);
                
                Appointment = await _context.Appointments.FindAsync(AppointmentId.Value);

                if (Appointment == null)
                {
                     _logger.LogWarning("Appointment with ID {AppointmentId} not found.", AppointmentId);
                     TempData["ErrorMessage"] = "The selected appointment could not be found.";
                     return RedirectToPage("/Doctor/Appointments");
                }

                _logger.LogInformation("Appointment found: DoctorId={DoctorId}, PatientId={PatientId}, Status={Status}", 
                    Appointment.DoctorId, Appointment.PatientId, Appointment.Status);

                // Check appointment status and provide appropriate feedback
                if (Appointment.Status == AppointmentStatus.Completed)
                {
                    _logger.LogInformation("Loading completed appointment consultation data");
                }
                else if (Appointment.Status == AppointmentStatus.Pending || Appointment.Status == AppointmentStatus.Confirmed)
                {
                    _logger.LogInformation("Loading pending/confirmed appointment for consultation entry");
                    TempData["InfoMessage"] = "This is a pending appointment. Please complete the consultation form below.";
                }
                else if (Appointment.Status == AppointmentStatus.Cancelled)
                {
                    _logger.LogWarning("Attempting to access cancelled appointment");
                    TempData["WarningMessage"] = "This appointment has been cancelled and cannot be consulted.";
                     return RedirectToPage("/Doctor/Appointments");
                }

                patientId = Appointment.PatientId;

                Patient = await _context.Patients
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.UserId == patientId);

                if (Patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", patientId);
                    TempData["ErrorMessage"] = "Patient not found.";
                    return RedirectToPage("/Doctor/Appointments");
                }

                // Decrypt patient data for display
                Patient.DecryptSensitiveData(_encryptionService, User);
                
                // Decrypt user data if available
                if (Patient.User != null)
                {
                    Patient.User.DecryptSensitiveData(_encryptionService, User);
                }

                _logger.LogInformation("Patient found: {PatientName}", Patient.FullName);

                // Security check: ensure the doctor is authorized to view this appointment
                if (Appointment.DoctorId != currentUser.Id && !User.IsInRole("Admin"))
                {
                    _logger.LogWarning("Unauthorized access attempt. Appointment DoctorId: {AppointmentDoctorId}, Current User: {CurrentUserId}", 
                        Appointment.DoctorId, currentUser.Id);
                    
                    // Instead of blocking access, provide a warning and allow access for doctors
                    if (User.IsInRole("Doctor") || User.IsInRole("Head Doctor"))
                    {
                        _logger.LogInformation("Allowing access for doctor role despite ID mismatch");
                        TempData["WarningMessage"] = $"This appointment is assigned to a different doctor, but you have access as a doctor. Proceed with caution.";
                    }
                    else
                {
                    TempData["ErrorMessage"] = "You do not have permission to view this appointment.";
                    return RedirectToPage("/Doctor/Appointments");
                    }
                }

                LatestVitalSigns = await _context.VitalSigns
                    .Where(v => v.PatientId == patientId)
                    .OrderByDescending(v => v.RecordedAt)
                    .FirstOrDefaultAsync();

                // Decrypt vital signs for display
                if (LatestVitalSigns != null)
                {
                    LatestVitalSigns.DecryptVitalSignData(_encryptionService, User);
                }

                MedicalRecords = await _context.MedicalRecords
                    .Where(m => m.PatientId == patientId)
                    .OrderByDescending(m => m.Date)
                    .ToListAsync();

                // Decrypt medical records for display
                foreach (var record in MedicalRecords)
                {
                    record.DecryptSensitiveData(_encryptionService, User);
                }

                // Load HEEADSSS Assessment data
                HEEADSSSAssessment = await _context.HEEADSSSAssessments
                    .Where(h => h.UserId == patientId)
                    .OrderByDescending(h => h.CreatedAt)
                    .FirstOrDefaultAsync();

                // Decrypt HEEADSSS assessment data for display
                if (HEEADSSSAssessment != null)
                {
                    HEEADSSSAssessment.DecryptSensitiveData(_encryptionService, User);
                    
                    // Manual decryption fallback for all HEEADSSS fields
                    HEEADSSSAssessment.FullName = SafeDecrypt(HEEADSSSAssessment.FullName);
                    HEEADSSSAssessment.Age = SafeDecrypt(HEEADSSSAssessment.Age);
                    HEEADSSSAssessment.Gender = SafeDecrypt(HEEADSSSAssessment.Gender);
                    HEEADSSSAssessment.Address = SafeDecrypt(HEEADSSSAssessment.Address);
                    HEEADSSSAssessment.ContactNumber = SafeDecrypt(HEEADSSSAssessment.ContactNumber);
                    HEEADSSSAssessment.HomeEnvironment = SafeDecrypt(HEEADSSSAssessment.HomeEnvironment);
                    HEEADSSSAssessment.FamilyRelationship = SafeDecrypt(HEEADSSSAssessment.FamilyRelationship);
                    HEEADSSSAssessment.HomeFamilyProblems = SafeDecrypt(HEEADSSSAssessment.HomeFamilyProblems);
                    HEEADSSSAssessment.HomeParentalListening = SafeDecrypt(HEEADSSSAssessment.HomeParentalListening);
                    HEEADSSSAssessment.SchoolPerformance = SafeDecrypt(HEEADSSSAssessment.SchoolPerformance);
                    HEEADSSSAssessment.AttendanceIssues = SafeDecrypt(HEEADSSSAssessment.AttendanceIssues);
                    HEEADSSSAssessment.CareerPlans = SafeDecrypt(HEEADSSSAssessment.CareerPlans);
                    HEEADSSSAssessment.EducationCurrentlyStudying = SafeDecrypt(HEEADSSSAssessment.EducationCurrentlyStudying);
                    HEEADSSSAssessment.Hobbies = SafeDecrypt(HEEADSSSAssessment.Hobbies);
                    HEEADSSSAssessment.PhysicalActivity = SafeDecrypt(HEEADSSSAssessment.PhysicalActivity);
                    HEEADSSSAssessment.ScreenTime = SafeDecrypt(HEEADSSSAssessment.ScreenTime);
                    HEEADSSSAssessment.ActivitiesRegularExercise = SafeDecrypt(HEEADSSSAssessment.ActivitiesRegularExercise);
                    HEEADSSSAssessment.DietDescription = SafeDecrypt(HEEADSSSAssessment.DietDescription);
                    HEEADSSSAssessment.WeightConcerns = SafeDecrypt(HEEADSSSAssessment.WeightConcerns);
                    HEEADSSSAssessment.SubstanceUse = SafeDecrypt(HEEADSSSAssessment.SubstanceUse);
                    HEEADSSSAssessment.DrugsTobaccoUse = SafeDecrypt(HEEADSSSAssessment.DrugsTobaccoUse);
                    HEEADSSSAssessment.SuicidalThoughts = SafeDecrypt(HEEADSSSAssessment.SuicidalThoughts);
                    HEEADSSSAssessment.FeelsSafeAtHome = SafeDecrypt(HEEADSSSAssessment.FeelsSafeAtHome);
                    HEEADSSSAssessment.SexualActivity = SafeDecrypt(HEEADSSSAssessment.SexualActivity);
                    HEEADSSSAssessment.SexualOrientation = SafeDecrypt(HEEADSSSAssessment.SexualOrientation);
                    HEEADSSSAssessment.SexualityPregnancyExperience = SafeDecrypt(HEEADSSSAssessment.SexualityPregnancyExperience);
                    HEEADSSSAssessment.SexualitySTIExperience = SafeDecrypt(HEEADSSSAssessment.SexualitySTIExperience);
                    HEEADSSSAssessment.SexualityProtectionUse = SafeDecrypt(HEEADSSSAssessment.SexualityProtectionUse);
                    HEEADSSSAssessment.AssessmentNotes = SafeDecrypt(HEEADSSSAssessment.AssessmentNotes);
                    HEEADSSSAssessment.AssessedBy = SafeDecrypt(HEEADSSSAssessment.AssessedBy);
                }

                // Load NCD Risk Assessment data
                NCDRiskAssessment = await _context.NCDRiskAssessments
                    .Where(n => n.UserId == patientId)
                    .OrderByDescending(n => n.CreatedAt)
                    .FirstOrDefaultAsync();

                // Decrypt NCD assessment data for display
                if (NCDRiskAssessment != null)
                {
                    NCDRiskAssessment.DecryptSensitiveData(_encryptionService, User);
                    
                    // Manual decryption fallback for NCD fields
                    NCDRiskAssessment.FirstName = SafeDecrypt(NCDRiskAssessment.FirstName);
                    NCDRiskAssessment.LastName = SafeDecrypt(NCDRiskAssessment.LastName);
                    NCDRiskAssessment.MiddleName = SafeDecrypt(NCDRiskAssessment.MiddleName);
                    NCDRiskAssessment.Edad = SafeDecrypt(NCDRiskAssessment.Edad);
                    NCDRiskAssessment.Kasarian = SafeDecrypt(NCDRiskAssessment.Kasarian);
                    NCDRiskAssessment.Address = SafeDecrypt(NCDRiskAssessment.Address);
                    NCDRiskAssessment.Telepono = SafeDecrypt(NCDRiskAssessment.Telepono);
                    NCDRiskAssessment.Weight = SafeDecrypt(NCDRiskAssessment.Weight);
                    NCDRiskAssessment.Height = SafeDecrypt(NCDRiskAssessment.Height);
                    NCDRiskAssessment.BMI = SafeDecrypt(NCDRiskAssessment.BMI);
                    NCDRiskAssessment.Waist = SafeDecrypt(NCDRiskAssessment.Waist);
                    NCDRiskAssessment.SmokingStatus = SafeDecrypt(NCDRiskAssessment.SmokingStatus);
                    NCDRiskAssessment.AlcoholFrequency = SafeDecrypt(NCDRiskAssessment.AlcoholFrequency);
                    NCDRiskAssessment.AlcoholStoppedDuration = SafeDecrypt(NCDRiskAssessment.AlcoholStoppedDuration);
                    NCDRiskAssessment.IsBingeDrinker = SafeDecrypt(NCDRiskAssessment.IsBingeDrinker);
                    NCDRiskAssessment.ExerciseDuration = SafeDecrypt(NCDRiskAssessment.ExerciseDuration);
                    NCDRiskAssessment.HighSaltIntake = SafeDecrypt(NCDRiskAssessment.HighSaltIntake);
                    NCDRiskAssessment.Smoked100Sticks = SafeDecrypt(NCDRiskAssessment.Smoked100Sticks);
                    NCDRiskAssessment.FormerSmoker = SafeDecrypt(NCDRiskAssessment.FormerSmoker);
                    NCDRiskAssessment.FastingBloodSugar = SafeDecrypt(NCDRiskAssessment.FastingBloodSugar);
                    NCDRiskAssessment.RandomBloodSugar = SafeDecrypt(NCDRiskAssessment.RandomBloodSugar);
                    NCDRiskAssessment.CholesterolResult = SafeDecrypt(NCDRiskAssessment.CholesterolResult);
                    NCDRiskAssessment.LeftArmMeanBP = SafeDecrypt(NCDRiskAssessment.LeftArmMeanBP);
                    NCDRiskAssessment.RightArmMeanBP = SafeDecrypt(NCDRiskAssessment.RightArmMeanBP);
                    NCDRiskAssessment.BaselineBP = SafeDecrypt(NCDRiskAssessment.BaselineBP);
                    NCDRiskAssessment.IDNo = SafeDecrypt(NCDRiskAssessment.IDNo);
                    NCDRiskAssessment.InterviewedBy = SafeDecrypt(NCDRiskAssessment.InterviewedBy);
                    NCDRiskAssessment.Designation = SafeDecrypt(NCDRiskAssessment.Designation);
                    NCDRiskAssessment.RiskStatus = SafeDecrypt(NCDRiskAssessment.RiskStatus);
                    NCDRiskAssessment.ChestPain = SafeDecrypt(NCDRiskAssessment.ChestPain);
                    NCDRiskAssessment.CreatedAt = SafeDecrypt(NCDRiskAssessment.CreatedAt);
                }

                // Load Adolescent Health Information
                AdolescentHealthInfo = await _context.AdolescentHealthInfo
                    .Where(a => a.UserId == patientId)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                // Decrypt Adolescent Health Information data for display
                if (AdolescentHealthInfo != null)
                {
                    AdolescentHealthInfo.DecryptSensitiveData(_encryptionService, User);
                    
                    // Manual decryption fallback for Adolescent Health Info fields
                    AdolescentHealthInfo.PatientName = SafeDecrypt(AdolescentHealthInfo.PatientName);
                    AdolescentHealthInfo.PatientAge = SafeDecrypt(AdolescentHealthInfo.PatientAge);
                    AdolescentHealthInfo.PatientGender = SafeDecrypt(AdolescentHealthInfo.PatientGender);
                    AdolescentHealthInfo.PatientAddress = SafeDecrypt(AdolescentHealthInfo.PatientAddress);
                    AdolescentHealthInfo.PatientContact = SafeDecrypt(AdolescentHealthInfo.PatientContact);
                    AdolescentHealthInfo.HeightCm = SafeDecrypt(AdolescentHealthInfo.HeightCm);
                    AdolescentHealthInfo.WeightKg = SafeDecrypt(AdolescentHealthInfo.WeightKg);
                    AdolescentHealthInfo.BMI = SafeDecrypt(AdolescentHealthInfo.BMI);
                    AdolescentHealthInfo.BMICategory = SafeDecrypt(AdolescentHealthInfo.BMICategory);
                    AdolescentHealthInfo.MRMMRDateGiven = SafeDecrypt(AdolescentHealthInfo.MRMMRDateGiven);
                    AdolescentHealthInfo.TdDateGiven = SafeDecrypt(AdolescentHealthInfo.TdDateGiven);
                    AdolescentHealthInfo.HPVDateGiven = SafeDecrypt(AdolescentHealthInfo.HPVDateGiven);
                    AdolescentHealthInfo.Temperature = SafeDecrypt(AdolescentHealthInfo.Temperature);
                    AdolescentHealthInfo.BloodPressure = SafeDecrypt(AdolescentHealthInfo.BloodPressure);
                    AdolescentHealthInfo.PulseRate = SafeDecrypt(AdolescentHealthInfo.PulseRate);
                    AdolescentHealthInfo.RespiratoryRate = SafeDecrypt(AdolescentHealthInfo.RespiratoryRate);
                    AdolescentHealthInfo.ChiefComplaint = SafeDecrypt(AdolescentHealthInfo.ChiefComplaint);
                    AdolescentHealthInfo.WorkingDiagnosis = SafeDecrypt(AdolescentHealthInfo.WorkingDiagnosis);
                    AdolescentHealthInfo.ReferredTo = SafeDecrypt(AdolescentHealthInfo.ReferredTo);
                    AdolescentHealthInfo.RecordedBy = SafeDecrypt(AdolescentHealthInfo.RecordedBy);
                }

                _logger.LogInformation("Loaded {MedicalRecordsCount} medical records, vital signs: {HasVitalSigns}, HEEADSSS: {HasHEEADSSS}, NCD: {HasNCD}", 
                    MedicalRecords.Count, LatestVitalSigns != null, HEEADSSSAssessment != null, NCDRiskAssessment != null);

                // For pending appointments, it's normal to have no medical records yet
                if (MedicalRecords.Count == 0)
                {
                    _logger.LogInformation("No medical records found for patient {PatientId}. This is normal for pending appointments.", patientId);
                }

                // Set flag to indicate data is loaded successfully
                IsDataLoaded = true;
        
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultation data for patient {PatientId}. Exception details: {Message}", 
                    patientId, ex.Message);
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            _logger.LogInformation("OnPostAsync called for id: {id}", id);
            _logger.LogInformation("Form data - ChiefComplaint: '{ChiefComplaint}', Diagnosis: '{Diagnosis}', Treatment: '{Treatment}', Notes: '{Notes}'", 
                ChiefComplaint, Diagnosis, Treatment, Notes);

            var appointmentToSave = await _context.Appointments.FindAsync(id);
            if (appointmentToSave == null)
            {
                _logger.LogWarning("Appointment with ID {id} not found.", id);
                TempData["ErrorMessage"] = "Appointment not found.";
                return RedirectToPage("/Doctor/Appointments");
            }

            // Use the appointment from the database to ensure data integrity
            AppointmentId = appointmentToSave.Id;
            PatientId = appointmentToSave.PatientId;

            if (!ModelState.IsValid)
            {
                // If model state is invalid, we need to reload the data for the page
                // because the properties are not persisted across postbacks.
                await OnGetAsync(PatientId, AppointmentId); 
                return Page();
            }

            var currentDoctor = await _userManager.GetUserAsync(User);
            if (currentDoctor == null)
            {
                ModelState.AddModelError("", "Authentication error. Please log in again.");
                await OnGetAsync(PatientId, AppointmentId);
                return Page();
            }

            // Decrypt doctor's data to get proper FullName
            currentDoctor.DecryptSensitiveData(_encryptionService, User);

            // Use a database transaction to ensure all or no changes are saved.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Re-fetch the appointment to ensure we have the correct PatientId
                var appointment = await _context.Appointments.FindAsync(AppointmentId.Value);
                if (appointment == null)
                {
                    TempData["ErrorMessage"] = "The appointment could not be found. It may have been cancelled.";
                    return RedirectToPage("/Doctor/Appointments");
                }

                // Explicitly set PatientId for the form's hidden field
                PatientId = appointment.PatientId;

                var medicalRecord = new MedicalRecord
                {
                    PatientId = appointment.PatientId, // Use the trusted PatientId from the database
                    AppointmentId = appointment.Id, // Assign the appointment ID to the medical record
                    DoctorId = currentDoctor.Id,
                    Date = DateTime.UtcNow,
                    ChiefComplaint = ChiefComplaint,
                    Diagnosis = Diagnosis,
                    Treatment = Treatment,
                    Notes = Notes,
                    Prescription = Prescribe ?? string.Empty, // Save prescription details
                    Type = "Consultation", // Or derive from appointment
                    Status = "Active"
                };

                // Encrypt sensitive consultation data before saving
                medicalRecord.EncryptSensitiveData(_encryptionService);

                _context.MedicalRecords.Add(medicalRecord);
                await _context.SaveChangesAsync(); // Save to get the MedicalRecord ID

                // Create prescription record if prescription details are provided
                if (!string.IsNullOrEmpty(Prescribe))
                {
                    var prescription = new Prescription
                    {
                        PatientId = appointment.PatientId,
                        DoctorId = currentDoctor.Id,
                        Diagnosis = Diagnosis ?? "Consultation diagnosis",
                        Duration = 7, // Default 7 days duration
                        Notes = Prescribe,
                        Status = PrescriptionStatus.Created,
                        PrescriptionDate = DateTime.UtcNow,
                        ValidUntil = DateTime.UtcNow.AddDays(7),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Encrypt prescription data
                    prescription.EncryptSensitiveData(_encryptionService);

                    _context.Prescriptions.Add(prescription);
                    await _context.SaveChangesAsync(); // Save to get the Prescription ID

                    _logger.LogInformation("Prescription created with ID {PrescriptionId} for patient {PatientId}", 
                        prescription.Id, appointment.PatientId);
                }

                // Finally, update the appointment status to Completed.
                appointment.Status = AppointmentStatus.Completed;
                appointment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Commit the transaction to make changes permanent

                // Create follow-up appointment and send reminder email if follow-up reason is provided
                if (!string.IsNullOrEmpty(FollowUpReason))
                {
                    try
                    {
                        // Use specified date and time, or default to next week if not provided
                        var followUpDate = FollowUpDate?.Date ?? DateTime.UtcNow.AddDays(7).Date;
                        var followUpTime = FollowUpTime ?? appointment.AppointmentTime; // Use specified time or same time as original appointment
                        
                        _logger.LogInformation("Creating follow-up appointment for patient {PatientId} on {FollowUpDate} at {FollowUpTime}", 
                            appointment.PatientId, followUpDate, followUpTime);
                        
                        var followUpAppointment = new Barangay.Models.Appointment
                        {
                            PatientId = appointment.PatientId,
                            DoctorId = appointment.DoctorId,
                            PatientName = appointment.PatientName,
                            AppointmentDate = followUpDate,
                            AppointmentTime = followUpTime,
                            Description = $"Follow-up appointment: {FollowUpReason}",
                            ReasonForVisit = FollowUpReason,
                            Status = AppointmentStatus.Pending,
                            Type = "Follow-up",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        
                        _context.Appointments.Add(followUpAppointment);
                        await _context.SaveChangesAsync();
                        
                        _logger.LogInformation("Follow-up appointment created with ID {FollowUpAppointmentId} for patient {PatientId}", 
                            followUpAppointment.Id, appointment.PatientId);
                        
                        // Send follow-up appointment reminder email immediately
                        var patient = await _context.Users.FirstOrDefaultAsync(u => u.Id == appointment.PatientId);
                        if (patient != null && !string.IsNullOrEmpty(patient.Email))
                        {
                            _logger.LogInformation("Sending follow-up reminder email to {PatientEmail} for appointment on {FollowUpDate} at {FollowUpTime}", 
                                patient.Email, followUpDate, followUpTime);
                            
                            // Include prescription details in the email if provided
                            var emailContent = FollowUpReason;
                            if (!string.IsNullOrEmpty(Prescribe))
                            {
                                emailContent += $"\n\nPrescription Details:\n{Prescribe}";
                            }
                            
                            await _appointmentReminderService.SendFollowUpReminderEmailAsync(followUpAppointment.Id, patient.Email, emailContent);
                            _logger.LogInformation("Follow-up reminder email sent successfully to patient {PatientEmail} for follow-up appointment {FollowUpAppointmentId}", 
                                patient.Email, followUpAppointment.Id);
                        }
                        else
                        {
                            _logger.LogWarning("Patient not found or email is null for follow-up appointment. PatientId: {PatientId}", appointment.PatientId);
                        }
                    }
                    catch (Exception followUpEx)
                    {
                        _logger.LogError(followUpEx, "Failed to create follow-up appointment or send reminder email for patient {PatientId}", appointment.PatientId);
                        // Don't fail the consultation if follow-up creation fails
                    }
                }

                // Send thank you email if no follow-up is needed
                if (string.IsNullOrEmpty(FollowUpReason))
                {
                    try
                    {
                        var patient = await _context.Users.FirstOrDefaultAsync(u => u.Id == appointment.PatientId);
                        if (patient != null && !string.IsNullOrEmpty(patient.Email))
                        {
                            _logger.LogInformation("Sending thank you email to {PatientEmail} for consultation completion", patient.Email);
                            
                            // Include prescription details in thank you email if provided
                            if (!string.IsNullOrEmpty(Prescribe))
                            {
                                // Create a temporary appointment with prescription details for email
                                var tempAppointment = new Barangay.Models.Appointment
                                {
                                    Id = appointment.Id,
                                    PatientId = appointment.PatientId,
                                    PatientName = appointment.PatientName,
                                    AppointmentDate = appointment.AppointmentDate,
                                    AppointmentTime = appointment.AppointmentTime,
                                    Description = $"Consultation completed. Prescription: {Prescribe}",
                                    ReasonForVisit = appointment.ReasonForVisit,
                                    Status = appointment.Status,
                                    Type = appointment.Type,
                                    CreatedAt = appointment.CreatedAt,
                                    UpdatedAt = appointment.UpdatedAt
                                };
                                await _appointmentReminderService.SendThankYouEmailAsync(patient.Email, patient.FullName, tempAppointment);
                            }
                            else
                            {
                                await _appointmentReminderService.SendThankYouEmailAsync(patient.Email, patient.FullName, appointment);
                            }
                            
                            _logger.LogInformation("Thank you email sent successfully to patient {PatientEmail}", patient.Email);
                        }
                    }
                    catch (Exception thankYouEx)
                    {
                        _logger.LogError(thankYouEx, "Failed to send thank you email to patient {PatientId}", appointment.PatientId);
                        // Don't fail the consultation if thank you email fails
                    }
                }

                TempData["SuccessMessage"] = "Consultation completed and all records saved successfully.";
                // Set a flag so the Appointments page can show a link to Monthly Reports
                TempData["AfterConsultation"] = "1";
                return RedirectToPage("/Doctor/Appointments");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while saving the consultation. Rolling back transaction.");
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "A critical error occurred while saving. The operation was cancelled to protect data integrity. Please try again.";
                // Reload the page data to allow the user to retry
                await OnGetAsync(PatientId, AppointmentId);
                return Page();
            }
        }

        // Consultation summary email methods removed - no longer needed

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



        // Helper method to get the current doctor's ID from claims
        private string GetCurrentDoctorId()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                _logger.LogWarning("User is not authenticated");
                return string.Empty;
            }
            
            // Get user ID from the nameidentifier claim
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Unable to get user ID from claims");
                return string.Empty;
            }
            
            _logger.LogInformation("User ID from claims: {UserId}", userId);
            
            // Verify the user is in the Doctor or Head Doctor role
            var isDoctor = User.IsInRole("Doctor") || User.IsInRole("Head Doctor");
            if (!isDoctor)
            {
                _logger.LogWarning("User {UserId} is not in the Doctor or Head Doctor role", userId);
                return string.Empty;
            }
            
            return userId;
        }

        // Helper method to get doctor ID with fallback options
        private async Task<string> GetCurrentDoctorIdWithFallbackAsync()
        {
            var doctorId = GetCurrentDoctorId();
            
            if (!string.IsNullOrEmpty(doctorId))
            {
                return doctorId;
            }

            // Fallback: Try to get the current user and check if they're a doctor
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                _logger.LogInformation("Fallback: Checking if user {UserId} is a doctor", currentUser.Id);
                
                var isDoctor = await _userManager.IsInRoleAsync(currentUser, "Doctor") || await _userManager.IsInRoleAsync(currentUser, "Head Doctor");
                if (isDoctor)
                {
                    _logger.LogInformation("Fallback: User {UserId} is confirmed as doctor", currentUser.Id);
                    return currentUser.Id;
                }
            }

            return string.Empty;
        }

        // Helper method for safe decryption
        private string SafeDecrypt(string? encryptedValue)
        {
            if (string.IsNullOrEmpty(encryptedValue))
                return encryptedValue ?? string.Empty;

            try
            {
                if (_encryptionService.IsEncrypted(encryptedValue))
                {
                    var decrypted = _encryptionService.DecryptForUser(encryptedValue, User);
                    return decrypted != encryptedValue && !decrypted.Contains("[ACCESS DENIED]") ? decrypted : encryptedValue;
                }
                return encryptedValue;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decrypt value: {Value}", encryptedValue?.Substring(0, Math.Min(20, encryptedValue?.Length ?? 0)));
                return encryptedValue;
            }
        }

        // Helper method to check if HEEADSSS assessment has meaningful data
        public bool HasHEEADSSSData()
        {
            if (HEEADSSSAssessment == null) return false;

            // Check if any meaningful data exists (not just default/empty values or meaningless entries)
            var hasMeaningfulData = false;

            // Check text fields for meaningful content (not just "sad", "action", etc.)
            var textFields = new[] {
                HEEADSSSAssessment.HomeEnvironment,
                HEEADSSSAssessment.FamilyRelationship,
                HEEADSSSAssessment.HomeFamilyProblems,
                HEEADSSSAssessment.HomeParentalListening,
                HEEADSSSAssessment.SchoolPerformance,
                HEEADSSSAssessment.CareerPlans,
                HEEADSSSAssessment.PhysicalActivity,
                HEEADSSSAssessment.DietDescription,
                HEEADSSSAssessment.DrugsTobaccoUse,
                HEEADSSSAssessment.SexualOrientation,
                HEEADSSSAssessment.SexualityPregnancyExperience,
                HEEADSSSAssessment.SexualitySTIExperience,
                HEEADSSSAssessment.SexualityProtectionUse,
                HEEADSSSAssessment.Hobbies,
                HEEADSSSAssessment.AssessmentNotes,
                HEEADSSSAssessment.AssessedBy
            };

            foreach (var field in textFields)
            {
                if (!string.IsNullOrEmpty(field) && 
                    !field.Equals("Not specified", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("Not provided", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("sad", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("action", StringComparison.OrdinalIgnoreCase) &&
                    field.Length > 3) // Must be more than 3 characters to be meaningful
                {
                    hasMeaningfulData = true;
                    break;
                }
            }

            // Check boolean fields for "true" values
            if (!hasMeaningfulData)
            {
                hasMeaningfulData = HEEADSSSAssessment.AttendanceIssues == "true" ||
                                   HEEADSSSAssessment.WeightConcerns == "true" ||
                                   HEEADSSSAssessment.SubstanceUse == "true" ||
                                   HEEADSSSAssessment.SuicidalThoughts == "true" ||
                                   HEEADSSSAssessment.FeelsSafeAtHome == "true" ||
                                   HEEADSSSAssessment.SexualActivity == "true";
            }

            return hasMeaningfulData;
        }

        // Helper method to check if NCD assessment has meaningful data
        public bool HasNCDData()
        {
            if (NCDRiskAssessment == null) return false;

            // Check if any meaningful data exists (not just default/empty values or meaningless entries)
            var hasMeaningfulData = false;

            // Check text fields for meaningful content
            var textFields = new[] {
                NCDRiskAssessment.FirstName,
                NCDRiskAssessment.LastName,
                NCDRiskAssessment.Edad,
                NCDRiskAssessment.Kasarian,
                NCDRiskAssessment.Address,
                NCDRiskAssessment.Telepono,
                NCDRiskAssessment.Weight,
                NCDRiskAssessment.Height,
                NCDRiskAssessment.BMI,
                NCDRiskAssessment.Waist,
                NCDRiskAssessment.SmokingStatus,
                NCDRiskAssessment.AlcoholFrequency,
                NCDRiskAssessment.ExerciseDuration,
                NCDRiskAssessment.FastingBloodSugar,
                NCDRiskAssessment.RandomBloodSugar,
                NCDRiskAssessment.CholesterolResult,
                NCDRiskAssessment.LeftArmMeanBP,
                NCDRiskAssessment.RightArmMeanBP,
                NCDRiskAssessment.BaselineBP,
                NCDRiskAssessment.InterviewedBy,
                NCDRiskAssessment.Designation,
                NCDRiskAssessment.RiskStatus,
                NCDRiskAssessment.ChestPain
            };

            foreach (var field in textFields)
            {
                if (!string.IsNullOrEmpty(field) && 
                    !field.Equals("Not specified", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("Not provided", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("Not recorded", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("None", StringComparison.OrdinalIgnoreCase) &&
                    !field.Equals("Non-smoker", StringComparison.OrdinalIgnoreCase) &&
                    field.Length > 3) // Must be more than 3 characters to be meaningful
                {
                    hasMeaningfulData = true;
                    break;
                }
            }

            // Check boolean fields for "true" values
            if (!hasMeaningfulData)
            {
                hasMeaningfulData = NCDRiskAssessment.HasDiabetes == "true" ||
                                   NCDRiskAssessment.HasHypertension == "true" ||
                                   NCDRiskAssessment.HasCancer == "true" ||
                                   NCDRiskAssessment.HasLungDisease == "true" ||
                                   NCDRiskAssessment.HasCOPD == "true" ||
                                   NCDRiskAssessment.HasEyeDisease == "true" ||
                                   NCDRiskAssessment.HasAsthma == "true" ||
                                   NCDRiskAssessment.HasDifficultyBreathing == "true" ||
                                   NCDRiskAssessment.HasChestPain == "true" ||
                                   NCDRiskAssessment.FamilyHasHypertension == "true" ||
                                   NCDRiskAssessment.FamilyHasHeartDisease == "true" ||
                                   NCDRiskAssessment.FamilyHasStroke == "true" ||
                                   NCDRiskAssessment.FamilyHasDiabetes == "true" ||
                                   NCDRiskAssessment.FamilyHasCancer == "true" ||
                                   NCDRiskAssessment.FamilyHasKidneyDisease == "true" ||
                                   NCDRiskAssessment.FamilyHasOtherDisease == "true" ||
                                   NCDRiskAssessment.Smoked100Sticks == "true" ||
                                   NCDRiskAssessment.FormerSmoker == "true" ||
                                   NCDRiskAssessment.HighSaltIntake == "true" ||
                                   NCDRiskAssessment.IsBingeDrinker == "true";
            }

            return hasMeaningfulData;
        }

        // Helper method to check if Adolescent Health Info has meaningful data
        public bool HasAdolescentHealthData()
        {
            if (AdolescentHealthInfo == null) return false;

            // Check if any meaningful data exists (not just default/empty values)
            return !string.IsNullOrEmpty(AdolescentHealthInfo.PatientName) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.PatientAge) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.PatientGender) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.PatientAddress) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.PatientContact) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.HeightCm) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.WeightKg) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.BMI) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.BMICategory) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.MRMMRDateGiven) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.TdDateGiven) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.HPVDateGiven) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.Temperature) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.BloodPressure) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.PulseRate) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.RespiratoryRate) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.ChiefComplaint) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.WorkingDiagnosis) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.ReferredTo) ||
                   !string.IsNullOrEmpty(AdolescentHealthInfo.RecordedBy);
        }

        public class PrescriptionMedicationViewModel
        {
            public int? Id { get; set; }
            
            [Display(Name = "Medicine")]
            public int MedicationId { get; set; }

            public string? MedicationName { get; set; } // To display in the view
            
            [Display(Name = "Dosage")]
            [RegularExpression(@"^[0-9]*\.?[0-9]+$", ErrorMessage = "Dosage must be a valid number")]
            public string? Dosage { get; set; }
            
            [Display(Name = "Instructions")]
            public string? Instructions { get; set; }
        }
    }
}
