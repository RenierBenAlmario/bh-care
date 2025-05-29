IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
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
    [FullName] nvarchar(500) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [ProfileImage] nvarchar(max) NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [LastActive] datetime2 NULL,
    [JoinDate] datetime2 NULL,
    [BirthDate] datetime2 NULL,
    [Address] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NULL,
    [Gender] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ProfilePicture] nvarchar(max) NULL,
    [PhilHealthId] nvarchar(max) NULL,
    [FirstName] nvarchar(max) NULL,
    [LastName] nvarchar(max) NULL,
    [EncryptedWorkingHours] nvarchar(max) NULL,
    [EncryptedStatus] nvarchar(max) NULL,
    [EncryptedFullName] nvarchar(max) NULL,
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
    CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Patients] (
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Status] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Room] nvarchar(20) NULL,
    [Diagnosis] nvarchar(500) NULL,
    [Alert] nvarchar(500) NULL,
    [Time] time NULL,
    [Address] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [EmergencyContact] nvarchar(max) NULL,
    [EmergencyContactNumber] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [UpdatedAt] datetime2 NULL,
    [Allergies] nvarchar(500) NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [CurrentMedications] nvarchar(max) NULL,
    [FullName] nvarchar(max) NOT NULL,
    [BirthDate] datetime2 NULL,
    CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [DateIssued] datetime2 NOT NULL,
    [Medications] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id])
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
    [PatientId] nvarchar(450) NULL,
    [PatientName] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NULL,
    [Address] nvarchar(max) NULL,
    [EmergencyContact] nvarchar(max) NULL,
    [EmergencyContactNumber] nvarchar(max) NULL,
    [Allergies] nvarchar(max) NULL,
    [MedicalHistory] nvarchar(max) NULL,
    [CurrentMedications] nvarchar(max) NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [AppointmentTime] time NOT NULL,
    [DoctorId] nvarchar(450) NULL,
    [ReasonForVisit] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [AgeValue] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] nvarchar(max) NULL,
    [AttachmentPath] nvarchar(max) NULL,
    [Prescription] nvarchar(max) NULL,
    [Instructions] nvarchar(max) NULL,
    [Attachments] nvarchar(max) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
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
    [DoctorId] nvarchar(max) NULL,
    [Date] datetime2 NOT NULL,
    [Type] nvarchar(max) NULL,
    [Status] nvarchar(max) NULL,
    [ChiefComplaint] nvarchar(max) NULL,
    [Diagnosis] nvarchar(max) NOT NULL,
    [Treatment] nvarchar(max) NULL,
    [Notes] nvarchar(max) NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
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
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE CASCADE
);
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

CREATE INDEX [IX_MedicalRecords_PatientId] ON [MedicalRecords] ([PatientId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE UNIQUE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250424173237_InitialCreate', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'CreatedAt');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [CreatedAt];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'DateIssued');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [DateIssued];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Medications');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [Medications];
GO

EXEC sp_rename N'[Prescriptions].[UpdatedAt]', N'PrescriptionDate', N'COLUMN';
GO

ALTER TABLE [Prescriptions] ADD [Notes] nvarchar(1000) NULL;
GO

ALTER TABLE [Patients] ADD [Weight] decimal(5,2) NULL;
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

CREATE TABLE [PrescriptionMedications] (
    [Id] int NOT NULL IDENTITY,
    [MedicationName] nvarchar(200) NOT NULL,
    [Dosage] nvarchar(100) NOT NULL,
    [Frequency] nvarchar(100) NOT NULL,
    [Duration] nvarchar(100) NOT NULL,
    [Instructions] nvarchar(500) NULL,
    [PrescriptionId] int NOT NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250426063930_UpdatePrescriptionSchema', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Name');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Patients] DROP COLUMN [Name];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'DateOfBirth');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [DateOfBirth];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'FullName');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Patients] ALTER COLUMN [FullName] nvarchar(100) NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250428154514_AddNameToPatient', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Prescriptions] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Prescriptions] ADD [Dosage] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Prescriptions] ADD [Instructions] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Prescriptions] ADD [Medication] nvarchar(max) NOT NULL DEFAULT N'';
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'MedicalHistory');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [Patients] ALTER COLUMN [MedicalHistory] text NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Gender');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Patients] ALTER COLUMN [Gender] nvarchar(10) NOT NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'EmergencyContactNumber');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var8 + '];');
UPDATE [Patients] SET [EmergencyContactNumber] = N'' WHERE [EmergencyContactNumber] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [EmergencyContactNumber] nvarchar(20) NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT N'' FOR [EmergencyContactNumber];
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'EmergencyContact');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var9 + '];');
UPDATE [Patients] SET [EmergencyContact] = N'' WHERE [EmergencyContact] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [EmergencyContact] nvarchar(100) NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT N'' FOR [EmergencyContact];
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Email');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var10 + '];');
UPDATE [Patients] SET [Email] = N'' WHERE [Email] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [Email] nvarchar(100) NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT N'' FOR [Email];
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'CurrentMedications');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [Patients] ALTER COLUMN [CurrentMedications] text NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'ContactNumber');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var12 + '];');
UPDATE [Patients] SET [ContactNumber] = N'' WHERE [ContactNumber] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [ContactNumber] nvarchar(20) NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT N'' FOR [ContactNumber];
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'BirthDate');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var13 + '];');
UPDATE [Patients] SET [BirthDate] = '0001-01-01T00:00:00.0000000' WHERE [BirthDate] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [BirthDate] datetime2 NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [BirthDate];
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Address');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var14 + '];');
UPDATE [Patients] SET [Address] = N'' WHERE [Address] IS NULL;
ALTER TABLE [Patients] ALTER COLUMN [Address] nvarchar(200) NOT NULL;
ALTER TABLE [Patients] ADD DEFAULT N'' FOR [Address];
GO

ALTER TABLE [Patients] ADD [BloodType] nvarchar(100) NULL;
GO

ALTER TABLE [Patients] ADD [Height] decimal(5,2) NULL;
GO

ALTER TABLE [MedicalRecords] ADD [Duration] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250429102719_AddHeightAndBloodTypeToPatient', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250429103542_AddReportRelatedTables', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Gender');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [Patients] ALTER COLUMN [Gender] nvarchar(50) NOT NULL;
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'AppointmentTime');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [AppointmentTime] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250430155358_FixTimeSpanHandling', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] ADD [DependentAge] int NULL;
GO

ALTER TABLE [Appointments] ADD [DependentFullName] nvarchar(max) NULL;
GO

ALTER TABLE [Appointments] ADD [RelationshipToDependent] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250501072124_AddDependentFields', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Patients_PatientId];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'UpdatedAt');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [Appointments] ADD DEFAULT (GETDATE()) FOR [UpdatedAt];
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Type');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Type] nvarchar(50) NULL;
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Status');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Status] nvarchar(20) NOT NULL;
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'RelationshipToDependent');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [RelationshipToDependent] nvarchar(50) NULL;
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'ReasonForVisit');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var21 + '];');
UPDATE [Appointments] SET [ReasonForVisit] = N'' WHERE [ReasonForVisit] IS NULL;
ALTER TABLE [Appointments] ALTER COLUMN [ReasonForVisit] nvarchar(1000) NOT NULL;
ALTER TABLE [Appointments] ADD DEFAULT N'' FOR [ReasonForVisit];
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Prescription');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Prescription] nvarchar(1000) NULL;
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'PatientName');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [PatientName] nvarchar(100) NOT NULL;
GO

DROP INDEX [IX_Appointments_PatientId] ON [Appointments];
DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'PatientId');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var24 + '];');
UPDATE [Appointments] SET [PatientId] = N'' WHERE [PatientId] IS NULL;
ALTER TABLE [Appointments] ALTER COLUMN [PatientId] nvarchar(450) NOT NULL;
ALTER TABLE [Appointments] ADD DEFAULT N'' FOR [PatientId];
CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'MedicalHistory');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [MedicalHistory] nvarchar(1000) NULL;
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Instructions');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Instructions] nvarchar(1000) NULL;
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Gender');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Gender] nvarchar(10) NULL;
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'EmergencyContactNumber');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [EmergencyContactNumber] nvarchar(20) NULL;
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'EmergencyContact');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [EmergencyContact] nvarchar(100) NULL;
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Description');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Description] nvarchar(1000) NULL;
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DependentFullName');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [DependentFullName] nvarchar(100) NULL;
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'CurrentMedications');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [CurrentMedications] nvarchar(500) NULL;
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'CreatedAt');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [Appointments] ADD DEFAULT (GETDATE()) FOR [CreatedAt];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'ContactNumber');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [ContactNumber] nvarchar(20) NULL;
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'AttachmentPath');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [AttachmentPath] nvarchar(500) NULL;
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Allergies');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Allergies] nvarchar(500) NULL;
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Address');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Address] nvarchar(200) NULL;
GO

ALTER TABLE [Appointments] ADD [PatientUserId] nvarchar(450) NULL;
GO

CREATE INDEX [IX_Appointments_PatientUserId] ON [Appointments] ([PatientUserId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250501072455_FixTimeSpanAndAgeHandling', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId];
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_PatientId];
GO

ALTER TABLE [Feedbacks] DROP CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
GO

ALTER TABLE [Notifications] DROP CONSTRAINT [FK_Notifications_AspNetUsers_UserId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_Patients_PatientId];
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Dosage');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [Dosage];
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Instructions');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [Instructions];
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Medication');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [Medication];
GO

ALTER TABLE [Prescriptions] ADD [ApplicationUserId] nvarchar(450) NULL;
GO

ALTER TABLE [Prescriptions] ADD [ApplicationUserId1] nvarchar(450) NULL;
GO

ALTER TABLE [Prescriptions] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'MedicationName');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [MedicationName] nvarchar(100) NOT NULL;
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Instructions');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var42 + '];');
UPDATE [PrescriptionMedications] SET [Instructions] = N'' WHERE [Instructions] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Instructions] nvarchar(500) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [Instructions];
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Frequency');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Frequency] nvarchar(50) NOT NULL;
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Duration');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Duration] nvarchar(50) NOT NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Dosage');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Dosage] nvarchar(50) NOT NULL;
GO

ALTER TABLE [PrescriptionMedications] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [PrescriptionMedications] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Type');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var46 + '];');
UPDATE [MedicalRecords] SET [Type] = N'' WHERE [Type] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Type] nvarchar(50) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Type];
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Treatment');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Treatment] nvarchar(2000) NULL;
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Status');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Status] nvarchar(100) NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Notes');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Notes] nvarchar(2000) NULL;
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'DoctorId');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var50 + '];');
UPDATE [MedicalRecords] SET [DoctorId] = N'' WHERE [DoctorId] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [DoctorId] nvarchar(450) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [DoctorId];
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Diagnosis');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Diagnosis] nvarchar(500) NOT NULL;
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'ChiefComplaint');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [ChiefComplaint] nvarchar(500) NULL;
GO

ALTER TABLE [MedicalRecords] ADD [ApplicationUserId] nvarchar(450) NULL;
GO

ALTER TABLE [MedicalRecords] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [MedicalRecords] ADD [Instructions] nvarchar(1000) NULL;
GO

ALTER TABLE [MedicalRecords] ADD [Prescription] nvarchar(1000) NULL;
GO

ALTER TABLE [MedicalRecords] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Feedbacks]') AND [c].[name] = N'Message');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [Feedbacks] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [Feedbacks] ALTER COLUMN [Message] nvarchar(1000) NOT NULL;
GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'WorkingHours');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [WorkingHours] nvarchar(50) NOT NULL;
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Specialization');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Specialization] nvarchar(100) NOT NULL;
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumber');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [PhoneNumber] nvarchar(20) NULL;
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FullName');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [FullName] nvarchar(100) NOT NULL;
GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Address');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Address] nvarchar(200) NULL;
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'AppointmentTime');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var59 + '];');
UPDATE [Appointments] SET [AppointmentTime] = '00:00:00' WHERE [AppointmentTime] IS NULL;
ALTER TABLE [Appointments] ALTER COLUMN [AppointmentTime] time NOT NULL;
ALTER TABLE [Appointments] ADD DEFAULT '00:00:00' FOR [AppointmentTime];
GO

CREATE INDEX [IX_Prescriptions_ApplicationUserId] ON [Prescriptions] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Prescriptions_ApplicationUserId1] ON [Prescriptions] ([ApplicationUserId1]);
GO

CREATE INDEX [IX_MedicalRecords_ApplicationUserId] ON [MedicalRecords] ([ApplicationUserId]);
GO

CREATE INDEX [IX_MedicalRecords_DoctorId] ON [MedicalRecords] ([DoctorId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Feedbacks] ADD CONSTRAINT [FK_Feedbacks_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Notifications] ADD CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId1] FOREIGN KEY ([ApplicationUserId1]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250501163902_FixNavigationPropertiesAndReferences', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId];
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_PatientId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_AspNetUsers_ApplicationUserId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId1];
GO

DROP INDEX [IX_Prescriptions_ApplicationUserId] ON [Prescriptions];
GO

DROP INDEX [IX_Prescriptions_ApplicationUserId1] ON [Prescriptions];
GO

DROP INDEX [IX_MedicalRecords_ApplicationUserId] ON [MedicalRecords];
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'ApplicationUserId');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [ApplicationUserId];
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'ApplicationUserId1');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [Prescriptions] DROP COLUMN [ApplicationUserId1];
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'ApplicationUserId');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var62 + '];');
ALTER TABLE [MedicalRecords] DROP COLUMN [ApplicationUserId];
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'ReasonForVisit');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [ReasonForVisit] nvarchar(500) NOT NULL;
GO

DROP INDEX [IX_Appointments_DoctorId] ON [Appointments];
DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'DoctorId');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var64 + '];');
UPDATE [Appointments] SET [DoctorId] = N'' WHERE [DoctorId] IS NULL;
ALTER TABLE [Appointments] ALTER COLUMN [DoctorId] nvarchar(450) NOT NULL;
ALTER TABLE [Appointments] ADD DEFAULT N'' FOR [DoctorId];
CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Description');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Description] nvarchar(500) NULL;
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502053106_AppointmentModelUpdate', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502053121_FixAppointmentValidation', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502053134_NewMigrationName', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] ADD [AppointmentTimeInput] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502115714_AddAppointmentTimeInput', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502120152_AddAppointmentTimeInputV2', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId];
GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'PrescriptionId');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var66 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [PrescriptionId] int NULL;
GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'MedicationName');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var67 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [MedicationName] nvarchar(100) NULL;
GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Instructions');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Instructions] nvarchar(500) NULL;
GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Frequency');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var69 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Frequency] nvarchar(50) NULL;
GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Duration');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var70 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Duration] nvarchar(50) NULL;
GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Dosage');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Dosage] nvarchar(50) NULL;
GO

ALTER TABLE [PrescriptionMedications] ADD [MedicalRecordId] int NULL;
GO

CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

ALTER TABLE [PrescriptionMedications] ADD CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [PrescriptionMedications] ADD CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250502162642_AddPrescriptionMedicationsToMedicalRecords', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId];
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_PatientId];
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Patients_PatientUserId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_AspNetUsers_PatientId];
GO

ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
GO

ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_PatientId];
GO

ALTER TABLE [VitalSigns] DROP CONSTRAINT [FK_VitalSigns_Patients_PatientId];
GO

DROP INDEX [IX_StaffMembers_UserId] ON [StaffMembers];
GO

DROP INDEX [IX_Appointments_PatientUserId] ON [Appointments];
GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'CreatedAt');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [PrescriptionMedications] DROP COLUMN [CreatedAt];
GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'UpdatedAt');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [PrescriptionMedications] DROP COLUMN [UpdatedAt];
GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'EncryptedWorkingHours');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [AspNetUsers] DROP COLUMN [EncryptedWorkingHours];
GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'PatientUserId');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [Appointments] DROP COLUMN [PatientUserId];
GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'UpdatedAt');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [Prescriptions] ALTER COLUMN [UpdatedAt] datetime2 NULL;
GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Status');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [Prescriptions] ALTER COLUMN [Status] int NOT NULL;
GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'Notes');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var78 + '];');
UPDATE [Prescriptions] SET [Notes] = N'' WHERE [Notes] IS NULL;
ALTER TABLE [Prescriptions] ALTER COLUMN [Notes] nvarchar(1000) NOT NULL;
ALTER TABLE [Prescriptions] ADD DEFAULT N'' FOR [Notes];
GO

DROP INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications];
DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'PrescriptionId');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var79 + '];');
UPDATE [PrescriptionMedications] SET [PrescriptionId] = 0 WHERE [PrescriptionId] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [PrescriptionId] int NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT 0 FOR [PrescriptionId];
CREATE INDEX [IX_PrescriptionMedications_PrescriptionId] ON [PrescriptionMedications] ([PrescriptionId]);
GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'MedicationName');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var80 + '];');
UPDATE [PrescriptionMedications] SET [MedicationName] = N'' WHERE [MedicationName] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [MedicationName] nvarchar(max) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [MedicationName];
GO

DROP INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications];
DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'MedicalRecordId');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var81 + '];');
UPDATE [PrescriptionMedications] SET [MedicalRecordId] = 0 WHERE [MedicalRecordId] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [MedicalRecordId] int NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT 0 FOR [MedicalRecordId];
CREATE INDEX [IX_PrescriptionMedications_MedicalRecordId] ON [PrescriptionMedications] ([MedicalRecordId]);
GO

DECLARE @var82 sysname;
SELECT @var82 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Instructions');
IF @var82 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var82 + '];');
UPDATE [PrescriptionMedications] SET [Instructions] = N'' WHERE [Instructions] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Instructions] nvarchar(500) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [Instructions];
GO

DECLARE @var83 sysname;
SELECT @var83 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Frequency');
IF @var83 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var83 + '];');
UPDATE [PrescriptionMedications] SET [Frequency] = N'' WHERE [Frequency] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Frequency] nvarchar(100) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [Frequency];
GO

DECLARE @var84 sysname;
SELECT @var84 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Duration');
IF @var84 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var84 + '];');
UPDATE [PrescriptionMedications] SET [Duration] = N'' WHERE [Duration] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Duration] nvarchar(max) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [Duration];
GO

DECLARE @var85 sysname;
SELECT @var85 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'Dosage');
IF @var85 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var85 + '];');
UPDATE [PrescriptionMedications] SET [Dosage] = N'' WHERE [Dosage] IS NULL;
ALTER TABLE [PrescriptionMedications] ALTER COLUMN [Dosage] nvarchar(100) NOT NULL;
ALTER TABLE [PrescriptionMedications] ADD DEFAULT N'' FOR [Dosage];
GO

ALTER TABLE [PrescriptionMedications] ADD [MedicationId] int NOT NULL DEFAULT 0;
GO

DECLARE @var86 sysname;
SELECT @var86 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Gender');
IF @var86 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var86 + '];');
ALTER TABLE [Patients] ALTER COLUMN [Gender] nvarchar(10) NOT NULL;
GO

DECLARE @var87 sysname;
SELECT @var87 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'UpdatedAt');
IF @var87 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var87 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [UpdatedAt] datetime2 NULL;
GO

DECLARE @var88 sysname;
SELECT @var88 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Type');
IF @var88 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var88 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Type] nvarchar(max) NOT NULL;
GO

DECLARE @var89 sysname;
SELECT @var89 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Treatment');
IF @var89 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var89 + '];');
UPDATE [MedicalRecords] SET [Treatment] = N'' WHERE [Treatment] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Treatment] nvarchar(1000) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Treatment];
GO

DECLARE @var90 sysname;
SELECT @var90 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Status');
IF @var90 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var90 + '];');
UPDATE [MedicalRecords] SET [Status] = N'' WHERE [Status] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Status] nvarchar(max) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Status];
GO

DECLARE @var91 sysname;
SELECT @var91 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Prescription');
IF @var91 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var91 + '];');
UPDATE [MedicalRecords] SET [Prescription] = N'' WHERE [Prescription] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Prescription] nvarchar(max) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Prescription];
GO

DECLARE @var92 sysname;
SELECT @var92 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Notes');
IF @var92 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var92 + '];');
UPDATE [MedicalRecords] SET [Notes] = N'' WHERE [Notes] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Notes] nvarchar(1000) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Notes];
GO

DECLARE @var93 sysname;
SELECT @var93 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Instructions');
IF @var93 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var93 + '];');
UPDATE [MedicalRecords] SET [Instructions] = N'' WHERE [Instructions] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [Instructions] nvarchar(max) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [Instructions];
GO

DECLARE @var94 sysname;
SELECT @var94 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Duration');
IF @var94 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var94 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Duration] nvarchar(max) NOT NULL;
GO

DECLARE @var95 sysname;
SELECT @var95 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'Diagnosis');
IF @var95 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var95 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [Diagnosis] nvarchar(1000) NOT NULL;
GO

DECLARE @var96 sysname;
SELECT @var96 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'ChiefComplaint');
IF @var96 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var96 + '];');
UPDATE [MedicalRecords] SET [ChiefComplaint] = N'' WHERE [ChiefComplaint] IS NULL;
ALTER TABLE [MedicalRecords] ALTER COLUMN [ChiefComplaint] nvarchar(max) NOT NULL;
ALTER TABLE [MedicalRecords] ADD DEFAULT N'' FOR [ChiefComplaint];
GO

ALTER TABLE [MedicalRecords] ADD [Medications] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [MedicalRecords] ADD [RecordDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var97 sysname;
SELECT @var97 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Feedbacks]') AND [c].[name] = N'Message');
IF @var97 IS NOT NULL EXEC(N'ALTER TABLE [Feedbacks] DROP CONSTRAINT [' + @var97 + '];');
ALTER TABLE [Feedbacks] ALTER COLUMN [Message] nvarchar(500) NOT NULL;
GO

DECLARE @var98 sysname;
SELECT @var98 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'WorkingHours');
IF @var98 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var98 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [WorkingHours] nvarchar(max) NOT NULL;
GO

DECLARE @var99 sysname;
SELECT @var99 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Specialization');
IF @var99 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var99 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [Specialization] nvarchar(max) NOT NULL;
GO

DECLARE @var100 sysname;
SELECT @var100 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'ProfilePicture');
IF @var100 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var100 + '];');
UPDATE [AspNetUsers] SET [ProfilePicture] = N'' WHERE [ProfilePicture] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [ProfilePicture] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [ProfilePicture];
GO

DECLARE @var101 sysname;
SELECT @var101 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhoneNumber');
IF @var101 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var101 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [PhoneNumber] nvarchar(max) NULL;
GO

DECLARE @var102 sysname;
SELECT @var102 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'PhilHealthId');
IF @var102 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var102 + '];');
UPDATE [AspNetUsers] SET [PhilHealthId] = N'' WHERE [PhilHealthId] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [PhilHealthId] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [PhilHealthId];
GO

DECLARE @var103 sysname;
SELECT @var103 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var103 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var103 + '];');
UPDATE [AspNetUsers] SET [LastName] = N'' WHERE [LastName] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [LastName];
GO

DECLARE @var104 sysname;
SELECT @var104 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastActive');
IF @var104 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var104 + '];');
UPDATE [AspNetUsers] SET [LastActive] = '0001-01-01T00:00:00.0000000' WHERE [LastActive] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastActive] datetime2 NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [LastActive];
GO

DECLARE @var105 sysname;
SELECT @var105 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'JoinDate');
IF @var105 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var105 + '];');
UPDATE [AspNetUsers] SET [JoinDate] = '0001-01-01T00:00:00.0000000' WHERE [JoinDate] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [JoinDate] datetime2 NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [JoinDate];
GO

DECLARE @var106 sysname;
SELECT @var106 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Gender');
IF @var106 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var106 + '];');
UPDATE [AspNetUsers] SET [Gender] = N'' WHERE [Gender] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [Gender] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [Gender];
GO

DECLARE @var107 sysname;
SELECT @var107 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FullName');
IF @var107 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var107 + '];');
ALTER TABLE [AspNetUsers] ALTER COLUMN [FullName] nvarchar(max) NOT NULL;
GO

DECLARE @var108 sysname;
SELECT @var108 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'FirstName');
IF @var108 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var108 + '];');
UPDATE [AspNetUsers] SET [FirstName] = N'' WHERE [FirstName] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [FirstName] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [FirstName];
GO

DECLARE @var109 sysname;
SELECT @var109 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'EncryptedStatus');
IF @var109 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var109 + '];');
UPDATE [AspNetUsers] SET [EncryptedStatus] = N'' WHERE [EncryptedStatus] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [EncryptedStatus] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [EncryptedStatus];
GO

DECLARE @var110 sysname;
SELECT @var110 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'EncryptedFullName');
IF @var110 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var110 + '];');
UPDATE [AspNetUsers] SET [EncryptedFullName] = N'' WHERE [EncryptedFullName] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [EncryptedFullName] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [EncryptedFullName];
GO

DECLARE @var111 sysname;
SELECT @var111 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Address');
IF @var111 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var111 + '];');
UPDATE [AspNetUsers] SET [Address] = N'' WHERE [Address] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [Address] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [Address];
GO

DECLARE @var112 sysname;
SELECT @var112 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'UpdatedAt');
IF @var112 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var112 + '];');
GO

DECLARE @var113 sysname;
SELECT @var113 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Status');
IF @var113 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var113 + '];');
ALTER TABLE [Appointments] ALTER COLUMN [Status] int NOT NULL;
GO

DECLARE @var114 sysname;
SELECT @var114 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'Description');
IF @var114 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var114 + '];');
UPDATE [Appointments] SET [Description] = N'' WHERE [Description] IS NULL;
ALTER TABLE [Appointments] ALTER COLUMN [Description] nvarchar(500) NOT NULL;
ALTER TABLE [Appointments] ADD DEFAULT N'' FOR [Description];
GO

DECLARE @var115 sysname;
SELECT @var115 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Appointments]') AND [c].[name] = N'CreatedAt');
IF @var115 IS NOT NULL EXEC(N'ALTER TABLE [Appointments] DROP CONSTRAINT [' + @var115 + '];');
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

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedications_MedicationId] ON [PrescriptionMedications] ([MedicationId]);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [PrescriptionMedications] ADD CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [VitalSigns] ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250507111754_FixApplicationUserProperties', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
GO

ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250507112314_FixMessageUserRelationships', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AspNetUsers] ADD [MiddleName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Suffix] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250507120943_AddMiddleNameAndSuffixToUser', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250507121025_AddMiddleNameAndSuffixToApplicationUser', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var116 sysname;
SELECT @var116 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'BirthDate');
IF @var116 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var116 + '];');
UPDATE [AspNetUsers] SET [BirthDate] = '2000-01-01T00:00:00.0000000' WHERE [BirthDate] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [BirthDate] datetime2 NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT '2000-01-01T00:00:00.0000000' FOR [BirthDate];
GO

UPDATE AspNetUsers SET BirthDate = '2000-01-01' WHERE BirthDate IS NULL
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250603000001_UpdateBirthDateToNonNullable', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId];
GO

ALTER TABLE [PrescriptionMedications] ADD CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250603000002_FixPrescriptionMedicationRelationship', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [FamilyMembers] DROP CONSTRAINT [FK_FamilyMembers_Patients_PatientId];
GO

ALTER TABLE [Patients] DROP CONSTRAINT [PK_Patients];
GO

DECLARE @var117 sysname;
SELECT @var117 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Patients]') AND [c].[name] = N'Id');
IF @var117 IS NOT NULL EXEC(N'ALTER TABLE [Patients] DROP CONSTRAINT [' + @var117 + '];');
ALTER TABLE [Patients] DROP COLUMN [Id];
GO

ALTER TABLE [Patients] ADD CONSTRAINT [PK_Patients] PRIMARY KEY ([UserId]);
GO

ALTER TABLE [FamilyMembers] ADD CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250603000003_FixPatientFamilyMemberRelationship', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [VitalSigns] DROP CONSTRAINT [FK_VitalSigns_Patients_PatientId];
GO

ALTER TABLE [FamilyMembers] DROP CONSTRAINT [FK_FamilyMembers_Patients_PatientId];
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_Patients_PatientUserId];
GO

ALTER TABLE [MedicalRecords] DROP CONSTRAINT [FK_MedicalRecords_Patients_PatientId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_Patients_PatientId];
GO

DECLARE @var118 sysname;
SELECT @var118 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VitalSigns]') AND [c].[name] = N'PatientId');
IF @var118 IS NOT NULL EXEC(N'ALTER TABLE [VitalSigns] DROP CONSTRAINT [' + @var118 + '];');
ALTER TABLE [VitalSigns] ALTER COLUMN [PatientId] nvarchar(450) NULL;
GO

DECLARE @var119 sysname;
SELECT @var119 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FamilyMembers]') AND [c].[name] = N'PatientId');
IF @var119 IS NOT NULL EXEC(N'ALTER TABLE [FamilyMembers] DROP CONSTRAINT [' + @var119 + '];');
ALTER TABLE [FamilyMembers] ALTER COLUMN [PatientId] nvarchar(450) NULL;
GO

DECLARE @var120 sysname;
SELECT @var120 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[MedicalRecords]') AND [c].[name] = N'PatientId');
IF @var120 IS NOT NULL EXEC(N'ALTER TABLE [MedicalRecords] DROP CONSTRAINT [' + @var120 + '];');
ALTER TABLE [MedicalRecords] ALTER COLUMN [PatientId] nvarchar(450) NULL;
GO

DECLARE @var121 sysname;
SELECT @var121 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'PatientId');
IF @var121 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var121 + '];');
ALTER TABLE [Prescriptions] ALTER COLUMN [PatientId] nvarchar(450) NULL;
GO

ALTER TABLE [VitalSigns] ADD CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [FamilyMembers] ADD CONSTRAINT [FK_FamilyMembers_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [MedicalRecords] ADD CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

                -- Update or remove orphaned records
                UPDATE VitalSigns
                SET PatientId = NULL
                WHERE PatientId NOT IN (SELECT UserId FROM Patients);
                UPDATE FamilyMembers
                SET PatientId = NULL
                WHERE PatientId NOT IN (SELECT UserId FROM Patients);
                UPDATE MedicalRecords
                SET PatientId = NULL
                WHERE PatientId NOT IN (SELECT UserId FROM Patients);
                UPDATE Prescriptions
                SET PatientId = NULL
                WHERE PatientId NOT IN (SELECT UserId FROM Patients);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250603000005_FixRemainingForeignKeyConstraints', N'8.0.2');
GO

COMMIT;
GO

