using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Barangay.Services;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.Globalization;
using Barangay.Extensions;
using Barangay.Helpers;

// Ensure this is the first line (no statements before namespace)
namespace Barangay.Controllers
{
    [Route("api/doctor")]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class DoctorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DoctorApiController> _logger;
        private readonly IEncryptionService _encryptionService;

        public DoctorApiController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<DoctorApiController> logger,
            IEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _encryptionService = encryptionService;
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { error = "User not found" });

            var appointmentsData = await _context.Appointments
                .Where(a => a.DoctorId == userId)
                .Include(a => a.Patient)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var appointments = appointmentsData.Select(a => new
            {
                id = a.Id,
                patientName = a.Patient?.FullName,
                patientId = a.PatientId,
                date = a.AppointmentDate,
                time = a.AppointmentTime,
                status = a.Status,
                description = a.Description,
                createdAt = a.CreatedAt.ToDateTimeString()
            }).ToList();

            return Ok(new { appointments });
        }

        [HttpGet("appointment/{id}")]
        public async Task<IActionResult> GetAppointmentDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { error = "User not found" });

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == userId);

            if (appointment == null) return NotFound(new { error = "Appointment not found" });

            return Ok(new
            {
                id = appointment.Id,
                patientName = appointment.Patient?.FullName ?? "Unknown",
                date = appointment.AppointmentDate,
                time = appointment.AppointmentTime,
                status = appointment.Status,
                description = appointment.Description ?? string.Empty,
                attachments = appointment.AttachmentPath != null ? new List<string> { appointment.AttachmentPath } : new List<string>(),
                updatedAt = appointment.UpdatedAt.ToDateTimeString()
            });
        }

        private string FormatTime(string timeString)
        {
            if (TimeSpan.TryParse(timeString, out TimeSpan timeSpan))
            {
                DateTime dateTime = DateTime.Today.Add(timeSpan);
                return dateTime.ToString("hh:mm tt", CultureInfo.InvariantCulture);
            }
            return timeString; // return original string if parsing fails
        }

        [HttpPost("appointment/{id}/update-status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized(new { error = "User not found" });

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && a.DoctorId == userId);

            if (appointment == null) return NotFound(new { error = "Appointment not found" });
            if (!Enum.IsDefined(typeof(AppointmentStatus), request.Status))
                return BadRequest(new { error = "Invalid status" });

            appointment.Status = request.Status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpPost("notify-patient")]
        public async Task<IActionResult> NotifyPatient([FromBody] NotificationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var patient = await _context.Patients.FindAsync(request.PatientId);
            if (patient == null) return NotFound(new { error = "Patient not found" });

            var notification = new Notification
            {
                UserId = patient.UserId,
                Title = "Message from Doctor",
                Message = request.Message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
                return Unauthorized();

            var patients = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.VitalSigns)
                .Select(p => new
                {
                    p.UserId,
                    p.Name,
                    p.Gender,
                    Age = 0, // Will be calculated after query
                    p.Status,
                    LastVisit = _context.Appointments
                        .Where(a => a.PatientId == p.UserId && a.Status != AppointmentStatus.Cancelled)
                        .OrderByDescending(a => a.AppointmentDate)
                        .Select(a => a.AppointmentDate)
                        .FirstOrDefault(),
                    p.ContactNumber,
                    p.Address,
                    p.MedicalHistory,
                    p.Allergies,
                    p.CurrentMedications,
                    UserBirthDate = p.User.BirthDate // Include birthdate for age calculation
                })
                .ToListAsync();

            // Calculate ages after query to avoid LINQ expression tree issues
            var patientsWithAge = patients.Select(p => new
            {
                p.UserId,
                p.Name,
                p.Gender,
                Age = CalculateAge(DateTime.TryParse(p.UserBirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue),
                p.Status,
                p.LastVisit,
                p.ContactNumber,
                p.Address,
                p.MedicalHistory,
                p.Allergies,
                p.CurrentMedications
            }).ToList();

            return Ok(patientsWithAge);
        }

        [HttpGet("patients/{patientId}")]
        public async Task<IActionResult> GetPatientDetails(string patientId)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
                return Unauthorized();

            var patient = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.VitalSigns)
                .Where(p => p.UserId == patientId)
                .Select(p => new
                {
                    p.UserId,
                    p.Name,
                    p.Gender,
                    Age = 0, // Will be calculated after query
                    p.Status,
                    LastVisit = _context.Appointments
                        .Where(a => a.PatientId == p.UserId && a.Status != AppointmentStatus.Cancelled)
                        .OrderByDescending(a => a.AppointmentDate)
                        .Select(a => a.AppointmentDate)
                        .FirstOrDefault(),
                    p.ContactNumber,
                    p.Address,
                    p.MedicalHistory,
                    p.Allergies,
                    p.CurrentMedications,
                    VitalSigns = p.VitalSigns.OrderByDescending(v => v.RecordedAt).Select(v => new
                    {
                        v.RecordedAt,
                        v.Temperature,
                        v.BloodPressure,
                        v.HeartRate,
                        v.SpO2,
                        v.RespiratoryRate,
                        v.Notes
                    }).FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        private int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
                return 0;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age))
                age--;
            return age;
        }

        [HttpPost("patient/{userId}/update-medical-info")]
        public async Task<IActionResult> UpdatePatientMedicalInfo(string userId, [FromBody] UpdateMedicalInfoRequest request)
        {
            try
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    return NotFound("Patient not found");
                }

                // Update medical information
                patient.MedicalHistory = request.MedicalHistory;
                patient.Allergies = request.Allergies;
                patient.CurrentMedications = request.CurrentMedications;
                patient.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Medical information updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient medical information");
                return StatusCode(500, "Error updating patient medical information");
            }
        }

        [HttpPost("prescriptions")]
        public async Task<IActionResult> SavePrescription([FromBody] Prescription prescription)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
                return Unauthorized();

            prescription.DoctorId = doctorId;
            prescription.PrescriptionDate = DateTime.Now;

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Prescription saved successfully" });
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GenerateReport([FromQuery] string reportType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
                return Unauthorized();

            var data = await GenerateReportData(reportType, startDate, endDate, doctorId);
            var pdfBytes = GeneratePdfReport(data, reportType, startDate, endDate);

            return File(pdfBytes, "application/pdf", $"{reportType}_report_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportData([FromQuery] string reportType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(doctorId))
                return Unauthorized();

            var data = await GenerateReportData(reportType, startDate, endDate, doctorId);
            var excelBytes = GenerateExcelReport(data, reportType, startDate, endDate);

            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"{reportType}_export_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.xlsx");
        }

        [HttpGet("generateReport")]
        public Task<IActionResult> GenerateReportAlias([FromQuery] string reportType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
            => GenerateReport(reportType, startDate, endDate);

        [HttpGet("exportData")]
        public Task<IActionResult> ExportDataAlias([FromQuery] string reportType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
            => ExportData(reportType, startDate, endDate);

        private async Task<ReportData> GenerateReportData(string reportType, DateTime startDate, DateTime endDate, string doctorId)
        {
            var data = new ReportData();
            
            switch (reportType.ToLower())
            {
                case "consultations":
                    data.Items = await _context.Appointments
                        .Where(a => a.DoctorId == doctorId && 
                               DateTimeHelper.IsDateGreaterThanOrEqual(a.AppointmentDate, startDate) && 
                               DateTimeHelper.IsDateLessThanOrEqual(a.AppointmentDate, endDate))
                        .OrderByDescending(a => a.AppointmentDate)
                        .ToListAsync();
                    break;

                case "prescriptions":
                    data.Items = await _context.Prescriptions
                        .Where(p => p.DoctorId == doctorId && 
                               p.PrescriptionDate >= startDate && 
                               p.PrescriptionDate <= endDate)
                        .Include(p => p.Patient)
                        .Include(p => p.PrescriptionMedicines)
                            .ThenInclude(pm => pm.Medication)
                        .OrderByDescending(p => p.PrescriptionDate)
                        .ToListAsync();
                    break;

                case "patients":
                    data.Items = await _context.Patients
                        .Where(p => p.CreatedAt >= startDate && 
                               p.CreatedAt <= endDate)
                        .OrderByDescending(p => p.CreatedAt)
                        .ToListAsync();
                    break;

                default:
                    throw new ArgumentException("Invalid report type");
            }

            return data;
        }

        private byte[] GeneratePdfReport(ReportData data, string reportType, DateTime startDate, DateTime endDate)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            // Add title
            var title = new Paragraph($"Report: {reportType}");
            title.SetTextAlignment(TextAlignment.CENTER);
            title.SetFontSize(20);
            title.SetProperty(Property.FONT_WEIGHT, "bold");
            document.Add(title);

            // Add date range
            var dateRange = new Paragraph($"Date Range: {DateTimeHelper.ToDateString(startDate)} - {DateTimeHelper.ToDateString(endDate)}");
            dateRange.SetTextAlignment(TextAlignment.CENTER);
            dateRange.SetFontSize(12);
            document.Add(dateRange);

            document.Add(new Paragraph("\n"));

            // Create table
            var columnCount = GetColumnCount(reportType);
            var table = new Table(columnCount);
            table.SetWidth(UnitValue.CreatePercentValue(100));

            // Add headers
            var headers = GetReportHeaders(reportType);
            foreach (var header in headers)
            {
                var headerText = new Paragraph(header);
                headerText.SetProperty(Property.FONT_WEIGHT, "bold");
                var cell = new Cell()
                    .Add(headerText)
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY);
                table.AddCell(cell);
            }

            // Add data rows
            foreach (var item in data.Items)
            {
                var values = GetItemValues(item, reportType);
                foreach (var value in values)
                {
                    table.AddCell(new Cell().Add(new Paragraph(value ?? string.Empty)));
                }
            }

            document.Add(table);
            document.Close();
            return memoryStream.ToArray();
        }

        private byte[] GenerateExcelReport(ReportData data, string reportType, DateTime startDate, DateTime endDate)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(reportType);

            // Add title
            worksheet.Cell("A1").Value = $"{reportType.ToUpper()} REPORT";
            worksheet.Cell("A2").Value = $"Period: {DateTimeHelper.ToDateString(startDate)} - {DateTimeHelper.ToDateString(endDate)}";

            // Add headers
            var headers = GetReportHeaders(reportType);
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(3, i + 1).Value = headers[i];
            }

            // Add data
            var row = 4;
            foreach (var item in data.Items)
            {
                var values = GetItemValues(item, reportType);
                for (int i = 0; i < values.Length; i++)
                {
                    worksheet.Cell(row, i + 1).Value = values[i];
                }
                row++;
            }

            // Style the worksheet
            var titleRange = worksheet.Range("A1:E1");
            titleRange.Merge();
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.FontSize = 14;
            titleRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var dateRange = worksheet.Range("A2:E2");
            dateRange.Merge();
            dateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var headerRange = worksheet.Range(3, 1, 3, headers.Length);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;

            worksheet.Columns().AdjustToContents();

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        private int GetColumnCount(string reportType)
        {
            return GetReportHeaders(reportType).Length;
        }

        private string[] GetReportHeaders(string reportType)
        {
            return reportType.ToLower() switch
            {
                "consultations" => new[] { "Date", "Patient", "Status", "Notes" },
                "prescriptions" => new[] { "Date", "Patient", "Medication", "Dosage", "Duration" },
                "patients" => new[] { "Name", "Age", "Gender", "Contact", "Last Visit" },
                _ => throw new ArgumentException("Invalid report type")
            };
        }

        private string[] GetItemValues(object item, string reportType)
        {
            switch (reportType.ToLower())
            {
                case "consultations":
                    var appointment = (Appointment)item;
                    return new string[] 
                    { 
                        DateTimeHelper.ToDateString(appointment.AppointmentDate),
                        appointment.Patient?.FullName ?? "Unknown",
                        appointment.Status.ToString(),
                        appointment.Description ?? ""
                    };

                case "prescriptions":
                    var prescription = (Prescription)item;
                    var firstMedicine = prescription.PrescriptionMedicines.FirstOrDefault();
                    return new string[] 
                    { 
                        DateTimeHelper.ToDateString(prescription.PrescriptionDate),
                        prescription.Patient?.FullName ?? "Unknown",
                        string.Join(", ", prescription.PrescriptionMedicines.Select(m => m.Medication?.Name ?? "Unknown")),
                        firstMedicine?.Dosage.ToString() ?? string.Empty,
                        prescription.Duration.ToString()
                    };

                case "patients":
                    var patient = (Patient)item;
                    var lastVisit = _context.Appointments
                        .Where(a => a.PatientId == patient.UserId && a.Status != AppointmentStatus.Cancelled)
                        .OrderByDescending(a => a.AppointmentDate)
                        .FirstOrDefault()?.AppointmentDate;

                    return new string[] 
                    { 
                        patient.FullName,
                        CalculateAge(patient.BirthDate).ToString(),
                        patient.Gender.ToString(),
                        patient.ContactNumber ?? "",
                        DateTimeHelper.ToDateString(lastVisit ?? DateTime.MinValue)
                    };

                default:
                    throw new ArgumentException("Invalid report type");
            }
        }

        private class ReportData
        {
            public IEnumerable<object> Items { get; set; } = new List<object>();
        }

        public class UpdateStatusRequest
        {
            public required AppointmentStatus Status { get; set; }
        }

        public class NotificationRequest
        {
            public int PatientId { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        public class UpdateMedicalInfoRequest
        {
            public string? MedicalHistory { get; set; }
            public string? Allergies { get; set; }
            public string? CurrentMedications { get; set; }
        }
    }
}

