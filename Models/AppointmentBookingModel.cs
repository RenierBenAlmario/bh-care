using System;

namespace Barangay.Models
{
    public class AppointmentBookingModel
    {
        public string DoctorId { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Description { get; set; } = string.Empty;
        public IFormFile? Attachment { get; set; }
    }
}