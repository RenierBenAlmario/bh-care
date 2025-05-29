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

namespace Barangay.Pages.User
{
    public class NCDRiskAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NCDRiskAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly Random _random = new Random();

        private static readonly string[] _healthFacilities = new[]
        {
            "Barangay Health Center 161",
            "Barangay Health Center 162",
            "Barangay Health Center 163",
            "Barangay Health Center 164",
            "Barangay Health Center 165",
            "City Health Center - Main",
            "City Health Center - North",
            "City Health Center - South",
            "Rural Health Unit I",
            "Rural Health Unit II"
        };

        public NCDRiskAssessmentModel(
            ApplicationDbContext context,
            ILogger<NCDRiskAssessmentModel> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            Assessment = new NCDRiskAssessmentViewModel();
        }

        [BindProperty]
        public NCDRiskAssessmentViewModel Assessment { get; set; }

        public string HealthFacility { get; set; }
        public string FamilyNo { get; set; }
        public bool FamilyNoPreexisting { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string appointmentId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found");
                    return NotFound("User not found");
                }

                // Get health facility based on user's address
                HealthFacility = GetHealthFacility(user);
                _logger.LogInformation($"Health Facility set to: {HealthFacility}");

                // Get or generate Family No
                var (familyNo, isPreexisting) = await GetOrGenerateFamilyNumberAsync(user);
                FamilyNo = familyNo;
                FamilyNoPreexisting = isPreexisting;
                _logger.LogInformation($"Family No set to: {FamilyNo} (Preexisting: {FamilyNoPreexisting})");

                // Initialize Assessment
                Assessment = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentId,
                    UserId = user.Id,
                    HealthFacility = HealthFacility,
                    FamilyNo = FamilyNo,
                    Address = user.Address ?? "",
                    Birthday = user.BirthDate,
                    Telepono = user.PhoneNumber ?? "",
                    Edad = CalculateAge(user.BirthDate)
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NCDRiskAssessment GET");
                StatusMessage = "Error loading assessment form. Please try again.";
                return RedirectToPage("/User/UserDashboard");
            }
        }

        private string GetHealthFacility(ApplicationUser user)
        {
            try
            {
                // Default health facility
                string defaultFacility = "Barangay Health Center 161";

                // If user has an address with a barangay number, use that
                if (!string.IsNullOrEmpty(user.Address))
                {
                    var words = user.Address.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var word in words)
                    {
                        if (int.TryParse(word, out int barangayNumber))
                        {
                            return $"Barangay Health Center {barangayNumber}";
                        }
                    }
                }

                _logger.LogInformation($"Using default health facility: {defaultFacility}");
                return defaultFacility;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting health facility");
                return "Barangay Health Center 161";
            }
        }

        private async Task<(string familyNo, bool isPreexisting)> GetOrGenerateFamilyNumberAsync(ApplicationUser user)
        {
            try
            {
                // Check for existing family number in NCDRiskAssessments
                var existingAssessment = await _context.NCDRiskAssessments
                    .Where(a => a.UserId == user.Id)
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                if (existingAssessment != null && !string.IsNullOrEmpty(existingAssessment.FamilyNo))
                {
                    _logger.LogInformation($"Found existing family number: {existingAssessment.FamilyNo}");
                    return (existingAssessment.FamilyNo, true);
                }

                // Get first letter of last name
                var lastName = user.LastName ?? user.UserName?.Split(' ').LastOrDefault() ?? "X";
                var firstLetter = lastName.Substring(0, 1).ToUpper();

                // Get highest sequence number for this letter
                var existingNumbers = await _context.NCDRiskAssessments
                    .Where(a => a.FamilyNo.StartsWith(firstLetter))
                    .Select(a => a.FamilyNo)
                    .ToListAsync();

                int maxSequence = 0;
                foreach (var number in existingNumbers)
                {
                    if (number.Length > 2 && int.TryParse(number.Substring(2), out int sequence))
                    {
                        maxSequence = Math.Max(maxSequence, sequence);
                }
                }

                var newFamilyNo = $"{firstLetter}-{(maxSequence + 1):D3}";
                _logger.LogInformation($"Generated new family number: {newFamilyNo}");
                return (newFamilyNo, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating family number");
                return ("ERR-001", false);
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            var referenceDate = new DateTime(2025, 5, 29, 13, 0, 0); // May 29, 2025, 1:00 PM PST
            var age = referenceDate.Year - birthDate.Year;
            
            if (birthDate.Date > referenceDate.AddYears(-age))
            {
                age--;
            }
            
            return age;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var assessment = new NCDRiskAssessment
                {
                    UserId = user.Id,
                    AppointmentId = int.Parse(Assessment.AppointmentId ?? "0"),
                    HealthFacility = Assessment.HealthFacility,
                    FamilyNo = Assessment.FamilyNo,
                    Address = Assessment.Address,
                    Barangay = Assessment.Barangay,
                    Birthday = Assessment.Birthday,
                    Telepono = Assessment.Telepono,
                    Edad = Assessment.Edad,
                    Kasarian = Assessment.Kasarian,
                    HasDiabetes = Assessment.HasDiabetes,
                    HasHypertension = Assessment.HasHypertension,
                    HasCancer = Assessment.HasCancer,
                    HasCOPD = Assessment.HasCOPD,
                    HasLungDisease = Assessment.HasLungDisease,
                    HasEyeDisease = Assessment.HasEyeDisease,
                    HighSaltIntake = Assessment.HighSaltIntake,
                    AlcoholFrequency = Assessment.AlcoholFrequency,
                    ExerciseDuration = Assessment.ExerciseDuration,
                    SmokingStatus = "Non-smoker", // Default value
                    RiskStatus = "Low Risk", // Default value
                    CreatedAt = DateTime.UtcNow
                };

                _context.NCDRiskAssessments.Add(assessment);
                await _context.SaveChangesAsync();

                StatusMessage = "Assessment saved successfully.";
                return RedirectToPage("/User/UserDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving assessment");
                ModelState.AddModelError("", "Error saving assessment. Please try again.");
                return Page();
            }
        }
    }
} 