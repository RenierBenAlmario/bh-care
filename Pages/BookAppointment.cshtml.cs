using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Barangay.Models;
using Barangay.Data;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Linq;
using Barangay.Extensions;

namespace Barangay.Pages
{
    [Authorize]
    public class BookAppointmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<BookAppointmentModel> _logger;
        private readonly IDatabaseDebugService _dbDebugService;
        private readonly IDataEncryptionService _encryptionService;

        public BookAppointmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            ILogger<BookAppointmentModel> logger,
            IDatabaseDebugService dbDebugService,
            IDataEncryptionService encryptionService)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _dbDebugService = dbDebugService;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public AppointmentBookingViewModel BookingModel { get; set; } = new();

        [BindProperty]
        public IFormFile? AttachmentFile { get; set; }

        [BindProperty]
        public NCDRiskAssessmentViewModel NCDModel { get; set; } = new();

        [BindProperty]
        public HEEADSSSAssessmentViewModel HEEADSSSModel { get; set; } = new();

        public List<Barangay.Models.Doctor> Doctors { get; set; } = new();

        public UserProfile UserProfile { get; set; } = new();

        // Default doctor used when there's no doctor selection on the UI
        public string DefaultDoctorId { get; set; } = string.Empty;

        [BindProperty]
        public bool BookingSuccess { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public UserDetailsViewModel UserDetails { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var doctorsInRole = await _userManager.GetUsersInRoleAsync("Doctor");
            try
            {
                // Initialize the booking model and set the first step
                BookingModel = new AppointmentBookingViewModel { CurrentStep = 1 };
                
                // Initialize NCD Risk Assessment Model
                NCDModel = new NCDRiskAssessmentViewModel();
                
                // Initialize HEEADSSS Assessment Model
                HEEADSSSModel = new HEEADSSSAssessmentViewModel();

                // Load user profile data to pre-fill certain fields
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                _logger.LogInformation($"BookAppointment - Before decryption: FullName='{user.FullName}', FirstName='{user.FirstName}', LastName='{user.LastName}', Email='{user.Email}'");

                // Decrypt user data for authorized users
                user = user.DecryptSensitiveData(_encryptionService, User);
                
                // Manually decrypt Email and PhoneNumber since they're not marked with [Encrypted] attribute
                if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
                {
                    user.Email = user.Email.DecryptForUser(_encryptionService, User);
                }
                if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
                {
                    user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
                }

                _logger.LogInformation($"BookAppointment - After decryption: FullName='{user.FullName}', FirstName='{user.FirstName}', LastName='{user.LastName}', Email='{user.Email}'");

                    // Set the public property for the Razor page
                    UserDetails = new UserDetailsViewModel
                    {
                        FullName = user.FullName ?? $"{user.FirstName} {user.LastName}".Trim(),
                        Age = CalculateAge(DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue)
                    };

                    _logger.LogInformation($"BookAppointment - UserDetails set: FullName='{UserDetails.FullName}', Age={UserDetails.Age}");

                    // Pre-fill name if available
                    if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
                    {
                        BookingModel.FirstName = user.FirstName;
                        BookingModel.LastName = user.LastName;
                        BookingModel.FullName = $"{user.FirstName} {user.LastName}";
                    }
                    else if (!string.IsNullOrEmpty(user.FullName))
                    {
                        // Split full name if first name and last name are not available
                        var nameParts = user.FullName.Split(' ');
                        if (nameParts.Length >= 2)
                        {
                            BookingModel.FirstName = nameParts[0];
                            BookingModel.LastName = nameParts[nameParts.Length - 1];
                            BookingModel.FullName = user.FullName;
                        }
                    }
                    
                    // Pre-fill address if available
                    if (!string.IsNullOrEmpty(user.Address))
                    {
                        BookingModel.Address = user.Address;
                        NCDModel.Address = user.Address;
                    }
                    
                    // Pre-fill date of birth if available
                    var userBirthDate = DateTime.TryParse(user.BirthDate, out var parsedUserBirthDate) ? parsedUserBirthDate : DateTime.MinValue;
                    if (userBirthDate != default(DateTime))
                    {
                        BookingModel.DateOfBirth = userBirthDate;
                        NCDModel.Birthday = userBirthDate.ToString("yyyy-MM-dd");
                        BookingModel.Age = CalculateAge(userBirthDate);
                    }
                    
                    // Pre-fill phone number if available
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        BookingModel.PhoneNumber = user.PhoneNumber;
                        NCDModel.Telepono = user.PhoneNumber;
                    }
                }

                // Load available doctors with safe fallback
                var doctorUsers = await _userManager.GetUsersInRoleAsync("Doctor");
                var dbDoctors = await _context.Doctors
                    .Where(d => doctorUsers.Select(du => du.Id).Contains(d.UserId))
                    .Include(d => d.User)
                    .ToListAsync();

                if (dbDoctors != null && dbDoctors.Any())
                {
                    Doctors = dbDoctors;
                }
                else
                {
                    // Fallback: build doctor list from AspNetUsers in Doctor role
                    Doctors = doctorUsers
                        .Select(u => new Barangay.Models.Doctor { Id = u.Id, UserId = u.Id, FullName = u.FullName ?? u.UserName })
                        .ToList();
                }

                DefaultDoctorId = Doctors.FirstOrDefault()?.UserId ?? string.Empty;
                
                _logger.LogInformation($"BookAppointment OnGetAsync - Found {Doctors.Count} doctors, DefaultDoctorId: {DefaultDoctorId}");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnGetAsync: {ErrorMessage}", ex.Message);
                
                // Initialize a basic model in case of error
                BookingModel = new AppointmentBookingViewModel { CurrentStep = 1 };
                NCDModel = new NCDRiskAssessmentViewModel();
                HEEADSSSModel = new HEEADSSSAssessmentViewModel();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Clear ModelState to bypass validation
                ModelState.Clear();
                
                if (string.IsNullOrEmpty(BookingModel.HealthFacilityId))
                {
                    BookingModel.HealthFacilityId = GenerateHealthFacilityId();
                }
                
                // Check if user is authenticated before proceeding
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // Instead of throwing an exception which causes logout, redirect to login
                    _logger.LogWarning("User is not authenticated when submitting form");
                    TempData["ErrorMessage"] = "Your session has expired. Please log in again to continue.";
                    return RedirectToPage("/Account/Login");
                }
                
                // Process form data from submitted fields
                try 
                {
                    // Extract form data directly from the form collection if the model binding didn't work
                    if (string.IsNullOrEmpty(BookingModel.TimeSlot) && Request.Form["timeSlot"].Count > 0)
                    {
                        BookingModel.TimeSlot = Request.Form["timeSlot"];
                        _logger.LogInformation($"Retrieved time slot from form: {BookingModel.TimeSlot}");
                    }
                    
                    if (string.IsNullOrEmpty(BookingModel.AppointmentDate) && Request.Form["appointmentDate"].Count > 0)
                    {
                        BookingModel.AppointmentDate = Request.Form["appointmentDate"];
                        _logger.LogInformation($"Retrieved appointment date from form: {BookingModel.AppointmentDate}");
                    }
                    
                    if (string.IsNullOrEmpty(BookingModel.Gender) && Request.Form["gender"].Count > 0)
                    {
                        BookingModel.Gender = Request.Form["gender"];
                    }
                    else if (string.IsNullOrEmpty(BookingModel.Gender))
                    {
                        // Try to get gender from user profile or default to "Not specified"
                        BookingModel.Gender = user.Gender ?? "Not specified";
                    }
                    
                    // Ensure we have a value for FullName
                    if (string.IsNullOrEmpty(BookingModel.FullName) && !string.IsNullOrEmpty(BookingModel.FirstName) && !string.IsNullOrEmpty(BookingModel.LastName))
                    {
                        BookingModel.FullName = $"{BookingModel.FirstName} {BookingModel.LastName}".Trim();
                    }
                    else if (string.IsNullOrEmpty(BookingModel.FullName) && Request.Form["fullName"].Count > 0)
                    {
                        BookingModel.FullName = Request.Form["fullName"];
                    }
                    
                    _logger.LogInformation("Form data processed successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Error processing form data: {ex.Message}");
                    // Continue with the data we have
                }
                
                // Process the form submission based on the current step
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Save all booking information to the database
                        SaveBookingInformationAsync();
                    
                        // Display a success message
                        TempData["SuccessMessage"] = "Your appointment has been booked successfully!";
                    
                        // Redirect to User Dashboard instead of Index to prevent logout
                        return RedirectToPage("/User/UserDashboard");
                }
                catch (Exception ex)
                {
                        _logger.LogError(ex, "Error saving booking information");
                        ModelState.AddModelError(string.Empty, "An error occurred while booking your appointment. Please try again later.");
                    return Page();
                }
            }
                else
            {
                    // If the model is invalid, redisplay the form with validation errors
                return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in OnPostAsync");
                TempData["ErrorMessage"] = "An error occurred while booking your appointment. Please try again later.";
                return RedirectToPage("/BookAppointment");
            }
        }

        // Add a new handler for AJAX requests to create appointments
        public async Task<IActionResult> OnPostCreateAjaxAsync()
        {
            try
            {
                _logger.LogInformation("Processing AJAX appointment creation request");
                
                // Clear ModelState to bypass validation
                ModelState.Clear();
                
                // Check if user is authenticated
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("User is not authenticated during AJAX appointment creation");
                    return new JsonResult(new { success = false, error = "User not authenticated" });
                }
                
                // Check if booking for someone else
                bool bookingForOther = false;
                string? relationship = null;
                
                if (Request.Form.TryGetValue("bookingForOther", out var bookingForOtherValue))
                {
                    bookingForOther = bookingForOtherValue.ToString().ToLower() == "true";
                    _logger.LogInformation("Received bookingForOther from form: {BookingForOther}", bookingForOther);
                }
                else if (Request.Form.TryGetValue("bookingForOtherHidden", out var bookingForOtherHiddenValue))
                {
                    bookingForOther = bookingForOtherHiddenValue.ToString().ToLower() == "true";
                    _logger.LogInformation("Received bookingForOtherHidden from form: {BookingForOther}", bookingForOther);
                }
                else
                {
                    _logger.LogWarning("bookingForOther not received from form");
                }
                
                // Fallback: If patient details are provided and different from logged-in user, assume booking for other
                if (!bookingForOther && Request.Form.TryGetValue("fullName", out var fullNameValue))
                {
                    var loggedInUser = await _userManager.GetUserAsync(User);
                    if (loggedInUser != null)
                    {
                        loggedInUser = loggedInUser.DecryptSensitiveData(_encryptionService, User);
                        if (!string.IsNullOrEmpty(fullNameValue) && !string.Equals(fullNameValue, loggedInUser.FullName, StringComparison.OrdinalIgnoreCase))
                        {
                            bookingForOther = true;
                            _logger.LogInformation("Detected booking for other based on different patient name: {PatientName} vs {LoggedInUser}", fullNameValue, loggedInUser.FullName);
                        }
                    }
                }
                
                if (bookingForOther && Request.Form.TryGetValue("relationship", out var relationshipValue))
                {
                    relationship = relationshipValue.ToString();
                }
                
                // Extract form data
                var bookingModel = new AppointmentBookingViewModel();

                if (Request.Form.TryGetValue("fullName", out var fullName))
                {
                    bookingModel.FullName = fullName;
                    _logger.LogInformation("Received fullName from form: {FullName}", fullName);
                    var nameParts = fullName.ToString().Split(' ');
                    if (nameParts.Length > 0)
                        bookingModel.FirstName = nameParts[0];
                    if (nameParts.Length > 1)
                        bookingModel.LastName = nameParts[nameParts.Length - 1];
                }
                else
                {
                    _logger.LogWarning("FullName not received from form");
                }
                
                if (Request.Form.TryGetValue("age", out var age) && int.TryParse(age, out int ageValue))
                {
                    bookingModel.Age = ageValue;
                    _logger.LogInformation("Received age from form: {Age}", ageValue);
                }
                
                if (Request.Form.TryGetValue("birthday", out var birthday) && DateTime.TryParse(birthday, out DateTime birthdayValue))
                {
                    bookingModel.Birthday = birthdayValue;
                    _logger.LogInformation("Received birthday from form: {Birthday}", birthdayValue);
                }
                else
                {
                    _logger.LogWarning("Birthday not received or invalid from form. Value: {Birthday}", birthday);
                }
                
                if (Request.Form.TryGetValue("phoneNumber", out var phoneNumber))
                {
                    bookingModel.PhoneNumber = phoneNumber;
                }
                
                if (Request.Form.TryGetValue("appointmentDate", out var appointmentDate))
                {
                    bookingModel.AppointmentDate = appointmentDate;
                }
                
                if (Request.Form.TryGetValue("timeSlot", out var timeSlot))
                {
                    bookingModel.TimeSlot = timeSlot;
                }
                
                if (Request.Form.TryGetValue("consultationType", out var consultationType))
                {
                    bookingModel.ConsultationType = consultationType;
                }
                
                if (Request.Form.TryGetValue("reasonForVisit", out var reasonForVisit))
                {
                    bookingModel.ReasonForVisit = reasonForVisit;
                }

                if (Request.Form.TryGetValue("DoctorId", out var doctorIdValue))
                {
                    bookingModel.DoctorId = doctorIdValue;
                }
                
                if (bookingForOther)
                {
                    bookingModel.Relationship = relationship;
                }

                // Verify that the selected time slot is still available
                var validationResult = await ValidateTimeSlotAsync(bookingModel);
                if (validationResult != null)
                {
                    return validationResult;
                }
                
                try
                {
                    var appointmentId = await CreateTemporaryAppointmentAsync(user.Id, bookingModel, bookingForOther);
                    
                    if (appointmentId > 0)
                    {
                        _logger.LogInformation($"Successfully created temporary appointment with ID: {appointmentId}");
                        
                        // Create a notification for the doctor
                        if (!string.IsNullOrEmpty(bookingModel.DoctorId))
                        {
                            var notification = new Notification
                            {
                                Title = "New Appointment",
                                Message = $"You have a new appointment with {bookingModel.FullName} on {bookingModel.AppointmentDate:d} at {bookingModel.TimeSlot:t}.",
                                UserId = bookingModel.DoctorId, // The user who the notification is for
                                RecipientId = bookingModel.DoctorId,
                                CreatedAt = DateTime.Now,
                                IsRead = false,
                                Type = "Info",
                                Link = "/Doctor/Appointments"
                            };
                            _context.Notifications.Add(notification);
                            await _context.SaveChangesAsync();
                        }

                        return new JsonResult(new { 
                            success = true, 
                            appointmentId = appointmentId,
                            age = bookingModel.Age, // Frontend expects 'age' not 'AgeValue'
                            bookingForOther = bookingForOther,
                            relationship = relationship,
                            message = "Appointment created as draft. Please complete the assessment form to finalize your booking."
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create temporary appointment");
                        return new JsonResult(new { success = false, error = "Failed to create appointment" });
                    }
                }
                catch (InvalidOperationException iex)
                {
                    _logger.LogWarning($"Invalid operation during appointment creation: {iex.Message}");
                    return new JsonResult(new { 
                        success = false, 
                        error = iex.Message,
                        errorType = "TimeSlotConflict"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating appointment");
                    return new JsonResult(new { 
                        success = false, 
                        error = "Error creating appointment: " + ex.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostCreateAjaxAsync");
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
        
        private async Task EnsurePatientRecordExistsAsync(string userId)
        {
            var patientExists = await _context.Patients.AnyAsync(p => p.UserId == userId);
            if (!patientExists)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var newPatient = new Patient
                    {
                        UserId = user.Id,
                        FullName = _encryptionService.Encrypt($"{user.FirstName} {user.LastName}".Trim()),
                        BirthDate = DateTime.TryParse(user.BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue,
                        Gender = user.Gender ?? "Unknown",
                        Address = _encryptionService.Encrypt(user.Address ?? "Not provided"),
                        ContactNumber = _encryptionService.Encrypt(user.PhoneNumber ?? "Not provided"),
                        Email = _encryptionService.Encrypt(user.Email ?? "no-email@bhcare.com"),
                        EmergencyContact = _encryptionService.Encrypt("Not provided"),
                        EmergencyContactNumber = _encryptionService.Encrypt("Not provided"),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Patients.Add(newPatient);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created new patient record for user ID: {userId}");
                }
            }
        }

        // Helper method to create a temporary appointment record and return its ID
        private async Task<int> CreateTemporaryAppointmentAsync(string userId, AppointmentBookingViewModel bookingModel, bool bookingForOther)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new Exception("User not found.");

                // Decrypt user data for authorized users
                user = user.DecryptSensitiveData(_encryptionService, User);
                
                // Manually decrypt Email and PhoneNumber since they're not marked with [Encrypted] attribute
                if (!string.IsNullOrEmpty(user.Email) && _encryptionService.IsEncrypted(user.Email))
                {
                    user.Email = user.Email.DecryptForUser(_encryptionService, User);
                }
                if (!string.IsNullOrEmpty(user.PhoneNumber) && _encryptionService.IsEncrypted(user.PhoneNumber))
                {
                    user.PhoneNumber = user.PhoneNumber.DecryptForUser(_encryptionService, User);
                }

                // If booking for self, ensure a patient record exists.
                if (!bookingForOther)
                {
                    await EnsurePatientRecordExistsAsync(userId);
                }

                DateTime appointmentDate = DateTime.Parse(bookingModel.AppointmentDate);
                // Convert from 12-hour format (e.g. "8:00 AM") to TimeSpan
                TimeSpan selectedApptTime;
                if (DateTime.TryParse(bookingModel.TimeSlot, out DateTime parsedTime))
                {
                    selectedApptTime = parsedTime.TimeOfDay;
                    _logger.LogInformation($"Successfully parsed time: {bookingModel.TimeSlot} to {selectedApptTime}");
                }
                else
                {
                    _logger.LogError($"Failed to parse time string: {bookingModel.TimeSlot}");
                    return -1; // Indicate failure with a negative value
                }
                string selectedConsultationType = bookingModel.ConsultationType ?? "medical";

                // Centralized time slot validation
                var validationResult = await ValidateTimeSlotAsync(bookingModel);
                if (validationResult != null)
                {
                    throw new InvalidOperationException("Time slot conflict.");
                }

                var patientName = bookingForOther ? bookingModel.FullName : user.FullName;
                // Use the age from the form if available, otherwise calculate from birth date
                var userBirthDate = DateTime.TryParse(user.BirthDate, out var parsedUserBirthDate) ? parsedUserBirthDate : DateTime.MinValue;
                var patientAge = bookingForOther ? bookingModel.Age : 
                    (bookingModel.Age > 0 ? bookingModel.Age : CalculateAge(userBirthDate));
                var patientBirthday = bookingForOther ? bookingModel.Birthday : userBirthDate;

                _logger.LogInformation("Appointment creation - bookingForOther: {BookingForOther}, patientName: {PatientName}, patientAge: {PatientAge}, patientBirthday: {PatientBirthday}", 
                    bookingForOther, patientName, patientAge, patientBirthday);

                // Use the selected doctor from the booking model
                var doctor = await _context.Users.FindAsync(bookingModel.DoctorId);
                
                var newAppointment = new Models.Appointment
                {
                    ApplicationUserId = userId, // Link to the user who booked the appointment
                    PatientId = userId, // Always set PatientId to the current user
                    PatientName = patientName,
                    AgeValue = patientAge,
                    DateOfBirth = patientBirthday,
                    Gender = bookingModel.Gender,
                    ContactNumber = bookingModel.PhoneNumber,
                    AppointmentDate = appointmentDate,
                    AppointmentTime = selectedApptTime,
                    Type = selectedConsultationType,
                    ReasonForVisit = bookingModel.ReasonForVisit,
                    Status = AppointmentStatus.Draft, // Save as Draft instead of Pending
                    DoctorId = doctor?.Id, // Assign a doctor if found
                    BookingForOther = bookingForOther,
                    Relationship = bookingForOther ? bookingModel.Relationship : null
                };

                _context.Appointments.Add(newAppointment);
                await _context.SaveChangesAsync();

                return newAppointment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating temporary appointment");
                return 0; // Indicate failure
            }
        }

        private string GenerateHealthFacilityId()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            var random = new Random();
            
            return new string(Enumerable.Repeat(letters, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray()) +
                new string(Enumerable.Repeat(numbers, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public IActionResult SaveBookingInformationAsync()
        {
            // This method appears to be a remnant of a previous implementation and is no longer called.
            // The logic is now handled by OnPostCreateAjaxAsync and CreateTemporaryAppointmentAsync.
            // To prevent confusion, it can be removed or marked as obsolete.
            _logger.LogWarning("SaveBookingInformationAsync was called, but it is considered obsolete.");
            return Page();
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

        private int GetConsultationTypeDuration(string consultationType)
        {
            // Return duration in minutes based on consultation type
            switch (consultationType?.ToLower())
            {
                case "general consult":
                    return 30;
                case "dental":
                    return 45;
                case "immunization":
                    return 20;
                case "prenatal & family planning":
                case "prenatal and family planning":
                    return 30;
                case "dots consult":
                    return 30;
                default:
                    return 30; // Default duration
            }
        }

        // Defines allowed days and time windows per consultation type
        // Windows are in 24-hour TimeSpan ranges and will be intersected with doctor availability
        private (HashSet<DayOfWeek> Days, List<(TimeSpan Start, TimeSpan End)> Windows) GetConsultationTypeSchedule(string consultationType)
        {
            var type = consultationType?.ToLower() ?? string.Empty;
            var days = new HashSet<DayOfWeek>();
            var windows = new List<(TimeSpan Start, TimeSpan End)>();

            switch (type)
            {
                // General Consult (8AM-11AM, 1PM-4PM, Mon-Fri) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                case "general consult":
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Tuesday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Thursday);
                    days.Add(DayOfWeek.Friday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(8), TimeSpan.FromHours(11)));
                    windows.Add((TimeSpan.FromHours(13), TimeSpan.FromHours(16)));
                    break;

                // Dental (8-11AM, Mon/Wed/Fri) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                case "dental":
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Friday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(8), TimeSpan.FromHours(11)));
                    break;

                // Immunization (8AM-12PM, Wed) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                case "immunization":
                    days.Add(DayOfWeek.Wednesday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(8), TimeSpan.FromHours(12)));
                    break;

                // Prenatal & Family Planning (8AM-11AM, 1PM-4PM, Mon/Wed/Fri) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                case "prenatal & family planning":
                case "prenatal and family planning":
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Friday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(8), TimeSpan.FromHours(11)));
                    windows.Add((TimeSpan.FromHours(13), TimeSpan.FromHours(16)));
                    break;

                // DOTS Consult (1-4PM, Mon-Fri) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                case "dots consult":
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Tuesday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Thursday);
                    days.Add(DayOfWeek.Friday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(13), TimeSpan.FromHours(16)));
                    break;


                // Default: fallback to general weekdays, full day window (will also be intersected with doctor availability) - TEMPORARILY INCLUDING WEEKENDS FOR TESTING
                default:
                    days.Add(DayOfWeek.Monday);
                    days.Add(DayOfWeek.Tuesday);
                    days.Add(DayOfWeek.Wednesday);
                    days.Add(DayOfWeek.Thursday);
                    days.Add(DayOfWeek.Friday);
                    // TEMPORARILY ADD WEEKENDS FOR TESTING
                    days.Add(DayOfWeek.Saturday);
                    days.Add(DayOfWeek.Sunday);
                    windows.Add((TimeSpan.FromHours(8), TimeSpan.FromHours(17)));
                    break;
            }

            return (days, windows);
        }

        public async Task<IActionResult> OnGetFixWeekendsAsync()
        {
            try
            {
                // Get all doctors
                var doctors = await _context.Users
                    .Where(u => _context.UserRoles
                        .Any(ur => ur.UserId == u.Id && 
                                   _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Doctor")))
                    .ToListAsync();

                var updatedCount = 0;
                var createdCount = 0;

                foreach (var doctor in doctors)
                {
                    // Check if DoctorAvailability exists
                    var availability = await _context.DoctorAvailabilities
                        .FirstOrDefaultAsync(da => da.DoctorId == doctor.Id);

                    if (availability == null)
                    {
                        // Create new availability with weekend support
                        availability = new DoctorAvailability
                        {
                            DoctorId = doctor.Id,
                            IsAvailable = true,
                            Monday = true,
                            Tuesday = true,
                            Wednesday = true,
                            Thursday = true,
                            Friday = true,
                            Saturday = true,  // ENABLE WEEKENDS
                            Sunday = true,    // ENABLE WEEKENDS
                            StartTime = new TimeSpan(8, 0, 0), // 8:00 AM
                            EndTime = new TimeSpan(17, 0, 0),  // 5:00 PM
                            LastUpdated = DateTime.Now
                        };

                        _context.DoctorAvailabilities.Add(availability);
                        createdCount++;
                    }
                    else
                    {
                        // Update existing availability
                        availability.Saturday = true;  // ENABLE WEEKENDS
                        availability.Sunday = true;    // ENABLE WEEKENDS
                        availability.IsAvailable = true;
                        availability.StartTime = new TimeSpan(8, 0, 0);
                        availability.EndTime = new TimeSpan(17, 0, 0);
                        availability.LastUpdated = DateTime.Now;
                        updatedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                return new JsonResult(new { 
                    success = true, 
                    message = $"Fixed weekend appointments for {doctors.Count} doctors! Updated {updatedCount} existing records and created {createdCount} new records.",
                    updatedCount = updatedCount,
                    createdCount = createdCount,
                    totalDoctors = doctors.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing weekend appointments");
                return new JsonResult(new { 
                    success = false, 
                    error = ex.Message 
                });
            }
        }

        public async Task<IActionResult> OnGetBookedTimeSlotsAsync(string date, string consultationType, string doctorId)
        {
            try
            {
                _logger.LogInformation($"Getting time slots for date: {date}, consultation type: {consultationType}, doctor: {doctorId}");

                if (string.IsNullOrEmpty(doctorId) || !DateTime.TryParse(date, out var selectedDate))
                {
                    _logger.LogWarning($"Invalid parameters: doctorId={doctorId}, date={date}");
                    return new JsonResult(new { availableSlots = new List<string>(), debug = "Invalid parameters" });
                }

                // Log all available doctors for debugging
                var allDoctors = await _context.DoctorAvailabilities.ToListAsync();
                _logger.LogInformation($"Found {allDoctors.Count} doctor availability records");
                foreach (var doc in allDoctors)
                {
                    _logger.LogInformation($"Doctor ID: {doc.DoctorId}, Available: {doc.IsAvailable}");
                }

                var bookedAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate.Date == selectedDate.Date
                                && a.Status != AppointmentStatus.Cancelled)
                    .Select(a => new { a.AppointmentTime, a.Type })
                    .ToListAsync();

                var dayOfWeek = selectedDate.DayOfWeek;
                _logger.LogInformation($"Selected date: {selectedDate:yyyy-MM-dd}, Day of week: {dayOfWeek}");

                var clinicSchedule = await _context.DoctorAvailabilities.FirstOrDefaultAsync(cs => cs.DoctorId == doctorId);
                int slotDuration = GetConsultationTypeDuration(consultationType);
                _logger.LogInformation($"Consultation duration: {slotDuration} minutes");

                if (clinicSchedule == null)
                {
                    _logger.LogWarning($"DATABASE CHECK: No clinic schedule found for doctor ID '{doctorId}'. Please ensure the DoctorAvailabilities table has a record for this doctor.");
                    var allSchedules = await _context.DoctorAvailabilities.ToListAsync();
                    _logger.LogWarning($"DATABASE CHECK: Found {allSchedules.Count} total records in DoctorAvailabilities table.");

                    // Create a default availability for the doctor - ENABLE WEEKENDS
                    clinicSchedule = new DoctorAvailability
                    {
                        DoctorId = doctorId,
                        IsAvailable = true,
                        Monday = true,
                        Tuesday = true,
                        Wednesday = true,
                        Thursday = true,
                        Friday = true,
                        Saturday = true,  // ENABLE SATURDAY
                        Sunday = true,    // ENABLE SUNDAY
                        StartTime = new TimeSpan(8, 0, 0), // 8:00 AM
                        EndTime = new TimeSpan(17, 0, 0),  // 5:00 PM
                        LastUpdated = DateTime.UtcNow
                    };

                    _context.DoctorAvailabilities.Add(clinicSchedule);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created default availability for doctor {doctorId}");

                    return new JsonResult(new { 
                        availableSlots = new List<string>(),
                        debug = "Created new doctor availability. Please try again." 
                    });
                }
                else
                {
                    _logger.LogInformation($"DATABASE CHECK: Found clinic schedule for doctor ID '{doctorId}'.");
                }

                bool isDoctorAvailableOnDay = dayOfWeek switch
                {
                    DayOfWeek.Monday => clinicSchedule.Monday,
                    DayOfWeek.Tuesday => clinicSchedule.Tuesday,
                    DayOfWeek.Wednesday => clinicSchedule.Wednesday,
                    DayOfWeek.Thursday => clinicSchedule.Thursday,
                    DayOfWeek.Friday => clinicSchedule.Friday,
                    DayOfWeek.Saturday => clinicSchedule.Saturday,
                    DayOfWeek.Sunday => clinicSchedule.Sunday,
                    _ => false
                };

                // Check consultation-type-specific schedule
                var (allowedDays, typeWindows) = GetConsultationTypeSchedule(consultationType);

                _logger.LogInformation($"Doctor availability on {dayOfWeek}: {isDoctorAvailableOnDay}, IsAvailable: {clinicSchedule.IsAvailable}");
                _logger.LogInformation($"Consultation windows: {string.Join(", ", typeWindows.Select(w => $"{w.Start}-{w.End}"))}");
                _logger.LogInformation($"Allowed days for consultation: {string.Join(", ", allowedDays)}");

                if (!clinicSchedule.IsAvailable || !isDoctorAvailableOnDay)
                {
                    _logger.LogWarning($"Doctor {doctorId} is not available on {dayOfWeek}");
                    return new JsonResult(new { 
                        availableSlots = new List<string>(),
                        debug = $"Doctor not available on {dayOfWeek}" 
                    });
                }

                if (!allowedDays.Contains(dayOfWeek))
                {
                    _logger.LogInformation($"Consultation type '{consultationType}' is not offered on {dayOfWeek}.");
                    return new JsonResult(new { 
                        availableSlots = new List<string>(),
                        debug = $"Consultation type '{consultationType}' is not offered on {dayOfWeek}" 
                    });
                }
                
                _logger.LogInformation($"Found clinic schedule: Start={clinicSchedule.StartTime}, End={clinicSchedule.EndTime}, Available={clinicSchedule.IsAvailable}");

                var availableSlotsSet = new HashSet<string>();

                // Intersect doctor availability with consultation windows
                foreach (var window in typeWindows)
                {
                    var rangeStart = clinicSchedule.StartTime > window.Start ? clinicSchedule.StartTime : window.Start;
                    var rangeEnd = clinicSchedule.EndTime < window.End ? clinicSchedule.EndTime : window.End;

                    _logger.LogInformation($"Window range: {rangeStart}-{rangeEnd} (doctor hours: {clinicSchedule.StartTime}-{clinicSchedule.EndTime}, consultation window: {window.Start}-{window.End})");

                    if (rangeStart >= rangeEnd)
                    {
                        _logger.LogWarning($"No overlap between doctor hours and consultation window: {rangeStart} >= {rangeEnd}");
                        continue; // No overlap between doctor hours and consultation window
                    }

                    _logger.LogInformation($"Generating time slots within window {rangeStart} - {rangeEnd} with {slotDuration} minute intervals");

                    var currentTime = rangeStart;
                    int slotsGenerated = 0;
                    int slotsAvailable = 0;
                    while (currentTime.Add(TimeSpan.FromMinutes(slotDuration)) <= rangeEnd)
                    {
                        var slotStart = currentTime;
                        var slotEnd = currentTime.Add(TimeSpan.FromMinutes(slotDuration));
                        slotsGenerated++;

                        // Remove the check for past times on the current day
                        // This allows booking any time slot regardless of current time

                        bool isBooked = bookedAppointments.Any(b =>
                        {
                            var bookedStart = b.AppointmentTime;
                            var bookedDuration = GetConsultationTypeDuration(string.IsNullOrWhiteSpace(b.Type) ? consultationType : b.Type);
                            var bookedEnd = bookedStart.Add(TimeSpan.FromMinutes(bookedDuration));
                            return slotStart < bookedEnd && bookedStart < slotEnd;
                        });

                        if (!isBooked)
                        {
                            // Format the time as "hh:mm AM/PM" instead of "hh:mm"
                            string formattedTime = DateTime.Today.Add(currentTime).ToString("h:mm tt");
                            availableSlotsSet.Add(formattedTime);
                            slotsAvailable++;
                        }

                        currentTime = currentTime.Add(TimeSpan.FromMinutes(slotDuration)); // Move to the next slot
                    }
                    _logger.LogInformation($"Generated {slotsGenerated} slots, {slotsAvailable} available");
                }

                var availableSlots = availableSlotsSet.OrderBy(t => DateTime.Parse(t).TimeOfDay).ToList();

                _logger.LogInformation($"Generated {availableSlots.Count} available time slots: [{string.Join(", ", availableSlots)}]");
                return new JsonResult(new { 
                    availableSlots,
                    debug = availableSlots.Count > 0 ? "Success" : "No time slots available after processing"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching booked time slots");
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> OnGetGetDefaultDoctorAsync()
        {
            try
            {
                _logger.LogInformation("Getting default doctor...");

                // First, try to get doctor from DoctorAvailabilities table
                var doctorAvailability = await _context.DoctorAvailabilities
                    .Where(da => da.IsAvailable)
                    .FirstOrDefaultAsync();

                if (doctorAvailability != null)
                {
                    _logger.LogInformation($"Found doctor from availability: {doctorAvailability.DoctorId}");
                    return new JsonResult(new { doctorId = doctorAvailability.DoctorId });
                }

                // If no availability found, try to get any doctor from users table
                var doctorUsers = await _userManager.GetUsersInRoleAsync("Doctor");
                _logger.LogInformation($"Found {doctorUsers.Count} doctors in role");
                
                if (doctorUsers.Any())
                {
                    var firstDoctor = doctorUsers.First();
                    _logger.LogInformation($"Using first doctor from users: {firstDoctor.Id}");
                    
                    // Create availability record for this doctor
                    var newAvailability = new DoctorAvailability
                    {
                        DoctorId = firstDoctor.Id,
                        IsAvailable = true,
                        Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true, Saturday = true, Sunday = true,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        LastUpdated = DateTime.Now
                    };
                    
                    _context.DoctorAvailabilities.Add(newAvailability);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created availability record for doctor {firstDoctor.Id}");
                    
                    return new JsonResult(new { doctorId = firstDoctor.Id });
                }

                // If still no doctors found, try to find any user with "doctor" in email
                var doctorByEmail = await _context.Users
                    .Where(u => u.Email.Contains("doctor") || u.UserName.Contains("doctor"))
                    .FirstOrDefaultAsync();

                if (doctorByEmail != null)
                {
                    _logger.LogInformation($"Found doctor by email: {doctorByEmail.Id}");
                    
                    // Assign Doctor role if not already assigned
                    if (!await _userManager.IsInRoleAsync(doctorByEmail, "Doctor"))
                    {
                        await _userManager.AddToRoleAsync(doctorByEmail, "Doctor");
                        _logger.LogInformation($"Assigned Doctor role to {doctorByEmail.Email}");
                    }
                    
                    // Create availability record
                    var newAvailability = new DoctorAvailability
                    {
                        DoctorId = doctorByEmail.Id,
                        IsAvailable = true,
                        Monday = true, Tuesday = true, Wednesday = true, Thursday = true, Friday = true, Saturday = true, Sunday = true,
                        StartTime = new TimeSpan(8, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0),
                        LastUpdated = DateTime.Now
                    };
                    
                    _context.DoctorAvailabilities.Add(newAvailability);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created availability record for doctor {doctorByEmail.Id}");
                    
                    return new JsonResult(new { doctorId = doctorByEmail.Id });
                }

                _logger.LogWarning("No doctors found in the system");
                return new JsonResult(new { doctorId = "", error = "No doctors available" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting default doctor");
                return new JsonResult(new { doctorId = "", error = ex.Message });
            }
        }

        private NCDRiskAssessment CreateNCDRiskAssessment(NCDRiskAssessmentViewModel model, int appointmentId)
        {
            return new NCDRiskAssessment
            {
                AppointmentId = appointmentId,
                UserId = model.UserId,
                HealthFacility = model.HealthFacility,
                FamilyNo = model.FamilyNo,
                Address = model.Address,
                Barangay = model.Barangay,
                Birthday = model.Birthday,
                Telepono = model.Telepono,
                Edad = model.Edad,
                Kasarian = model.Kasarian,
                Relihiyon = model.Relihiyon,
                HasDiabetes = model.HasDiabetes,
                HasHypertension = model.HasHypertension,
                HasCancer = model.HasCancer,
                CancerType = model.CancerType,
                HasCOPD = model.HasCOPD,
                HasLungDisease = model.HasLungDisease,
                HasEyeDisease = model.HasEyeDisease,
                FamilyHasHypertension = model.FamilyHasHypertension,
                FamilyHasHeartDisease = model.FamilyHasHeartDisease,
                FamilyHasStroke = model.FamilyHasStroke,
                FamilyHasDiabetes = model.FamilyHasDiabetes,
                FamilyHasCancer = model.FamilyHasCancer,
                FamilyHasKidneyDisease = model.FamilyHasKidneyDisease,
                FamilyHasOtherDisease = model.FamilyHasOtherDisease,
                FamilyOtherDiseaseDetails = model.FamilyOtherDiseaseDetails,
                HighSaltIntake = model.HighSaltIntake,
                AlcoholFrequency = model.AlcoholFrequency,
                AlcoholConsumption = model.AlcoholConsumption,
                ExerciseDuration = model.ExerciseDuration,
                SmokingStatus = model.SmokingStatus,
                AppointmentType = model.AppointmentType
            };
        }

        private HEEADSSSAssessment CreateHEEADSSSAssessment(HEEADSSSAssessmentViewModel model, int appointmentId)
        {
            return new HEEADSSSAssessment
            {
                AppointmentId = appointmentId.ToString(),
                UserId = model.UserId,
                HealthFacility = model.HealthFacility,
                FamilyNo = model.FamilyNo,
                FullName = model.FullName,
                Age = model.Age.ToString(),
                Birthday = model.Birthday,
                Gender = model.Gender,
                Address = model.Address,
                ContactNumber = model.ContactNumber,
                HomeFamilyProblems = model.HomeFamilyProblems,
                HomeParentalListening = model.HomeParentalListening,
                HomeParentalBlame = model.HomeParentalBlame,
                HomeFamilyChanges = model.HomeFamilyChanges,
                EducationCurrentlyStudying = model.EducationCurrentlyStudying,
                EducationWorking = model.EducationWorking,
                EducationSchoolWorkProblems = model.EducationSchoolWorkProblems,
                EducationBullying = model.EducationBullying,
                EatingBodyImageSatisfaction = model.EatingBodyImageSatisfaction,
                EatingDisorderedEatingBehaviors = model.EatingDisorderedEatingBehaviors,
                EatingWeightComments = model.EatingWeightComments,
                ActivitiesParticipation = model.ActivitiesParticipation,
                ActivitiesRegularExercise = model.ActivitiesRegularExercise,
                ActivitiesScreenTime = model.ActivitiesScreenTime,
                DrugsTobaccoUse = model.DrugsTobaccoUse,
                DrugsAlcoholUse = model.DrugsAlcoholUse,
                DrugsIllicitDrugUse = model.DrugsIllicitDrugUse,
                SexualityBodyConcerns = model.SexualityBodyConcerns,
                SexualityIntimateRelationships = model.SexualityIntimateRelationships,
                SexualityPartners = model.SexualityPartners,
                SexualitySexualOrientation = model.SexualitySexualOrientation,
                SexualityPregnancy = model.SexualityPregnancy,
                SexualitySTI = model.SexualitySTI,
                SexualityProtection = model.SexualityProtection,
                SuicideDepressionFeelings = model.SuicideDepressionFeelings,
                SuicideSelfHarmThoughts = model.SuicideSelfHarmThoughts,
                SuicideFamilyHistory = model.SuicideFamilyHistory,
                SafetyPhysicalAbuse = model.SafetyPhysicalAbuse,
                SafetyRelationshipViolence = model.SafetyRelationshipViolence,
                SafetyProtectiveGear = model.SafetyProtectiveGear,
                SafetyGunsAtHome = model.SafetyGunsAtHome,
                Notes = model.Notes,
                RecommendedActions = model.RecommendedActions,
                AssessedBy = model.AssessedBy
            };
        }

        private async Task<JsonResult?> ValidateTimeSlotAsync(AppointmentBookingViewModel bookingModel)
        {
            if (!string.IsNullOrEmpty(bookingModel.AppointmentDate) && !string.IsNullOrEmpty(bookingModel.TimeSlot))
            {
                try
                {
                    DateTime selectedApptDate = DateTime.Parse(bookingModel.AppointmentDate);
                    // Convert from 12-hour format (e.g. "8:00 AM") to TimeSpan
                    TimeSpan selectedApptTime;
                    if (DateTime.TryParse(bookingModel.TimeSlot, out DateTime parsedTime))
                    {
                        selectedApptTime = parsedTime.TimeOfDay;
                        _logger.LogInformation($"Successfully parsed time: {bookingModel.TimeSlot} to {selectedApptTime}");
                    }
                    else
                    {
                        _logger.LogError($"Failed to parse time string: {bookingModel.TimeSlot}");
                        return new JsonResult(new 
                        { 
                            success = false, 
                            error = "Invalid time format. Please select a valid time.", 
                            errorType = "ValidationError" 
                        });
                    }
                    string selectedConsultationType = bookingModel.ConsultationType ?? "medical";

                    // Check for conflicts with existing appointments
                    var existingAppointments = await _context.Appointments
                        .Where(a => a.AppointmentDate.Date == selectedApptDate.Date &&
                                        a.Status != AppointmentStatus.Cancelled)
                        .Select(a => new { a.AppointmentTime, a.Type })
                        .ToListAsync();

                    foreach (var existing in existingAppointments)
                    {
                        int existingBuffer = GetConsultationTypeDuration(existing.Type);
                        int newAppointmentBuffer = GetConsultationTypeDuration(selectedConsultationType);
                        double timeDifference = Math.Abs((existing.AppointmentTime - selectedApptTime).TotalMinutes);

                        if (timeDifference < (existingBuffer + newAppointmentBuffer) / 2 &&
                            (existing.Type.Equals(selectedConsultationType, StringComparison.OrdinalIgnoreCase) ||
                             existingBuffer + newAppointmentBuffer >= 30))
                        {
                            _logger.LogWarning($"Time slot conflict detected: {selectedApptTime} conflicts with existing appointment at {existing.AppointmentTime}");
                            return new JsonResult(new
                            {
                                success = false,
                                error = "This time slot has already been booked. Please select a different time.",
                                errorType = "TimeSlotConflict"
                            });
                        }
                    }

                    // Also check ConsultationTimeSlots table
                    var bookedTimeSlots = await _context.ConsultationTimeSlots
                        .Where(cts => cts.StartTime.Date == selectedApptDate.Date && cts.IsBooked)
                        .Select(cts => new { Time = cts.StartTime.TimeOfDay, cts.ConsultationType })
                        .ToListAsync();

                    foreach (var booked in bookedTimeSlots)
                    {
                        int bookedBuffer = GetConsultationTypeDuration(booked.ConsultationType);
                        int newAppointmentBuffer = GetConsultationTypeDuration(selectedConsultationType);
                        double timeDifference = Math.Abs((booked.Time - selectedApptTime).TotalMinutes);

                        if (timeDifference < (bookedBuffer + newAppointmentBuffer) / 2 &&
                            (booked.ConsultationType.Equals(selectedConsultationType, StringComparison.OrdinalIgnoreCase) ||
                             bookedBuffer + newAppointmentBuffer >= 30))
                        {
                            _logger.LogWarning($"Time slot conflict detected: {selectedApptTime} conflicts with booked time slot at {booked.Time}");
                            return new JsonResult(new
                            {
                                success = false,
                                error = "This time slot has already been booked. Please select a different time.",
                                errorType = "TimeSlotConflict"
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking time slot availability");
                    return new JsonResult(new
                    {
                        success = false,
                        error = "Error checking time slot availability. Please try again.",
                        errorType = "ValidationError"
                    });
                }
            }
            return null;
        }
    }

    public class UserDetailsViewModel
    {
        public string FullName { get; set; }
        public int Age { get; set; }
    }

    public class AppointmentBookingViewModel
    {
        public int CurrentStep { get; set; } = 1;
        public int? AppointmentId { get; set; }
        public string DoctorId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? AppointmentDate { get; set; }
        public string? TimeSlot { get; set; }
        public string? ConsultationType { get; set; }
        public string? ReasonForVisit { get; set; }
        public string? Symptoms { get; set; }
        public string? HealthFacilityId { get; set; }
        public bool BookingForOther { get; set; }
        public string? Relationship { get; set; }
        public bool HasFamilyNumber { get; set; }
        public string? FamilyNumber { get; set; }
        public decimal? Temperature { get; set; }
        public string? BloodPressure { get; set; }
        public int? PulseRate { get; set; }
        public int? SelectedTimeSlotId { get; set; }
        public List<ConsultationTimeSlot> AvailableTimeSlots { get; set; } = new List<ConsultationTimeSlot>();
    }


    public class HEEADSSSAssessmentViewModel
    {
        public string? UserId { get; set; }
        public string? HealthFacility { get; set; }
        public string? FamilyNo { get; set; }
        public int? AppointmentId { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? ContactNumber { get; set; }
        public string? HomeFamilyProblems { get; set; }
        public string? HomeParentalListening { get; set; }
        public string? HomeParentalBlame { get; set; }
        public string? HomeFamilyChanges { get; set; }
        public string? EducationCurrentlyStudying { get; set; }
        public string? EducationWorking { get; set; }
        public string? EducationSchoolWorkProblems { get; set; }
        public string? EducationBullying { get; set; }
        public string? EatingBodyImageSatisfaction { get; set; }
        public string? EatingDisorderedEatingBehaviors { get; set; } //eating disorders
        public string? EatingWeightComments { get; set; }
        public string? ActivitiesParticipation { get; set; }
        public string? ActivitiesRegularExercise { get; set; }
        public string? ActivitiesScreenTime { get; set; }
        public string? DrugsTobaccoUse { get; set; }
        public string? DrugsAlcoholUse { get; set; }
        public string? DrugsIllicitDrugUse { get; set; }
        public string? SexualityBodyConcerns { get; set; }
        public string? SexualityIntimateRelationships { get; set; }
        public string? SexualityPartners { get; set; }
        public string? SexualitySexualOrientation { get; set; }
        public string? SexualityPregnancy { get; set; }
        public string? SexualitySTI { get; set; }
        public string? SexualityProtection { get; set; }
        public string? SuicideDepressionFeelings { get; set; }
        public string? SuicideSelfHarmThoughts { get; set; }
        public string? SuicideFamilyHistory { get; set; }
        public string? SafetyPhysicalAbuse { get; set; }
        public string? SafetyRelationshipViolence { get; set; }
        public string? SafetyProtectiveGear { get; set; }
        public string? SafetyGunsAtHome { get; set; }
        public string? Notes { get; set; }
        public string? RecommendedActions { get; set; }
        public string? AssessedBy { get; set; }
    }
}