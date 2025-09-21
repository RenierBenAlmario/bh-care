using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Barangay.Services;

namespace Barangay.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class PatientResultsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IDataEncryptionService _encryptionService;

        public PatientResultsModel(IConfiguration configuration, IDataEncryptionService encryptionService)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _encryptionService = encryptionService;
        }

        public string CurrentDoctorName { get; set; }
        public List<AppointmentViewModel> TodayAppointments { get; set; } = new List<AppointmentViewModel>();
        public string ErrorMessage { get; set; }
        public DateTime CurrentDate { get; set; } = DateTime.Now;
        public int? SelectedAppointmentId { get; set; }

        public async Task<IActionResult> OnGetAsync(int? appointmentId)
        {
            try
            {
                // Get current doctor name from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    ErrorMessage = "User not authenticated properly";
                    return Page();
                }

                var userId = userIdClaim.Value;
                CurrentDoctorName = User.FindFirstValue(ClaimTypes.Name) ?? "Doctor";
                SelectedAppointmentId = appointmentId;

                // Get today's appointments for the current doctor
                await GetTodayAppointments(userId);
                
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                return Page();
            }
        }

        private async Task GetTodayAppointments(string doctorId)
        {
            TodayAppointments = new List<AppointmentViewModel>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Set specific target date for demo purposes: July 15, 2025
                var targetDate = new DateTime(2025, 7, 15);
                Console.WriteLine($"Searching for appointments on {targetDate.ToString("yyyy-MM-dd")} for doctor {doctorId}/{CurrentDoctorName}");

                // Query to get appointments for July 15, 2025 for the current doctor
                using var command = new SqlCommand(@"
                    SELECT a.Id, a.PatientName, a.AppointmentTime, a.Type, a.Status,
                           p.FirstName, p.LastName, p.BirthDate, p.Gender
                    FROM [Barangay].[dbo].[Appointments] a
                    LEFT JOIN [Barangay].[dbo].[AspNetUsers] p ON a.PatientId = p.Id
                    WHERE CONVERT(date, a.AppointmentDate) = @TargetDate
                    AND a.DoctorId = @DoctorId
                    ORDER BY a.AppointmentTime", connection);
                
                command.Parameters.AddWithValue("@TargetDate", targetDate);
                command.Parameters.AddWithValue("@DoctorId", doctorId);


                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var appointment = new AppointmentViewModel
                    {
                        AppointmentId = reader.GetInt32(0),
                        Patient = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        Time = reader.IsDBNull(2) ? TimeSpan.Zero : reader.GetTimeSpan(2),
                        Type = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Status = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                        PatientFullName = $"{reader.GetString(5)} {reader.GetString(6)}",
                        PatientAge = DateTime.Today.Year - reader.GetDateTime(7).Year,
                        PatientGender = reader.IsDBNull(8) ? string.Empty : reader.GetString(8)
                    };

                    // Fetch vital signs
                    await FetchVitalSigns(connection, appointment);

                    // Fetch HEADSSS assessment
                    await FetchHeadsssAssessment(connection, appointment);

                    // Fetch NCD assessment
                    await FetchNcdAssessment(connection, appointment);

                    TodayAppointments.Add(appointment);
                }
            }
        }

        private async Task FetchVitalSigns(SqlConnection connection, AppointmentViewModel appointment)
        {
            using var command = new SqlCommand(@"
                SELECT Temperature, RespiratoryRate, OxygenSaturation, Weight, Height, BloodPressure, Notes, RecordedBy
                FROM [Barangay].[dbo].[VitalSigns]
                WHERE AppointmentId = @AppointmentId", connection);

            command.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                appointment.VitalSigns = new VitalSignsViewModel
                {
                    Temperature = reader.IsDBNull(0) ? null : (float?)reader.GetDouble(0),
                    RespiratoryRate = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1),
                    OxygenSaturation = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2),
                    Weight = reader.IsDBNull(3) ? null : (float?)reader.GetDouble(3),
                    Height = reader.IsDBNull(4) ? null : (int?)reader.GetInt32(4),
                    BloodPressure = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Notes = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    RecordedBy = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                };
            }
            else
            {
                appointment.VitalSigns = new VitalSignsViewModel();
            }
        }

        private async Task FetchHeadsssAssessment(SqlConnection connection, AppointmentViewModel appointment)
        {
            using var command = new SqlCommand(@"
                SELECT HomeFamilyProblems, SuicidalThoughts, IsConsulted, RecordedBy
                FROM [Barangay].[dbo].[HEEADSSSAssessments]
                WHERE AppointmentId = @AppointmentId", connection);

            command.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                // Get the raw encrypted data
                var homeFamilyProblems = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                var suicidalThoughts = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                var recordedBy = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

                // Decrypt the encrypted fields
                appointment.HeadsssAssessment = new HeadsssAssessmentViewModel
                {
                    HomeFamilyProblems = _encryptionService.IsEncrypted(homeFamilyProblems) 
                        ? _encryptionService.DecryptForUser(homeFamilyProblems, User) 
                        : homeFamilyProblems,
                    SuicidalThoughts = _encryptionService.IsEncrypted(suicidalThoughts) 
                        ? _encryptionService.DecryptForUser(suicidalThoughts, User) 
                        : suicidalThoughts,
                    IsConsulted = reader.IsDBNull(2) ? false : reader.GetBoolean(2),
                    RecordedBy = recordedBy // This field is not encrypted
                };
            }
            else
            {
                appointment.HeadsssAssessment = new HeadsssAssessmentViewModel();
            }
        }

        private async Task FetchNcdAssessment(SqlConnection connection, AppointmentViewModel appointment)
        {
            using var command = new SqlCommand(@"
                SELECT HasDiabetes, ChestPain, RecordedBy
                FROM [Barangay].[dbo].[NCDRiskAssessments]
                WHERE AppointmentId = @AppointmentId", connection);

            command.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                appointment.NcdAssessment = new NcdAssessmentViewModel
                {
                    HasDiabetes = reader.IsDBNull(0) ? false : reader.GetBoolean(0),
                    ChestPain = reader.IsDBNull(1) ? false : reader.GetBoolean(1),
                    RecordedBy = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                };
            }
            else
            {
                appointment.NcdAssessment = new NcdAssessmentViewModel();
            }
        }

        public class AppointmentViewModel
        {
            public int AppointmentId { get; set; }
            public string Patient { get; set; }
            public TimeSpan Time { get; set; }
            public string Type { get; set; }
            public string Status { get; set; }
            public string PatientFullName { get; set; }
            public int PatientAge { get; set; }
            public string PatientGender { get; set; }
            public VitalSignsViewModel VitalSigns { get; set; }
            public HeadsssAssessmentViewModel HeadsssAssessment { get; set; }
            public NcdAssessmentViewModel NcdAssessment { get; set; }

            public string FormattedTime => Time.ToString(@"hh\:mm");
        }

        public class VitalSignsViewModel
        {
            public float? Temperature { get; set; }
            public int? RespiratoryRate { get; set; }
            public int? OxygenSaturation { get; set; }
            public float? Weight { get; set; }
            public int? Height { get; set; }
            public string BloodPressure { get; set; }
            public string Notes { get; set; }
            public string RecordedBy { get; set; }

            public bool IsTemperatureAbnormal => Temperature.HasValue && Temperature > 38.0;
            public bool IsRespiratoryRateAbnormal => RespiratoryRate.HasValue && (RespiratoryRate < 12 || RespiratoryRate > 20);
            public bool IsOxygenSaturationAbnormal => OxygenSaturation.HasValue && OxygenSaturation < 95;
        }

        public class HeadsssAssessmentViewModel
        {
            public string HomeFamilyProblems { get; set; }
            public string SuicidalThoughts { get; set; }
            public bool IsConsulted { get; set; }
            public string RecordedBy { get; set; }

            public bool HasSuicidalThoughts => !string.IsNullOrEmpty(SuicidalThoughts) && SuicidalThoughts.ToLower() == "yes";
        }

        public class NcdAssessmentViewModel
        {
            public bool HasDiabetes { get; set; }
            public bool ChestPain { get; set; }
            public string RecordedBy { get; set; }
        }
    }
} 