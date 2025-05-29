using System;
using System.Collections.Generic;

namespace Barangay.TempModels;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string EncryptedStatus { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string EncryptedFullName { get; set; } = null!;

    public string Specialization { get; set; } = null!;

    public bool IsActive { get; set; }

    public string WorkingDays { get; set; } = null!;

    public string WorkingHours { get; set; } = null!;

    public int MaxDailyPatients { get; set; }

    public DateTime BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string ProfilePicture { get; set; } = null!;

    public string ProfileImage { get; set; } = null!;

    public string PhilHealthId { get; set; } = null!;

    public DateTime LastActive { get; set; }

    public DateTime JoinDate { get; set; }

    public bool HasAgreedToTerms { get; set; }

    public DateTime? AgreedAt { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string? LastName { get; set; }

    public string? Suffix { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
