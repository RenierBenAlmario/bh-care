using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class IntegratedAssessment
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? FamilyNo { get; set; }

    public string? HealthFacility { get; set; }

    public string? Address { get; set; }

    public string? Barangay { get; set; }

    public DateTime? Birthday { get; set; }

    public string? Telepono { get; set; }

    public int? Edad { get; set; }

    public string? Kasarian { get; set; }

    public string? Relihiyon { get; set; }

    public bool HasDiabetes { get; set; }

    public bool HasHypertension { get; set; }

    public bool HasCancer { get; set; }

    public bool HasCopd { get; set; }

    public bool HasLungDisease { get; set; }

    public bool HasEyeDisease { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
