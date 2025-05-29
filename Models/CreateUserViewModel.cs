using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Address { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        // Guardian information fields
        [StringLength(100)]
        public string? GuardianFullName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? GuardianContactNumber { get; set; }

        public string? Gender { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Specialization { get; set; }

        [Required]
        public string Role { get; set; } = string.Empty;
    }
} 