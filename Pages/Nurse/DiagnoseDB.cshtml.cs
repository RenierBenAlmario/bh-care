using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using Barangay.Models;
using Barangay.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Barangay.Helpers;

namespace Barangay.Pages.Nurse
{
    [Authorize(Roles = "Nurse,Head Nurse")]
    public class DiagnoseDBModel : PageModel
    {
        private readonly ILogger<DiagnoseDBModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly string _connectionString;
        
        public List<Barangay.Models.Appointment> AppointmentResults { get; set; } = new List<Barangay.Models.Appointment>();
        public string DiagnosticOutput { get; set; } = "";
        public string ResultMessage { get; set; } = "";
        public List<Barangay.Models.Appointment> Appointments { get; set; } = new();
        public List<MedicalRecord> MedicalRecords { get; set; } = new();

        public DiagnoseDBModel(
            ILogger<DiagnoseDBModel> logger, 
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? 
                throw new ArgumentNullException(nameof(configuration), "Connection string 'DefaultConnection' not found");
        }

        public async Task OnGetAsync()
        {
            var diagnosticSB = new StringBuilder();
            
            try
            {
                // Get all appointments using EF Core
                AppointmentResults = await _context.Appointments
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
                
                diagnosticSB.AppendLine($"Found {AppointmentResults.Count} appointments via EF Core");
                
                // Check if there are any appointments for today
                var todayAppointments = AppointmentResults
                    .Where(a => a.AppointmentDate.Date == DateTime.Today.Date)
                    .ToList();
                
                diagnosticSB.AppendLine($"Found {todayAppointments.Count} appointments for today ({DateTime.Today:yyyy-MM-dd})");
                
                // Check for appointments with missing values
                var appointmentsWithNoAgeValue = AppointmentResults
                    .Where(a => a.AgeValue == 0)
                    .Count();
                
                diagnosticSB.AppendLine($"Found {appointmentsWithNoAgeValue} appointments with AgeValue = 0");
                
                // Use raw SQL to diagnose further
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    diagnosticSB.AppendLine("Connected to database directly using ADO.NET");
                    
                    var sql = @"
                        SELECT TOP 20 Id, PatientName, AppointmentDate, Status, AgeValue, CreatedAt
                        FROM [Barangay].[dbo].[Appointments]
                        ORDER BY Id DESC";
                    
                    using var command = new SqlCommand(sql, connection);
                    using var reader = await command.ExecuteReaderAsync();
                    
                    int rowCount = 0;
                    while (await reader.ReadAsync())
                    {
                        rowCount++;
                    }
                    
                    diagnosticSB.AppendLine($"Raw SQL query found {rowCount} recent appointments");
                    
                    // Count today's appointments using SQL
                    sql = @"
                        SELECT COUNT(*)
                        FROM [Barangay].[dbo].[Appointments]
                        WHERE CAST(AppointmentDate AS DATE) = CAST(@Today AS DATE)";
                    
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Today", DateTime.Today);
                    
                    // Reuse connection but create new command
                    var result = await command.ExecuteScalarAsync();
                    var todayCount = result != null ? (int)result : 0;
                    diagnosticSB.AppendLine($"SQL query found {todayCount} appointments for today");

                    // Check if MedicalRecords table has the required columns
                    sql = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'MedicalRecords' 
                        AND COLUMN_NAME IN ('ApplicationUserId', 'Medications', 'RecordDate')";
                    
                    command.CommandText = sql;
                    command.Parameters.Clear();
                    
                    result = await command.ExecuteScalarAsync();
                    var columnCount = result != null ? (int)result : 0;
                    diagnosticSB.AppendLine($"MedicalRecords table has {columnCount}/3 required columns");
                }
            }
            catch (Exception ex)
            {
                diagnosticSB.AppendLine($"ERROR: {ex.Message}");
                diagnosticSB.AppendLine($"Stack Trace: {ex.StackTrace}");
                _logger.LogError(ex, "Error in database diagnostics");
            }
            
            DiagnosticOutput = diagnosticSB.ToString();

            try
            {
                // Get appointments
                Appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenBy(a => a.AppointmentTime)
                    .ToListAsync();

                // Get medical records with safe column access
                // Use a raw SQL query that only selects columns we know exist
                var medicalRecordsSql = @"
                    SELECT m.Id, m.PatientId, m.DoctorId, m.Diagnosis, m.Treatment, 
                           m.Notes, m.CreatedAt, m.UpdatedAt, 
                           COALESCE(m.RecordDate, m.CreatedAt) AS Date,
                           COALESCE(m.Type, '') AS Type,
                           COALESCE(m.ChiefComplaint, '') AS ChiefComplaint,
                           COALESCE(m.Status, '') AS Status,
                           COALESCE(m.Duration, '') AS Duration,
                           COALESCE(m.Medications, '') AS Medications,
                           COALESCE(m.Prescription, '') AS Prescription,
                           COALESCE(m.Instructions, '') AS Instructions,
                           COALESCE(m.ApplicationUserId, '') AS ApplicationUserId
                    FROM MedicalRecords m
                    ORDER BY COALESCE(m.RecordDate, m.CreatedAt) DESC";

                MedicalRecords = await _context.MedicalRecords
                    .FromSqlRaw(medicalRecordsSql)
                    .Include(m => m.Patient)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading medical records or appointments");
                diagnosticSB.AppendLine($"ERROR loading records: {ex.Message}");
                DiagnosticOutput = diagnosticSB.ToString();
            }
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Get form values
                var patientName = Request.Form["patientName"].ToString();
                var appointmentDateString = Request.Form["appointmentDate"].ToString();
                var appointmentTimeString = Request.Form["appointmentTime"].ToString();
                
                // Parse and validate date and time
                if (!DateTime.TryParse(appointmentDateString, out DateTime appointmentDate))
                {
                    ResultMessage = $"Error: Invalid date format: {appointmentDateString}";
                    await OnGetAsync();
                    return Page();
                }
                
                if (appointmentDate.Date != DateTime.Today.Date)
                {
                    ResultMessage = $"Error: Date must be today's date.";
                    await OnGetAsync();
                    return Page();
                }
                
                if (!TimeSpan.TryParse(appointmentTimeString, out TimeSpan appointmentTime))
                {
                    ResultMessage = $"Error: Invalid time format: {appointmentTimeString}";
                    await OnGetAsync();
                    return Page();
                }
                
                // Create the test appointment
                var testAppointment = new Barangay.Models.Appointment
                {
                    PatientId = "test-patient-id",
                    PatientName = patientName,
                    DoctorId = "test-doctor-id",
                    AppointmentDate = appointmentDate,
                    AppointmentTime = appointmentTime,
                    ReasonForVisit = "Test appointment for diagnostics",
                    Description = "Test appointment for diagnostics",
                    Status = AppointmentStatus.Pending,
                    AgeValue = 25,
                    Type = "Regular",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    // Add ApplicationUserId for the doctor
                    ApplicationUserId = "test-doctor-id"
                };
                
                // Save to database
                _context.Appointments.Add(testAppointment);
                await _context.SaveChangesAsync();
                
                ResultMessage = $"Test appointment created successfully! ID: {testAppointment.Id}, Date: {appointmentDate:yyyy-MM-dd}, Time: {appointmentTime}";
                
                // Reload page data
                await OnGetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                ResultMessage = $"Error creating test appointment: {ex.Message}";
                _logger.LogError(ex, "Error creating test appointment");
                await OnGetAsync();
                return Page();
            }
        }
    }
} 