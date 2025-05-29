using System;
using System.Collections.Generic;

namespace Barangay.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalStaff { get; set; }
        public int DoctorCount { get; set; }
        public int NurseCount { get; set; }
        public int PendingAppointments { get; set; }
        public int ActiveStaff { get; set; }
        public List<StaffMemberViewModel> StaffMembers { get; set; }
    }

    public class StaffMemberViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime? LastActive { get; set; }
    }

    public class StaffDetailsViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastActive { get; set; }
        public List<DocumentViewModel> Documents { get; set; }
    }

    public class DocumentViewModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime UploadDate { get; set; }
    }
} 