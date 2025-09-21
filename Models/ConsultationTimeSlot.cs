using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class ConsultationTimeSlot
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Consultation Type")]
        public string ConsultationType { get; set; }
        
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        
        [Required]
        [Display(Name = "Is Booked")]
        public bool IsBooked { get; set; } = false;
        
        [Display(Name = "Booked By")]
        public string BookedById { get; set; }
        
        [Display(Name = "Booked At")]
        public DateTime? BookedAt { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 