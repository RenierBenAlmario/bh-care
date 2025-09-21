using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using Barangay.Extensions;
using System.Security.Claims;

namespace Barangay.Pages.Admin
{
    public class HEEADSSSFormManagementModel : PageModel
    {
        private readonly ILogger<HEEADSSSFormManagementModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IDataEncryptionService _encryptionService;

        public HEEADSSSFormManagementModel(ILogger<HEEADSSSFormManagementModel> logger, ApplicationDbContext context, IWebHostEnvironment environment, IDataEncryptionService encryptionService)
        {
            _logger = logger;
            _context = context;
            _environment = environment;
            _encryptionService = encryptionService;
        }

        public List<UploadedImage> UploadedImages { get; set; } = new List<UploadedImage>();

        public class UploadedImage
        {
            public string FileName { get; set; } = string.Empty;
            public string Page { get; set; } = string.Empty;
            public DateTime UploadDate { get; set; }
            public long FileSize { get; set; }
            public bool IsActive { get; set; }
        }

        public async Task OnGetAsync()
        {
            await LoadUploadedImages();
        }

        private async Task LoadUploadedImages()
        {
            try
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "heeadsss");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                    return;
                }

                var files = Directory.GetFiles(uploadsPath, "*.jpg")
                    .Concat(Directory.GetFiles(uploadsPath, "*.png"))
                    .Concat(Directory.GetFiles(uploadsPath, "*.jpeg"))
                    .OrderByDescending(f => System.IO.File.GetCreationTime(f))
                    .ToList();

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file);
                    var fileInfo = new System.IO.FileInfo(file);
                    
                    // Extract page number from filename
                    var page = "Unknown";
                    if (fileName.Contains("-1-")) page = "Page 1";
                    else if (fileName.Contains("-2-")) page = "Page 2";

                    UploadedImages.Add(new UploadedImage
                    {
                        FileName = fileName,
                        Page = page,
                        UploadDate = fileInfo.CreationTime,
                        FileSize = fileInfo.Length,
                        IsActive = false // Default to inactive
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading uploaded images");
            }
        }

        public async Task<IActionResult> OnPostUploadAsync(IFormFile imageFile, string pageNumber, string description)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please select a file to upload.";
                    return RedirectToPage();
                }

                if (string.IsNullOrEmpty(pageNumber))
                {
                    TempData["ErrorMessage"] = "Please select a page number.";
                    return RedirectToPage();
                }

                // Validate file size (5MB limit)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    TempData["ErrorMessage"] = "File size must be less than 5MB.";
                    return RedirectToPage();
                }

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Only JPG, JPEG, and PNG files are allowed.";
                    return RedirectToPage();
                }

                // Create uploads directory
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "heeadsss");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate unique filename
                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = $"heeadsss-form-{pageNumber.ToLower()}-{timestamp}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                _logger.LogInformation($"Admin {User.Identity?.Name} uploaded HEEADSSS form image: {fileName}");
                TempData["SuccessMessage"] = $"HEEADSSS form image uploaded successfully: {fileName}";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading HEEADSSS form image");
                TempData["ErrorMessage"] = "An error occurred while uploading the file.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostProcessImageForEditAsync(string fileName, string pageNumber)
        {
            try
            {
                _logger.LogInformation($"Admin {User.Identity?.Name} processed HEEADSSS form image for editing: {fileName}");

                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "heeadsss");
                var filePath = Path.Combine(uploadsPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return new JsonResult(new { success = false, message = "File not found" });
                }

                // Process image with OCR simulation
                var (isReadable, extractedData) = ProcessImageWithOCR(filePath, pageNumber);

                return new JsonResult(new
                {
                    success = true,
                    isReadable = isReadable,
                    data = extractedData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing HEEADSSS image for editing: {fileName}");
                return new JsonResult(new { success = false, message = "Error processing image" });
            }
        }

        private (bool, object) ProcessImageWithOCR(string filePath, string pageNumber)
        {
            // Simulate OCR processing with a 2-second delay
            System.Threading.Thread.Sleep(2000);

            // Simulate a 70% chance of the image being readable
            bool isReadable = new Random().Next(100) < 70;

            if (isReadable)
            {
                // Simulate extracted data based on page number
                if (pageNumber == "1")
                {
                    return (true, new
                    {
                        healthFacility = "Barangay Health Center",
                        familyNo = "FAM001",
                        fullName = "MARIA SANTOS",
                        age = 16,
                        gender = "F",
                        address = "123 Main Street",
                        contactNumber = "09123456789",
                        homeEnvironment = "Stable",
                        familyRelationship = "Good",
                        homeFamilyProblems = "None",
                        homeParentalListening = "Always",
                        homeParentalBlame = "Never",
                        homeFamilyChanges = "None recently"
                    });
                }
                else if (pageNumber == "2")
                {
                    return (true, new
                    {
                        hobbies = "Reading, Sports",
                        physicalActivity = "Regular",
                        screenTime = "Moderate",
                        activitiesParticipation = "Active",
                        activitiesRegularExercise = "Yes",
                        activitiesScreenTime = "2-3 hours daily",
                        substanceUse = false,
                        substanceType = "None",
                        drugsTobaccoUse = "No",
                        drugsAlcoholUse = "No",
                        drugsIllicitDrugUse = "No",
                        datingRelationships = "Single",
                        sexualActivity = false,
                        sexualOrientation = "Heterosexual",
                        sexualityBodyConcerns = "None",
                        sexualityIntimateRelationships = "None",
                        sexualityPartners = "None",
                        sexualitySexualOrientation = "Heterosexual",
                        sexualityPregnancy = "No",
                        sexualitySTI = "No",
                        sexualityProtection = "N/A",
                        moodChanges = "None",
                        suicidalThoughts = false,
                        selfHarmBehavior = false,
                        feelsSafeAtHome = true,
                        feelsSafeAtSchool = true,
                        experiencedBullying = false,
                        personalStrengths = "Good communication",
                        supportSystems = "Family, Friends",
                        copingMechanisms = "Talking, Exercise",
                        safetyPhysicalAbuse = false,
                        safetyRelationshipViolence = false,
                        safetyProtectiveGear = "Yes",
                        safetyGunsAtHome = false,
                        suicideDepressionFeelings = false,
                        suicideSelfHarmThoughts = false,
                        suicideFamilyHistory = false,
                        assessmentNotes = "No significant concerns",
                        recommendedActions = "Continue monitoring",
                        followUpPlan = "Regular check-ups",
                        notes = "Patient appears healthy",
                        assessedBy = "Dr. Smith"
                    });
                }
            }

            // Return empty data for unreadable images
            return (false, new { });
        }

        public async Task<IActionResult> OnPostSaveFormDataAsync(string fileName, string pageNumber, IFormCollection formData)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(pageNumber))
                {
                    return new JsonResult(new { success = false, message = "File name or page number not specified" });
                }

                // Check if this is Page 2 and if there's an existing record to update
                if (pageNumber == "2")
                {
                    // Look for existing record (most recent one with similar data)
                    var existingRecord = await _context.HEEADSSSAssessments
                        .OrderByDescending(n => n.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (existingRecord != null)
                    {
                        // Update existing record with Page 2 data
                        await UpdateExistingHEEADSSSRecord(existingRecord, formData, pageNumber);
                        _logger.LogInformation($"Admin {User.Identity?.Name} updated existing HEEADSSS record {existingRecord.Id} with Page {pageNumber} data");
                        return new JsonResult(new 
                        { 
                            success = true, 
                            message = $"HEEADSSS form data updated successfully with Page {pageNumber} information",
                            heeadsssId = existingRecord.Id,
                            isUpdate = true
                        });
                    }
                }

                // Create new record (for Page 1 or if no existing record found for Page 2)
                var result = await SaveToHEEADSSSDatabase(formData, pageNumber);
                
                if (result.Success)
                {
                    _logger.LogInformation($"Admin {User.Identity?.Name} saved HEEADSSS form data for {fileName} (Page {pageNumber}) to database with ID: {result.HEEADSSSId}");
                    return new JsonResult(new 
                    { 
                        success = true, 
                        message = $"HEEADSSS form data saved successfully to database for Page {pageNumber}",
                        heeadsssId = result.HEEADSSSId,
                        isUpdate = false
                    });
                }
                else
                {
                    return new JsonResult(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving HEEADSSS form data: {ex.Message}");
                return new JsonResult(new { success = false, message = "An error occurred while saving the form data" });
            }
        }

        private async Task<(bool Success, int? HEEADSSSId, string ErrorMessage)> SaveToHEEADSSSDatabase(IFormCollection formData, string pageNumber)
        {
            try
            {
                // Create new HEEADSSS Assessment record
                var heeadsssAssessment = new HEEADSSSAssessment
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AssessedBy = User.Identity?.Name ?? "Admin"
                };

                // Map form data to HEEADSSSAssessment properties
                MapFormDataToHEEADSSS(formData, heeadsssAssessment);

                // Encrypt sensitive data before saving
                heeadsssAssessment.EncryptSensitiveData(_encryptionService);

                // Add to database
                _context.HEEADSSSAssessments.Add(heeadsssAssessment);
                await _context.SaveChangesAsync();

                return (true, (int?)heeadsssAssessment.Id, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving HEEADSSS data to database: {ex.Message}");
                return (false, null, $"Database error: {ex.Message}");
            }
        }

        private async Task UpdateExistingHEEADSSSRecord(HEEADSSSAssessment existingRecord, IFormCollection formData, string pageNumber)
        {
            try
            {
                // Update the existing record with Page 2 data
                existingRecord.UpdatedAt = DateTime.UtcNow;
                
                // Map Page 2 specific data to existing record
                MapPageDataToHEEADSSS(formData, existingRecord, pageNumber);
                
                // Encrypt sensitive data before saving
                existingRecord.EncryptSensitiveData(_encryptionService);
                
                // Save changes
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating existing HEEADSSS record: {ex.Message}");
                throw;
            }
        }

        private void MapPageDataToHEEADSSS(IFormCollection formData, HEEADSSSAssessment heeadsssAssessment, string pageNumber)
        {
            if (pageNumber == "2")
            {
                // Activities, Drugs, Sexuality, Safety, Suicide/Depression
                if (formData.ContainsKey("hobbies"))
                    heeadsssAssessment.Hobbies = formData["hobbies"].ToString();
                
                if (formData.ContainsKey("physicalActivity"))
                    heeadsssAssessment.PhysicalActivity = formData["physicalActivity"].ToString();
                
                if (formData.ContainsKey("screenTime"))
                    heeadsssAssessment.ScreenTime = formData["screenTime"].ToString();
                
                if (formData.ContainsKey("activitiesParticipation"))
                    heeadsssAssessment.ActivitiesParticipation = formData["activitiesParticipation"].ToString();
                
                if (formData.ContainsKey("activitiesRegularExercise"))
                    heeadsssAssessment.ActivitiesRegularExercise = formData["activitiesRegularExercise"].ToString();
                
                if (formData.ContainsKey("activitiesScreenTime"))
                    heeadsssAssessment.ActivitiesScreenTime = formData["activitiesScreenTime"].ToString();
                
                if (formData.ContainsKey("substanceUse"))
                    heeadsssAssessment.SubstanceUse = formData["substanceUse"].ToString();
                
                if (formData.ContainsKey("substanceType"))
                    heeadsssAssessment.SubstanceType = formData["substanceType"].ToString();
                
                if (formData.ContainsKey("drugsTobaccoUse"))
                    heeadsssAssessment.DrugsTobaccoUse = formData["drugsTobaccoUse"].ToString();
                
                if (formData.ContainsKey("drugsAlcoholUse"))
                    heeadsssAssessment.DrugsAlcoholUse = formData["drugsAlcoholUse"].ToString();
                
                if (formData.ContainsKey("drugsIllicitDrugUse"))
                    heeadsssAssessment.DrugsIllicitDrugUse = formData["drugsIllicitDrugUse"].ToString();
                
                if (formData.ContainsKey("datingRelationships"))
                    heeadsssAssessment.DatingRelationships = formData["datingRelationships"].ToString();
                
                if (formData.ContainsKey("sexualActivity"))
                    heeadsssAssessment.SexualActivity = formData["sexualActivity"].ToString();
                
                if (formData.ContainsKey("sexualOrientation"))
                    heeadsssAssessment.SexualOrientation = formData["sexualOrientation"].ToString();
                
                if (formData.ContainsKey("sexualityBodyConcerns"))
                    heeadsssAssessment.SexualityBodyConcerns = formData["sexualityBodyConcerns"].ToString();
                
                if (formData.ContainsKey("sexualityIntimateRelationships"))
                    heeadsssAssessment.SexualityIntimateRelationships = formData["sexualityIntimateRelationships"].ToString();
                
                if (formData.ContainsKey("sexualityPartners"))
                    heeadsssAssessment.SexualityPartners = formData["sexualityPartners"].ToString();
                
                if (formData.ContainsKey("sexualitySexualOrientation"))
                    heeadsssAssessment.SexualitySexualOrientation = formData["sexualitySexualOrientation"].ToString();
                
                if (formData.ContainsKey("sexualityPregnancy"))
                    heeadsssAssessment.SexualityPregnancy = formData["sexualityPregnancy"].ToString();
                
                if (formData.ContainsKey("sexualitySTI"))
                    heeadsssAssessment.SexualitySTI = formData["sexualitySTI"].ToString();
                
                if (formData.ContainsKey("sexualityProtection"))
                    heeadsssAssessment.SexualityProtection = formData["sexualityProtection"].ToString();
                
                if (formData.ContainsKey("moodChanges"))
                    heeadsssAssessment.MoodChanges = formData["moodChanges"].ToString();
                
                if (formData.ContainsKey("suicidalThoughts"))
                    heeadsssAssessment.SuicidalThoughts = formData["suicidalThoughts"].ToString();
                
                if (formData.ContainsKey("selfHarmBehavior"))
                    heeadsssAssessment.SelfHarmBehavior = formData["selfHarmBehavior"].ToString();
                
                if (formData.ContainsKey("feelsSafeAtHome"))
                    heeadsssAssessment.FeelsSafeAtHome = formData["feelsSafeAtHome"].ToString();
                
                if (formData.ContainsKey("feelsSafeAtSchool"))
                    heeadsssAssessment.FeelsSafeAtSchool = formData["feelsSafeAtSchool"].ToString();
                
                if (formData.ContainsKey("experiencedBullying"))
                    heeadsssAssessment.ExperiencedBullying = formData["experiencedBullying"].ToString();
                
                if (formData.ContainsKey("personalStrengths"))
                    heeadsssAssessment.PersonalStrengths = formData["personalStrengths"].ToString();
                
                if (formData.ContainsKey("supportSystems"))
                    heeadsssAssessment.SupportSystems = formData["supportSystems"].ToString();
                
                if (formData.ContainsKey("copingMechanisms"))
                    heeadsssAssessment.CopingMechanisms = formData["copingMechanisms"].ToString();
                
                if (formData.ContainsKey("safetyPhysicalAbuse"))
                    heeadsssAssessment.SafetyPhysicalAbuse = formData["safetyPhysicalAbuse"].ToString();
                
                if (formData.ContainsKey("safetyRelationshipViolence"))
                    heeadsssAssessment.SafetyRelationshipViolence = formData["safetyRelationshipViolence"].ToString();
                
                if (formData.ContainsKey("safetyProtectiveGear"))
                    heeadsssAssessment.SafetyProtectiveGear = formData["safetyProtectiveGear"].ToString();
                
                if (formData.ContainsKey("safetyGunsAtHome"))
                    heeadsssAssessment.SafetyGunsAtHome = formData["safetyGunsAtHome"].ToString();
                
                if (formData.ContainsKey("suicideDepressionFeelings"))
                    heeadsssAssessment.SuicideDepressionFeelings = formData["suicideDepressionFeelings"].ToString();
                
                if (formData.ContainsKey("suicideSelfHarmThoughts"))
                    heeadsssAssessment.SuicideSelfHarmThoughts = formData["suicideSelfHarmThoughts"].ToString();
                
                if (formData.ContainsKey("suicideFamilyHistory"))
                    heeadsssAssessment.SuicideFamilyHistory = formData["suicideFamilyHistory"].ToString();
                
                if (formData.ContainsKey("assessmentNotes"))
                    heeadsssAssessment.AssessmentNotes = formData["assessmentNotes"].ToString();
                
                if (formData.ContainsKey("recommendedActions"))
                    heeadsssAssessment.RecommendedActions = formData["recommendedActions"].ToString();
                
                if (formData.ContainsKey("followUpPlan"))
                    heeadsssAssessment.FollowUpPlan = formData["followUpPlan"].ToString();
                
                if (formData.ContainsKey("notes"))
                    heeadsssAssessment.Notes = formData["notes"].ToString();
                
                if (formData.ContainsKey("assessedBy"))
                    heeadsssAssessment.AssessedBy = formData["assessedBy"].ToString();
            }
        }

        private void MapFormDataToHEEADSSS(IFormCollection formData, HEEADSSSAssessment heeadsssAssessment)
        {
            // Health Facility Information
            heeadsssAssessment.HealthFacility = formData["healthFacility"].ToString();
            heeadsssAssessment.FamilyNo = formData["familyNo"].ToString();
            
            // Personal Information
            heeadsssAssessment.FullName = formData["fullName"].ToString();
            
            // Parse age
            if (int.TryParse(formData["age"].ToString(), out int age))
            {
                heeadsssAssessment.Age = age.ToString();
            }
            
            heeadsssAssessment.Gender = formData["gender"].ToString();
            heeadsssAssessment.Address = formData["address"].ToString();
            heeadsssAssessment.ContactNumber = formData["contactNumber"].ToString();
            
            // Home Environment
            heeadsssAssessment.HomeEnvironment = formData["homeEnvironment"].ToString();
            heeadsssAssessment.FamilyRelationship = formData["familyRelationship"].ToString();
            heeadsssAssessment.HomeFamilyProblems = formData["homeFamilyProblems"].ToString();
            heeadsssAssessment.HomeParentalListening = formData["homeParentalListening"].ToString();
            heeadsssAssessment.HomeParentalBlame = formData["homeParentalBlame"].ToString();
            heeadsssAssessment.HomeFamilyChanges = formData["homeFamilyChanges"].ToString();

            // Set default values for required fields
            heeadsssAssessment.UserId = null; // No specific user associated with form upload
            heeadsssAssessment.AppointmentId = null; // No appointment associated with form upload
        }

        public async Task<IActionResult> OnPostSetActiveImageAsync(string fileName, string page)
        {
            try
            {
                _logger.LogInformation($"Admin {User.Identity?.Name} set active HEEADSSS image: {fileName}");

                // Here you would implement logic to set an image as active
                // For now, just return success
                return new JsonResult(new { success = true, message = "Image set as active" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error setting active HEEADSSS image: {fileName}");
                return new JsonResult(new { success = false, message = "Error setting active image" });
            }
        }

        public async Task<IActionResult> OnPostDeleteImageAsync(string fileName)
        {
            try
            {
                _logger.LogInformation($"Admin {User.Identity?.Name} deleted HEEADSSS form image: {fileName}");

                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "heeadsss");
                var filePath = Path.Combine(uploadsPath, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return new JsonResult(new { success = true, message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting HEEADSSS image: {fileName}");
                return new JsonResult(new { success = false, message = "Error deleting image" });
            }
        }
    }
}
