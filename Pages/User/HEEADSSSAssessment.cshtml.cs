using System;
using System.Threading.Tasks;
using System.Linq;
using Barangay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Barangay.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Barangay.Pages.User
{
    public class HEEADSSSAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HEEADSSSAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public HEEADSSSAssessmentModel(
            ApplicationDbContext context,
            ILogger<HEEADSSSAssessmentModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public HEEADSSSAssessmentViewModel Assessment { get; set; }

        [BindProperty]
        public string HealthFacility { get; set; } = "Barangay Health Center 161";

        [BindProperty]
        public string FamilyNo { get; set; }

        [BindProperty]
        public int? AppointmentId { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Set default values
            HealthFacility = "Barangay Health Center 161";
            ViewData["PatientName"] = user.FullName;
            ViewData["PatientAge"] = CalculateAge(user.BirthDate);
            ViewData["PatientPhone"] = user.PhoneNumber ?? string.Empty;

            // Initialize Assessment if not already initialized
            if (Assessment == null)
            {
                Assessment = new HEEADSSSAssessmentViewModel
                {
                    UserId = user.Id,
                    HealthFacility = HealthFacility,
                    Birthday = DateTime.Now.AddYears(-15).Date, // Default to 15 years ago
                    Age = CalculateAge(user.BirthDate),
                    Gender = user.Gender ?? string.Empty,
                    Address = user.Address ?? string.Empty,
                    ContactNumber = user.PhoneNumber ?? string.Empty
                };
            }

            // Try to find existing family number for the user
            var existingAssessment = await _context.HEEADSSSAssessments
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingAssessment != null)
            {
                Assessment.FamilyNo = existingAssessment.FamilyNo;
            }
            else
            {
                // Generate new family number
                var lastFamilyNo = await _context.HEEADSSSAssessments
                    .Where(a => a.FamilyNo.StartsWith(user.LastName[0].ToString()))
                    .OrderByDescending(a => a.FamilyNo)
                    .Select(a => a.FamilyNo)
                    .FirstOrDefaultAsync();

                int sequence = 1;
                if (lastFamilyNo != null)
                {
                    var parts = lastFamilyNo.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int lastSequence))
                    {
                        sequence = lastSequence + 1;
                    }
                }

                Assessment.FamilyNo = $"{user.LastName[0]}-{sequence:D3}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Remove validation for optional fields
                ModelState.Remove("Assessment.FamilyOtherDiseaseDetails");
                ModelState.Remove("Assessment.CancerType");
                ModelState.Remove("Assessment.AppointmentType");
                ModelState.Remove("Assessment.Religion");
                
                // Validate CancerType only if Cancer is selected
                if (Assessment.CancerType == null)
                {
                    ModelState.Remove("Assessment.CancerType");
                }
                
                if (!ModelState.IsValid)
                {
                    return new JsonResult(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Generate family number if not provided
                if (string.IsNullOrEmpty(Assessment.FamilyNo))
                {
                    Assessment.FamilyNo = await GetOrGenerateFamilyNumber(user);
                }

                // Map view model to database model
                var assessment = new HEEADSSSAssessment
                {
                    UserId = user.Id,
                    AppointmentId = Assessment.AppointmentId,
                    HealthFacility = Assessment.HealthFacility,
                    FamilyNo = Assessment.FamilyNo,
                    FullName = user.FullName,
                    Birthday = Assessment.Birthday,
                    Age = Assessment.Age,
                    Gender = Assessment.Gender,
                    Address = Assessment.Address,
                    ContactNumber = Assessment.ContactNumber,
                    
                    // HOME
                    HomeEnvironment = Assessment.HomeEnvironment,
                    FamilyRelationship = Assessment.FamilyRelationship,
                    HomeFamilyProblems = Assessment.HomeFamilyProblems,
                    HomeParentalListening = Assessment.HomeParentalListening,
                    HomeParentalBlame = Assessment.HomeParentalBlame,
                    HomeFamilyChanges = Assessment.HomeFamilyChanges,
                    
                    // EDUCATION
                    SchoolPerformance = Assessment.SchoolPerformance,
                    AttendanceIssues = Assessment.AttendanceIssues,
                    CareerPlans = Assessment.CareerPlans,
                    EducationCurrentlyStudying = Assessment.EducationCurrentlyStudying,
                    EducationWorking = Assessment.EducationWorking,
                    EducationSchoolWorkProblems = Assessment.EducationSchoolWorkProblems,
                    EducationBullying = Assessment.EducationBullying,
                    EducationEmployment = Assessment.EducationEmployment,
                    
                    // EATING HABITS
                    DietDescription = Assessment.DietDescription,
                    WeightConcerns = Assessment.WeightConcerns,
                    EatingDisorderSymptoms = Assessment.EatingDisorderSymptoms,
                    EatingBodyImageSatisfaction = Assessment.EatingBodyImageSatisfaction,
                    EatingDisorderedEatingBehaviors = Assessment.EatingDisorderedEatingBehaviors,
                    EatingWeightComments = Assessment.EatingWeightComments,
                    
                    // ACTIVITIES
                    Hobbies = Assessment.Hobbies,
                    PhysicalActivity = Assessment.PhysicalActivity,
                    ScreenTime = Assessment.ScreenTime,
                    ActivitiesParticipation = Assessment.ActivitiesParticipation,
                    ActivitiesRegularExercise = Assessment.ActivitiesRegularExercise,
                    ActivitiesScreenTime = Assessment.ActivitiesScreenTime,
                    
                    // DRUGS
                    SubstanceUse = Assessment.SubstanceUse,
                    SubstanceType = Assessment.SubstanceType,
                    DrugsTobaccoUse = Assessment.DrugsTobaccoUse,
                    DrugsAlcoholUse = Assessment.DrugsAlcoholUse,
                    DrugsIllicitDrugUse = Assessment.DrugsIllicitDrugUse,
                    
                    // SEXUALITY
                    DatingRelationships = Assessment.DatingRelationships,
                    SexualActivity = Assessment.SexualActivity,
                    SexualOrientation = Assessment.SexualOrientation,
                    SexualityBodyConcerns = Assessment.SexualityBodyConcerns,
                    SexualityIntimateRelationships = Assessment.SexualityIntimateRelationships,
                    SexualityPartners = Assessment.SexualityPartners,
                    SexualitySexualOrientation = Assessment.SexualitySexualOrientation,
                    SexualityPregnancy = Assessment.SexualityPregnancy,
                    SexualitySTI = Assessment.SexualitySTI,
                    SexualityProtection = Assessment.SexualityProtection,
                    
                    // SUICIDE/DEPRESSION
                    MoodChanges = Assessment.MoodChanges,
                    SuicidalThoughts = Assessment.SuicidalThoughts,
                    SelfHarmBehavior = Assessment.SelfHarmBehavior,
                    
                    // SAFETY
                    FeelsSafeAtHome = Assessment.FeelsSafeAtHome,
                    FeelsSafeAtSchool = Assessment.FeelsSafeAtSchool,
                    ExperiencedBullying = Assessment.ExperiencedBullying,
                    
                    // STRENGTHS
                    PersonalStrengths = Assessment.PersonalStrengths,
                    SupportSystems = Assessment.SupportSystems,
                    CopingMechanisms = Assessment.CopingMechanisms,
                    
                    // SAFETY/WEAPONS/VIOLENCE
                    SafetyPhysicalAbuse = Assessment.SafetyPhysicalAbuse,
                    SafetyRelationshipViolence = Assessment.SafetyRelationshipViolence,
                    SafetyProtectiveGear = Assessment.SafetyProtectiveGear,
                    SafetyGunsAtHome = Assessment.SafetyGunsAtHome,
                    
                    // SUICIDE/DEPRESSION
                    SuicideDepressionFeelings = Assessment.SuicideDepressionFeelings,
                    SuicideSelfHarmThoughts = Assessment.SuicideSelfHarmThoughts,
                    SuicideFamilyHistory = Assessment.SuicideFamilyHistory,
                    
                    // Assessment Information
                    AssessmentNotes = Assessment.AssessmentNotes,
                    RecommendedActions = Assessment.RecommendedActions,
                    FollowUpPlan = Assessment.FollowUpPlan,
                    Notes = Assessment.Notes,
                    AssessedBy = Assessment.AssessedBy,
                    
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.HEEADSSSAssessments.Add(assessment);
                await _context.SaveChangesAsync();

                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving HEEADSSS assessment");
                return new JsonResult(new { success = false, error = "An error occurred while saving the assessment. Please try again." });
            }
        }

        private int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
            {
                return 0;
            }

            var referenceDate = new DateTime(2025, 5, 29, 11, 32, 0); // May 29, 2025, 11:32 AM PST
            var age = referenceDate.Year - birthDate.Value.Year;
            if (referenceDate.Month < birthDate.Value.Month || 
                (referenceDate.Month == birthDate.Value.Month && referenceDate.Day < birthDate.Value.Day))
            {
                age--;
            }
            return age;
        }

        private async Task<string> GetOrGenerateFamilyNumber(ApplicationUser user)
        {
            // Check if user already has a family number
            var existingAssessment = await _context.HEEADSSSAssessments
                .Where(a => a.UserId == user.Id)
                .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

            if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.FamilyNo))
                {
                return existingAssessment.FamilyNo;
                }

            // Get the first letter of the user's last name
            var lastName = user.LastName ?? user.FullName?.Split(' ').LastOrDefault() ?? "X";
            var firstLetter = lastName.Substring(0, 1).ToUpper();

            // Get the highest sequence number for this letter
            var lastNumber = await _context.HEEADSSSAssessments
                .Where(a => a.FamilyNo.StartsWith(firstLetter + "-"))
                .Select(a => a.FamilyNo.Substring(2))
                .Where(n => n.All(char.IsDigit))
                .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                .MaxAsync();
                
            // Generate new family number
            var newSequence = lastNumber + 1;
            return $"{firstLetter}-{newSequence:D3}"; // Format: X-001, X-002, etc.
        }
    }
} 