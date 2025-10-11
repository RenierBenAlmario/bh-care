using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class MedicalHistory
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public string ChiefComplaint { get; set; } = null!;

    public string HistoryOfPresentIllness { get; set; } = null!;

    public string? Allergies { get; set; }

    public string? CurrentMedications { get; set; }

    public string PastMedicalHistory { get; set; } = null!;

    public string FamilyHistory { get; set; } = null!;

    public string PersonalSocialHistory { get; set; } = null!;

    public string ReviewOfSystems { get; set; } = null!;

    public string PhysicalExamination { get; set; } = null!;

    public DateTime DateRecorded { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
