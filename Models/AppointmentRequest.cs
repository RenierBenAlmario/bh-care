namespace Barangay.Models
{
    public class AppointmentRequest
    {
        public string? PatientName { get; set; } // Made nullable
        public DateTime AppointmentDate { get; set; }
        public string? PatientId { get; set; } // Made nullable
        public string? AppointmentTime { get; set; } // Made nullable
        public string? ReasonForVisit { get; set; } // Made nullable
        public string? DoctorId { get; set; } // Made nullable
        // Add other necessary properties
    }
}