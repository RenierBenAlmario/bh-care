using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using Barangay.Extensions;
using Barangay.Helpers;

namespace Barangay.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(450)]
        public string PatientId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(450)]
        public string DoctorId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PatientName { get; set; } = string.Empty;
        
        // Dependent-related properties
        [StringLength(100)]
        public string? DependentFullName { get; set; }
        public int? DependentAge { get; set; }
        [StringLength(50)]
        public string? RelationshipToDependent { get; set; }
        
        // Basic Info
        [StringLength(10)]
        public string? Gender { get; set; }
        [StringLength(20)]
        public string? ContactNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [StringLength(200)]
        public string? Address { get; set; }
        
        // Emergency Contact
        [StringLength(100)]
        public string? EmergencyContact { get; set; }
        [StringLength(20)]
        public string? EmergencyContactNumber { get; set; }
        
        // Medical Info
        [StringLength(500)]
        public string? Allergies { get; set; }
        [StringLength(1000)]
        public string? MedicalHistory { get; set; }
        [StringLength(500)]
        public string? CurrentMedications { get; set; }
        public string? AttachmentsData { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }
        
        [Required]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentTimeInput { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string ReasonForVisit { get; set; } = string.Empty;
        
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        
        public int AgeValue { get; set; }
        
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(50)]
        public string? Type { get; set; }
        
        [StringLength(500)]
        public string? AttachmentPath { get; set; }
        
        [StringLength(1000)]
        public string? Prescription { get; set; }
        
        [StringLength(1000)]
        public string? Instructions { get; set; }
        
        // Added ApplicationUserId property to match database schema
        [StringLength(450)]
        public string? ApplicationUserId { get; set; }
        
        // Navigation properties - Updated for Patient to use the Patient model
        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; } = null!;
        
        [ForeignKey(nameof(DoctorId))]
        [InverseProperty("DoctorAppointments")]
        public virtual ApplicationUser Doctor { get; set; } = null!;
        
        // Navigation properties for attachments and files
        public virtual ICollection<AppointmentAttachment> Attachments { get; set; } = new List<AppointmentAttachment>();
        public virtual ICollection<AppointmentFile> Files { get; set; } = new List<AppointmentFile>();

        // Helper methods
        public string GetFormattedTime()
        {
            return DateTimeHelper.FormatTime(AppointmentTime);
        }

        public string GetFormattedDate()
        {
            return DateTimeHelper.FormatDate(AppointmentDate);
        }

        public string GetFormattedDateTime()
        {
            return DateTimeHelper.GetFormattedDateTime(AppointmentDate, AppointmentTime);
        }

        public bool IsDateEqual(DateTime date)
        {
            return DateTimeHelper.AreDatesEqual(AppointmentDate, date);
        }

        public bool IsDateBefore(DateTime date)
        {
            return DateTimeHelper.IsDateBefore(AppointmentDate, date);
        }

        public bool IsDateAfter(DateTime date)
        {
            return DateTimeHelper.IsDateAfter(AppointmentDate, date);
        }

        // Helper property for age
        public string Age => AgeValue.ToString();

        // Helper method to get DateTime from appointment date and time
        public DateTime GetAppointmentDateTime()
        {
            return AppointmentDate.Date.Add(AppointmentTime);
        }

        // Helper method to set date and time from DateTime
        public void SetAppointmentDateTime(DateTime dateTime)
        {
            AppointmentDate = dateTime.Date;
            AppointmentTime = dateTime.TimeOfDay;
        }

        // Helper method to compare dates
        public bool IsDateGreaterThanOrEqual(DateTime date)
        {
            return DateTimeHelper.IsDateGreaterThanOrEqual(AppointmentDate, date);
        }

        public bool IsDateLessThanOrEqual(DateTime date)
        {
            return DateTimeHelper.IsDateLessThanOrEqual(AppointmentDate, date);
        }

        public bool IsTimeBefore(TimeSpan time)
        {
            return AppointmentTime < time;
        }

        public bool IsTimeAfter(TimeSpan time)
        {
            return AppointmentTime > time;
        }

        public bool IsTimeEqual(TimeSpan time)
        {
            return AppointmentTime == time;
        }
    }
}
