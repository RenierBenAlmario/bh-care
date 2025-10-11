using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class AdolescentHealthInfo
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? AppointmentId { get; set; }

    public string? PatientName { get; set; }

    public string? PatientAge { get; set; }

    public string? PatientGender { get; set; }

    public string? PatientAddress { get; set; }

    public string? PatientContact { get; set; }

    public string? HeightCm { get; set; }

    public string? WeightKg { get; set; }

    public string? Bmi { get; set; }

    public string? Bmicategory { get; set; }

    public string? MrmmrdateGiven { get; set; }

    public string? TdDateGiven { get; set; }

    public string? HpvdateGiven { get; set; }

    public string? Temperature { get; set; }

    public string? BloodPressure { get; set; }

    public string? PulseRate { get; set; }

    public string? RespiratoryRate { get; set; }

    public string? ChiefComplaint { get; set; }

    public string? WorkingDiagnosis { get; set; }

    public string? ReferredTo { get; set; }

    public string? DateOfMenarche { get; set; }

    public string? AgeOf1stPregnancy { get; set; }

    public string? ObscoreGravida { get; set; }

    public string? ObscoreParity { get; set; }

    public string? HistoryOfPresentIllness { get; set; }

    public string? PhysicalExaminationFindings { get; set; }

    public string? PastMedicalHistory { get; set; }

    public string? FamilyHistory { get; set; }

    public string? Management { get; set; }

    public string? ReasonForReferral { get; set; }

    public string? FollowUpDate { get; set; }

    public string? RecordedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
