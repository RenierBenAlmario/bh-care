using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        [MaxLength(100)]
        public string? Manufacturer { get; set; }
    }
} 