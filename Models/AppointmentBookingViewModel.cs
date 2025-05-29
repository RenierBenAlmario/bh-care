using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class AppointmentBookingViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        [Range(0, 120, ErrorMessage = "Age must be between 0 and 120")]
        [Display(Name = "Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [Display(Name = "Appointment Date")]
        public string AppointmentDate { get; set; }

        [Required(ErrorMessage = "Consultation type is required")]
        [Display(Name = "Consultation Type")]
        public string ConsultationType { get; set; }

        [Required(ErrorMessage = "Time slot is required")]
        [Display(Name = "Appointment Time")]
        public int? SelectedTimeSlotId { get; set; }

        [Required(ErrorMessage = "Reason for visit is required")]
        [Display(Name = "Reason for Visit")]
        public string ReasonForVisit { get; set; }

        [Display(Name = "Booking for Someone Else")]
        public bool BookingForOther { get; set; }

        [Display(Name = "Current Step")]
        public int CurrentStep { get; set; } = 1;

        [Display(Name = "Family Number")]
        public string FamilyNumber { get; set; }

        public bool HasFamilyNumber { get; set; }

        [Display(Name = "Temperature")]
        public decimal? Temperature { get; set; }
        
        [Display(Name = "Blood Pressure")]
        public string BloodPressure { get; set; }
        
        [Display(Name = "Pulse Rate")]
        public int? PulseRate { get; set; }
        
        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }
        
        public static List<string> ConsultationTypes => new List<string>
        {
            "Medical Consultation",
            "Dental Consultation",
            "Immunization",
            "BP/Sugar/Weight Check-up",
            "Family Planning"
        };
        
        public List<ConsultationTimeSlot> AvailableTimeSlots { get; set; } = new();
    }
} 