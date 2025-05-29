BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Suffix');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Suffix] nvarchar(max) NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508150111_MakeLastNameAndSuffixOptional', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [EncryptedStatus] nvarchar(max) NOT NULL,
    [EncryptedFullName] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [PhilHealthId] nvarchar(max) NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NULL,
    [Suffix] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Medications] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientGroup] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [EmergencyContact] nvarchar(100) NOT NULL,
    [EmergencyContactNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] text NULL,
    [CurrentMedications] text NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BloodType] nvarchar(100) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [WorkingDays] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [JoinDate] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [DependentFullName] nvarchar(100) NULL,
    [DependentAge] int NULL,
    [RelationshipToDependent] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(200) NULL,
    [EmergencyContact] nvarchar(100) NULL,
    [EmergencyContactNumber] nvarchar(20) NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(1000) NULL,
    [CurrentMedications] nvarchar(500) NULL,
    [Attachments] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [AppointmentTimeInput] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ReasonForVisit] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(50) NULL,
    [AttachmentPath] nvarchar(500) NULL,
    [Prescription] nvarchar(1000) NULL,
    [Instructions] nvarchar(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FamilyMembers] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(50) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(200) NULL,
    [Age] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FamilyMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FamilyMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [RecordDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Treatment] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [ChiefComplaint] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Prescription] nvarchar(max) NOT NULL,
    [Instructions] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VitalSigns] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Temperature] decimal(5,2) NULL,
    [BloodPressure] nvarchar(20) NULL,
    [HeartRate] int NULL,
    [RespiratoryRate] int NULL,
    [SpO2] decimal(5,2) NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AppointmentFiles] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationId] int NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [MedicalRecordId] int NOT NULL,
    [MedicationName] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508150438_MakeLastNameAndSuffixNullable', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [EncryptedStatus] nvarchar(max) NOT NULL,
    [EncryptedFullName] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [PhilHealthId] nvarchar(max) NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NULL,
    [Suffix] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Medications] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientGroup] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [EmergencyContact] nvarchar(100) NOT NULL,
    [EmergencyContactNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] text NULL,
    [CurrentMedications] text NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BloodType] nvarchar(100) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [WorkingDays] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [JoinDate] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [DependentFullName] nvarchar(100) NULL,
    [DependentAge] int NULL,
    [RelationshipToDependent] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(200) NULL,
    [EmergencyContact] nvarchar(100) NULL,
    [EmergencyContactNumber] nvarchar(20) NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(1000) NULL,
    [CurrentMedications] nvarchar(500) NULL,
    [Attachments] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [AppointmentTimeInput] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ReasonForVisit] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(50) NULL,
    [AttachmentPath] nvarchar(500) NULL,
    [Prescription] nvarchar(1000) NULL,
    [Instructions] nvarchar(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FamilyMembers] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(50) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(200) NULL,
    [Age] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FamilyMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FamilyMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [RecordDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Treatment] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [ChiefComplaint] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Prescription] nvarchar(max) NOT NULL,
    [Instructions] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VitalSigns] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Temperature] decimal(5,2) NULL,
    [BloodPressure] nvarchar(20) NULL,
    [HeartRate] int NULL,
    [RespiratoryRate] int NULL,
    [SpO2] decimal(5,2) NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AppointmentFiles] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationId] int NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [MedicalRecordId] int NOT NULL,
    [MedicationName] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508151056_UpdateNullableLastNameAndSuffix', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [EncryptedStatus] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [EncryptedFullName] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [PhilHealthId] nvarchar(max) NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NULL,
    [Suffix] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Medications] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientGroup] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [EmergencyContact] nvarchar(100) NOT NULL,
    [EmergencyContactNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] text NULL,
    [CurrentMedications] text NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BloodType] nvarchar(100) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [WorkingDays] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [JoinDate] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [DependentFullName] nvarchar(100) NULL,
    [DependentAge] int NULL,
    [RelationshipToDependent] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(200) NULL,
    [EmergencyContact] nvarchar(100) NULL,
    [EmergencyContactNumber] nvarchar(20) NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(1000) NULL,
    [CurrentMedications] nvarchar(500) NULL,
    [Attachments] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [AppointmentTimeInput] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ReasonForVisit] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(50) NULL,
    [AttachmentPath] nvarchar(500) NULL,
    [Prescription] nvarchar(1000) NULL,
    [Instructions] nvarchar(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FamilyMembers] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(50) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(200) NULL,
    [Age] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FamilyMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FamilyMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [RecordDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Treatment] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [ChiefComplaint] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Prescription] nvarchar(max) NOT NULL,
    [Instructions] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VitalSigns] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Temperature] decimal(5,2) NULL,
    [BloodPressure] nvarchar(20) NULL,
    [HeartRate] int NULL,
    [RespiratoryRate] int NULL,
    [SpO2] decimal(5,2) NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AppointmentFiles] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationId] int NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [MedicalRecordId] int NOT NULL,
    [MedicationName] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508155707_AddStatusAndFixMiddleName', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [EncryptedStatus] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [EncryptedFullName] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [PhilHealthId] nvarchar(max) NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NULL,
    [Suffix] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Medications] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientGroup] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [EmergencyContact] nvarchar(100) NOT NULL,
    [EmergencyContactNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] text NULL,
    [CurrentMedications] text NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BloodType] nvarchar(100) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [WorkingDays] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [JoinDate] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [DependentFullName] nvarchar(100) NULL,
    [DependentAge] int NULL,
    [RelationshipToDependent] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(200) NULL,
    [EmergencyContact] nvarchar(100) NULL,
    [EmergencyContactNumber] nvarchar(20) NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(1000) NULL,
    [CurrentMedications] nvarchar(500) NULL,
    [Attachments] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [AppointmentTimeInput] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ReasonForVisit] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(50) NULL,
    [AttachmentPath] nvarchar(500) NULL,
    [Prescription] nvarchar(1000) NULL,
    [Instructions] nvarchar(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FamilyMembers] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(50) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(200) NULL,
    [Age] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FamilyMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FamilyMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [RecordDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Treatment] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [ChiefComplaint] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Prescription] nvarchar(max) NOT NULL,
    [Instructions] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VitalSigns] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Temperature] decimal(5,2) NULL,
    [BloodPressure] nvarchar(20) NULL,
    [HeartRate] int NULL,
    [RespiratoryRate] int NULL,
    [SpO2] decimal(5,2) NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AppointmentFiles] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationId] int NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [MedicalRecordId] int NOT NULL,
    [MedicationName] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508155825_AddStatusToApplicationUser', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [EncryptedStatus] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [EncryptedFullName] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [PhilHealthId] nvarchar(max) NOT NULL,
    [LastActive] datetime2 NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [MiddleName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NULL,
    [Suffix] nvarchar(max) NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Medications] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Category] nvarchar(100) NULL,
    [Manufacturer] nvarchar(100) NULL,
    CONSTRAINT [PK_Medications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Email] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Feedbacks] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [Message] nvarchar(500) NOT NULL,
    [Comment] nvarchar(1000) NOT NULL,
    [Rating] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Feedbacks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [SenderId] nvarchar(450) NOT NULL,
    [ReceiverId] nvarchar(450) NOT NULL,
    [Content] nvarchar(1000) NOT NULL,
    [SenderName] nvarchar(max) NOT NULL,
    [RecipientGroup] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [Gender] nvarchar(10) NOT NULL,
    [BirthDate] datetime2 NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [ContactNumber] nvarchar(20) NOT NULL,
    [EmergencyContact] nvarchar(100) NOT NULL,
    [EmergencyContactNumber] nvarchar(20) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] text NULL,
    [CurrentMedications] text NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [BloodType] nvarchar(100) NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NULL,
    [Position] nvarchar(max) NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [WorkingDays] nvarchar(max) NULL,
    [WorkingHours] nvarchar(max) NULL,
    [JoinDate] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Appointments] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PatientName] nvarchar(100) NOT NULL,
    [DependentFullName] nvarchar(100) NULL,
    [DependentAge] int NULL,
    [RelationshipToDependent] nvarchar(50) NULL,
    [Gender] nvarchar(10) NULL,
    [ContactNumber] nvarchar(20) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(200) NULL,
    [EmergencyContact] nvarchar(100) NULL,
    [EmergencyContactNumber] nvarchar(20) NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(1000) NULL,
    [CurrentMedications] nvarchar(500) NULL,
    [Attachments] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [AppointmentTimeInput] nvarchar(max) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [ReasonForVisit] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(50) NULL,
    [AttachmentPath] nvarchar(500) NULL,
    [Prescription] nvarchar(1000) NULL,
    [Instructions] nvarchar(1000) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [FamilyMembers] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Relationship] nvarchar(50) NOT NULL,
    [ContactNumber] nvarchar(20) NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(200) NULL,
    [Age] int NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    CONSTRAINT [PK_FamilyMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FamilyMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [RecordDate] datetime2 NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Treatment] nvarchar(1000) NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [ChiefComplaint] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Prescription] nvarchar(max) NOT NULL,
    [Instructions] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [VitalSigns] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [Temperature] decimal(5,2) NULL,
    [BloodPressure] nvarchar(20) NULL,
    [HeartRate] int NULL,
    [RespiratoryRate] int NULL,
    [SpO2] decimal(5,2) NULL,
    [Weight] decimal(5,2) NULL,
    [Height] decimal(5,2) NULL,
    [RecordedAt] datetime2 NOT NULL,
    [Notes] nvarchar(1000) NULL,
    CONSTRAINT [PK_VitalSigns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AppointmentFiles] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AppointmentFiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationId] int NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [MedicalRecordId] int NOT NULL,
    [MedicationName] nvarchar(max) NOT NULL,
    [Duration] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250508164043_AddMissingStatusColumn', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250511082823_UpdateDatabase_20250511_162811', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId]
                    PRINT 'Dropped FK_Messages_AspNetUsers_SenderId constraint'
                END
                IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
                BEGIN
                    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId]
                    PRINT 'Dropped FK_Messages_AspNetUsers_ReceiverId constraint'
                END
GO

                ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
                    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
                ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
                    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250511083240_FixCascadePaths', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250511084736_AddMissingAspNetUsersColumns', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'Title');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Notifications] ALTER COLUMN [Title] nvarchar(max) NOT NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Notifications]') AND [c].[name] = N'Message');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Notifications] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Notifications] ALTER COLUMN [Message] nvarchar(max) NOT NULL;
GO

ALTER TABLE [Notifications] ADD [Link] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Notifications] ADD [ReadAt] datetime2 NULL;
GO

CREATE TABLE [UserDocuments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [DocumentType] nvarchar(50) NOT NULL,
    [FilePath] nvarchar(255) NOT NULL,
    [FileName] nvarchar(255) NOT NULL,
    [FileSize] bigint NOT NULL,
    [ContentType] nvarchar(100) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [ApprovedAt] datetime2 NULL,
    [ApprovedBy] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_UserDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250511101239_AddMissingNotificationColumns', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [AgreedAt] datetime2 NULL;
GO

ALTER TABLE [AspNetUsers] ADD [HasAgreedToTerms] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250511120000_AddAgreedToTermsColumns', N'8.0.2');
GO

COMMIT;
GO

