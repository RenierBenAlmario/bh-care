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
    public class PrintNCDAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PrintNCDAssessmentModel> _logger;
        private readonly IPermissionService _permissionService;
        private readonly IDataEncryptionService _encryptionService;

        public PrintNCDAssessmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PrintNCDAssessmentModel> logger,
            IPermissionService permissionService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _permissionService = permissionService;
            _encryptionService = encryptionService;
        }

        public NCDRiskAssessmentViewModel NCDRiskAssessment { get; set; }

        public async Task<IActionResult> OnGetAsync(int appointmentId)
        {
            try
            {
                // Nurses have permission to print assessments by default
                _logger.LogInformation("Nurse printing NCD assessment for appointment {AppointmentId}", appointmentId);

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

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading NCD assessment for printing, appointment {AppointmentId}", appointmentId);
                TempData["StatusMessage"] = "Error: Unable to load assessment for printing.";
                return RedirectToPage("/Nurse/AppointmentDetails", new { id = appointmentId });
            }
        }
    }
}
