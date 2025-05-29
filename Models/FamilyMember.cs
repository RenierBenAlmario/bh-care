using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barangay.Models
{
    // Update the FamilyMember model to use string for PatientId
    public class FamilyMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        // Change from int to string to match Patient.UserId
        public string PatientId { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; } = null!;

        [Required]
        public int Age { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Family Number")]
        public string FamilyNumber { get; set; } = string.Empty;

        public string? MedicalHistory { get; set; }
        public string? Allergies { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}