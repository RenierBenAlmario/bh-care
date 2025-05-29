using System.ComponentModel.DataAnnotations;

namespace Barangay.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        // Guardian information fields for users under 18
        [StringLength(100)]
        public string? GuardianFullName { get; set; }

        [StringLength(20)]
        [Phone]
        public string? GuardianContactNumber { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
} 