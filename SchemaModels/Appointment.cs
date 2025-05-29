using System;
using System.Collections.Generic;

namespace Barangay.SchemaModels;

public partial class Appointment
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public string PatientName { get; set; } = null!;

    public string? Gender { get; set; }

    public string? ContactNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public string? EmergencyContact { get; set; }

    public string? EmergencyContactNumber { get; set; }

    public string? Allergies { get; set; }

    public string? MedicalHistory { get; set; }

    public string? CurrentMedications { get; set; }

    public DateTime AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string DoctorId { get; set; } = null!;

    public string ReasonForVisit { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public int AgeValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string? Type { get; set; }

    public string? AttachmentPath { get; set; }

    public string? Prescription { get; set; }

    public string? Instructions { get; set; }

    public string? Attachments { get; set; }

    public int? DependentAge { get; set; }

    public string? DependentFullName { get; set; }

    public string? RelationshipToDependent { get; set; }

    public string? PatientUserId { get; set; }

    public string AppointmentTimeInput { get; set; } = null!;

    public string? FamilyNumber { get; set; }

    public string? HealthFacilityId { get; set; }
}
