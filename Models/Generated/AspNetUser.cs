using System;
using System.Collections.Generic;

namespace Barangay.Models.Generated;

public partial class AspNetUser
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string EncryptedStatus { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string EncryptedFullName { get; set; } = null!;

    public string Specialization { get; set; } = null!;

    public bool IsActive { get; set; }

    public string WorkingDays { get; set; } = null!;

    public string WorkingHours { get; set; } = null!;

    public int MaxDailyPatients { get; set; }

    public DateTime? BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    public string Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string ProfilePicture { get; set; } = null!;

    public string ProfileImage { get; set; } = null!;

    public string PhilHealthId { get; set; } = null!;

    public DateTime LastActive { get; set; }

    public DateTime JoinDate { get; set; }

    public int UserType { get; set; }

    public bool HasAgreedToTerms { get; set; }

    public DateTime? AgreedAt { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Suffix { get; set; }

    public string? FullName { get; set; }

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

    public string? CivilStatus { get; set; }

    public string? Occupation { get; set; }

    public string? Religion { get; set; }

    public int UserNumber { get; set; }

    public string Barangay { get; set; } = null!;

    public bool AppointmentReminders { get; set; }

    public bool HealthTips { get; set; }

    public bool PrescriptionAlerts { get; set; }

    public string? Age { get; set; }

    public virtual ICollection<Appointment> AppointmentApplicationUsers { get; set; } = new List<Appointment>();

    public virtual ICollection<AppointmentAttachment> AppointmentAttachments { get; set; } = new List<AppointmentAttachment>();

    public virtual ICollection<Appointment> AppointmentDoctors { get; set; } = new List<Appointment>();

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<DoctorAvailability> DoctorAvailabilities { get; set; } = new List<DoctorAvailability>();

    public virtual ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual GuardianInformation? GuardianInformation { get; set; }

    public virtual ICollection<HealthReport> HealthReportDoctors { get; set; } = new List<HealthReport>();

    public virtual ICollection<HealthReport> HealthReportUsers { get; set; } = new List<HealthReport>();

    public virtual ICollection<MedicalRecord> MedicalRecordApplicationUsers { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<MedicalRecord> MedicalRecordDoctors { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<NcdriskAssessment> NcdriskAssessments { get; set; } = new List<NcdriskAssessment>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PasswordResetOtp> PasswordResetOtps { get; set; } = new List<PasswordResetOtp>();

    public virtual Patient? Patient { get; set; }

    public virtual ICollection<Prescription> PrescriptionApplicationUsers { get; set; } = new List<Prescription>();

    public virtual ICollection<Prescription> PrescriptionDoctors { get; set; } = new List<Prescription>();

    public virtual ICollection<StaffMember> StaffMembers { get; set; } = new List<StaffMember>();

    public virtual ICollection<UserDocument> UserDocumentApprovedByNavigations { get; set; } = new List<UserDocument>();

    public virtual ICollection<UserDocument> UserDocumentUsers { get; set; } = new List<UserDocument>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<UserSuspension> UserSuspensions { get; set; } = new List<UserSuspension>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
