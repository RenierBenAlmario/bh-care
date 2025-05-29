using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string Status { get; set; } = string.Empty;
        
        // Initialize the FullName property
        public string FullName { get; set; } = string.Empty;
    }
}