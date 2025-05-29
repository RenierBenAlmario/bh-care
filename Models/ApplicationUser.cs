using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string EncryptedStatus { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string EncryptedFullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;
        public string WorkingDays { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public int MaxDailyPatients { get; set; } = 20;
        public DateTime BirthDate { get; set; } = DateTime.Today;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public string ProfilePicture { get; set; } = string.Empty;
        public string ProfileImage { get; set; } = string.Empty;
        public string PhilHealthId { get; set; } = string.Empty;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public UserType UserType { get; set; } = UserType.Patient;
        
        // Terms agreement tracking
        public bool HasAgreedToTerms { get; set; } = false;
        public DateTime? AgreedAt { get; set; }
        
        // Modified properties to be nullable
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Suffix { get; set; }

        // For backward compatibility - removed NotMapped attribute to match database column
        public string FullName 
        { 
            get => $"{FirstName ?? ""} {MiddleName ?? ""} {LastName ?? ""}".Trim();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FirstName = string.Empty;
                    MiddleName = string.Empty;
                    LastName = string.Empty;
                    Name = string.Empty;
                    return;
                }

                var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    FirstName = parts[0];
                    MiddleName = parts[1];
                    LastName = string.Join(" ", parts.Skip(2));
                }
                else if (parts.Length == 2)
                {
                    FirstName = parts[0];
                    LastName = parts[1];
                    MiddleName = string.Empty;
                }
                else if (parts.Length == 1)
                {
                    FirstName = parts[0];
                    MiddleName = string.Empty;
                    LastName = string.Empty;
                }
                Name = value;
            }
        }

        // Navigation properties for appointments
        [InverseProperty("Doctor")]
        public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
        
        // Navigation properties for medical records - only keep Doctor relationship
        [InverseProperty("Doctor")]
        public virtual ICollection<MedicalRecord> DoctorMedicalRecords { get; set; } = new List<MedicalRecord>();
        
        // Navigation properties for prescriptions - only keep Doctor relationship
        [InverseProperty("Doctor")]
        public virtual ICollection<Prescription> DoctorPrescriptions { get; set; } = new List<Prescription>();

        // Navigation property for user documents
        [InverseProperty(nameof(UserDocument.User))]
        public virtual ICollection<UserDocument> UserDocuments { get; set; } = new List<UserDocument>();
        
        // Navigation property for approved documents
        [InverseProperty(nameof(UserDocument.Approver))]
        public virtual ICollection<UserDocument> ApprovedDocuments { get; set; } = new List<UserDocument>();

        // Navigation property for user permissions
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}


