using System;

namespace Barangay.Models
{
    public class StaffData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string WorkingDays { get; set; } = string.Empty;
        public string WorkingHours { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public int MaxDailyPatients { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
    }
} 