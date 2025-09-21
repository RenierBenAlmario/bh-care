using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic; // Added for List

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class AppointmentDetailsModel : PageModel
    {
        private readonly EncryptedDbContext _context;
        private readonly ILogger<AppointmentDetailsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;

        public AppointmentDetailsModel(
            EncryptedDbContext context,
            ILogger<AppointmentDetailsModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
        }

        public AppointmentsModel.AppointmentViewModel Appointment { get; set; }
        public NCDRiskAssessment NCDRiskAssessment { get; set; }
        public HEEADSSSAssessment HEEADSSSAssessment { get; set; }
        public int PatientAge { get; set; }
        public bool HasNCDAssessment { get; set; }
        public bool HasHEEADSSSAssessment { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Appointment ID not provided");
                return NotFound("Appointment ID must be provided");
            }

            try
            {
                _logger.LogInformation("Loading appointment details for ID: {Id}", id);

                // Get the appointment
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    _logger.LogWarning("Appointment with ID {Id} not found", id);
                    return NotFound("Appointment not found");
                }

                // Convert to view model
                Appointment = new AppointmentsModel.AppointmentViewModel
                {
                    Id = appointment.Id,
                    PatientId = appointment.PatientId,
                    PatientName = string.IsNullOrEmpty(appointment.PatientName) ?
                        (appointment.Patient != null ? appointment.Patient.FullName : "Unknown") : appointment.PatientName,
                    AppointmentDate = appointment.AppointmentDate,
                    AppointmentTime = appointment.AppointmentTime,
                    DoctorId = appointment.DoctorId,
                    DoctorName = appointment.Doctor?.FullName ?? "Not Assigned",
                    Status = appointment.Status,
                    Type = appointment.Type ?? "General",
                    Description = appointment.Description
                };

                // Load the patient details
                if (appointment.Patient != null)
                {
                    // Calculate patient's age
                    PatientAge = appointment.Patient.Age;
                }

                // Check for NCD Risk Assessment existence
                HasNCDAssessment = await _context.NCDRiskAssessments
                    .AnyAsync(a => a.AppointmentId == id);

                // Load NCD Risk Assessment if it exists
                if (HasNCDAssessment)
                {
                    try {
                        NCDRiskAssessment = await _context.NCDRiskAssessments
                            .Where(a => a.AppointmentId == id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
                        
                        // Decrypt NCD Risk Assessment data
                        if (NCDRiskAssessment != null)
                        {
                            NCDRiskAssessment.DecryptSensitiveData(_encryptionService, User);
                        }
                        
                        _logger.LogInformation("Successfully loaded and decrypted NCDRiskAssessment data");
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error loading NCD Risk Assessment data for appointment ID {Id}", id);
                        HasNCDAssessment = false;
                    }
                }

                // Check for HEEADSSS Assessment existence
                // Since AppointmentId is encrypted, we need to check all records and decrypt them
                var allHEEADSSSAssessments = await _context.HEEADSSSAssessments
                    .AsNoTracking()
                    .ToListAsync();

                HEEADSSSAssessment = null;
                HasHEEADSSSAssessment = false;

                foreach (var assessment in allHEEADSSSAssessments)
                {
                    try
                    {
                        // Decrypt the AppointmentId to check if it matches
                        var decryptedAppointmentId = _encryptionService.DecryptForUser(assessment.AppointmentId, User);
                        if (decryptedAppointmentId == id.ToString())
                        {
                            HEEADSSSAssessment = assessment;
                            HasHEEADSSSAssessment = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt AppointmentId for HEEADSSS assessment {Id}", assessment.Id);
                        // Continue checking other assessments
                    }
                }

                // Load HEEADSSS Assessment if it exists
                if (HasHEEADSSSAssessment && HEEADSSSAssessment != null)
                {
                    try {
                        
                        // Decrypt HEEADSSS Assessment data
                        if (HEEADSSSAssessment != null)
                        {
                            HEEADSSSAssessment.DecryptSensitiveData(_encryptionService, User);
                        }
                        
                        _logger.LogInformation("Successfully loaded and decrypted HEEADSSS Assessment data");
                    }
                    catch (Exception ex) {
                        _logger.LogError(ex, "Error loading HEEADSSS Assessment data for appointment ID {Id}", id);
                        HasHEEADSSSAssessment = false;
                    }
                }
                
                _logger.LogInformation("Assessment flags - NCD: {HasNCD}, HEEADSSS: {HasHEEADSSS}", HasNCDAssessment, HasHEEADSSSAssessment);

                // Add additional properties to track history
                if (NCDRiskAssessment != null)
                {
                    _logger.LogInformation("NCD Risk Assessment creation date: {Date}", NCDRiskAssessment.CreatedAt);
                }

                if (HEEADSSSAssessment != null)
                {
                    _logger.LogInformation("HEEADSSS Assessment creation date: {Date}", HEEADSSSAssessment.CreatedAt);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading appointment details for ID: {Id}", id);
                StatusMessage = "Error loading appointment details. Please try again later.";
                return RedirectToPage("/Nurse/Appointments");
            }
        }
    }
} 