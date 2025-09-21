using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class FeedbackRating
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string ServiceType { get; set; }
        
        public int? AppointmentId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string Comments { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
} 