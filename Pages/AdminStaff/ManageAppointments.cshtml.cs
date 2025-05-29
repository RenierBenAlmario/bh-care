using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Barangay.Data;
using Barangay.Models;
using Barangay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Barangay.Pages.AdminStaff
{
    [Authorize(Roles = "Admin Staff")]
    [Authorize(Policy = "AccessAdminDashboard")]
    public class ManageAppointmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ManageAppointmentsModel> _logger;

        public ManageAppointmentsModel(
            ApplicationDbContext context,
            ILogger<ManageAppointmentsModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string DateFilter { get; set; } = "all";

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; } = "all";

        [BindProperty(SupportsGet = true)]
        public string DoctorFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public int StartRecord { get; set; }
        public int EndRecord { get; set; }
        public int CurrentPage => PageNumber;

        public List<SelectListItem> DoctorsList { get; set; }
        public List<AdminAppointmentViewModel> Appointments { get; set; } = new List<AdminAppointmentViewModel>();
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public List<Doctor> Doctors { get; set; } = new List<Doctor>();
        public List<string> TimeSlots { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Initialize page parameters
            if (PageNumber < 1) PageNumber = 1;

            // Load doctors for dropdown
            await LoadDoctorsAsync();

            // Load patients for modal
            await LoadPatientsAsync();

            // Generate time slots in 30-minute intervals
            GenerateTimeSlots();

            // Load appointments with filtering
            await LoadAppointmentsAsync();

            return Page();
        }

        private void GenerateTimeSlots()
        {
            // Generate 30-minute time slots from 8 AM to 5 PM
            DateTime startTime = DateTime.Today.AddHours(8); // 8:00 AM
            DateTime endTime = DateTime.Today.AddHours(17).AddMinutes(30); // 5:30 PM

            while (startTime < endTime)
            {
                TimeSlots.Add(startTime.ToString("h:mm tt"));
                startTime = startTime.AddMinutes(30);
            }
        }

        private async Task LoadDoctorsAsync()
        {
            // Get all doctors from Doctors table
            var doctors = await _context.Doctors
                .OrderBy(d => d.Name)
                .ToListAsync();

            // Store doctors for appointment creation
            Doctors = doctors.Select(d => new Doctor
            {
                Id = d.Id.ToString(),
                FullName = d.Name,
                Department = d.Specialization ?? "General"
            }).ToList();

            // Create select list for filter dropdown
            DoctorsList = doctors.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = $"Dr. {d.Name}"
            }).ToList();
        }

        private async Task LoadPatientsAsync()
        {
            // Get all active patients
            Patients = await _context.Patients
                .OrderBy(p => p.FullName)
                .ToListAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            // Build query with filtering
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .AsQueryable();

            // Apply date filter
            switch (DateFilter?.ToLower())
            {
                case "today":
                    var today = DateTime.Today;
                    query = query.Where(a => a.AppointmentDate.Date == today);
                    break;
                case "tomorrow":
                    var tomorrow = DateTime.Today.AddDays(1);
                    query = query.Where(a => a.AppointmentDate.Date == tomorrow);
                    break;
                case "week":
                    var startOfWeek = GetStartOfWeek(DateTime.Today);
                    var endOfWeek = startOfWeek.AddDays(6);
                    query = query.Where(a => a.AppointmentDate.Date >= startOfWeek && a.AppointmentDate.Date <= endOfWeek);
                    break;
                case "month":
                    var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                    query = query.Where(a => a.AppointmentDate.Date >= startOfMonth && a.AppointmentDate.Date <= endOfMonth);
                    break;
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter.ToLower() != "all")
            {
                AppointmentStatus status;
                switch (StatusFilter.ToLower())
                {
                    case "pending":
                        status = AppointmentStatus.Pending;
                        break;
                    case "confirmed":
                        status = AppointmentStatus.Confirmed;
                        break;
                    case "completed":
                        status = AppointmentStatus.Completed;
                        break;
                    case "cancelled":
                        status = AppointmentStatus.Cancelled;
                        break;
                    default:
                        status = AppointmentStatus.Pending;
                        break;
                }
                query = query.Where(a => a.Status == status);
            }

            // Apply doctor filter
            if (!string.IsNullOrEmpty(DoctorFilter))
            {
                query = query.Where(a => a.DoctorId == DoctorFilter);
            }

            // Apply search term
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(a =>
                    a.Patient.FullName.Contains(SearchTerm) ||
                    a.Patient.ContactNumber.Contains(SearchTerm));
            }

            // Count total records
            TotalRecords = await query.CountAsync();

            // Calculate pagination
            TotalPages = (int)Math.Ceiling(TotalRecords / (double)PageSize);
            StartRecord = (PageNumber - 1) * PageSize + 1;
            EndRecord = Math.Min(StartRecord + PageSize - 1, TotalRecords);

            // Apply ordering and pagination
            var appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Map to view model
            Appointments = appointments.Select(a => new AdminAppointmentViewModel
            {
                Id = a.Id,
                PatientId = int.Parse(a.PatientId),
                PatientName = a.Patient.FullName,
                PatientContact = a.Patient.ContactNumber,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                Department = a.Doctor.Specialization ?? "General",
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = DateTime.Today.Add(a.AppointmentTime),
                AppointmentType = a.Type,
                Status = a.Status.ToString(),
                Notes = a.Description ?? ""
            }).ToList();
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            // Assuming week starts on Monday
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        public async Task<IActionResult> OnGetConfirmAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                ErrorMessage = "Appointment not found.";
                return RedirectToPage();
            }

            appointment.Status = AppointmentStatus.Confirmed;
            await _context.SaveChangesAsync();
            SuccessMessage = "Appointment confirmed successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCompleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                ErrorMessage = "Appointment not found.";
                return RedirectToPage();
            }

            appointment.Status = AppointmentStatus.Completed;
            await _context.SaveChangesAsync();
            SuccessMessage = "Appointment marked as completed.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetCancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                ErrorMessage = "Appointment not found.";
                return RedirectToPage();
            }

            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();
            SuccessMessage = "Appointment cancelled successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDeleteAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                ErrorMessage = "Appointment not found.";
                return RedirectToPage();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            SuccessMessage = "Appointment deleted successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync(int patientId, string doctorId, DateTime appointmentDate, 
            string appointmentTime, string appointmentType, string status, string notes)
        {
            try
            {
                // Parse time string to TimeSpan
                if (!TimeSpan.TryParse(appointmentTime, out TimeSpan timeSpan))
                {
                    // Try alternative parsing for AM/PM format
                    DateTime tempTime;
                    if (DateTime.TryParse(appointmentTime, out tempTime))
                    {
                        timeSpan = tempTime.TimeOfDay;
                    }
                    else
                    {
                        ErrorMessage = "Invalid time format.";
                        return RedirectToPage();
                    }
                }

                // Parse status string to enum
                AppointmentStatus appointmentStatus;
                switch (status.ToLower())
                {
                    case "confirmed":
                        appointmentStatus = AppointmentStatus.Confirmed;
                        break;
                    default:
                        appointmentStatus = AppointmentStatus.Pending;
                        break;
                }

                // Create new appointment
                var appointment = new Models.Appointment
                {
                    PatientId = patientId.ToString(),
                    DoctorId = doctorId,
                    AppointmentDate = appointmentDate.Date,
                    AppointmentTime = timeSpan,
                    Type = appointmentType,
                    Status = appointmentStatus,
                    Description = notes,
                    CreatedAt = DateTime.Now
                };

                await _context.Appointments.AddAsync(appointment);
                await _context.SaveChangesAsync();

                SuccessMessage = "Appointment created successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                ErrorMessage = "Error creating appointment: " + ex.Message;
            }

            return RedirectToPage();
        }
    }

    public class AdminAppointmentViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientContact { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Department { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string AppointmentType { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
    
    public class Doctor
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
    }
} 