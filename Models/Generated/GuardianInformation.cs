using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class GuardianInformation
{
    public int GuardianId { get; set; }

    public string UserId { get; set; } = null!;

    public string GuardianFirstName { get; set; } = null!;

    public string GuardianLastName { get; set; } = null!;

    public byte[] ResidencyProof { get; set; } = null!;

    public string ResidencyProofPath { get; set; } = null!;

    public string ProofType { get; set; } = null!;

    public string ConsentStatus { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
