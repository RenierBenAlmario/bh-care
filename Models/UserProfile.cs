using System;

namespace Barangay.Models
{
    public class UserProfile
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? PhilHealthNo { get; set; }
        public string? BarangayNo { get; set; }
        public string? FamilyNo { get; set; }
        public string? BloodType { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? Allergies { get; set; }
        public string? MedicalConditions { get; set; }
        public string? Medications { get; set; }
        public string? ProfilePicture { get; set; }
    }
}