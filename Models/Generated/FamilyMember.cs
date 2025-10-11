using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class FamilyMember
{
    public int Id { get; set; }

    public string PatientId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Relationship { get; set; } = null!;

    public string? ContactNumber { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public int Age { get; set; }

    public string UserId { get; set; } = null!;

    public string FamilyNumber { get; set; } = null!;

    public string? MedicalHistory { get; set; }

    public string? Allergies { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Patient Patient { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
