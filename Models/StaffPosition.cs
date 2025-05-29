using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class StaffPosition
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        
        // Default positions
        public static class DefaultPositions
        {
            public const string Admin = "Admin";
            public const string Doctor = "Doctor";
            public const string Nurse = "Nurse";
            public const string IT = "IT";
        }
    }
} 