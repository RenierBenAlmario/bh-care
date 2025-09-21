using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;

namespace Barangay.Pages.User
{
    public class TestDatabaseModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;
        private readonly ILogger<TestDatabaseModel> _logger;

        public TestDatabaseModel(
            ApplicationDbContext context,
            IDataEncryptionService encryptionService,
            ILogger<TestDatabaseModel> logger)
        {
            _context = context;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                _logger.LogInformation("Testing database connection and encryption service");

                // Test database connection
                var canConnect = await _context.Database.CanConnectAsync();
                _logger.LogInformation("Database connection test: {CanConnect}", canConnect);

                if (!canConnect)
                {
                    return new JsonResult(new { success = false, error = "Cannot connect to database" });
                }

                // Test encryption service
                var testData = "Test encryption data";
                var encrypted = _encryptionService.Encrypt(testData);
                var decrypted = _encryptionService.Decrypt(encrypted);
                _logger.LogInformation("Encryption test: Original='{Original}', Encrypted='{Encrypted}', Decrypted='{Decrypted}'", 
                    testData, encrypted, decrypted);

                if (decrypted != testData)
                {
                    return new JsonResult(new { success = false, error = "Encryption service not working properly" });
                }

                // Test NCDRiskAssessments table structure
                var tableExists = await _context.Database.ExecuteSqlRawAsync("SELECT COUNT(*) FROM NCDRiskAssessments") >= 0;
                _logger.LogInformation("NCDRiskAssessments table test: {TableExists}", tableExists);

                // Test inserting a simple record
                var testAssessment = new NCDRiskAssessment
                {
                    UserId = "test-user-id",
                    HealthFacility = "Test Facility",
                    FamilyNo = _encryptionService.Encrypt("TEST-001"),
                    Address = _encryptionService.Encrypt("Test Address"),
                    Barangay = "Test Barangay",
                    Birthday = DateTime.Now.AddYears(-30).ToString("yyyy-MM-dd"),
                    Telepono = _encryptionService.Encrypt("1234567890"),
                    Edad = "30",
                    Kasarian = "Male",
                    Relihiyon = "Test Religion",
                    HasDiabetes = "false",
                    HasHypertension = "false",
                    HasCancer = "false",
                    HasCOPD = "false",
                    HasLungDisease = "false",
                    HasEyeDisease = "false",
                    CancerType = _encryptionService.Encrypt("None"),
                    FamilyHasHypertension = "false",
                    FamilyHasHeartDisease = "false",
                    FamilyHasStroke = "false",
                    FamilyHasDiabetes = "false",
                    FamilyHasCancer = "false",
                    FamilyHasKidneyDisease = "false",
                    FamilyHasOtherDisease = "false",
                    FamilyOtherDiseaseDetails = _encryptionService.Encrypt("None"),
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
                    message = "Database and encryption service working properly",
                    canConnect = canConnect,
                    encryptionWorking = decrypted == testData,
                    tableExists = tableExists,
                    testInsertSuccessful = rowsAffected > 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database test");
                return new JsonResult(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
