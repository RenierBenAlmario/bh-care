using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Barangay.Models.Generated;

public partial class BhcareDbContext : DbContext
{
    public BhcareDbContext()
    {
    }

    public BhcareDbContext(DbContextOptions<BhcareDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdolescentHealthInfo> AdolescentHealthInfos { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentAttachment> AppointmentAttachments { get; set; }

    public virtual DbSet<AppointmentFile> AppointmentFiles { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<ConsultationTimeSlot> ConsultationTimeSlots { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }

    public virtual DbSet<EmailSuspension> EmailSuspensions { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<FamilyMember> FamilyMembers { get; set; }

    public virtual DbSet<FamilyRecord> FamilyRecords { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackRating> FeedbackRatings { get; set; }

    public virtual DbSet<GuardianInformation> GuardianInformations { get; set; }

    public virtual DbSet<HealthReport> HealthReports { get; set; }

    public virtual DbSet<Heeadsssassessment> Heeadsssassessments { get; set; }

    public virtual DbSet<ImmunizationRecord> ImmunizationRecords { get; set; }

    public virtual DbSet<IntegratedAssessment> IntegratedAssessments { get; set; }

    public virtual DbSet<LabResult> LabResults { get; set; }

    public virtual DbSet<MedicalHistory> MedicalHistories { get; set; }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Medication> Medications { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<NcdriskAssessment> NcdriskAssessments { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<PatientHistory> PatientHistories { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Prescription> Prescriptions { get; set; }

    public virtual DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<StaffMember> StaffMembers { get; set; }

    public virtual DbSet<StaffPermission> StaffPermissions { get; set; }

    public virtual DbSet<StaffPosition> StaffPositions { get; set; }

    public virtual DbSet<UrlToken> UrlTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDocument> UserDocuments { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserSuspension> UserSuspensions { get; set; }

    public virtual DbSet<VitalSign> VitalSigns { get; set; }

    public virtual DbSet<VitalSignsBackupEncryptRegularField> VitalSignsBackupEncryptRegularFields { get; set; }

    public virtual DbSet<VitalSignsBackupStringConversion> VitalSignsBackupStringConversions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:bhcare.database.windows.net,1433;Initial Catalog=bhcareDB;Persist Security Info=False;User ID=bhcare;Password=Thebenzzz10;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdolescentHealthInfo>(entity =>
        {
            entity.ToTable("AdolescentHealthInfo");

            entity.Property(e => e.Bmi).HasColumnName("BMI");
            entity.Property(e => e.Bmicategory).HasColumnName("BMICategory");
            entity.Property(e => e.HpvdateGiven).HasColumnName("HPVDateGiven");
            entity.Property(e => e.MrmmrdateGiven).HasColumnName("MRMMRDateGiven");
            entity.Property(e => e.ObscoreGravida).HasColumnName("OBScoreGravida");
            entity.Property(e => e.ObscoreParity).HasColumnName("OBScoreParity");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_Appointments_ApplicationUserId");

            entity.HasIndex(e => e.DoctorId, "IX_Appointments_DoctorId");

            entity.HasIndex(e => e.PatientId, "IX_Appointments_PatientId");

            entity.HasIndex(e => e.PatientUserId, "IX_Appointments_PatientUserId");

            entity.Property(e => e.Address).HasMaxLength(1000);
            entity.Property(e => e.Allergies).HasMaxLength(2000);
            entity.Property(e => e.AttachmentPath).HasMaxLength(500);
            entity.Property(e => e.ContactNumber).HasMaxLength(100);
            entity.Property(e => e.CurrentMedications).HasMaxLength(2000);
            entity.Property(e => e.DependentFullName).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.EmergencyContact).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Instructions).HasMaxLength(2000);
            entity.Property(e => e.MedicalHistory).HasMaxLength(2000);
            entity.Property(e => e.PatientName).HasMaxLength(1000);
            entity.Property(e => e.Prescription).HasMaxLength(2000);
            entity.Property(e => e.ReasonForVisit).HasMaxLength(2000);
            entity.Property(e => e.RelationshipToDependent).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.AppointmentApplicationUsers).HasForeignKey(d => d.ApplicationUserId);

            entity.HasOne(d => d.Doctor).WithMany(p => p.AppointmentDoctors).HasForeignKey(d => d.DoctorId);

            entity.HasOne(d => d.Patient).WithMany(p => p.AppointmentPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PatientUser).WithMany(p => p.AppointmentPatientUsers).HasForeignKey(d => d.PatientUserId);
        });

        modelBuilder.Entity<AppointmentAttachment>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_AppointmentAttachments_ApplicationUserId");

            entity.HasIndex(e => e.AppointmentId, "IX_AppointmentAttachments_AppointmentId");

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.AppointmentAttachments).HasForeignKey(d => d.ApplicationUserId);

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentAttachments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AppointmentFile>(entity =>
        {
            entity.HasIndex(e => e.AppointmentId, "IX_AppointmentFiles_AppointmentId");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentFiles)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.AppointmentReminders).HasDefaultValue(true);
            entity.Property(e => e.Barangay).HasDefaultValue("");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FullName).HasComputedColumnSql("(Trim((isnull([FirstName]+' ','')+isnull([MiddleName]+' ',''))+isnull([LastName],'')))", false);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PrescriptionAlerts).HasDefaultValue(true);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Doctors_UserId").IsUnique();

            entity.Property(e => e.FullName).HasDefaultValue("");

            entity.HasOne(d => d.User).WithOne(p => p.Doctor).HasForeignKey<Doctor>(d => d.UserId);
        });

        modelBuilder.Entity<DoctorAvailability>(entity =>
        {
            entity.HasIndex(e => e.DoctorId, "IX_DoctorAvailabilities_DoctorId");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorAvailabilities).HasForeignKey(d => d.DoctorId);
        });

        modelBuilder.Entity<EmailSuspension>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_EmailSuspensions_Email");

            entity.HasIndex(e => e.IsActive, "IX_EmailSuspensions_IsActive");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.SuspensionLevel).HasMaxLength(50);
            entity.Property(e => e.SuspensionReason).HasMaxLength(50);
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailVer__3214EC0754FE38EA");

            entity.HasIndex(e => e.Email, "IX_EmailVerifications_Email").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.VerificationCode).HasMaxLength(10);
        });

        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasIndex(e => e.PatientId, "IX_FamilyMembers_PatientId");

            entity.HasIndex(e => e.UserId, "IX_FamilyMembers_UserId");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FamilyNumber).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Relationship).HasMaxLength(50);

            entity.HasOne(d => d.Patient).WithMany(p => p.FamilyMembers)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.FamilyMembers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Feedbacks_UserId");

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.Message).HasMaxLength(500);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FeedbackRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC07144EDBAA");

            entity.Property(e => e.Comments).HasMaxLength(1000);
        });

        modelBuilder.Entity<GuardianInformation>(entity =>
        {
            entity.HasKey(e => e.GuardianId);

            entity.ToTable("GuardianInformation");

            entity.HasIndex(e => e.UserId, "IX_GuardianInformation_UserId").IsUnique();

            entity.Property(e => e.GuardianFirstName).HasMaxLength(100);
            entity.Property(e => e.GuardianLastName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.GuardianInformation)
                .HasForeignKey<GuardianInformation>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HealthReport>(entity =>
        {
            entity.HasIndex(e => e.DoctorId, "IX_HealthReports_DoctorId");

            entity.HasIndex(e => e.UserId, "IX_HealthReports_UserId");

            entity.Property(e => e.BloodPressure).HasMaxLength(20);
            entity.Property(e => e.BloodSugar).HasColumnType("decimal(5, 1)");
            entity.Property(e => e.PhysicalActivity).HasMaxLength(100);
            entity.Property(e => e.Temperature).HasColumnType("decimal(4, 1)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 1)");

            entity.HasOne(d => d.Doctor).WithMany(p => p.HealthReportDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.HealthReportUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Heeadsssassessment>(entity =>
        {
            entity.ToTable("HEEADSSSAssessments");

            entity.Property(e => e.Bmi).HasColumnName("BMI");
            entity.Property(e => e.Bminormal).HasColumnName("BMINormal");
            entity.Property(e => e.Bmiobese).HasColumnName("BMIObese");
            entity.Property(e => e.Bmioverweight).HasColumnName("BMIOverweight");
            entity.Property(e => e.Bmiunderweight).HasColumnName("BMIUnderweight");
            entity.Property(e => e.ImmunizationHpv).HasColumnName("ImmunizationHPV");
            entity.Property(e => e.ImmunizationMr).HasColumnName("ImmunizationMR");
            entity.Property(e => e.Is4Ps).HasMaxLength(4000);
            entity.Property(e => e.IsNhpts)
                .HasMaxLength(4000)
                .HasColumnName("IsNHPTS");
            entity.Property(e => e.IsOwnPhilHealth).HasMaxLength(4000);
            entity.Property(e => e.IsPhilHealthBeneficiaryOnly).HasMaxLength(4000);
            entity.Property(e => e.Obscore).HasColumnName("OBScore");
            entity.Property(e => e.PhilHealthPin)
                .HasMaxLength(4000)
                .HasColumnName("PhilHealthPIN");
            entity.Property(e => e.SexualityHarassment).HasMaxLength(4000);
            entity.Property(e => e.SexualityHealthConcerns).HasMaxLength(4000);
            entity.Property(e => e.SexualityPartnersCount).HasMaxLength(4000);
            entity.Property(e => e.SexualityPregnancyExperience).HasMaxLength(4000);
            entity.Property(e => e.SexualityProtectionUse).HasMaxLength(4000);
            entity.Property(e => e.SexualitySti).HasColumnName("SexualitySTI");
            entity.Property(e => e.SexualityStiexperience)
                .HasMaxLength(4000)
                .HasColumnName("SexualitySTIExperience");
            entity.Property(e => e.VitalBp).HasColumnName("VitalBP");
            entity.Property(e => e.VitalPr).HasColumnName("VitalPR");
            entity.Property(e => e.VitalRr).HasColumnName("VitalRR");
        });

        modelBuilder.Entity<ImmunizationRecord>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(4000);
            entity.Property(e => e.Barangay).HasMaxLength(4000);
            entity.Property(e => e.BcgvaccineDate)
                .HasMaxLength(4000)
                .HasColumnName("BCGVaccineDate");
            entity.Property(e => e.BcgvaccineRemarks)
                .HasMaxLength(4000)
                .HasColumnName("BCGVaccineRemarks");
            entity.Property(e => e.BirthHeight).HasMaxLength(4000);
            entity.Property(e => e.BirthWeight).HasMaxLength(4000);
            entity.Property(e => e.ChildName).HasMaxLength(4000);
            entity.Property(e => e.ContactNumber).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt).HasMaxLength(4000);
            entity.Property(e => e.CreatedBy).HasMaxLength(4000);
            entity.Property(e => e.DateOfBirth).HasMaxLength(4000);
            entity.Property(e => e.Email).HasMaxLength(4000);
            entity.Property(e => e.FamilyNumber).HasMaxLength(4000);
            entity.Property(e => e.FatherName).HasMaxLength(4000);
            entity.Property(e => e.HealthCenter).HasMaxLength(4000);
            entity.Property(e => e.HepatitisBvaccineDate)
                .HasMaxLength(4000)
                .HasColumnName("HepatitisBVaccineDate");
            entity.Property(e => e.HepatitisBvaccineRemarks)
                .HasMaxLength(4000)
                .HasColumnName("HepatitisBVaccineRemarks");
            entity.Property(e => e.Ipv1date)
                .HasMaxLength(4000)
                .HasColumnName("IPV1Date");
            entity.Property(e => e.Ipv1remarks)
                .HasMaxLength(4000)
                .HasColumnName("IPV1Remarks");
            entity.Property(e => e.Ipv2date)
                .HasMaxLength(4000)
                .HasColumnName("IPV2Date");
            entity.Property(e => e.Ipv2remarks)
                .HasMaxLength(4000)
                .HasColumnName("IPV2Remarks");
            entity.Property(e => e.Mmr1date)
                .HasMaxLength(4000)
                .HasColumnName("MMR1Date");
            entity.Property(e => e.Mmr1remarks)
                .HasMaxLength(4000)
                .HasColumnName("MMR1Remarks");
            entity.Property(e => e.Mmr2date)
                .HasMaxLength(4000)
                .HasColumnName("MMR2Date");
            entity.Property(e => e.Mmr2remarks)
                .HasMaxLength(4000)
                .HasColumnName("MMR2Remarks");
            entity.Property(e => e.MotherName).HasMaxLength(4000);
            entity.Property(e => e.Opv1date)
                .HasMaxLength(4000)
                .HasColumnName("OPV1Date");
            entity.Property(e => e.Opv1remarks)
                .HasMaxLength(4000)
                .HasColumnName("OPV1Remarks");
            entity.Property(e => e.Opv2date)
                .HasMaxLength(4000)
                .HasColumnName("OPV2Date");
            entity.Property(e => e.Opv2remarks)
                .HasMaxLength(4000)
                .HasColumnName("OPV2Remarks");
            entity.Property(e => e.Opv3date)
                .HasMaxLength(4000)
                .HasColumnName("OPV3Date");
            entity.Property(e => e.Opv3remarks)
                .HasMaxLength(4000)
                .HasColumnName("OPV3Remarks");
            entity.Property(e => e.Pcv1date)
                .HasMaxLength(4000)
                .HasColumnName("PCV1Date");
            entity.Property(e => e.Pcv1remarks)
                .HasMaxLength(4000)
                .HasColumnName("PCV1Remarks");
            entity.Property(e => e.Pcv2date)
                .HasMaxLength(4000)
                .HasColumnName("PCV2Date");
            entity.Property(e => e.Pcv2remarks)
                .HasMaxLength(4000)
                .HasColumnName("PCV2Remarks");
            entity.Property(e => e.Pcv3date)
                .HasMaxLength(4000)
                .HasColumnName("PCV3Date");
            entity.Property(e => e.Pcv3remarks)
                .HasMaxLength(4000)
                .HasColumnName("PCV3Remarks");
            entity.Property(e => e.Pentavalent1Date).HasMaxLength(4000);
            entity.Property(e => e.Pentavalent1Remarks).HasMaxLength(4000);
            entity.Property(e => e.Pentavalent2Date).HasMaxLength(4000);
            entity.Property(e => e.Pentavalent2Remarks).HasMaxLength(4000);
            entity.Property(e => e.Pentavalent3Date).HasMaxLength(4000);
            entity.Property(e => e.Pentavalent3Remarks).HasMaxLength(4000);
            entity.Property(e => e.PlaceOfBirth).HasMaxLength(4000);
            entity.Property(e => e.Sex).HasMaxLength(4000);
            entity.Property(e => e.Status).HasMaxLength(4000);
            entity.Property(e => e.UpdatedAt).HasMaxLength(4000);
            entity.Property(e => e.UpdatedBy).HasMaxLength(4000);
        });

        modelBuilder.Entity<IntegratedAssessment>(entity =>
        {
            entity.Property(e => e.FamilyNo).HasMaxLength(50);
            entity.Property(e => e.HasCopd).HasColumnName("HasCOPD");
            entity.Property(e => e.HealthFacility).HasMaxLength(200);
        });

        modelBuilder.Entity<LabResult>(entity =>
        {
            entity.HasIndex(e => e.PatientId, "IX_LabResults_PatientId");

            entity.HasOne(d => d.Patient).WithMany(p => p.LabResults).HasForeignKey(d => d.PatientId);
        });

        modelBuilder.Entity<MedicalHistory>(entity =>
        {
            entity.HasIndex(e => e.PatientId, "IX_MedicalHistories_PatientId");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalHistories).HasForeignKey(d => d.PatientId);
        });

        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_MedicalRecords_ApplicationUserId");

            entity.HasIndex(e => e.AppointmentId, "IX_MedicalRecords_AppointmentId");

            entity.HasIndex(e => e.DoctorId, "IX_MedicalRecords_DoctorId");

            entity.HasIndex(e => e.PatientId, "IX_MedicalRecords_PatientId");

            entity.Property(e => e.Diagnosis).HasMaxLength(2000);
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.Treatment).HasMaxLength(2000);

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.MedicalRecordApplicationUsers).HasForeignKey(d => d.ApplicationUserId);

            entity.HasOne(d => d.Appointment).WithMany(p => p.MedicalRecords).HasForeignKey(d => d.AppointmentId);

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedicalRecordDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Medication>(entity =>
        {
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasIndex(e => e.ReceiverId, "IX_Messages_ReceiverId");

            entity.HasIndex(e => e.SenderId, "IX_Messages_SenderId");

            entity.Property(e => e.Content).HasMaxLength(1000);

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<NcdriskAssessment>(entity =>
        {
            entity.ToTable("NCDRiskAssessments");

            entity.HasIndex(e => e.AppointmentId, "IX_NCDRiskAssessments_AppointmentId");

            entity.HasIndex(e => e.UserId, "IX_NCDRiskAssessments_UserId");

            entity.Property(e => e.Address).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeBeer).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeWhisky).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeWine).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount1Bottle320ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount2Bottle640ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount3to4WineGlasses300ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmountLessThan3Shot45ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmountMoreThan4Shots75ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholConsumption).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequency).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequency1to3TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequencyMoreThan4TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.AlcoholInom).HasMaxLength(4000);
            entity.Property(e => e.AlcoholOkasyon).HasMaxLength(4000);
            entity.Property(e => e.AlcoholPerOccasion).HasMaxLength(4000);
            entity.Property(e => e.AlcoholStoppedDuration).HasMaxLength(4000);
            entity.Property(e => e.AppointmentType).HasMaxLength(4000);
            entity.Property(e => e.AssessmentDate).HasMaxLength(4000);
            entity.Property(e => e.Barangay).HasMaxLength(4000);
            entity.Property(e => e.BaselineBp)
                .HasMaxLength(4000)
                .HasColumnName("BaselineBP");
            entity.Property(e => e.BeerConsumption1).HasMaxLength(4000);
            entity.Property(e => e.BeerConsumption2).HasMaxLength(4000);
            entity.Property(e => e.BeerConsumption3).HasMaxLength(4000);
            entity.Property(e => e.Birthday).HasMaxLength(4000);
            entity.Property(e => e.BloodSugarStatus).HasMaxLength(4000);
            entity.Property(e => e.Bmi)
                .HasMaxLength(4000)
                .HasColumnName("BMI");
            entity.Property(e => e.Bmistatus)
                .HasMaxLength(4000)
                .HasColumnName("BMIStatus");
            entity.Property(e => e.Bpstatus)
                .HasMaxLength(4000)
                .HasColumnName("BPStatus");
            entity.Property(e => e.BreastCancerScreened).HasMaxLength(4000);
            entity.Property(e => e.CancerMedication).HasMaxLength(4000);
            entity.Property(e => e.CancerScreeningStatus).HasMaxLength(4000);
            entity.Property(e => e.CancerSite).HasMaxLength(200);
            entity.Property(e => e.CancerType).HasMaxLength(4000);
            entity.Property(e => e.CancerYear).HasMaxLength(4000);
            entity.Property(e => e.CervicalCancerScreened).HasMaxLength(4000);
            entity.Property(e => e.ChestPain).HasMaxLength(4000);
            entity.Property(e => e.ChestPainLocation).HasMaxLength(4000);
            entity.Property(e => e.ChestPainSpreadsToArm).HasMaxLength(4000);
            entity.Property(e => e.ChestPainValue).HasMaxLength(4000);
            entity.Property(e => e.CholesterolResult).HasMaxLength(4000);
            entity.Property(e => e.CholesterolStatus).HasMaxLength(4000);
            entity.Property(e => e.CivilStatus).HasMaxLength(4000);
            entity.Property(e => e.CombinationExercise).HasMaxLength(4000);
            entity.Property(e => e.Copdmedication)
                .HasMaxLength(100)
                .HasColumnName("COPDMedication");
            entity.Property(e => e.Copdyear)
                .HasMaxLength(50)
                .HasColumnName("COPDYear");
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(4000)
                .HasDefaultValue("");
            entity.Property(e => e.DateAssessment).HasMaxLength(4000);
            entity.Property(e => e.DateOfAssessment).HasMaxLength(4000);
            entity.Property(e => e.Designation).HasMaxLength(4000);
            entity.Property(e => e.DiabetesMedication).HasMaxLength(4000);
            entity.Property(e => e.DiabetesYear).HasMaxLength(4000);
            entity.Property(e => e.DoctorName).HasMaxLength(4000);
            entity.Property(e => e.DrinksAlcohol).HasMaxLength(4000);
            entity.Property(e => e.DrinksBeer).HasMaxLength(4000);
            entity.Property(e => e.DrinksWhiskyGinBrandy).HasMaxLength(4000);
            entity.Property(e => e.DrinksWine).HasMaxLength(4000);
            entity.Property(e => e.EatsFattyFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsFishDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsFruitsDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsMeatDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsOilyFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsSweetFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsVegetablesDaily).HasMaxLength(4000);
            entity.Property(e => e.Edad).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoDuration).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoRegular).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoType).HasMaxLength(4000);
            entity.Property(e => e.ExerciseDuration).HasMaxLength(4000);
            entity.Property(e => e.EyeDiseaseMedication).HasMaxLength(4000);
            entity.Property(e => e.EyeDiseaseYear).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasCancer).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasDiabetes).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasHeartDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasHypertension).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasKidneyDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasOtherDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasStroke).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOther).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyNo).HasMaxLength(4000);
            entity.Property(e => e.FamilyOtherDiseaseDetails).HasMaxLength(4000);
            entity.Property(e => e.FastingBloodSugar).HasMaxLength(4000);
            entity.Property(e => e.FirstName).HasMaxLength(4000);
            entity.Property(e => e.FormerSmoker).HasMaxLength(4000);
            entity.Property(e => e.HasAsthma).HasMaxLength(4000);
            entity.Property(e => e.HasCancer).HasMaxLength(4000);
            entity.Property(e => e.HasChestPain).HasMaxLength(4000);
            entity.Property(e => e.HasCopd)
                .HasMaxLength(4000)
                .HasColumnName("HasCOPD");
            entity.Property(e => e.HasDiabetes).HasMaxLength(4000);
            entity.Property(e => e.HasDifficultyBreathing).HasMaxLength(4000);
            entity.Property(e => e.HasEnoughExercise).HasMaxLength(4000);
            entity.Property(e => e.HasEyeDisease).HasMaxLength(4000);
            entity.Property(e => e.HasEyeDiseaseCondition).HasMaxLength(4000);
            entity.Property(e => e.HasHighSaltIntake).HasMaxLength(4000);
            entity.Property(e => e.HasHistoryOfSmoking).HasMaxLength(4000);
            entity.Property(e => e.HasHypertension).HasMaxLength(4000);
            entity.Property(e => e.HasLungDisease).HasMaxLength(4000);
            entity.Property(e => e.HasLungDiseaseNonInfectious).HasMaxLength(4000);
            entity.Property(e => e.HasNoRegularExercise).HasMaxLength(4000);
            entity.Property(e => e.HasPolydipsia).HasMaxLength(4000);
            entity.Property(e => e.HasPolyphagia).HasMaxLength(4000);
            entity.Property(e => e.HasPolyuria).HasMaxLength(4000);
            entity.Property(e => e.HasStress).HasMaxLength(4000);
            entity.Property(e => e.HasStrokeSymptoms).HasMaxLength(4000);
            entity.Property(e => e.HasUnhealthyDiet).HasMaxLength(4000);
            entity.Property(e => e.HasUrineKetones).HasMaxLength(4000);
            entity.Property(e => e.HasUrineProtein).HasMaxLength(4000);
            entity.Property(e => e.HasWeightLoss).HasMaxLength(4000);
            entity.Property(e => e.HealthFacility).HasMaxLength(4000);
            entity.Property(e => e.HealthFacilityName).HasMaxLength(4000);
            entity.Property(e => e.Height).HasMaxLength(4000);
            entity.Property(e => e.HighSaltIntake).HasMaxLength(4000);
            entity.Property(e => e.Hip).HasMaxLength(4000);
            entity.Property(e => e.HypertensionMedication).HasMaxLength(4000);
            entity.Property(e => e.HypertensionYear).HasMaxLength(4000);
            entity.Property(e => e.Idno)
                .HasMaxLength(4000)
                .HasColumnName("IDNo");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(4000)
                .HasColumnName("IDNumber");
            entity.Property(e => e.InsufficientPhysicalActivity).HasMaxLength(4000);
            entity.Property(e => e.InterviewedBy).HasMaxLength(4000);
            entity.Property(e => e.IsBingeDrinker).HasMaxLength(4000);
            entity.Property(e => e.Kasarian).HasMaxLength(4000);
            entity.Property(e => e.LastName).HasMaxLength(4000);
            entity.Property(e => e.LeftArmMeanBp)
                .HasMaxLength(4000)
                .HasColumnName("LeftArmMeanBP");
            entity.Property(e => e.LossOfConsciousnessLessThan10Min).HasMaxLength(4000);
            entity.Property(e => e.LungDiseaseMedication).HasMaxLength(4000);
            entity.Property(e => e.LungDiseaseYear).HasMaxLength(4000);
            entity.Property(e => e.MiddleName).HasMaxLength(4000);
            entity.Property(e => e.ModerateIntensityExercise).HasMaxLength(4000);
            entity.Property(e => e.NeverSmokedButExposedToSmoke).HasMaxLength(4000);
            entity.Property(e => e.NumbnessWhenWalkingFast).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonKumakainMamantika).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonKumakainMatatamis).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasGulay).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasIsda).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasKarne).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasPratas).HasMaxLength(4000);
            entity.Property(e => e.Occupation).HasMaxLength(4000);
            entity.Property(e => e.PainLastsMoreThan30Min).HasMaxLength(4000);
            entity.Property(e => e.PainRelievedWithRest).HasMaxLength(4000);
            entity.Property(e => e.Pananakit21).HasMaxLength(4000);
            entity.Property(e => e.Pananakit22).HasMaxLength(4000);
            entity.Property(e => e.Pananakit23).HasMaxLength(4000);
            entity.Property(e => e.Pananakit24).HasMaxLength(4000);
            entity.Property(e => e.Pananakit25).HasMaxLength(4000);
            entity.Property(e => e.Pananakit26).HasMaxLength(4000);
            entity.Property(e => e.Pananakit27).HasMaxLength(4000);
            entity.Property(e => e.Pananakit28).HasMaxLength(4000);
            entity.Property(e => e.PatientSignature).HasMaxLength(4000);
            entity.Property(e => e.RandomBloodSugar).HasMaxLength(4000);
            entity.Property(e => e.Relihiyon).HasMaxLength(4000);
            entity.Property(e => e.RightArmMeanBp)
                .HasMaxLength(4000)
                .HasColumnName("RightArmMeanBP");
            entity.Property(e => e.RiskPercentage).HasMaxLength(4000);
            entity.Property(e => e.RiskStatus).HasMaxLength(4000);
            entity.Property(e => e.SeeDoctorIfYes).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoKadami).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoSticks).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoTumigil).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoUsok).HasMaxLength(4000);
            entity.Property(e => e.Smoked100Sticks).HasMaxLength(4000);
            entity.Property(e => e.SmokingQuitDuration).HasMaxLength(4000);
            entity.Property(e => e.SmokingStatus).HasMaxLength(4000);
            entity.Property(e => e.StressEpekto).HasMaxLength(4000);
            entity.Property(e => e.StressMadalas).HasMaxLength(4000);
            entity.Property(e => e.StressSino).HasMaxLength(4000);
            entity.Property(e => e.Telepono).HasMaxLength(4000);
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(4000)
                .HasDefaultValue("");
            entity.Property(e => e.UrineKetones).HasMaxLength(4000);
            entity.Property(e => e.UrineProtein).HasMaxLength(4000);
            entity.Property(e => e.VigorousIntensityExercise).HasMaxLength(4000);
            entity.Property(e => e.Waist).HasMaxLength(4000);
            entity.Property(e => e.Weight).HasMaxLength(4000);
            entity.Property(e => e.WhiskyConsumption1).HasMaxLength(4000);
            entity.Property(e => e.WhiskyConsumption2).HasMaxLength(4000);
            entity.Property(e => e.Whratio)
                .HasMaxLength(4000)
                .HasColumnName("WHRatio");
            entity.Property(e => e.Whstatus)
                .HasMaxLength(4000)
                .HasColumnName("WHStatus");
            entity.Property(e => e.WineConsumption1).HasMaxLength(4000);
            entity.Property(e => e.WineConsumption2).HasMaxLength(4000);

            entity.HasOne(d => d.Appointment).WithMany(p => p.NcdriskAssessments)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User).WithMany(p => p.NcdriskAssessments).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_Notifications_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PasswordResetOtp>(entity =>
        {
            entity.ToTable("PasswordResetOTPs");

            entity.HasIndex(e => e.UserId, "IX_PasswordResetOTPs_UserId");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Otp)
                .HasMaxLength(6)
                .HasColumnName("OTP");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetOtps).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.Address).HasMaxLength(1000);
            entity.Property(e => e.Alert).HasMaxLength(2000);
            entity.Property(e => e.Allergies).HasMaxLength(2000);
            entity.Property(e => e.BloodType).HasMaxLength(100);
            entity.Property(e => e.ContactNumber).HasMaxLength(100);
            entity.Property(e => e.CurrentMedications).HasColumnType("text");
            entity.Property(e => e.Diagnosis).HasMaxLength(2000);
            entity.Property(e => e.Email).HasMaxLength(500);
            entity.Property(e => e.EmergencyContact).HasMaxLength(500);
            entity.Property(e => e.EmergencyContactNumber).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(1000);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MedicalHistory).HasColumnType("text");
            entity.Property(e => e.Room).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithOne(p => p.Patient).HasForeignKey<Patient>(d => d.UserId);
        });

        modelBuilder.Entity<PatientHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PatientH__3214EC07D15936DB");

            entity.HasIndex(e => e.AppointmentId, "IX_PatientHistories_AppointmentId");

            entity.HasIndex(e => e.PatientId, "IX_PatientHistories_PatientId");

            entity.Property(e => e.Diagnosis).HasMaxLength(500);
            entity.Property(e => e.Medications).HasMaxLength(500);
            entity.Property(e => e.Notes).HasColumnType("ntext");
            entity.Property(e => e.Symptoms).HasColumnType("ntext");
            entity.Property(e => e.Treatment).HasColumnType("ntext");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.StaffPositions).WithMany(p => p.Permissions)
                .UsingEntity<Dictionary<string, object>>(
                    "StaffPositionPermission",
                    r => r.HasOne<StaffPosition>().WithMany()
                        .HasForeignKey("StaffPositionId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("PermissionId", "StaffPositionId");
                        j.ToTable("StaffPositionPermission");
                        j.HasIndex(new[] { "StaffPositionId" }, "IX_StaffPositionPermission_StaffPositionId");
                    });
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_Prescriptions_ApplicationUserId");

            entity.HasIndex(e => e.DoctorId, "IX_Prescriptions_DoctorId");

            entity.HasIndex(e => e.PatientId, "IX_Prescriptions_PatientId");

            entity.HasIndex(e => e.PatientUserId, "IX_Prescriptions_PatientUserId");

            entity.Property(e => e.Diagnosis).HasMaxLength(2000);
            entity.Property(e => e.Notes).HasMaxLength(2000);

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.PrescriptionApplicationUsers).HasForeignKey(d => d.ApplicationUserId);

            entity.HasOne(d => d.Doctor).WithMany(p => p.PrescriptionDoctors)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Patient).WithMany(p => p.PrescriptionPatients)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PatientUser).WithMany(p => p.PrescriptionPatientUsers).HasForeignKey(d => d.PatientUserId);
        });

        modelBuilder.Entity<PrescriptionMedication>(entity =>
        {
            entity.HasIndex(e => e.MedicalRecordId, "IX_PrescriptionMedications_MedicalRecordId");

            entity.HasIndex(e => e.MedicationId, "IX_PrescriptionMedications_MedicationId");

            entity.HasIndex(e => e.PrescriptionId, "IX_PrescriptionMedications_PrescriptionId");

            entity.Property(e => e.Dosage).HasMaxLength(100);
            entity.Property(e => e.Frequency).HasMaxLength(100);
            entity.Property(e => e.Instructions).HasMaxLength(500);
            entity.Property(e => e.MedicationName).HasDefaultValue("");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasDefaultValue("");

            entity.HasOne(d => d.MedicalRecord).WithMany(p => p.PrescriptionMedications)
                .HasForeignKey(d => d.MedicalRecordId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Medication).WithMany(p => p.PrescriptionMedications).HasForeignKey(d => d.MedicationId);

            entity.HasOne(d => d.Prescription).WithMany(p => p.PrescriptionMedications).HasForeignKey(d => d.PrescriptionId);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasIndex(e => e.PermissionId, "IX_RolePermissions_PermissionId");

            entity.HasIndex(e => e.RoleId, "IX_RolePermissions_RoleId");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions).HasForeignKey(d => d.PermissionId);

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<StaffMember>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_StaffMembers_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.StaffMembers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<StaffPermission>(entity =>
        {
            entity.HasIndex(e => e.PermissionId, "IX_StaffPermissions_PermissionId");

            entity.HasIndex(e => e.StaffMemberId, "IX_StaffPermissions_StaffMemberId");

            entity.HasOne(d => d.Permission).WithMany(p => p.StaffPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StaffMember).WithMany(p => p.StaffPermissions)
                .HasForeignKey(d => d.StaffMemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StaffPosition>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<UrlToken>(entity =>
        {
            entity.HasIndex(e => e.ExpiresAt, "IX_UrlTokens_ExpiresAt");

            entity.HasIndex(e => e.ResourceId, "IX_UrlTokens_ResourceId");

            entity.HasIndex(e => e.Token, "IX_UrlTokens_Token");

            entity.Property(e => e.ClientIp).HasMaxLength(45);
            entity.Property(e => e.OriginalUrl).HasMaxLength(500);
            entity.Property(e => e.ResourceType).HasMaxLength(50);
            entity.Property(e => e.Token).HasMaxLength(500);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
        });

        modelBuilder.Entity<UserDocument>(entity =>
        {
            entity.HasIndex(e => e.ApprovedBy, "IX_UserDocuments_ApprovedBy");

            entity.HasIndex(e => e.Status, "IX_UserDocuments_Status");

            entity.HasIndex(e => e.UploadDate, "IX_UserDocuments_UploadDate");

            entity.HasIndex(e => e.UserId, "IX_UserDocuments_UserId");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.UserDocumentApprovedByNavigations).HasForeignKey(d => d.ApprovedBy);

            entity.HasOne(d => d.User).WithMany(p => p.UserDocumentUsers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasIndex(e => e.PermissionId, "IX_UserPermissions_PermissionId");

            entity.HasIndex(e => e.UserId, "IX_UserPermissions_UserId");

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserSuspension>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserSuspensions_UserId");

            entity.Property(e => e.SuspensionLevel).HasMaxLength(50);
            entity.Property(e => e.SuspensionReason).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.UserSuspensions).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<VitalSign>(entity =>
        {
            entity.HasIndex(e => e.PatientId, "IX_VitalSigns_PatientId");

            entity.Property(e => e.BloodPressure).HasMaxLength(20);
            entity.Property(e => e.EncryptedBloodPressure).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeartRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeight).HasMaxLength(1000);
            entity.Property(e => e.EncryptedRespiratoryRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedSpO2).HasMaxLength(1000);
            entity.Property(e => e.EncryptedTemperature).HasMaxLength(1000);
            entity.Property(e => e.EncryptedWeight).HasMaxLength(1000);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.SpO2).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Patient).WithMany(p => p.VitalSigns).HasForeignKey(d => d.PatientId);
        });

        modelBuilder.Entity<VitalSignsBackupEncryptRegularField>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VitalSigns_Backup_EncryptRegularFields");

            entity.Property(e => e.BloodPressure).HasMaxLength(20);
            entity.Property(e => e.EncryptedBloodPressure).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeartRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeight).HasMaxLength(1000);
            entity.Property(e => e.EncryptedRespiratoryRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedSpO2).HasMaxLength(1000);
            entity.Property(e => e.EncryptedTemperature).HasMaxLength(1000);
            entity.Property(e => e.EncryptedWeight).HasMaxLength(1000);
            entity.Property(e => e.HeartRate).HasMaxLength(50);
            entity.Property(e => e.Height).HasMaxLength(50);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.PatientId).HasMaxLength(450);
            entity.Property(e => e.RespiratoryRate).HasMaxLength(50);
            entity.Property(e => e.SpO2).HasMaxLength(50);
            entity.Property(e => e.Temperature).HasMaxLength(50);
            entity.Property(e => e.Weight).HasMaxLength(50);
        });

        modelBuilder.Entity<VitalSignsBackupStringConversion>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VitalSigns_Backup_StringConversion");

            entity.Property(e => e.BloodPressure).HasMaxLength(20);
            entity.Property(e => e.EncryptedBloodPressure).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeartRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedHeight).HasMaxLength(1000);
            entity.Property(e => e.EncryptedRespiratoryRate).HasMaxLength(1000);
            entity.Property(e => e.EncryptedSpO2).HasMaxLength(1000);
            entity.Property(e => e.EncryptedTemperature).HasMaxLength(1000);
            entity.Property(e => e.EncryptedWeight).HasMaxLength(1000);
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.PatientId).HasMaxLength(450);
            entity.Property(e => e.SpO2).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Temperature).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
