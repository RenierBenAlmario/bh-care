using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class PasswordResetOtp
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Otp { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsUsed { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
