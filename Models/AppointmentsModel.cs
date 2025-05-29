using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class AppointmentsModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public required string Status { get; set; }
        public decimal Fee { get; set; }  // Added Fee property
        
        // Navigation property for Patient
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        // Navigation property for Doctor
        public int DoctorId { get; set; }
        public Staff? Doctor { get; set; }

        public required string Condition { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; } 
            
        public string? Attachments { get; set; }
    }
}

