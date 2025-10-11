using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class EmailVerification
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string VerificationCode { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? VerifiedAt { get; set; }
}
