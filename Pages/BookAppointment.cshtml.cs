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

        public BookAppointmentModel(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment,
            ILogger<BookAppointmentModel> logger,
            IDatabaseDebugService dbDebugService)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _dbDebugService = dbDebugService;
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

        [BindProperty]
        public bool BookingSuccess { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
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
                    // Set ViewBag data for JavaScript
                    ViewData["UserDetails"] = new
                    {
                        FullName = user.FullName ?? $"{user.FirstName} {user.LastName}".Trim(),
                        Age = CalculateAge(user.BirthDate)
                    };

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
                    if (user.BirthDate != default(DateTime))
                    {
                        BookingModel.DateOfBirth = user.BirthDate;
                        NCDModel.Birthday = user.BirthDate;
                        BookingModel.Age = CalculateAge(user.BirthDate);
                    }
                    
                    // Pre-fill phone number if available
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        BookingModel.PhoneNumber = user.PhoneNumber;
                        NCDModel.Telepono = user.PhoneNumber;
                    }
                }

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
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Generate Health Facility ID
                string healthFacilityId = GenerateHealthFacilityId();
                
                // Generate Family Number
                string familyNumber = await GenerateFamilyNumberAsync(BookingModel.LastName);
                
                // Save the booking information
                var appointment = await SaveAppointmentAsync(healthFacilityId, familyNumber);
                
                // Set success flag and message
                BookingSuccess = true;
                StatusMessage = "Your appointment has been successfully booked!";
                
                // Check if we need to redirect to an assessment form
                var redirectToAssessment = Request.Form["redirectToAssessment"].ToString();
                if (!string.IsNullOrEmpty(redirectToAssessment) && redirectToAssessment == "true")
                {
                    // Determine which assessment to redirect to based on age range
                    var ageRange = Request.Form["ageRange"].ToString();
                    if (ageRange == "adolescent")
                    {
                        // Redirect to HEEADSSS Assessment for ages 10-19
                        return RedirectToPage("/User/HEEADSSSAssessment", new { appointmentId = appointment.Id });
                    }
                    else if (ageRange == "adult")
                    {
                        // Redirect to NCD Risk Assessment for ages 20+
                        return RedirectToPage("/User/NCDRiskAssessment", new { appointmentId = appointment.Id });
                    }
                }
                
                return RedirectToPage("/User/UserDashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostAsync: {ErrorMessage}", ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred while booking your appointment. Please try again later.");
                return Page();
            }
        }

        private async Task<Appointment> SaveAppointmentAsync(string healthFacilityId, string familyNumber)
        {
            // Get current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Make sure BookingModel is populated with form data
            if (string.IsNullOrEmpty(BookingModel.FirstName) && Request.Form.ContainsKey("fullName"))
            {
                var fullName = Request.Form["fullName"].ToString();
                var nameParts = fullName.Split(' ');
                if (nameParts.Length > 1)
                {
                    BookingModel.FirstName = nameParts[0];
                    BookingModel.LastName = nameParts[nameParts.Length - 1];
                }
                else
                {
                    BookingModel.FirstName = fullName;
                    BookingModel.LastName = "";
                }
            }
            
            // Create a new appointment record
            var appointment = new Appointment
            {
                PatientId = user.Id,
                PatientName = Request.Form.ContainsKey("fullName") ? 
                    Request.Form["fullName"].ToString() : 
                    $"{BookingModel.FirstName} {BookingModel.LastName}",
                AppointmentDate = DateTime.Parse(Request.Form["appointmentDate"]),
                Type = Request.Form["consultationType"],
                AppointmentTimeInput = Request.Form["timeSlot"],
                AppointmentTime = TimeSpan.Parse(Request.Form["timeSlot"]),
                Description = Request.Form["reasonForVisit"],
                AgeValue = int.Parse(Request.Form["age"]),  // Convert string to int since database field is int
                ContactNumber = Request.Form["phoneNumber"],
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DependentFullName = Request.Form["bookingForOther"] == "on" ? Request.Form["fullName"].ToString() : null,
                RelationshipToDependent = Request.Form["relationship"]
                // HealthFacilityId and FamilyNumber are not properties of the Appointment model
            };
            
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            return appointment;
        }

        private string GenerateHealthFacilityId()
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            var random = new Random();
            
            string facilityId = "HF-";
            
            // Add 3 random letters
            for (int i = 0; i < 3; i++)
            {
                facilityId += letters[random.Next(letters.Length)];
            }
            
            // Add 3 random numbers
            for (int i = 0; i < 3; i++)
            {
                facilityId += numbers[random.Next(numbers.Length)];
            }
            
            return facilityId;
        }

        private async Task<string> GenerateFamilyNumberAsync(string lastName)
        {
            if (string.IsNullOrEmpty(lastName)) return string.Empty;
            
            // First letter of surname
            char firstLetter = char.ToUpper(lastName[0]);
            
            // Check existing family numbers with this prefix to get max sequence number
            var existingNumbers = await _context.FamilyRecords
                .Where(f => f.FamilyNumber.StartsWith(firstLetter.ToString()))
                .Select(f => f.FamilyNumber)
                .ToListAsync();
            
            int maxSequenceNumber = 0;
            
            foreach (var number in existingNumbers)
            {
                if (number.Length > 1 && int.TryParse(number.Substring(1), out int sequenceNumber))
                {
                    maxSequenceNumber = Math.Max(maxSequenceNumber, sequenceNumber);
                }
            }
            
            // Create new family number with incremented sequence - format: P001, P002, etc.
            string newFamilyNumber = $"{firstLetter}{(maxSequenceNumber + 1):000}";
            
            return newFamilyNumber;
        }

        public async Task<IActionResult> OnPostNextStepAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                // Process data based on the current step
                switch (BookingModel.CurrentStep)
                {
                    case 1: // Consultation Type step
                        if (string.IsNullOrEmpty(BookingModel.ConsultationType))
                        {
                            ModelState.AddModelError("BookingModel.ConsultationType", "Please select a consultation type");
                            return Page();
                        }
                        break;
                        
                    case 2: // Family Number step
                        if (BookingModel.HasFamilyNumber)
                        {
                            if (string.IsNullOrEmpty(BookingModel.FamilyNumber))
                            {
                                ModelState.AddModelError("BookingModel.FamilyNumber", "Please enter your family number");
                                return Page();
                            }
                            
                            // Validate family number against database
                            var familyRecord = await _context.FamilyRecords
                                .FirstOrDefaultAsync(fr => fr.FamilyNumber == BookingModel.FamilyNumber);
                            
                            if (familyRecord == null)
                            {
                                ModelState.AddModelError("BookingModel.FamilyNumber", "Family number not found");
                                return Page();
                            }
                        }
                        else
                        {
                            // Validate personal details if no family number
                            if (string.IsNullOrEmpty(BookingModel.FirstName))
                            {
                                ModelState.AddModelError("BookingModel.FirstName", "Please enter your first name");
                                return Page();
                            }
                            
                            if (string.IsNullOrEmpty(BookingModel.LastName))
                            {
                                ModelState.AddModelError("BookingModel.LastName", "Please enter your last name");
                                return Page();
                            }
                            
                            if (BookingModel.DateOfBirth == default(DateTime))
                            {
                                ModelState.AddModelError("BookingModel.DateOfBirth", "Please enter your date of birth");
                                return Page();
                            }
                            
                            if (string.IsNullOrEmpty(BookingModel.Address))
                            {
                                ModelState.AddModelError("BookingModel.Address", "Please enter your address");
                                return Page();
                            }
                            
                            // Generate a new family number and create record
                            var newFamilyNumber = await GenerateUniqueFamilyNumberAsync();
                            
                            var newFamilyRecord = new FamilyRecord
                            {
                                FamilyNumber = newFamilyNumber,
                                FirstName = BookingModel.FirstName,
                                LastName = BookingModel.LastName,
                                DateOfBirth = BookingModel.DateOfBirth,
                                Address = BookingModel.Address,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            };
                            
                            _context.FamilyRecords.Add(newFamilyRecord);
                            await _context.SaveChangesAsync();
                            
                            // Update the booking model with the new family number
                            BookingModel.FamilyNumber = newFamilyNumber;
                            
                            // Associate with the current user (without setting FamilyNumber)
                            var user = await _userManager.GetUserAsync(User);
                            if (user != null)
                            {
                                var familyMember = new FamilyMember
                                {
                                    UserId = user.Id,
                                    PatientId = user.Id, // Set PatientId to UserId as a default
                                    Name = $"{BookingModel.FirstName} {BookingModel.LastName}",
                                    Relationship = "Self",  // Default to self
                                    Age = CalculateAge(BookingModel.DateOfBirth)
                                };

                                _context.FamilyMembers.Add(familyMember);
                                await _context.SaveChangesAsync();
                            }
                        }
                        break;
                        
                    case 3: // Vital Signs step
                        if (BookingModel.Temperature == null)
                        {
                            ModelState.AddModelError("BookingModel.Temperature", "Please enter your temperature");
                            return Page();
                        }
                        
                        if (string.IsNullOrEmpty(BookingModel.BloodPressure))
                        {
                            ModelState.AddModelError("BookingModel.BloodPressure", "Please enter your blood pressure");
                            return Page();
                        }
                        
                        if (BookingModel.PulseRate == null)
                        {
                            ModelState.AddModelError("BookingModel.PulseRate", "Please enter your pulse rate");
                            return Page();
                        }
                        break;
                        
                    case 4: // Assessment Form step
                        if (string.IsNullOrEmpty(BookingModel.ReasonForVisit))
                        {
                            ModelState.AddModelError("BookingModel.ReasonForVisit", "Please enter your reason for visit");
                            return Page();
                        }
                        
                        if (string.IsNullOrEmpty(BookingModel.Symptoms))
                        {
                            ModelState.AddModelError("BookingModel.Symptoms", "Please enter your symptoms");
                            return Page();
                        }
                        
                        // Load available consultation time slots for the selected consultation type
                        try
                        {
                        BookingModel.AvailableTimeSlots = await GetAvailableTimeSlotsAsync(BookingModel.ConsultationType);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error loading time slots");
                            ModelState.AddModelError(string.Empty, "Unable to load available time slots. Please try again.");
                            return Page();
                        }
                        break;
                        
                    case 5: // Final step
                        if (BookingModel.SelectedTimeSlotId == null)
                        {
                            ModelState.AddModelError("BookingModel.SelectedTimeSlotId", "Please select a consultation time");
                            
                            // Reload available time slots
                            try
                            {
                            BookingModel.AvailableTimeSlots = await GetAvailableTimeSlotsAsync(BookingModel.ConsultationType);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error reloading time slots");
                                ModelState.AddModelError(string.Empty, "Unable to load available time slots. Please try again.");
                            }
                            return Page();
                        }
                        
                        // Store all the information in session or database
                        try
                        {
                        await SaveBookingInformationAsync();
                        StatusMessage = "Appointment booking processed successfully!";
                        return RedirectToPage("/User/UserDashboard");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error saving booking information");
                            ModelState.AddModelError(string.Empty, ex.Message);
                            
                            // Reload available time slots
                            try
                            {
                                BookingModel.AvailableTimeSlots = await GetAvailableTimeSlotsAsync(BookingModel.ConsultationType);
                            }
                            catch (Exception slotEx)
                            {
                                _logger.LogError(slotEx, "Error reloading time slots after save failure");
                            }
                            return Page();
                        }
                }

                // Move to the next step
                BookingModel.CurrentStep++;
                
                // Handle time slot loading for step 5
                if (BookingModel.CurrentStep == 5)
                {
                    try
                {
                    BookingModel.AvailableTimeSlots = await GetAvailableTimeSlotsAsync(BookingModel.ConsultationType);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error loading time slots for step 5");
                        ModelState.AddModelError(string.Empty, "Unable to load available time slots. Please try again.");
                        BookingModel.CurrentStep--;
                        return Page();
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnPostNextStepAsync");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                BookingModel.CurrentStep = Math.Max(1, BookingModel.CurrentStep);
                return Page();
            }
        }

        public IActionResult OnPostPreviousStepAsync()
        {
            // Move to the previous step if not on the first step
            if (BookingModel.CurrentStep > 1)
            {
                BookingModel.CurrentStep--;
            }

            return Page();
        }

        private async Task<string> GenerateUniqueFamilyNumberAsync()
        {
            string prefix = "F";
            string dateCode = DateTime.Now.ToString("yyMMdd");
            
            // Get the count of records created today to use as a sequence number
            int sequenceNumber = await _context.FamilyRecords
                .CountAsync(fr => fr.CreatedAt.Date == DateTime.Today) + 1;
            
            return $"{prefix}{dateCode}{sequenceNumber:D4}";
        }

        private async Task<List<ConsultationTimeSlot>> GetAvailableTimeSlotsAsync(string consultationType)
        {
            // Check if there are saved time slots for the consultation type
            var existingSlots = await _context.ConsultationTimeSlots
                .Where(cts => cts.ConsultationType == consultationType && !cts.IsBooked)
                .ToListAsync();
            
            // If no existing slots, generate some sample time slots
            if (!existingSlots.Any())
            {
                var sampleSlots = GenerateSampleTimeSlots(consultationType);
                await _context.ConsultationTimeSlots.AddRangeAsync(sampleSlots);
                await _context.SaveChangesAsync();
                return sampleSlots;
            }
            
            return existingSlots;
        }

        private List<ConsultationTimeSlot> GenerateSampleTimeSlots(string consultationType)
        {
            var slots = new List<ConsultationTimeSlot>();
            var today = DateTime.Today;
            
            // Generate slots from 9 AM to 4 PM with 30-minute intervals for the next 5 days
            for (int day = 0; day < 5; day++)
            {
                var currentDate = today.AddDays(day);
                
                // Skip weekends
                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                
                for (int hour = 9; hour < 16; hour++)
                {
                    for (int minute = 0; minute < 60; minute += 30)
                    {
                        slots.Add(new ConsultationTimeSlot
                        {
                            ConsultationType = consultationType,
                            StartTime = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, hour, minute, 0),
                            IsBooked = false,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
            
            return slots;
        }

        private async Task SaveBookingInformationAsync()
            {
                // Get the current user
                var userId = (await _userManager.GetUserAsync(User))?.Id;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID is null or empty when saving booking information");
                throw new InvalidOperationException("User not found");
                }

                // Save vital signs
            var saveVitalSignsResult = await _dbDebugService.TryDatabaseOperation(async () =>
            {
                var vitalSign = new VitalSign
                {
                    PatientId = userId,
                    Temperature = BookingModel.Temperature,
                    BloodPressure = BookingModel.BloodPressure,
                    HeartRate = BookingModel.PulseRate,
                    RecordedAt = DateTime.Now
                };
                _context.VitalSigns.Add(vitalSign);
            }, "Saving vital signs");

            if (!saveVitalSignsResult.success)
            {
                throw new Exception($"Failed to save vital signs: {saveVitalSignsResult.message}");
            }
                
                // Save assessment
            var saveAssessmentResult = await _dbDebugService.TryDatabaseOperation(async () =>
            {
                var assessment = new Assessment
                {
                    FamilyNumber = BookingModel.FamilyNumber ?? "Unknown",
                    ReasonForVisit = BookingModel.ReasonForVisit,
                    Symptoms = BookingModel.Symptoms,
                    CreatedAt = DateTime.Now
                };
                _context.Assessments.Add(assessment);
            }, "Saving assessment");

            if (!saveAssessmentResult.success)
            {
                throw new Exception($"Failed to save assessment: {saveAssessmentResult.message}");
            }
                
                // Mark the selected time slot as booked
                if (BookingModel.SelectedTimeSlotId.HasValue)
            {
                var updateTimeSlotResult = await _dbDebugService.TryDatabaseOperation(async () =>
                {
                    var selectedSlot = await _context.ConsultationTimeSlots
                        .FirstOrDefaultAsync(cts => cts.Id == BookingModel.SelectedTimeSlotId);
                    
                    if (selectedSlot != null)
                    {
                        selectedSlot.IsBooked = true;
                    }
                    else
                    {
                        throw new InvalidOperationException("Selected time slot not found");
                    }
                }, "Updating time slot booking status");

                if (!updateTimeSlotResult.success)
                {
                    throw new Exception($"Failed to update time slot: {updateTimeSlotResult.message}");
                }
            }
        }

        private async Task LoadDoctorsAsync()
        {
            var staffMembers = await _context.StaffMembers
                .Where(s => s.Role == "Doctor" && s.IsActive)
                .Select(s => new
                {
                    UserId = s.UserId,
                    Name = s.Name,
                    Department = s.Department,
                    Specialization = s.Specialization
                })
                .ToListAsync();
                
            Doctors = staffMembers.Select(s => new Barangay.Models.Doctor 
            {
                UserId = s.UserId,
                Name = s.Name,
                Specialization = s.Specialization ?? "Not Specified"
            }).ToList();
        }

        private bool IsTimeSlotAvailable(string doctorId, DateTime date, TimeSpan time)
        {
            // Replace int with string if you're working with string values
            string workingHours = "9:00-17:00"; // Get this from doctor's profile
            
            // Parse working hours properly
            string[] hours = workingHours.Split('-');
            TimeSpan startTime = TimeSpan.Parse(hours[0]);
            TimeSpan endTime = TimeSpan.Parse(hours[1]);
            
            return time >= startTime && time <= endTime;
        }

        // Helper method to calculate age from date of birth
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
