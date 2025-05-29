using System;
using System.ComponentModel.DataAnnotations;

namespace Barangay.Models
{
    public class AppointmentFile
    {
        public int Id { get; set; }
        
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        public string OriginalFileName { get; set; } = string.Empty;
        
        [Required]
        public string ContentType { get; set; } = string.Empty;
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        public DateTime UploadedAt { get; set; }
        
        // Navigation property
        public virtual Appointment? Appointment { get; set; }
    }
}