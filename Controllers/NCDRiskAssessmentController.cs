using Microsoft.AspNetCore.Mvc;
using Barangay.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using Barangay.Data;
using Microsoft.EntityFrameworkCore;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NCDRiskAssessmentController : ControllerBase
    {
        private readonly ILogger<NCDRiskAssessmentController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IDataEncryptionService _encryptionService;

        public NCDRiskAssessmentController(ILogger<NCDRiskAssessmentController> logger, ApplicationDbContext context, IDataEncryptionService encryptionService)
        {
            _logger = logger;
            _context = context;
            _encryptionService = encryptionService;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAssessment([FromForm] string jsonData)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            try
            {
                _logger.LogInformation("[{Timestamp}] === API CONTROLLER: SUBMIT ASSESSMENT CALLED ===", timestamp);
                _logger.LogInformation("[{Timestamp}] Request method: {Method}", timestamp, Request.Method);
                _logger.LogInformation("[{Timestamp}] Request content type: {ContentType}", timestamp, Request.ContentType);
                
                if (string.IsNullOrEmpty(jsonData))
                {
                    _logger.LogError("[{Timestamp}] JSON data is null or empty", timestamp);
                    return BadRequest(new { success = false, error = "No JSON data provided" });
                }
                
                _logger.LogInformation("[{Timestamp}] JSON data length: {Length}", timestamp, jsonData.Length);
                _logger.LogInformation("[{Timestamp}] JSON data preview: {Preview}", timestamp, jsonData.Substring(0, Math.Min(100, jsonData.Length)) + "...");
                
                // Deserialize the JSON data
                var assessment = JsonSerializer.Deserialize<NCDRiskAssessmentViewModel>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    Converters = { new FlexibleStringConverter(), new FlexibleIntConverter() }
                });
                
                if (assessment == null)
                {
                    _logger.LogError("[{Timestamp}] Failed to deserialize assessment data", timestamp);
                    return BadRequest(new { success = false, error = "Failed to deserialize assessment data" });
                }
                
                _logger.LogInformation("[{Timestamp}] Assessment deserialized successfully. UserId: {UserId}, AppointmentId: {AppointmentId}", 
                    timestamp, assessment.UserId, assessment.AppointmentId);
                
                // Create a simple entity with only essential fields
                var ncdAssessment = new NCDRiskAssessment
                {
                    UserId = assessment.UserId,
                    AppointmentId = assessment.AppointmentId,
                    HealthFacility = assessment.HealthFacility ?? "Unknown",
                    FamilyNo = assessment.FamilyNo ?? "UNKNOWN-000",
                    FirstName = assessment.FirstName ?? "Unknown",
                    MiddleName = assessment.MiddleName,
                    LastName = assessment.LastName ?? "Unknown",
                    Address = assessment.Address,
                    Barangay = assessment.Barangay,
                    Telepono = assessment.Telepono,
                    Birthday = assessment.Birthday,
                    Edad = assessment.Edad,
                    Kasarian = assessment.Kasarian,
                    Relihiyon = assessment.Relihiyon,
                    CivilStatus = assessment.CivilStatus,
                    Occupation = assessment.Occupation,
                    AppointmentType = assessment.AppointmentType ?? "General Checkup",
                    
                    // Medical History - with safe defaults
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
                    HasLungDisease = assessment.HasLungDisease,
                    LungDiseaseYear = assessment.LungDiseaseYear,
                    LungDiseaseMedication = assessment.LungDiseaseMedication,
                    HasCOPD = assessment.HasCOPD,
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
                    
                    // Lifestyle
                    HighSaltIntake = assessment.HighSaltIntake,
                    HasNoRegularExercise = assessment.HasNoRegularExercise,
                    
                    // Health Conditions
                    HasDifficultyBreathing = assessment.HasDifficultyBreathing,
                    HasAsthma = assessment.HasAsthma,
                    
                    // Legacy properties
                    ChestPain = assessment.ChestPain,
                    ChestPainValue = assessment.ChestPainValue,
                    RiskStatus = assessment.RiskStatus ?? "Low Risk",
                    SmokingStatus = assessment.SmokingStatus ?? "Non-smoker",
                    AlcoholConsumption = assessment.AlcoholConsumption ?? "None",
                    
                    // Timestamps
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };
                
                _logger.LogInformation("[{Timestamp}] Entity created successfully", timestamp);
                
                // Validate required fields
                if (string.IsNullOrEmpty(ncdAssessment.UserId))
                {
                    _logger.LogError("[{Timestamp}] UserId is null or empty", timestamp);
                    return BadRequest(new { success = false, error = "UserId is required" });
                }
                
                if (ncdAssessment.AppointmentId == null)
                {
                    _logger.LogError("[{Timestamp}] AppointmentId is null", timestamp);
                    return BadRequest(new { success = false, error = "AppointmentId is required" });
                }
                
                _logger.LogInformation("[{Timestamp}] Validation passed. UserId: {UserId}, AppointmentId: {AppointmentId}", 
                    timestamp, ncdAssessment.UserId, ncdAssessment.AppointmentId);
                
                // Encrypt sensitive data before saving
                _logger.LogInformation("[{Timestamp}] Encrypting sensitive data", timestamp);
                try
                {
                    ncdAssessment.EncryptSensitiveData(_encryptionService);
                    _logger.LogInformation("[{Timestamp}] Encryption completed successfully", timestamp);
                }
                catch (Exception encEx)
                {
                    _logger.LogError(encEx, "[{Timestamp}] Encryption failed: {Error}", timestamp, encEx.Message);
                    return BadRequest(new { success = false, error = "Encryption failed. Please try again." });
                }
                
                // Save to database
                _logger.LogInformation("[{Timestamp}] Adding assessment to database context", timestamp);
                _context.NCDRiskAssessments.Add(ncdAssessment);
                
                _logger.LogInformation("[{Timestamp}] Calling SaveChangesAsync", timestamp);
                var changesSaved = await _context.SaveChangesAsync();
                _logger.LogInformation("[{Timestamp}] SaveChangesAsync completed. Changes saved: {Changes}", timestamp, changesSaved);
                
                _logger.LogInformation("[{Timestamp}] Assessment saved to database successfully. ID: {Id}", timestamp, ncdAssessment.Id);
                
                // Update appointment status to InProgress (Ongoing) for nurse/doctor tasks
                if (ncdAssessment.AppointmentId.HasValue)
                {
                    _logger.LogInformation("[{Timestamp}] Updating appointment status to InProgress (Ongoing)", timestamp);
                    var appointment = await _context.Appointments.FindAsync(ncdAssessment.AppointmentId.Value);
                    if (appointment != null)
                    {
                        appointment.Status = AppointmentStatus.InProgress; // 2 = InProgress (Ongoing)
                        appointment.UpdatedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("[{Timestamp}] Appointment status updated to InProgress (Ongoing)", timestamp);
                    }
                    else
                    {
                        _logger.LogWarning("[{Timestamp}] Appointment not found for ID: {AppointmentId}", timestamp, ncdAssessment.AppointmentId);
                    }
                }
                
                return Ok(new { success = true, message = "Assessment saved successfully!", id = ncdAssessment.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Timestamp}] ERROR: {Error}", timestamp, ex.Message);
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        
        private int? ParseAppointmentId(string appointmentId)
        {
            if (string.IsNullOrEmpty(appointmentId))
                return null;
                
            // Handle format like "1-1416" - extract the number after the dash
            if (appointmentId.Contains('-'))
            {
                var parts = appointmentId.Split('-');
                if (parts.Length >= 2 && int.TryParse(parts[1], out int id))
                {
                    return id;
                }
            }
            
            // Try to parse as direct integer
            if (int.TryParse(appointmentId, out int directId))
            {
                return directId;
            }
            
            return null;
        }
    }
    
    // Custom JSON converter to handle flexible type conversion
    public class FlexibleStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? string.Empty;
            }
            else if (reader.TokenType == JsonTokenType.True)
            {
                return "true";
            }
            else if (reader.TokenType == JsonTokenType.False)
            {
                return "false";
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32().ToString();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return string.Empty;
            }
            else
            {
                throw new JsonException($"Cannot convert {reader.TokenType} to string");
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    // Custom JSON converter to handle flexible int? conversion
    public class FlexibleIntConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return null;
                
                if (int.TryParse(stringValue, out int intValue))
                    return intValue;
                
                return null;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }
            else if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            else
            {
                throw new JsonException($"Cannot convert {reader.TokenType} to int?");
            }
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
