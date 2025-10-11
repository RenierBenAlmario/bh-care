using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class Appointment
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public string? DoctorId { get; set; }

    public string PatientName { get; set; } = null!;

    public string? DependentFullName { get; set; }

    public int? DependentAge { get; set; }

    public string? RelationshipToDependent { get; set; }

    public string? Gender { get; set; }

    public string? ContactNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public string? Allergies { get; set; }

    public string? MedicalHistory { get; set; }

    public string? CurrentMedications { get; set; }

    public string? AttachmentsData { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string AppointmentTimeInput { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ReasonForVisit { get; set; } = null!;

    public int Status { get; set; }

    public int AgeValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Type { get; set; }

    public string? AttachmentPath { get; set; }

    public string? Prescription { get; set; }

    public string? Instructions { get; set; }

    public string? ApplicationUserId { get; set; }

    public string? PatientUserId { get; set; }

    public virtual AspNetUser? ApplicationUser { get; set; }

    public virtual ICollection<AppointmentAttachment> AppointmentAttachments { get; set; } = new List<AppointmentAttachment>();

    public virtual ICollection<AppointmentFile> AppointmentFiles { get; set; } = new List<AppointmentFile>();

    public virtual AspNetUser? Doctor { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<NcdriskAssessment> NcdriskAssessments { get; set; } = new List<NcdriskAssessment>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual Patient? PatientUser { get; set; }
}
