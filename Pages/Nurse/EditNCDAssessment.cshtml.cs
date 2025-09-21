using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Barangay.Services;
using System.Security.Claims;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class EditNCDAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditNCDAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public EditNCDAssessmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<EditNCDAssessmentModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public NCDRiskAssessmentViewModel NCDRiskAssessment { get; set; }

        public int AppointmentId { get; set; }
        public string UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            try
            {
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse editing NCD assessment for appointment {AppointmentId}", appointmentId);

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
                }

                // Decrypt sensitive data for display
                assessment.DecryptSensitiveData(_encryptionService, User);

                // Convert to view model
                NCDRiskAssessment = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = assessment.UserId.ToString(),
                    HealthFacility = assessment.HealthFacility,
                    FamilyNo = assessment.FamilyNo,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = assessment.Birthday,
                    Telepono = assessment.Telepono,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    HasDiabetes = assessment.HasDiabetes,
                    DiabetesYear = assessment.DiabetesYear,
                    DiabetesMedication = assessment.DiabetesMedication,
                    HasHypertension = assessment.HasHypertension,
                    HypertensionYear = assessment.HypertensionYear,
                    HypertensionMedication = assessment.HypertensionMedication,
                    HasCancer = assessment.HasCancer,
                    CancerType = assessment.CancerType,
                    CancerYear = assessment.CancerYear,
                    CancerMedication = assessment.CancerMedication,
                    HasCOPD = assessment.HasCOPD,
                    HasLungDisease = assessment.HasLungDisease,
                    LungDiseaseYear = assessment.LungDiseaseYear,
                    LungDiseaseMedication = assessment.LungDiseaseMedication,
                    HasEyeDisease = assessment.HasEyeDisease,
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    ExerciseDuration = assessment.ExerciseDuration,
                    AppointmentType = assessment.AppointmentType,
                    SmokingStatus = assessment.SmokingStatus,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    FirstName = assessment.FirstName,
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName,
                    Occupation = assessment.Occupation,
                    CivilStatus = assessment.CivilStatus,
                    FamilyHasHypertension = assessment.FamilyHasHypertension,
                    FamilyHasHeartDisease = assessment.FamilyHasHeartDisease,
                    FamilyHasStroke = assessment.FamilyHasStroke,
                    FamilyHasDiabetes = assessment.FamilyHasDiabetes,
                    FamilyHasCancer = assessment.FamilyHasCancer,
                    FamilyHasKidneyDisease = assessment.FamilyHasKidneyDisease,
                    FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                    RiskStatus = assessment.RiskStatus
                };

                AppointmentId = appointmentId;
                UserId = assessment.UserId;

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for editing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for editing.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Nurses have permission to edit assessments by default
                _logger.LogInformation("Nurse updating NCD assessment for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var assessment = await _context.NCDRiskAssessments
                    .FirstOrDefaultAsync(n => n.AppointmentId == NCDRiskAssessment.AppointmentId);

                if (assessment == null)
                {
                    TempData["StatusMessage"] = "Error: Assessment not found.";
                    return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
                }

                // Update assessment
                assessment.HealthFacility = NCDRiskAssessment.HealthFacility;
                assessment.FamilyNo = NCDRiskAssessment.FamilyNo;
                assessment.Address = NCDRiskAssessment.Address;
                assessment.Barangay = NCDRiskAssessment.Barangay;
                assessment.Birthday = NCDRiskAssessment.Birthday;
                assessment.Telepono = NCDRiskAssessment.Telepono;
                assessment.Edad = NCDRiskAssessment.Edad;
                assessment.Kasarian = NCDRiskAssessment.Kasarian;
                assessment.Relihiyon = NCDRiskAssessment.Relihiyon;
                assessment.HasDiabetes = NCDRiskAssessment.HasDiabetes;
                assessment.DiabetesYear = NCDRiskAssessment.DiabetesYear;
                assessment.DiabetesMedication = NCDRiskAssessment.DiabetesMedication;
                assessment.HasHypertension = NCDRiskAssessment.HasHypertension;
                assessment.HypertensionYear = NCDRiskAssessment.HypertensionYear;
                assessment.HypertensionMedication = NCDRiskAssessment.HypertensionMedication;
                assessment.HasCancer = NCDRiskAssessment.HasCancer;
                assessment.CancerType = NCDRiskAssessment.CancerType;
                assessment.CancerYear = NCDRiskAssessment.CancerYear;
                assessment.CancerMedication = NCDRiskAssessment.CancerMedication;
                assessment.HasCOPD = NCDRiskAssessment.HasCOPD;
                assessment.HasLungDisease = NCDRiskAssessment.HasLungDisease;
                assessment.LungDiseaseYear = NCDRiskAssessment.LungDiseaseYear;
                assessment.LungDiseaseMedication = NCDRiskAssessment.LungDiseaseMedication;
                assessment.HasEyeDisease = NCDRiskAssessment.HasEyeDisease;
                assessment.HighSaltIntake = NCDRiskAssessment.HighSaltIntake;
                assessment.AlcoholFrequency = NCDRiskAssessment.AlcoholFrequency;
                assessment.ExerciseDuration = NCDRiskAssessment.ExerciseDuration;
                assessment.AppointmentType = NCDRiskAssessment.AppointmentType;
                assessment.SmokingStatus = NCDRiskAssessment.SmokingStatus;
                assessment.AlcoholConsumption = NCDRiskAssessment.AlcoholConsumption;
                assessment.FirstName = NCDRiskAssessment.FirstName;
                assessment.MiddleName = NCDRiskAssessment.MiddleName;
                assessment.LastName = NCDRiskAssessment.LastName;
                assessment.Occupation = NCDRiskAssessment.Occupation;
                assessment.CivilStatus = NCDRiskAssessment.CivilStatus;
                assessment.FamilyHasHypertension = NCDRiskAssessment.FamilyHasHypertension;
                assessment.FamilyHasHeartDisease = NCDRiskAssessment.FamilyHasHeartDisease;
                assessment.FamilyHasStroke = NCDRiskAssessment.FamilyHasStroke;
                assessment.FamilyHasDiabetes = NCDRiskAssessment.FamilyHasDiabetes;
                assessment.FamilyHasCancer = NCDRiskAssessment.FamilyHasCancer;
                assessment.FamilyHasKidneyDisease = NCDRiskAssessment.FamilyHasKidneyDisease;
                assessment.FamilyOtherDiseaseDetails = NCDRiskAssessment.FamilyOtherDiseaseDetails;
                assessment.RiskStatus = NCDRiskAssessment.RiskStatus;
                assessment.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                await _context.SaveChangesAsync();

                _logger.LogInformation("NCD Risk Assessment updated successfully for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);
                TempData["StatusMessage"] = "NCD Risk Assessment updated successfully.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = NCDRiskAssessment.AppointmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating NCD assessment for appointment {AppointmentId}", NCDRiskAssessment.AppointmentId);
                TempData["StatusMessage"] = "Error: Unable to update assessment.";
                return Page();
            }
        }
    }
}
