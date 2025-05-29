using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Category { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
        public virtual ICollection<StaffPosition> StaffPositions { get; set; } = new List<StaffPosition>();

        // Default permissions for standard roles
        public static class DefaultPermissions
        {
            public const string ApproveUsers = "ApproveUsers";
            public const string DeleteUsers = "DeleteUsers";
            public const string ViewReports = "ViewReports";
            public const string ManageUsers = "ManageUsers";
            public const string ManageAppointments = "ManageAppointments";
            public const string ManageMedicalRecords = "ManageMedicalRecords";
            public const string AccessAdminDashboard = "AccessAdminDashboard";
            public const string AccessDoctorDashboard = "AccessDoctorDashboard";
        }
    }
} 