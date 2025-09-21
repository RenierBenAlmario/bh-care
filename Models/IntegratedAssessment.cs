using System;
using System.ComponentModel.DataAnnotations;
using Barangay.Data; // For ApplicationUser if needed as a navigation property

namespace Barangay.Models
{
    public class IntegratedAssessment
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        // Optional: Navigation property to ApplicationUser
        // public ApplicationUser User { get; set; }

        [StringLength(50)]
        public string? FamilyNo { get; set; }

        [StringLength(200)]
        public string? HealthFacility { get; set; }

        // Add other relevant assessment fields as per your form steps
        // Example from AppointmentInputModel for Step 1:
        public string? Address { get; set; }
        public string? Barangay { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Telepono { get; set; }
        public int? Edad { get; set; }
        public string? Kasarian { get; set; }
        public string? Relihiyon { get; set; }

        // Example from AppointmentInputModel for Step 2 (Past Medical History):
        public bool HasDiabetes { get; set; }
        public bool HasHypertension { get; set; }
        public bool HasCancer { get; set; }
        public bool HasCOPD { get; set; }
        public bool HasLungDisease { get; set; }
        public bool HasEyeDisease { get; set; }
        
        // You would continue adding all fields that need to be saved from the multi-step form

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        // If this assessment is linked to a specific appointment:
        // public int? AppointmentId { get; set; }
        // public Appointment Appointment { get; set; }
    }
} 