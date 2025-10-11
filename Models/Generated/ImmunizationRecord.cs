using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class ImmunizationRecord
{
    public int Id { get; set; }

    public string ChildName { get; set; } = null!;

    public string DateOfBirth { get; set; } = null!;

    public string? PlaceOfBirth { get; set; }

    public string? Address { get; set; }

    public string MotherName { get; set; } = null!;

    public string? FatherName { get; set; }

    public string? Sex { get; set; }

    public string? BirthHeight { get; set; }

    public string? BirthWeight { get; set; }

    public string? HealthCenter { get; set; }

    public string? Barangay { get; set; }

    public string? FamilyNumber { get; set; }

    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public string? BcgvaccineDate { get; set; }

    public string? BcgvaccineRemarks { get; set; }

    public string? HepatitisBvaccineDate { get; set; }

    public string? HepatitisBvaccineRemarks { get; set; }

    public string? Pentavalent1Date { get; set; }

    public string? Pentavalent1Remarks { get; set; }

    public string? Pentavalent2Date { get; set; }

    public string? Pentavalent2Remarks { get; set; }

    public string? Pentavalent3Date { get; set; }

    public string? Pentavalent3Remarks { get; set; }

    public string? Opv1date { get; set; }

    public string? Opv1remarks { get; set; }

    public string? Opv2date { get; set; }

    public string? Opv2remarks { get; set; }

    public string? Opv3date { get; set; }

    public string? Opv3remarks { get; set; }

    public string? Ipv1date { get; set; }

    public string? Ipv1remarks { get; set; }

    public string? Ipv2date { get; set; }

    public string? Ipv2remarks { get; set; }

    public string? Pcv1date { get; set; }

    public string? Pcv1remarks { get; set; }

    public string? Pcv2date { get; set; }

    public string? Pcv2remarks { get; set; }

    public string? Pcv3date { get; set; }

    public string? Pcv3remarks { get; set; }

    public string? Mmr1date { get; set; }

    public string? Mmr1remarks { get; set; }

    public string? Mmr2date { get; set; }

    public string? Mmr2remarks { get; set; }

    public string CreatedAt { get; set; } = null!;

    public string UpdatedAt { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public string UpdatedBy { get; set; } = null!;

    public string Status { get; set; } = null!;
}
