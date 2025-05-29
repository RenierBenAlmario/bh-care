using System;
using System.ComponentModel.DataAnnotations;
using Barangay.Models.Validation;

namespace Barangay.Models.Appointments
{
    public class AppointmentUpdateModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor/Staff is required")]
        public string StaffId { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        public string AppointmentDate { get; set; }

        [Required(ErrorMessage = "Appointment time is required")]
        [DataType(DataType.Time)]
        public string AppointmentTime { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        public bool IsForDependent { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Dependent name is required when booking for a dependent")]
        public string? DependentName { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Dependent age is required when booking for a dependent")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        public int? DependentAge { get; set; }

        [RequiredIf("IsForDependent", true, ErrorMessage = "Relationship to dependent is required when booking for a dependent")]
        public string? RelationshipToDependent { get; set; }

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

            if (Id <= 0 || string.IsNullOrEmpty(StaffId) || string.IsNullOrEmpty(AppointmentDate) || 
                string.IsNullOrEmpty(AppointmentTime) || string.IsNullOrEmpty(Description) ||
                string.IsNullOrEmpty(Status))
                return false;

            return true;
        }
    }
} 