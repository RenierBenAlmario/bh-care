using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;
using System.Security.Claims;

namespace Barangay.Controllers.Api
{
    [Route("api/update")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class UpdateResultsController : ControllerBase
    {
        private readonly string _connectionString;

        public UpdateResultsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPatch("results")]
        public async Task<IActionResult> UpdateResults([FromBody] UpdateResultsDto updateData)
        {
            try
            {
                if (updateData == null || updateData.AppointmentId <= 0)
                {
                    return BadRequest("Invalid update data");
                }

                var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var doctorName = User.FindFirstValue(ClaimTypes.Name) ?? "Doctor";
                
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
                            // Check if record exists
                            var checkCmd = new SqlCommand(
                                "SELECT COUNT(*) FROM [Barangay].[dbo].[VitalSigns] WHERE AppointmentId = @AppointmentId", 
                                connection, transaction);
                            checkCmd.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            var result = await checkCmd.ExecuteScalarAsync();
                            var recordExists = result != null && Convert.ToInt32(result) > 0;
                            
                            if (recordExists)
                            {
                                // Update existing record
                                using var command = new SqlCommand(@"
                                    UPDATE [Barangay].[dbo].[VitalSigns]
                                    SET Temperature = @Temperature,
                                        RespiratoryRate = @RespiratoryRate,
                                        OxygenSaturation = @OxygenSaturation,
                                        Weight = @Weight,
                                        Height = @Height,
                                        BloodPressure = @BloodPressure,
                                        Notes = @Notes,
                                        LastEditedBy = @LastEditedBy,
                                        LastEditedDate = @LastEditedDate
                                    WHERE AppointmentId = @AppointmentId", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@Temperature", updateData.VitalSigns.Temperature ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@RespiratoryRate", updateData.VitalSigns.RespiratoryRate ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@OxygenSaturation", updateData.VitalSigns.OxygenSaturation ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Weight", updateData.VitalSigns.Weight ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Height", updateData.VitalSigns.Height ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@BloodPressure", updateData.VitalSigns.BloodPressure ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Notes", updateData.VitalSigns.Notes ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@LastEditedBy", doctorName);
                                command.Parameters.AddWithValue("@LastEditedDate", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                // Insert new record
                                using var command = new SqlCommand(@"
                                    INSERT INTO [Barangay].[dbo].[VitalSigns]
                                    (AppointmentId, Temperature, RespiratoryRate, OxygenSaturation, 
                                     Weight, Height, BloodPressure, Notes, RecordedBy, RecordedAt)
                                    VALUES
                                    (@AppointmentId, @Temperature, @RespiratoryRate, @OxygenSaturation,
                                     @Weight, @Height, @BloodPressure, @Notes, @RecordedBy, @RecordedAt)", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@Temperature", updateData.VitalSigns.Temperature ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@RespiratoryRate", updateData.VitalSigns.RespiratoryRate ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@OxygenSaturation", updateData.VitalSigns.OxygenSaturation ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Weight", updateData.VitalSigns.Weight ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Height", updateData.VitalSigns.Height ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@BloodPressure", updateData.VitalSigns.BloodPressure ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@Notes", updateData.VitalSigns.Notes ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@RecordedBy", doctorName);
                                command.Parameters.AddWithValue("@RecordedAt", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        
                        // Update HEADSSS assessment if provided
                        if (updateData.HeadsssAssessment != null)
                        {
                            // Check if record exists
                            var checkCmd = new SqlCommand(
                                "SELECT COUNT(*) FROM [Barangay].[dbo].[HEEADSSSAssessments] WHERE AppointmentId = @AppointmentId", 
                                connection, transaction);
                            checkCmd.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            var result = await checkCmd.ExecuteScalarAsync();
                            var recordExists = result != null && Convert.ToInt32(result) > 0;
                            
                            if (recordExists)
                            {
                                // Update existing record
                                using var command = new SqlCommand(@"
                                    UPDATE [Barangay].[dbo].[HEEADSSSAssessments]
                                    SET HomeFamilyProblems = @HomeFamilyProblems,
                                        SuicidalThoughts = @SuicidalThoughts,
                                        IsConsulted = @IsConsulted,
                                        LastEditedBy = @LastEditedBy,
                                        LastEditedDate = @LastEditedDate
                                    WHERE AppointmentId = @AppointmentId", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@HomeFamilyProblems", updateData.HeadsssAssessment.HomeFamilyProblems ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@SuicidalThoughts", updateData.HeadsssAssessment.SuicidalThoughts ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@IsConsulted", updateData.IsConsulted);
                                command.Parameters.AddWithValue("@LastEditedBy", doctorName);
                                command.Parameters.AddWithValue("@LastEditedDate", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                // Get patient information from appointment
                                var patientInfoCmd = new SqlCommand(
                                    "SELECT Patient, PatientId FROM [Barangay].[dbo].[Appointments] WHERE AppointmentId = @AppointmentId", 
                                    connection, transaction);
                                patientInfoCmd.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                
                                string patientName = string.Empty;
                                string patientId = string.Empty;
                                
                                using (var reader = await patientInfoCmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        patientName = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                        patientId = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                    }
                                }
                                
                                // Insert new record
                                using var command = new SqlCommand(@"
                                    INSERT INTO [Barangay].[dbo].[HEEADSSSAssessments]
                                    (AppointmentId, PatientId, FullName, HomeFamilyProblems, 
                                     SuicidalThoughts, IsConsulted, RecordedBy, RecordedAt)
                                    VALUES
                                    (@AppointmentId, @PatientId, @FullName, @HomeFamilyProblems,
                                     @SuicidalThoughts, @IsConsulted, @RecordedBy, @RecordedAt)", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@PatientId", patientId);
                                command.Parameters.AddWithValue("@FullName", patientName);
                                command.Parameters.AddWithValue("@HomeFamilyProblems", updateData.HeadsssAssessment.HomeFamilyProblems ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@SuicidalThoughts", updateData.HeadsssAssessment.SuicidalThoughts ?? (object)DBNull.Value);
                                command.Parameters.AddWithValue("@IsConsulted", updateData.IsConsulted);
                                command.Parameters.AddWithValue("@RecordedBy", doctorName);
                                command.Parameters.AddWithValue("@RecordedAt", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        
                        // Update NCD assessment if provided
                        if (updateData.NcdAssessment != null)
                        {
                            // Check if record exists
                            var checkCmd = new SqlCommand(
                                "SELECT COUNT(*) FROM [Barangay].[dbo].[NCDRiskAssessments] WHERE AppointmentId = @AppointmentId", 
                                connection, transaction);
                            checkCmd.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            
                            var result = await checkCmd.ExecuteScalarAsync();
                            var recordExists = result != null && Convert.ToInt32(result) > 0;
                            
                            if (recordExists)
                            {
                                // Update existing record
                                using var command = new SqlCommand(@"
                                    UPDATE [Barangay].[dbo].[NCDRiskAssessments]
                                    SET HasDiabetes = @HasDiabetes,
                                        ChestPain = @ChestPain,
                                        LastEditedBy = @LastEditedBy,
                                        LastEditedDate = @LastEditedDate
                                    WHERE AppointmentId = @AppointmentId", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@HasDiabetes", updateData.NcdAssessment.HasDiabetes);
                                command.Parameters.AddWithValue("@ChestPain", updateData.NcdAssessment.ChestPain);
                                command.Parameters.AddWithValue("@LastEditedBy", doctorName);
                                command.Parameters.AddWithValue("@LastEditedDate", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                // Get patient information from appointment
                                var patientInfoCmd = new SqlCommand(
                                    "SELECT PatientId FROM [Barangay].[dbo].[Appointments] WHERE AppointmentId = @AppointmentId", 
                                    connection, transaction);
                                patientInfoCmd.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                
                                string patientId = string.Empty;
                                
                                using (var reader = await patientInfoCmd.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        patientId = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                                    }
                                }
                                
                                // Insert new record
                                using var command = new SqlCommand(@"
                                    INSERT INTO [Barangay].[dbo].[NCDRiskAssessments]
                                    (AppointmentId, PatientId, HasDiabetes, ChestPain, RecordedBy, RecordedAt)
                                    VALUES
                                    (@AppointmentId, @PatientId, @HasDiabetes, @ChestPain, @RecordedBy, @RecordedAt)", connection, transaction);
                                
                                command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                                command.Parameters.AddWithValue("@PatientId", patientId);
                                command.Parameters.AddWithValue("@HasDiabetes", updateData.NcdAssessment.HasDiabetes);
                                command.Parameters.AddWithValue("@ChestPain", updateData.NcdAssessment.ChestPain);
                                command.Parameters.AddWithValue("@RecordedBy", doctorName);
                                command.Parameters.AddWithValue("@RecordedAt", DateTime.Now);
                                
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        
                        // Update the appointment status to indicate it has been consulted
                        if (updateData.IsConsulted)
                        {
                            using var command = new SqlCommand(@"
                                UPDATE [Barangay].[dbo].[Appointments]
                                SET Status = 'Completed',
                                    UpdatedAt = @UpdatedAt
                                WHERE AppointmentId = @AppointmentId", connection, transaction);
                            
                            command.Parameters.AddWithValue("@AppointmentId", updateData.AppointmentId);
                            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                            
                            await command.ExecuteNonQueryAsync();
                        }
                        
                        // Commit the transaction
                        transaction.Commit();
                        
                        return Ok(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction on error
                        transaction.Rollback();
                        return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"An error occurred: {ex.Message}" });
            }
        }
    }

    public class UpdateResultsDto
    {
        public int AppointmentId { get; set; }
        public VitalSignsUpdateDto? VitalSigns { get; set; }
        public HeadsssAssessmentUpdateDto? HeadsssAssessment { get; set; }
        public NcdAssessmentUpdateDto? NcdAssessment { get; set; }
        public string? LastEditedBy { get; set; }
        public bool IsConsulted { get; set; }
    }

    public class VitalSignsUpdateDto
    {
        public float? Temperature { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? OxygenSaturation { get; set; }
        public float? Weight { get; set; }
        public int? Height { get; set; }
        public string? BloodPressure { get; set; }
        public string? Notes { get; set; }
    }

    public class HeadsssAssessmentUpdateDto
    {
        public string? HomeFamilyProblems { get; set; }
        public string? SuicidalThoughts { get; set; }
        [JsonIgnore]
        public bool IsConsulted { get; set; }
    }

    public class NcdAssessmentUpdateDto
    {
        public bool HasDiabetes { get; set; }
        public bool ChestPain { get; set; }
    }
} 