using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Patient
{
    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string Address { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string EmergencyContact { get; set; } = null!;

    public string EmergencyContactNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Status { get; set; }

    public string? Room { get; set; }

    public string? Diagnosis { get; set; }

    public string? Alert { get; set; }

    public TimeOnly? Time { get; set; }

    public string? Allergies { get; set; }

    public string? MedicalHistory { get; set; }

    public string? CurrentMedications { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? BloodType { get; set; }

    public virtual ICollection<Appointment> AppointmentPatientUsers { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointment> AppointmentPatients { get; set; } = new List<Appointment>();

    public virtual ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();

    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Prescription> PrescriptionPatientUsers { get; set; } = new List<Prescription>();

    public virtual ICollection<Prescription> PrescriptionPatients { get; set; } = new List<Prescription>();

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
}
