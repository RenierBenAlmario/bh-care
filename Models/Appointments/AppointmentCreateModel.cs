using System;
using System.ComponentModel.DataAnnotations;
using Barangay.Models.Validation;

namespace Barangay.Models.Appointments
{
    public class AppointmentCreateModel
    {
        [Required(ErrorMessage = "Doctor/Staff is required")]
        public string StaffId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "Appointment date must be in the future")]
        public string AppointmentDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Appointment time is required")]
        [DataType(DataType.Time)]
        public string AppointmentTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public bool IsForDependent { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Dependent name is required when booking for a dependent")]
        [StringLength(100, ErrorMessage = "Dependent name cannot exceed 100 characters")]
        public string? DependentName { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Dependent age is required when booking for a dependent")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int? DependentAge { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Relationship to dependent is required when booking for a dependent")]
        [StringLength(50, ErrorMessage = "Relationship cannot exceed 50 characters")]
        public string? RelationshipToDependent { get; set; }

        public bool ValidateDateTime()
        {
            if (!DateTime.TryParse(AppointmentDate, out DateTime date))
                return false;

            if (!TimeSpan.TryParse(AppointmentTime, out TimeSpan time))
                return false;

            var appointmentDateTime = date.Add(time);
            return appointmentDateTime > DateTime.Now;
        }

        public bool Validate()
        {
            if (IsForDependent)
            {
                if (string.IsNullOrEmpty(DependentName))
                    return false;
                if (!DependentAge.HasValue)
                    return false;
                if (string.IsNullOrEmpty(RelationshipToDependent))
                    return false;
            }

            if (string.IsNullOrEmpty(StaffId) || string.IsNullOrEmpty(AppointmentDate) || 
                string.IsNullOrEmpty(AppointmentTime) || string.IsNullOrEmpty(Description))
                return false;

            return ValidateDateTime();
        }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return false;

            if (!DateTime.TryParse(value.ToString(), out DateTime date))
                return false;

            return date.Date >= DateTime.Today;
        }
    }
} 