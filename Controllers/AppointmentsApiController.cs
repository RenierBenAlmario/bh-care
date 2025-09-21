using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Barangay.Services;
using Barangay.Extensions;

namespace Barangay.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class AppointmentsApiController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IDataEncryptionService _encryptionService;

        public AppointmentsApiController(IConfiguration configuration, IDataEncryptionService encryptionService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _encryptionService = encryptionService;
        }

        [HttpGet("appointments/today")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetTodayAppointments()
        {
            try
            {
                var appointments = new List<AppointmentDto>();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Get today's appointments (using July 14, 2025 as specified)
                    using var command = new SqlCommand(@"
                        SELECT AppointmentId, Patient, AppointmentTime, Doctor, Type, Status 
                        FROM [Barangay].[dbo].[Appointments]
                        WHERE CONVERT(date, AppointmentDate) = '2025-07-14'
                        ORDER BY AppointmentTime", connection);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    while (await reader.ReadAsync())
                    {
                        appointments.Add(new AppointmentDto
                        {
                            AppointmentId = reader.GetInt32(0),
                            Patient = reader.GetString(1),
                            Time = reader.GetTimeSpan(2).ToString(@"hh\:mm"),
                            Doctor = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            Type = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            Status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5)
                        });
                    }
                }
                
                return appointments;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("patient-details")]
        public async Task<ActionResult<PatientDetailsDto>> GetPatientDetails(int appointmentId)
        {
            try
            {
                var patientDetails = new PatientDetailsDto();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Query to get patient details from HEADSSS assessment
                    using var command = new SqlCommand(@"
                        SELECT h.FullName, h.Age, h.Gender
                        FROM [Barangay].[dbo].[HEEADSSSAssessments] h
                        JOIN [Barangay].[dbo].[Appointments] a ON h.AppointmentId = a.AppointmentId
                        WHERE a.AppointmentId = @AppointmentId", connection);
                    
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    if (await reader.ReadAsync())
                    {
                        // Get the raw encrypted data
                        var fullName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        var gender = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                        
                        // Decrypt the encrypted fields
                        patientDetails.FullName = _encryptionService.IsEncrypted(fullName) 
                            ? _encryptionService.DecryptForUser(fullName, User) 
                            : fullName;
                        patientDetails.Age = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        patientDetails.Gender = _encryptionService.IsEncrypted(gender) 
                            ? _encryptionService.DecryptForUser(gender, User) 
                            : gender;
                    }
                }
                
                return patientDetails;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("vitals")]
        public async Task<ActionResult<VitalSignsDto>> GetVitalSigns(int appointmentId)
        {
            try
            {
                var vitalSigns = new VitalSignsDto();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Query to get vital signs
                    using var command = new SqlCommand(@"
                        SELECT Temperature, RespiratoryRate, OxygenSaturation, Weight, Height, BloodPressure, Notes
                        FROM [Barangay].[dbo].[VitalSigns]
                        WHERE AppointmentId = @AppointmentId", connection);
                    
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    if (await reader.ReadAsync())
                    {
                        vitalSigns.Temperature = reader.IsDBNull(0) ? 0 : (float)reader.GetDouble(0);
                        vitalSigns.RespiratoryRate = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        vitalSigns.OxygenSaturation = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                        vitalSigns.Weight = reader.IsDBNull(3) ? 0 : (float)reader.GetDouble(3);
                        vitalSigns.Height = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                        vitalSigns.BloodPressure = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                        vitalSigns.Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
                    }
                }
                
                return vitalSigns;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("headsss-assessment")]
        public async Task<ActionResult<HeadsssAssessmentDto>> GetHeadsssAssessment(int appointmentId)
        {
            try
            {
                var assessment = new HeadsssAssessmentDto();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Query to get HEADSSS assessment data
                    using var command = new SqlCommand(@"
                        SELECT HomeFamilyProblems, SuicidalThoughts, IsConsulted
                        FROM [Barangay].[dbo].[HEEADSSSAssessments]
                        WHERE AppointmentId = @AppointmentId", connection);
                    
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    if (await reader.ReadAsync())
                    {
                        // Decrypt the encrypted fields before returning
                        var homeFamilyProblems = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        var suicidalThoughts = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                        
                        // Decrypt if the data is encrypted
                        assessment.HomeFamilyProblems = _encryptionService.IsEncrypted(homeFamilyProblems) 
                            ? _encryptionService.DecryptForUser(homeFamilyProblems, User) 
                            : homeFamilyProblems;
                        
                        assessment.SuicidalThoughts = _encryptionService.IsEncrypted(suicidalThoughts) 
                            ? _encryptionService.DecryptForUser(suicidalThoughts, User) 
                            : suicidalThoughts;
                        
                        assessment.IsConsulted = reader.IsDBNull(2) ? "0" : reader.GetBoolean(2) ? "1" : "0";
                    }
                }
                
                return assessment;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ncd-assessment")]
        public async Task<ActionResult<NcdAssessmentDto>> GetNcdAssessment(int appointmentId)
        {
            try
            {
                var assessment = new NcdAssessmentDto();
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Query to get NCD assessment data
                    using var command = new SqlCommand(@"
                        SELECT HasDiabetes, ChestPain
                        FROM [Barangay].[dbo].[NCDRiskAssessments]
                        WHERE AppointmentId = @AppointmentId", connection);
                    
                    command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                    
                    using var reader = await command.ExecuteReaderAsync();
                    
                    if (await reader.ReadAsync())
                    {
                        assessment.HasDiabetes = reader.IsDBNull(0) ? "0" : reader.GetBoolean(0) ? "1" : "0";
                        assessment.ChestPain = reader.IsDBNull(1) ? "0" : reader.GetBoolean(1) ? "1" : "0";
                    }
                }
                
                return assessment;
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPatch("update/results")]
        public async Task<ActionResult> UpdateResults([FromBody] UpdateResultsDto updateData)
        {
            try
            {
                if (updateData == null || updateData.AppointmentId <= 0)
                {
                    return BadRequest("Invalid update data");
                }
                
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Begin transaction
                    using var transaction = connection.BeginTransaction();
                    
                    try
                    {
                        // Update vital signs if provided
                        if (updateData.VitalSigns != null)
                        {
                            using var command = new SqlCommand(@"
                                UPDATE [Barangay].[dbo].[VitalSigns]
                                SET Temperature = @Temperature,
                                    RespiratoryRate = @RespiratoryRate,
                                    OxygenSaturation = @OxygenSaturation,
                                    Weight = @Weight,
                                    Height = @Height,
                                    BloodPressure = @BloodPressure,
                                    Notes = @Notes,
                                    LastUpdatedBy = @LastEditedBy,
                                    LastUpdatedDate = GETDATE()
                                WHERE AppointmentId = @AppointmentId", connection, transaction);
                            
                            command.Parameters.AddWithValue("@Temperature", updateData.VitalSigns.Temperature);
                            command.Parameters.AddWithValue("@RespiratoryRate", updateData.VitalSigns.RespiratoryRate);
                            command.Parameters.AddWithValue("@OxygenSaturation", updateData.VitalSigns.OxygenSaturation);
                            command.Parameters.AddWithValue("@Weight", updateData.VitalSigns.Weight);
                            command.Parameters.AddWithValue("@Height", updateData.VitalSigns.Height);
                            command.Parameters.AddWithValue("@BloodPressure", updateData.VitalSigns.BloodPressure ?? string.Empty);
                            command.Parameters.AddWithValue("@Notes", updateData.VitalSigns.Notes ?? string.Empty);
                            command.Parameters.AddWithValue("@LastEditedBy", updateData.LastEditedBy);
                            command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        // Update HEADSSS assessment if provided
                        if (updateData.HeadsssAssessment != null)
                        {
                            using var command = new SqlCommand(@"
                                UPDATE [Barangay].[dbo].[HEEADSSSAssessments]
                                SET HomeFamilyProblems = @HomeFamilyProblems,
                                    SuicidalThoughts = @SuicidalThoughts,
                                    LastUpdatedBy = @LastEditedBy,
                                    LastUpdatedDate = GETDATE(),
                                    IsConsulted = CASE WHEN @IsConsulted = 1 THEN 1 ELSE IsConsulted END
                                WHERE AppointmentId = @AppointmentId", connection, transaction);
                            
                            command.Parameters.AddWithValue("@HomeFamilyProblems", updateData.HeadsssAssessment.HomeFamilyProblems ?? string.Empty);
                            command.Parameters.AddWithValue("@SuicidalThoughts", updateData.HeadsssAssessment.SuicidalThoughts ?? string.Empty);
                            command.Parameters.AddWithValue("@LastEditedBy", updateData.LastEditedBy);
                            command.Parameters.AddWithValue("@IsConsulted", updateData.IsConsulted ? 1 : 0);
                            command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        // Update NCD assessment if provided
                        if (updateData.NcdAssessment != null)
                        {
                            using var command = new SqlCommand(@"
                                UPDATE [Barangay].[dbo].[NCDRiskAssessments]
                                SET HasDiabetes = @HasDiabetes,
                                    ChestPain = @ChestPain,
                                    LastUpdatedBy = @LastEditedBy,
                                    LastUpdatedDate = GETDATE()
                                WHERE AppointmentId = @AppointmentId", connection, transaction);
                            
                            command.Parameters.AddWithValue("@HasDiabetes", updateData.NcdAssessment.HasDiabetes == "1");
                            command.Parameters.AddWithValue("@ChestPain", updateData.NcdAssessment.ChestPain == "1");
                            command.Parameters.AddWithValue("@LastEditedBy", updateData.LastEditedBy);
                            command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        // Update appointment status if marking as consulted
                        if (updateData.IsConsulted)
                        {
                            using var command = new SqlCommand(@"
                                UPDATE [Barangay].[dbo].[Appointments]
                                SET Status = 'Completed'
                                WHERE AppointmentId = @AppointmentId", connection, transaction);
                            
                            command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        // Commit transaction
                        transaction.Commit();
                        
                        return Ok(new { success = true, message = "Records updated successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Rollback transaction on error
                        transaction.Rollback();
                        throw new Exception($"Error updating records: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public string Patient { get; set; }
        public string Time { get; set; }
        public string Doctor { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class PatientDetailsDto
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
    }

    public class VitalSignsDto
    {
        public float Temperature { get; set; }
        public int RespiratoryRate { get; set; }
        public int OxygenSaturation { get; set; }
        public float Weight { get; set; }
        public int Height { get; set; }
        public string BloodPressure { get; set; }
        public string Notes { get; set; }
    }

    public class HeadsssAssessmentDto
    {
        public string HomeFamilyProblems { get; set; }
        public string SuicidalThoughts { get; set; }
        public string IsConsulted { get; set; }
    }

    public class NcdAssessmentDto
    {
        public string HasDiabetes { get; set; }
        public string ChestPain { get; set; }
    }

    public class UpdateResultsDto
    {
        public int AppointmentId { get; set; }
        public VitalSignsDto VitalSigns { get; set; }
        public HeadsssAssessmentDto HeadsssAssessment { get; set; }
        public NcdAssessmentDto NcdAssessment { get; set; }
        public string LastEditedBy { get; set; }
        public bool IsConsulted { get; set; }
    }
} 