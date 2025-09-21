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
using System.Collections.Generic; // Added for Dictionary
using System.Data; // Added for DBNull
using Barangay.Services;
using Barangay.Extensions; // Added for DecryptSensitiveData extension method
using Microsoft.AspNetCore.Authorization;

namespace Barangay.Pages.User
{
    [Authorize]
    public class NCDRiskAssessmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NCDRiskAssessmentModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataEncryptionService _encryptionService;
        private static readonly Random _random = new Random();

        private static readonly string[] _healthFacilities = new[]
        {
            "Barangay 158",
            "Barangay 159", 
            "Barangay 160",
            "Barangay 161"
        };

        public NCDRiskAssessmentModel(
            ApplicationDbContext context,
            ILogger<NCDRiskAssessmentModel> logger,
            UserManager<ApplicationUser> userManager,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _encryptionService = encryptionService;
            Assessment = new NCDRiskAssessmentViewModel();
        }

        [BindProperty]
        public NCDRiskAssessmentViewModel Assessment { get; set; }

        public string HealthFacility { get; set; }
        public string FamilyNo { get; set; }
        public bool FamilyNoPreexisting { get; set; }
        public int? CalculatedAge { get; set; }

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

                HealthFacility = GetHealthFacility(user);
                _logger.LogInformation("Health Facility set to: {HealthFacility}", HealthFacility);

                try
                {
                    (string familyNo, bool isPreexisting) = await GetOrGenerateFamilyNumberAsync(user);
                    FamilyNo = familyNo;
                    FamilyNoPreexisting = isPreexisting;
                    _logger.LogInformation("Family No set to: {FamilyNo} (Preexisting: {FamilyNoPreexisting})", FamilyNo, FamilyNoPreexisting);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting family number, using default");
                    string lastNameInitial = !string.IsNullOrEmpty(user.LastName) ? user.LastName.Substring(0, 1) : "X";
                    FamilyNo = $"{lastNameInitial}-001";
                    FamilyNoPreexisting = false;
                }

                int? appointmentIdInt = null;
                if (int.TryParse(appointmentId, out int parsedId))
                {
                    appointmentIdInt = parsedId;
                }

                Assessment = new NCDRiskAssessmentViewModel
                {
                    AppointmentId = appointmentIdInt,
                    UserId = user.Id,
                    HealthFacility = HealthFacility,
                    FamilyNo = FamilyNo,
                    Address = user.Address ?? "",
                    Barangay = "122", // Default barangay
                    Birthday = DateTime.TryParse(user.BirthDate, out var parsedBirthDateForForm) && parsedBirthDateForForm != DateTime.MinValue ? parsedBirthDateForForm.ToString("yyyy-MM-dd") : null,
                    Telepono = user.PhoneNumber ?? "",
                    Kasarian = user.Gender == "Male" ? "Lalaki" : user.Gender == "Female" ? "Babae" : "",
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Occupation = user.Occupation,
                    CivilStatus = user.CivilStatus,
                    Relihiyon = user.Religion
                };

                var userBirthDate = DateTime.TryParse(user.BirthDate, out var parsedUserBirthDate) ? parsedUserBirthDate : DateTime.MinValue;
                if (userBirthDate != DateTime.MinValue)
                {
                    var age = CalculateAge(userBirthDate);
                    Assessment.Edad = age.ToString();
                    CalculatedAge = age;
                    _logger.LogInformation("Calculated age: {Age}", CalculatedAge);
                }
                else
                {
                    _logger.LogWarning("Birthday not available for age calculation.");
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in NCDRiskAssessment GET");
                StatusMessage = "Error loading assessment page. Please try again.";
                return RedirectToPage("/Index");
            }
        }

        private string GetHealthFacility(ApplicationUser user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Address))
            {
                return _healthFacilities[0];
            }

            int hashCode = Math.Abs(user.Address.GetHashCode());
            int index = hashCode % _healthFacilities.Length;
            return _healthFacilities[index];
        }

        private async Task<(string familyNo, bool isPreexisting)> GetOrGenerateFamilyNumberAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Check if user already has a family number
            var existingAssessment = await _context.NCDRiskAssessments
                .Where(a => a.UserId == user.Id && !string.IsNullOrEmpty(a.FamilyNo))
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingAssessment != null)
            {
                // Decrypt FamilyNo if it's encrypted
                var decryptedFamilyNo = existingAssessment.FamilyNo;
                if (!string.IsNullOrEmpty(decryptedFamilyNo) && _encryptionService.CanUserDecrypt(User))
                {
                    try
                    {
                        decryptedFamilyNo = _encryptionService.Decrypt(decryptedFamilyNo);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to decrypt FamilyNo, using encrypted value");
                    }
                }
                return (decryptedFamilyNo ?? "UNKNOWN-000", true);
            }

            // Generate new family number based on first letter of last name
            string lastNameInitial = (user.LastName?.Length > 0) ? user.LastName.Substring(0, 1).ToUpper() : "X";
            
            // Find the next sequential number for this letter
            var existingFamilyNumbers = await _context.NCDRiskAssessments
                .Where(a => !string.IsNullOrEmpty(a.FamilyNo) && a.FamilyNo.StartsWith($"{lastNameInitial}."))
                .Select(a => a.FamilyNo)
                .ToListAsync();

            int nextNumber = 1;
            if (existingFamilyNumbers.Any())
            {
                var numbers = existingFamilyNumbers
                    .Where(fn => fn.StartsWith($"{lastNameInitial}."))
                    .Select(fn => 
                    {
                        var parts = fn.Split('.');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                            return num;
                        return 0;
                    })
                    .Where(num => num > 0)
                    .OrderByDescending(num => num)
                    .ToList();

                nextNumber = numbers.Any() ? numbers.First() + 1 : 1;
            }

            string newFamilyNo = $"{lastNameInitial}.{nextNumber:D3}";
            return (newFamilyNo, false);
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            _logger.LogInformation("Calculated age for birthdate {BirthDate}: {Age} years", birthDate.ToString("M/d/yyyy"), age);
            return age;
        }

        public class AgeCalculationRequest
        {
            public DateTime Birthday { get; set; }
        }

        // Handler for AJAX calls to generate family number
        public async Task<IActionResult> OnPostGenerateFamilyNumberAsync([FromBody] GenerateFamilyNumberRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return new JsonResult(new { success = false, error = "User not found" });
                }

                // Check if user already has a family number
                var existingAssessment = await _context.NCDRiskAssessments
                    .Where(a => a.UserId == user.Id && !string.IsNullOrEmpty(a.FamilyNo))
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefaultAsync();

                if (existingAssessment != null)
                {
                    // Decrypt FamilyNo if it's encrypted
                    var decryptedFamilyNo = existingAssessment.FamilyNo;
                    if (!string.IsNullOrEmpty(decryptedFamilyNo) && _encryptionService.CanUserDecrypt(User))
                    {
                        try
                        {
                            decryptedFamilyNo = _encryptionService.Decrypt(decryptedFamilyNo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to decrypt FamilyNo, using encrypted value");
                        }
                    }
                    return new JsonResult(new { 
                        success = true, 
                        familyNo = decryptedFamilyNo, 
                        isPreexisting = true 
                    });
                }

                // Generate new family number based on first letter of last name
                string lastNameInitial = (user.LastName?.Length > 0) ? user.LastName.Substring(0, 1).ToUpper() : "X";
                
                // Find the next sequential number for this letter
                var existingFamilyNumbers = await _context.NCDRiskAssessments
                    .Where(a => !string.IsNullOrEmpty(a.FamilyNo) && a.FamilyNo.StartsWith($"{lastNameInitial}."))
                    .Select(a => a.FamilyNo)
                    .ToListAsync();

                int nextNumber = 1;
                if (existingFamilyNumbers.Any())
                {
                    var numbers = existingFamilyNumbers
                        .Where(fn => fn.StartsWith($"{lastNameInitial}."))
                        .Select(fn => 
                        {
                            var parts = fn.Split('.');
                            if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                                return num;
                            return 0;
                        })
                        .Where(num => num > 0)
                        .OrderByDescending(num => num)
                        .ToList();

                    nextNumber = numbers.Any() ? numbers.First() + 1 : 1;
                }

                string newFamilyNo = $"{lastNameInitial}.{nextNumber:D3}";
                
                return new JsonResult(new { 
                    success = true, 
                    familyNo = newFamilyNo, 
                    isPreexisting = false 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating family number");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }

        public class GenerateFamilyNumberRequest
        {
            public string LastName { get; set; } = string.Empty;
        }

        public IActionResult OnGetTestEndpoint()
        {
            _logger.LogInformation("Test endpoint called successfully");
            return new JsonResult(new { success = true, message = "Test endpoint working" });
        }

        public async Task<IActionResult> OnPostSubmitAssessmentAsync(string encryptedData)
        {
            try
            {
                _logger.LogInformation("=== SUBMIT ASSESSMENT CALLED ===");
                _logger.LogInformation("Request method: {Method}", Request.Method);
                _logger.LogInformation("Request content type: {ContentType}", Request.ContentType);
                
                if (string.IsNullOrEmpty(encryptedData))
                {
                    _logger.LogError("Encrypted data is null or empty");
                    return new JsonResult(new { success = false, error = "No encrypted data provided" });
                }
                
                _logger.LogInformation("Encrypted data length: {Length}", encryptedData.Length);
                _logger.LogInformation("Encrypted data preview: {Preview}", encryptedData.Substring(0, Math.Min(50, encryptedData.Length)) + "...");
                
                // Decrypt the data
                var decryptedJson = DecryptData(encryptedData);
                if (string.IsNullOrEmpty(decryptedJson))
                {
                    _logger.LogError("Failed to decrypt data");
                    return new JsonResult(new { success = false, error = "Failed to decrypt data" });
                }
                
                _logger.LogInformation("Decrypted JSON length: {Length}", decryptedJson.Length);
                
                // Deserialize the decrypted JSON
                var assessment = System.Text.Json.JsonSerializer.Deserialize<NCDRiskAssessmentViewModel>(decryptedJson, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                });
                
                if (assessment == null)
                {
                    _logger.LogError("Failed to deserialize assessment data");
                    return new JsonResult(new { success = false, error = "Failed to deserialize assessment data" });
                }
                
                _logger.LogInformation("Assessment deserialized successfully. UserId: {UserId}, AppointmentId: {AppointmentId}", 
                    assessment.UserId, assessment.AppointmentId);
                
                // Validate required fields
                if (string.IsNullOrEmpty(assessment.UserId))
                {
                    _logger.LogError("UserId is required but not provided");
                    return new JsonResult(new { success = false, error = "User ID is required" });
                }
                
                // Parse AppointmentId
                int? appointmentIdInt = assessment.AppointmentId;
                if (appointmentIdInt.HasValue)
                {
                    _logger.LogInformation("Using AppointmentId: {AppointmentId}", appointmentIdInt);
                }
                else
                {
                    _logger.LogWarning("AppointmentId is null or invalid: {AppointmentId}", assessment.AppointmentId);
                }
                
                // Verify appointment exists if AppointmentId is provided
                if (appointmentIdInt.HasValue)
                {
                    var appointmentExists = await _context.Appointments.AnyAsync(a => a.Id == appointmentIdInt.Value);
                    if (!appointmentExists)
                    {
                        _logger.LogError("Appointment with ID {AppointmentId} does not exist", appointmentIdInt.Value);
                        return new JsonResult(new { success = false, error = "Appointment not found" });
                    }
                }
                
                // Map ViewModel to Entity
                var ncdEntity = new NCDRiskAssessment
                {
                    UserId = assessment.UserId,
                    AppointmentId = appointmentIdInt,
                    HealthFacility = assessment.HealthFacility ?? "Barangay Health Center",
                    FamilyNo = assessment.FamilyNo,
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Birthday = assessment.Birthday,
                    Telepono = assessment.Telepono,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    CivilStatus = assessment.CivilStatus,
                    FirstName = assessment.FirstName,
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName,
                    Occupation = assessment.Occupation,
                    AppointmentType = assessment.AppointmentType ?? "General Checkup",
                    
                    // Medical History
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
                    
                    // Family History
                    FamilyHasHypertension = assessment.FamilyHasHypertension,
                    FamilyHasHeartDisease = assessment.FamilyHasHeartDisease,
                    FamilyHasStroke = assessment.FamilyHasStroke,
                    FamilyHasDiabetes = assessment.FamilyHasDiabetes,
                    FamilyHasCancer = assessment.FamilyHasCancer,
                    FamilyHasKidneyDisease = assessment.FamilyHasKidneyDisease,
                    FamilyHasOtherDisease = assessment.FamilyHasOtherDisease,
                    FamilyOtherDiseaseDetails = assessment.FamilyOtherDiseaseDetails,
                    
                    // Lifestyle Factors
                    SmokingStatus = assessment.SmokingStatus ?? "Non-smoker",
                    HighSaltIntake = assessment.HighSaltIntake,
                    AlcoholFrequency = assessment.AlcoholFrequency,
                    AlcoholConsumption = assessment.AlcoholConsumption,
                    ExerciseDuration = assessment.ExerciseDuration,
                    HasNoRegularExercise = assessment.HasNoRegularExercise,
                    
                    // Health Conditions
                    HasDifficultyBreathing = assessment.HasDifficultyBreathing,
                    HasAsthma = assessment.HasAsthma,
                    
                    // Risk Status
                    RiskStatus = assessment.RiskStatus ?? "Low Risk",
                    
                    // System Fields
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                _logger.LogInformation("Created NCD entity with UserId: {UserId}, AppointmentId: {AppointmentId}", 
                    ncdEntity.UserId, ncdEntity.AppointmentId);
                
                // Encrypt sensitive data before saving
                _logger.LogInformation("Encrypting sensitive data for NCD assessment");
                try
                {
                    ncdEntity.EncryptSensitiveData(_encryptionService);
                    _logger.LogInformation("Encryption completed successfully");
                }
                catch (Exception encEx)
                {
                    _logger.LogError(encEx, "Encryption failed: {Error}", encEx.Message);
                    return new JsonResult(new { success = false, error = "Encryption failed. Please try again." });
                }
                
                // Add to context
                _context.NCDRiskAssessments.Add(ncdEntity);
                
                // Save to database
                var rowsAffected = await _context.SaveChangesAsync();
                _logger.LogInformation("Database save completed. Rows affected: {RowsAffected}", rowsAffected);
                
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("NCD Risk Assessment saved successfully with ID: {Id}", ncdEntity.Id);
                    return new JsonResult(new { 
                        success = true, 
                        message = "Assessment submitted successfully!",
                        assessmentId = ncdEntity.Id,
                        rowsAffected = rowsAffected
                    });
                }
                else
                {
                    _logger.LogError("No rows were affected during save operation");
                    return new JsonResult(new { 
                        success = false, 
                        error = "Failed to save assessment - no rows affected" 
                    });
                }
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error: {Message}", dbEx.Message);
                return new JsonResult(new { 
                    success = false, 
                    error = "Database error occurred while saving assessment",
                    details = dbEx.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR: {Error}", ex.Message);
                return new JsonResult(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
        
        // Decryption method that matches the JavaScript encryption
        private string DecryptData(string encryptedData)
        {
            try
            {
                _logger.LogInformation("Starting decryption process");
                
                // Use the same key as JavaScript (in production, this should be in appsettings.json)
                const string ENCRYPTION_KEY = "BHCARE_2024_SECRET_KEY_32BYTES_LONG";
                
                // Convert key to bytes (first 32 bytes for AES-256)
                var keyBytes = System.Text.Encoding.UTF8.GetBytes(ENCRYPTION_KEY).Take(32).ToArray();
                
                // Decode the base64 encrypted data
                var encryptedBytes = Convert.FromBase64String(encryptedData);
                
                // Extract IV (first 16 bytes) and encrypted data
                var iv = new byte[16];
                var encrypted = new byte[encryptedBytes.Length - 16];
                
                Array.Copy(encryptedBytes, 0, iv, 0, 16);
                Array.Copy(encryptedBytes, 16, encrypted, 0, encrypted.Length);
                
                // Decrypt using AES-256-CBC
                using (var aes = System.Security.Cryptography.Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                    aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
                    
                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(encrypted))
                    using (var csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        var decryptedText = srDecrypt.ReadToEnd();
                        _logger.LogInformation("Decryption successful. Decrypted text length: {Length}", decryptedText.Length);
                        return decryptedText;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed: {Error}", ex.Message);
                return null;
            }
        }
        
        private string SafeEncrypt(string plainText)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    return plainText;
                
                // Temporarily disable encryption to fix 400 error
                _logger.LogInformation("Encryption temporarily disabled, returning plain text for length: {Length}", plainText.Length);
                return plainText;
                
                /*
                if (_encryptionService == null)
                {
                    _logger.LogWarning("Encryption service is null, returning plain text");
                    return plainText;
                }
                
                var encrypted = _encryptionService.Encrypt(plainText);
                _logger.LogInformation("Successfully encrypted text of length: {Length}", plainText.Length);
                return encrypted;
                */
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed for text: {Text}, using plain text", plainText);
                return plainText; // Return plain text if encryption fails
            }
        }
        

        // Simple database test endpoint
        public async Task<IActionResult> OnGetTestDatabaseAsync()
        {
            try
            {
                _logger.LogInformation("Testing database connection");
                
                // Test basic connection
                var canConnect = await _context.Database.CanConnectAsync();
                _logger.LogInformation("Database connection test: {CanConnect}", canConnect);
                
                if (!canConnect)
                {
                    return new JsonResult(new { success = false, error = "Cannot connect to database" });
                }
                
                // Test simple query
                var count = await _context.NCDRiskAssessments.CountAsync();
                _logger.LogInformation("NCDRiskAssessments table count: {Count}", count);
                
                // Test inserting a simple record
                var testAssessment = new NCDRiskAssessment
                {
                    UserId = "test-user-id",
                    HealthFacility = "Test Facility",
                    FamilyNo = "TEST-001",
                    Address = "Test Address",
                    Barangay = "Test Barangay",
                    Birthday = DateTime.Now.AddYears(-30).ToString("yyyy-MM-dd"),
                    Telepono = "1234567890",
                    Edad = "30",
                    Kasarian = "Male",
                    Relihiyon = "Test Religion",
                    HasDiabetes = "false",
                    HasHypertension = "false",
                    HasCancer = "false",
                    HasCOPD = "false",
                    HasLungDisease = "false",
                    HasEyeDisease = "false",
                    CancerType = "None",
                    FamilyHasHypertension = "false",
                    FamilyHasHeartDisease = "false",
                    FamilyHasStroke = "false",
                    FamilyHasDiabetes = "false",
                    FamilyHasCancer = "false",
                    FamilyHasKidneyDisease = "false",
                    FamilyHasOtherDisease = "false",
                    FamilyOtherDiseaseDetails = "None",
                    HighSaltIntake = "false",
                    AlcoholFrequency = "None",
                    AlcoholConsumption = "None",
                    ExerciseDuration = "None",
                    SmokingStatus = "Non-smoker",
                    RiskStatus = "Low Risk",
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    AppointmentType = "Test",
                    FirstName = "Test",
                    MiddleName = "Test",
                    LastName = "User",
                    Occupation = "Test",
                    CivilStatus = "Single",
                    HasDifficultyBreathing = "false",
                    HasAsthma = "false",
                    HasNoRegularExercise = "false"
                };

                _context.NCDRiskAssessments.Add(testAssessment);
                var rowsAffected = await _context.SaveChangesAsync();
                _logger.LogInformation("Test record inserted successfully. Rows affected: {RowsAffected}", rowsAffected);

                // Clean up test record
                _context.NCDRiskAssessments.Remove(testAssessment);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Test record cleaned up");
                
                return new JsonResult(new { 
                    success = true, 
                    message = "Database connection working",
                    canConnect = canConnect,
                    tableCount = count,
                    testInsertSuccessful = rowsAffected > 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database test failed: {Message}", ex.Message);
                return new JsonResult(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    
        // AJAX endpoint to get next sequential family number
        public async Task<IActionResult> OnGetNextFamilyNumberAsync(string letterPrefix)
        {
            try
            {
                if (string.IsNullOrEmpty(letterPrefix) || letterPrefix.Length == 0)
                {
                    return new JsonResult(new { success = false, error = "Letter prefix is required" });
                }

                string firstLetter = letterPrefix.Substring(0, 1).ToUpper();
                
                // Get the highest sequence number for this letter from both assessment types
                int lastNCDNumber = await _context.NCDRiskAssessments
                    .Where(a => a.FamilyNo != null && a.FamilyNo.StartsWith(firstLetter + "-"))
                    .Select(a => a.FamilyNo.Substring(2))
                    .Where(n => n.All(char.IsDigit))
                    .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                    .MaxAsync();
                    
                int lastHEEADSSSNumber = await _context.HEEADSSSAssessments
                    .Where(a => a.FamilyNo != null && a.FamilyNo.StartsWith(firstLetter + "-"))
                    .Select(a => a.FamilyNo.Substring(2))
                    .Where(n => n.All(char.IsDigit))
                    .Select(n => int.Parse(n))
                    .DefaultIfEmpty(0)
                    .MaxAsync();
                    
                // Take the highest of the two numbers
                int lastNumber = Math.Max(lastNCDNumber, lastHEEADSSSNumber);
                
                // Generate new family number
                int newSequence = lastNumber + 1;
                string newFamilyNo = $"{firstLetter}-{newSequence:D3}"; // Format: X-001, X-002, etc.
                
                return new JsonResult(new { 
                    success = true, 
                    familyNo = newFamilyNo,
                    lastNumber = lastNumber,
                    newSequence = newSequence
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating next family number");
                return new JsonResult(new { success = false, error = "Error generating next family number" });
            }
        }
    }
}
