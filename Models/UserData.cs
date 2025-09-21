using System;
using System.Collections.Generic;

namespace Barangay.Models
{
    public class UserData
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastActive { get; set; }
        public bool IsActive { get; set; }
        public string PhilHealthId { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public List<UserDocument> Documents { get; set; } = new();

        public string ContactNumber { get; set; } = string.Empty;
        
        // Guardian consent properties
        public bool RequiresGuardianConsent { get; set; }
        public bool HasGuardianConsent { get; set; }
        public string GuardianFirstName { get; set; } = string.Empty;
        public string GuardianLastName { get; set; } = string.Empty;
        public int? GuardianInformationId { get; set; }
        public byte[]? ResidencyProof { get; set; }
        public bool HasResidencyProof { get; set; }
        
        // Helper property to calculate age based on current date
        public int Age
        {
            get 
            {
                var today = DateTime.Today;
                var birthDate = DateTime.TryParse(BirthDate, out var parsedBirthDate) ? parsedBirthDate : DateTime.MinValue;
                var age = today.Year - birthDate.Year;
                if (birthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

    }
} 


