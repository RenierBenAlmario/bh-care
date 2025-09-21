using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Models.Appointments;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Barangay.Services;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Barangay.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;
        private readonly IAppointmentService _appointmentService;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger,
            IAppointmentService appointmentService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                return Json(new { 
                    firstName = user.FirstName,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user info");
                return StatusCode(500, "Error fetching user information");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestHealthReport()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                var report = await _context.HealthReports
                    .Where(hr => hr.UserId == user.Id)
                    .OrderByDescending(hr => hr.CheckupDate)
                    .Select(hr => new {
                        hr.Id,
                        hr.CheckupDate,
                        hr.BloodPressure,
                        hr.HeartRate,
                        hr.BloodSugar,
                        hr.Weight,
                        hr.Temperature,
                        hr.PhysicalActivity,
                        hr.Notes,
                        DoctorName = hr.Doctor.FirstName + " " + hr.Doctor.LastName
                    })
                    .FirstOrDefaultAsync();

                return Json(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching health report");
                return StatusCode(500, "Error fetching health report");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                var today = DateTime.Today;
                var isDoctor = await _userManager.IsInRoleAsync(user, "Doctor");

                var query = _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .AsQueryable();

                if (isDoctor)
                    query = query.Where(a => a.DoctorId == user.Id);
                else
                    query = query.Where(a => a.PatientId == user.Id);

                var appointments = await query.ToListAsync();

                var result = new
                {
                    today = appointments.Where(a => a.AppointmentDate.Date == today)
                        .OrderBy(a => a.AppointmentTime),
                    upcoming = appointments.Where(a => a.AppointmentDate.Date > today)
                        .OrderBy(a => a.AppointmentDate),
                    past = appointments.Where(a => a.AppointmentDate.Date < today)
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(5)
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching appointments");
                return StatusCode(500, "Error fetching appointments");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadHealthReport(int reportId)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                var report = await _context.HealthReports
                    .Include(hr => hr.Doctor)
                    .FirstOrDefaultAsync(hr => hr.Id == reportId && hr.UserId == user.Id);

                if (report == null) return NotFound("Report not found");

                var content = GenerateReportContent(report);
                var fileName = $"health_report_{report.CheckupDate:yyyy_MM_dd}.pdf";

                return File(content, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading health report");
                return StatusCode(500, "Error downloading health report");
            }
        }

        private byte[] GenerateReportContent(HealthReport report)
        {
            // Implement PDF generation logic here
            // For now, returning dummy content
            return System.Text.Encoding.UTF8.GetBytes("Sample Report Content");
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleCheckup([FromBody] AppointmentRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound("User not found");

                var appointment = new Appointment
                {
                    PatientId = user.Id,
                    DoctorId = request.DoctorId,
                    AppointmentDate = request.Date,
                    AppointmentTime = request.Time,
                    Type = "Checkup",
                    Status = AppointmentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Checkup scheduled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling checkup");
                return StatusCode(500, "Error scheduling checkup");
            }
        }

        // GET: User/BookAppointment
        public async Task<IActionResult> BookAppointment()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found when accessing BookAppointment");
                return NotFound("User not found");
            }

            // Get available doctors
            var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
            
            // Calculate age from birth date
            var age = 0;
            var birthDate = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue;
            if (birthDate != DateTime.MinValue)
            {
                age = DateTime.Now.Year - birthDate.Year;
                if (birthDate.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }
            }

            ViewBag.UserDetails = new
            {
                FullName = $"{user.FirstName} {user.LastName}",
                Birthday = birthDate != DateTime.MinValue ? birthDate : DateTime.Today,
                Age = age,
                PhoneNumber = user.PhoneNumber
            };

            ViewBag.Doctors = doctors;
            return View();
        }

        // POST: User/BookAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment([FromForm] AppointmentCreateModel model)
        {
            try
            {
                _logger.LogInformation("Starting BookAppointment POST action at {Time}", DateTime.UtcNow);
                _logger.LogInformation("Connection string: {ConnectionString}", 
                    _context.Database.GetConnectionString()?.Replace("Password=", "Password=***"));

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state: {@ModelState}", 
                        ModelState.ToDictionary(m => m.Key, m => m.Value?.Errors.Select(e => e.ErrorMessage)));
                    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                    ViewBag.Doctors = doctors;
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("User not found. User.Identity.Name: {UserName}, IsAuthenticated: {IsAuthenticated}", 
                        User.Identity?.Name, User.Identity?.IsAuthenticated);
                    return NotFound("User not found");
                }

                _logger.LogInformation("User details: {@UserDetails}", new 
                    { 
                        user.Id, 
                        user.UserName, 
                        user.Email,
                        user.PhoneNumber,
                        IsInRole = await _userManager.IsInRoleAsync(user, "Patient")
                    });

                // Parse date and time
                if (!DateTime.TryParse(model.AppointmentDate, out DateTime appointmentDate))
                {
                    _logger.LogWarning("Invalid appointment date format: {Date}", model.AppointmentDate);
                    ModelState.AddModelError("AppointmentDate", "Invalid date format");
                    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                    ViewBag.Doctors = doctors;
                    return View(model);
                }

                if (!TimeSpan.TryParse(model.AppointmentTime, out TimeSpan appointmentTime))
                {
                    _logger.LogWarning("Invalid appointment time format: {Time}", model.AppointmentTime);
                    ModelState.AddModelError("AppointmentTime", "Invalid time format");
                    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                    ViewBag.Doctors = doctors;
                    return View(model);
                }

                // Verify doctor exists
                var doctor = await _userManager.FindByIdAsync(model.StaffId);
                if (doctor == null)
                {
                    _logger.LogError("Doctor not found. StaffId: {StaffId}", model.StaffId);
                    ModelState.AddModelError("StaffId", "Selected doctor not found");
                    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                    ViewBag.Doctors = doctors;
                    return View(model);
                }

                // Enable detailed SQL logging
                _context.Database.SetCommandTimeout(30); // 30 seconds timeout
                var connection = _context.Database.GetDbConnection();
                _logger.LogInformation("Database connection state before transaction: {State}", connection.State);

                using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
                try
                {
                    // Log the current transaction details
                    _logger.LogInformation("Transaction started with ReadCommitted isolation level");

                    var appointment = new Appointment
                    {
                        PatientId = user.Id,
                        DoctorId = model.StaffId,
                        PatientName = model.IsForDependent ? model.DependentName! : $"{user.FirstName} {user.LastName}",
                        AppointmentDate = appointmentDate,
                        AppointmentTime = appointmentTime,
                        AppointmentTimeInput = model.AppointmentTime,
                        Description = model.Description,
                        ReasonForVisit = model.Description,
                        Status = AppointmentStatus.Pending,
                        AgeValue = model.IsForDependent ? model.DependentAge ?? 0 : 
                            CalculateAge(DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        RelationshipToDependent = model.IsForDependent ? model.RelationshipToDependent : null,
                        ContactNumber = user.PhoneNumber,
                        DependentFullName = model.IsForDependent ? model.DependentName : null,
                        DependentAge = model.IsForDependent ? model.DependentAge : null,
                        Type = "General Checkup"
                    };

                    _logger.LogInformation("Attempting to insert appointment: {@AppointmentDetails}", new
                    {
                        appointment.PatientId,
                        appointment.DoctorId,
                        appointment.AppointmentDate,
                        appointment.AppointmentTime,
                        appointment.Status,
                        appointment.Description,
                        IsForDependent = model.IsForDependent
                    });
                    
                    _context.Appointments.Add(appointment);

                    // Log the SQL command
                    var sql = _context.Database.GetDbConnection().CreateCommand();
                    _logger.LogInformation("SQL Command: {Command}", sql.CommandText);

                    // Save changes and get the number of affected rows
                    var affectedRows = await _context.SaveChangesAsync();
                    _logger.LogInformation("SaveChanges affected {AffectedRows} rows", affectedRows);

                    // Verify the appointment was created with a new context
                    using (var verificationContext = new ApplicationDbContext(
                        new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseSqlServer(connection)
                            .Options))
                    {
                        verificationContext.Database.SetDbConnection(connection);
                        
                        var createdAppointment = await verificationContext.Appointments
                            .AsNoTracking()
                            .FirstOrDefaultAsync(a => a.Id == appointment.Id);

                        if (createdAppointment == null)
                        {
                            _logger.LogError("Failed to verify appointment creation. AppointmentId: {AppointmentId}", 
                                appointment.Id);
                            await transaction.RollbackAsync();
                            throw new Exception("Failed to verify appointment creation");
                        }

                        _logger.LogInformation("Successfully verified appointment creation. AppointmentId: {AppointmentId}", 
                            appointment.Id);
                    }

                    await transaction.CommitAsync();
                    _logger.LogInformation("Transaction committed successfully");

                    // Add a delay and verify again after commit
                    await Task.Delay(500);

                    var postCommitAppointment = await _context.Appointments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == appointment.Id);

                    if (postCommitAppointment == null)
                    {
                        _logger.LogError("Appointment not found after commit. AppointmentId: {AppointmentId}", 
                            appointment.Id);
                    }
                    else
                    {
                        _logger.LogInformation("Appointment verified after commit. AppointmentId: {AppointmentId}", 
                            appointment.Id);
                    }

                    // Store appointment ID and creation time in TempData
                    TempData["LastCreatedAppointmentId"] = appointment.Id;
                    TempData["AppointmentCreationTime"] = DateTime.UtcNow.ToString("O");

                    _logger.LogInformation("Redirecting to NCDRiskAssessment. AppointmentId: {AppointmentId}", 
                        appointment.Id);

                    return RedirectToAction(nameof(NCDRiskAssessment), new { appointmentId = appointment.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment: {ErrorMessage}", ex.Message);
                    await transaction.RollbackAsync();
                    _logger.LogInformation("Transaction rolled back due to error");

                    ModelState.AddModelError("", "An error occurred while booking the appointment. Please try again.");
                    var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                    ViewBag.Doctors = doctors;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in BookAppointment: {ErrorMessage}", ex.Message);
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                var doctors = await _userManager.GetUsersInRoleAsync("Doctor");
                ViewBag.Doctors = doctors;
                return View(model);
            }
        }

        // GET: User/NCDRiskAssessment
        [HttpGet]
        public async Task<IActionResult> NCDRiskAssessment(int appointmentId)
        {
            try
            {
                _logger.LogInformation("Starting NCDRiskAssessment GET action. AppointmentId: {AppointmentId}, Time: {Time}", 
                    appointmentId, DateTime.UtcNow);

                // Log database connection state
                var connection = _context.Database.GetDbConnection();
                _logger.LogInformation("Database connection state: {State}", connection.State);
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("User not found in NCDRiskAssessment. User.Identity.Name: {UserName}, IsAuthenticated: {IsAuthenticated}", 
                        User.Identity?.Name, User.Identity?.IsAuthenticated);
                    return NotFound("User not found");
                }

                _logger.LogInformation("User details in NCDRiskAssessment: {@UserDetails}", new { 
                    user.Id, 
                    user.UserName, 
                    user.Email,
                    user.PhoneNumber,
                    IsInRole = await _userManager.IsInRoleAsync(user, "Patient")
                });

                // Verify against TempData
                var lastCreatedAppointmentId = TempData.Peek("LastCreatedAppointmentId") as int?;
                var creationTimeStr = TempData.Peek("AppointmentCreationTime") as string;

                _logger.LogInformation("TempData state: LastCreatedAppointmentId: {LastCreatedId}, CreationTime: {CreationTime}", 
                    lastCreatedAppointmentId, creationTimeStr);

                if (lastCreatedAppointmentId.HasValue)
                {
                    if (lastCreatedAppointmentId.Value != appointmentId)
                    {
                        _logger.LogWarning("Appointment ID mismatch. Expected: {ExpectedId}, Actual: {ActualId}", 
                            lastCreatedAppointmentId.Value, appointmentId);
                    }

                    // Check if we're within a reasonable time window from appointment creation
                    if (creationTimeStr != null && DateTime.TryParse(creationTimeStr, out DateTime creationTime))
                    {
                        var timeSinceCreation = DateTime.UtcNow - creationTime;
                        _logger.LogInformation("Time since appointment creation: {Seconds} seconds", 
                            timeSinceCreation.TotalSeconds);

                        if (timeSinceCreation.TotalSeconds > 30)
                        {
                            _logger.LogWarning("Accessing NCDRiskAssessment after significant delay: {Seconds}s", 
                                timeSinceCreation.TotalSeconds);
                        }
                    }
                }

                // Add retry logic for appointment retrieval
                Appointment? appointment = null;
                int retryCount = 0;
                const int maxRetries = 3;
                const int delayMs = 200;

                while (retryCount < maxRetries && appointment == null)
                {
                    try
                    {
                        // First try with tracking disabled
                        appointment = await _context.Appointments
                            .AsNoTracking()
                            .Include(a => a.Doctor)
                            .FirstOrDefaultAsync(a => a.Id == appointmentId);

                        if (appointment == null)
                        {
                            _logger.LogWarning("Attempt {RetryCount}: Appointment not found with AsNoTracking. " +
                                "AppointmentId: {AppointmentId}", retryCount + 1, appointmentId);

                            // Try again with tracking enabled
                            appointment = await _context.Appointments
                                .Include(a => a.Doctor)
                                .FirstOrDefaultAsync(a => a.Id == appointmentId);

                            if (appointment == null)
                            {
                                _logger.LogWarning("Attempt {RetryCount}: Appointment not found with tracking enabled. " +
                                    "AppointmentId: {AppointmentId}", retryCount + 1, appointmentId);

                                // Log all appointments for this user for debugging
                                var userAppointments = await _context.Appointments
                                    .AsNoTracking()
                                    .Where(a => a.PatientId == user.Id)
                                    .OrderByDescending(a => a.CreatedAt)
                                    .Take(5)
                                    .Select(a => new { a.Id, a.AppointmentDate, a.Status, a.CreatedAt })
                                    .ToListAsync();

                                _logger.LogInformation("User's recent appointments: {@UserAppointments}", userAppointments);

                                // Check if the appointment exists but with a different PatientId
                                var appointmentExists = await _context.Appointments
                                    .AsNoTracking()
                                    .AnyAsync(a => a.Id == appointmentId);

                                if (appointmentExists)
                                {
                                    var foundAppointment = await _context.Appointments
                                        .AsNoTracking()
                                        .Select(a => new { a.Id, a.PatientId, a.DoctorId, a.CreatedAt })
                                        .FirstOrDefaultAsync(a => a.Id == appointmentId);

                                    _logger.LogWarning("Appointment exists but with different details: {@AppointmentDetails}", 
                                        foundAppointment);
                                }

                                // Try direct SQL query as last resort
                                using (var command = connection.CreateCommand())
                                {
                                    command.CommandText = "SELECT TOP 1 Id, PatientId, DoctorId, CreatedAt FROM Appointments WHERE Id = @Id";
                                    var parameter = command.CreateParameter();
                                    parameter.ParameterName = "@Id";
                                    parameter.Value = appointmentId;
                                    command.Parameters.Add(parameter);

                                    if (connection.State != System.Data.ConnectionState.Open)
                                        await connection.OpenAsync();

                                    using var reader = await command.ExecuteReaderAsync();
                                    if (await reader.ReadAsync())
                                    {
                                        _logger.LogInformation("Found appointment via direct SQL: Id={Id}, PatientId={PatientId}, CreatedAt={CreatedAt}",
                                            reader.GetInt32(0),
                                            reader.GetString(1),
                                            reader.GetDateTime(3));
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Appointment not found via direct SQL query");
                                    }
                                }

                                await Task.Delay(delayMs * (retryCount + 1));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error retrieving appointment on attempt {RetryCount}", retryCount + 1);
                        await Task.Delay(delayMs * (retryCount + 1));
                    }

                    retryCount++;
                }

                if (appointment == null)
                {
                    _logger.LogError("Failed to retrieve appointment after {MaxRetries} retries. " +
                        "AppointmentId: {AppointmentId}, UserId: {UserId}", maxRetries, appointmentId, user.Id);
                    return NotFound("Appointment not found");
                }

                // Verify the appointment belongs to the current user
                if (appointment.PatientId != user.Id)
                {
                    _logger.LogError("Appointment belongs to different user. AppointmentId: {AppointmentId}, " +
                        "Expected UserId: {ExpectedUserId}, Actual UserId: {ActualUserId}",
                        appointmentId, user.Id, appointment.PatientId);
                    return NotFound("Appointment not found");
                }

                _logger.LogInformation("Found appointment: {@Appointment}", new
                {
                    appointment.Id,
                    appointment.AppointmentDate,
                    appointment.AppointmentTime,
                    appointment.Status,
                    appointment.PatientId,
                    appointment.DoctorId,
                    appointment.CreatedAt
                });

                // Check if assessment already exists
                var existingAssessment = await _context.NCDRiskAssessments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.AppointmentId == appointmentId);

                if (existingAssessment != null)
                {
                    _logger.LogInformation("Found existing assessment for appointment: {AppointmentId}", appointmentId);
                    return View(existingAssessment);
                }

                // Create new assessment
                var assessment = new NCDRiskAssessment
                {
                    UserId = user.Id,
                    AppointmentId = appointmentId,
                    Address = user.Address ?? string.Empty,
                    Birthday = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) && parsedBirthDate != DateTime.MinValue ? parsedBirthDate.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd"),
                    Telepono = user.PhoneNumber ?? string.Empty,
                    Edad = appointment.AgeValue.ToString(),
                    CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _logger.LogInformation("Created new assessment: {@Assessment}", new
                {
                    assessment.UserId,
                    assessment.AppointmentId,
                    assessment.Edad,
                    assessment.CreatedAt
                });

                return View(assessment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accessing NCDRiskAssessment. AppointmentId: {AppointmentId}", appointmentId);
                return RedirectToAction("Error", "Home", new { message = "Error accessing NCD Risk Assessment" });
            }
        }

        // POST: User/NCDRiskAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NCDRiskAssessment([FromForm] NCDRiskAssessment assessment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state in NCDRiskAssessment POST");
                    return View(assessment);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User not found when saving assessment");
                    return NotFound("User not found");
                }

                var appointment = await _context.Appointments
                    .FirstOrDefaultAsync(a => a.Id == assessment.AppointmentId && a.PatientId == user.Id);

                if (appointment == null)
                {
                    _logger.LogWarning($"Appointment not found when saving assessment. AppointmentId: {assessment.AppointmentId}");
                    return NotFound("Appointment not found");
                }

                assessment.UserId = user.Id;
                assessment.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                assessment.UpdatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                _context.NCDRiskAssessments.Add(assessment);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully saved assessment for appointment: {assessment.AppointmentId}");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving assessment. AppointmentId: {assessment.AppointmentId}");
                ModelState.AddModelError("", "An error occurred while saving the assessment. Please try again.");
                return View(assessment);
            }
        }

        private async Task<string> GetHealthFacilityName()
        {
            // Implement logic to get health facility name from configuration or database
            return await Task.FromResult("Barangay Health Center");
        }

        private async Task<string> GenerateFamilyNumber()
        {
            // Implement logic to generate a unique family number
            return await Task.FromResult("F" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimeSlots(string doctorId, string date)
        {
            if (string.IsNullOrEmpty(doctorId) || !DateTime.TryParse(date, out DateTime appointmentDate))
            {
                return BadRequest();
            }

            var bookedSlots = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && 
                           a.AppointmentDate.Date == appointmentDate.Date &&
                           a.Status != AppointmentStatus.Cancelled)
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            // Generate time slots from 9 AM to 5 PM with 30-minute intervals
            var availableSlots = new List<string>();
            var startTime = new TimeSpan(9, 0, 0);
            var endTime = new TimeSpan(17, 0, 0);

            while (startTime <= endTime)
            {
                if (!bookedSlots.Contains(startTime))
                {
                    availableSlots.Add(startTime.ToString(@"hh\:mm"));
                }
                startTime = startTime.Add(TimeSpan.FromMinutes(30));
            }

            return Json(availableSlots);
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            // Handle default/invalid birth dates
            if (dateOfBirth == default(DateTime) || dateOfBirth == DateTime.MinValue || dateOfBirth.Year < 1900)
            {
                return 0; // Return 0 for invalid birth dates
            }
            
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    public class AppointmentRequest
    {
        public string DoctorId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
} 