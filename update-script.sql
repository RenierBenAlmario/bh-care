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
    [HasAgreedToTerms] bit NOT NULL,
    [AgreedAt] datetime2 NULL,
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

CREATE TABLE [Assessments] (
    [Id] int NOT NULL IDENTITY,
    [FamilyNumber] nvarchar(max) NOT NULL,
    [ReasonForVisit] nvarchar(max) NOT NULL,
    [Symptoms] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Assessments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ConsultationTimeSlots] (
    [Id] int NOT NULL IDENTITY,
    [ConsultationType] nvarchar(max) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [IsBooked] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ConsultationTimeSlots] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FamilyRecords] (
    [Id] int NOT NULL IDENTITY,
    [FamilyNumber] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FamilyRecords] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [HEEADSSSAssessments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [AppointmentId] int NOT NULL,
    [HealthFacility] nvarchar(max) NOT NULL,
    [FamilyNo] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Age] int NOT NULL,
    [HomeFamilyProblems] nvarchar(max) NOT NULL,
    [HomeParentalListening] nvarchar(max) NOT NULL,
    [HomeParentalBlame] nvarchar(max) NOT NULL,
    [HomeFamilyChanges] nvarchar(max) NOT NULL,
    [EducationCurrentlyStudying] nvarchar(max) NOT NULL,
    [EducationWorking] nvarchar(max) NOT NULL,
    [EducationSchoolWorkProblems] nvarchar(max) NOT NULL,
    [EducationBullying] nvarchar(max) NOT NULL,
    [EatingBodyImageSatisfaction] nvarchar(max) NOT NULL,
    [EatingDisorderedEatingBehaviors] nvarchar(max) NOT NULL,
    [EatingWeightComments] nvarchar(max) NOT NULL,
    [ActivitiesParticipation] nvarchar(max) NOT NULL,
    [ActivitiesRegularExercise] nvarchar(max) NOT NULL,
    [ActivitiesScreenTime] nvarchar(max) NOT NULL,
    [DrugsTobaccoUse] nvarchar(max) NOT NULL,
    [DrugsAlcoholUse] nvarchar(max) NOT NULL,
    [DrugsIllicitDrugUse] nvarchar(max) NOT NULL,
    [SexualityBodyConcerns] nvarchar(max) NOT NULL,
    [SexualityIntimateRelationships] nvarchar(max) NOT NULL,
    [SexualityPartners] nvarchar(max) NOT NULL,
    [SexualitySexualOrientation] nvarchar(max) NOT NULL,
    [SexualityPregnancy] nvarchar(max) NOT NULL,
    [SexualitySTI] nvarchar(max) NOT NULL,
    [SexualityProtection] nvarchar(max) NOT NULL,
    [SafetyPhysicalAbuse] nvarchar(max) NOT NULL,
    [SafetyRelationshipViolence] nvarchar(max) NOT NULL,
    [SafetyProtectiveGear] nvarchar(max) NOT NULL,
    [SafetyGunsAtHome] nvarchar(max) NOT NULL,
    [SuicideDepressionFeelings] nvarchar(max) NOT NULL,
    [SuicideSelfHarmThoughts] nvarchar(max) NOT NULL,
    [SuicideFamilyHistory] nvarchar(max) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [AssessedBy] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_HEEADSSSAssessments] PRIMARY KEY ([Id])
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

CREATE TABLE [NCDRiskAssessments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [AppointmentId] int NOT NULL,
    [HealthFacility] nvarchar(max) NOT NULL,
    [FamilyNo] nvarchar(max) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Barangay] nvarchar(max) NOT NULL,
    [Birthday] datetime2 NULL,
    [Telepono] nvarchar(max) NOT NULL,
    [Edad] int NULL,
    [Kasarian] nvarchar(max) NOT NULL,
    [Relihiyon] nvarchar(max) NOT NULL,
    [HasDiabetes] bit NOT NULL,
    [HasHypertension] bit NOT NULL,
    [HasCancer] bit NOT NULL,
    [HasCOPD] bit NOT NULL,
    [HasLungDisease] bit NOT NULL,
    [HasEyeDisease] bit NOT NULL,
    [CancerType] nvarchar(max) NOT NULL,
    [HasChestPain] bit NOT NULL,
    [ChestPainLocation] bit NOT NULL,
    [ChestPainWhenWalking] bit NOT NULL,
    [StopsWhenPain] bit NOT NULL,
    [PainRelievedWithRest] bit NOT NULL,
    [PainGoneIn10Min] bit NOT NULL,
    [PainMoreThan30Min] bit NOT NULL,
    [HasNumbness] bit NOT NULL,
    [FamilyHasHypertension] bit NOT NULL,
    [FamilyHasHeartDisease] bit NOT NULL,
    [FamilyHasStroke] bit NOT NULL,
    [FamilyHasDiabetes] bit NOT NULL,
    [FamilyHasCancer] bit NOT NULL,
    [FamilyHasKidneyDisease] bit NOT NULL,
    [FamilyHasOtherDisease] bit NOT NULL,
    [FamilyOtherDiseaseDetails] nvarchar(max) NOT NULL,
    [EatsVegetables] bit NOT NULL,
    [EatsFruits] bit NOT NULL,
    [EatsFish] bit NOT NULL,
    [EatsMeat] bit NOT NULL,
    [EatsProcessedFood] bit NOT NULL,
    [HasHighSaltIntake] bit NOT NULL,
    [EatsSaltyFood] bit NOT NULL,
    [EatsSweetFood] bit NOT NULL,
    [EatsFattyFood] bit NOT NULL,
    [DrinksAlcohol] bit NOT NULL,
    [DrinksBeer] bit NOT NULL,
    [DrinksWine] bit NOT NULL,
    [DrinksHardLiquor] bit NOT NULL,
    [IsBingeDrinker] bit NOT NULL,
    [AlcoholFrequency] nvarchar(max) NOT NULL,
    [HasRegularExercise] bit NOT NULL,
    [HasNoRegularExercise] bit NOT NULL,
    [ExerciseDuration] nvarchar(max) NOT NULL,
    [RiskStatus] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_NCDRiskAssessments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Permissions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StaffPositions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_StaffPositions] PRIMARY KEY ([Id])
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
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id])
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Doctors] (
    [Id] nvarchar(450) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NOT NULL,
    [LicenseNumber] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Doctors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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
    [Title] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [Link] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ReadAt] datetime2 NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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
    CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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

CREATE TABLE [UserDocuments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [FileName] nvarchar(256) NOT NULL,
    [FilePath] nvarchar(256) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ApprovedAt] datetime2 NULL,
    [ApprovedBy] nvarchar(256) NOT NULL,
    [FileSize] bigint NOT NULL,
    [ContentType] nvarchar(100) NOT NULL,
    [UploadDate] datetime2 NOT NULL,
    CONSTRAINT [PK_UserDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserPermissions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id])
);
GO

CREATE TABLE [StaffPositionPermission] (
    [PermissionId] int NOT NULL,
    [StaffPositionId] int NOT NULL,
    CONSTRAINT [PK_StaffPositionPermission] PRIMARY KEY ([PermissionId], [StaffPositionId]),
    CONSTRAINT [FK_StaffPositionPermission_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]),
    CONSTRAINT [FK_StaffPositionPermission_StaffPositions_StaffPositionId] FOREIGN KEY ([StaffPositionId]) REFERENCES [StaffPositions] ([Id])
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
    [AttachmentsData] nvarchar(max) NULL,
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
    [ApplicationUserId] nvarchar(450) NULL,
    [PatientUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]),
    CONSTRAINT [FK_Appointments_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId])
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
    [FamilyNumber] nvarchar(50) NOT NULL,
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
    [ApplicationUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
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
    [ApplicationUserId] nvarchar(450) NULL,
    [PatientUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]),
    CONSTRAINT [FK_Prescriptions_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId])
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
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
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
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id])
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
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id])
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
    [PrescriptionId1] int NULL,
    CONSTRAINT [PK_PrescriptionMedications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId1] FOREIGN KEY ([PrescriptionId1]) REFERENCES [Prescriptions] ([Id])
);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
GO

CREATE INDEX [IX_Appointments_ApplicationUserId] ON [Appointments] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_Appointments_PatientUserId] ON [Appointments] ([PatientUserId]);
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

CREATE INDEX [IX_MedicalRecords_ApplicationUserId] ON [MedicalRecords] ([ApplicationUserId]);
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

CREATE INDEX [IX_PrescriptionMedications_PrescriptionId1] ON [PrescriptionMedications] ([PrescriptionId1]);
GO

CREATE INDEX [IX_Prescriptions_ApplicationUserId] ON [Prescriptions] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_Prescriptions_PatientUserId] ON [Prescriptions] ([PatientUserId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_StaffPositionPermission_StaffPositionId] ON [StaffPositionPermission] ([StaffPositionId]);
GO

CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);
GO

CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions] ([PermissionId]);
GO

CREATE INDEX [IX_UserPermissions_UserId] ON [UserPermissions] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250524060601_InitialCreate', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StaffMembers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

CREATE TABLE [StaffPermissions] (
    [Id] int NOT NULL IDENTITY,
    [StaffMemberId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [GrantedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StaffPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]),
    CONSTRAINT [FK_StaffPermissions_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id])
);
GO

CREATE INDEX [IX_StaffPermissions_PermissionId] ON [StaffPermissions] ([PermissionId]);
GO

CREATE INDEX [IX_StaffPermissions_StaffMemberId] ON [StaffPermissions] ([StaffMemberId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250524080949_AddCreatedAtToStaffMember', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250526141302_UpdateAppointmentAttachments', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'FullName');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [StaffMembers] DROP COLUMN [FullName];
GO

EXEC sp_rename N'[AspNetUsers].[FullName]', N'Name', N'COLUMN';
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'WorkingHours');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var1 + '];');
UPDATE [StaffMembers] SET [WorkingHours] = N'' WHERE [WorkingHours] IS NULL;
ALTER TABLE [StaffMembers] ALTER COLUMN [WorkingHours] nvarchar(max) NOT NULL;
ALTER TABLE [StaffMembers] ADD DEFAULT N'' FOR [WorkingHours];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'WorkingDays');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var2 + '];');
UPDATE [StaffMembers] SET [WorkingDays] = N'' WHERE [WorkingDays] IS NULL;
ALTER TABLE [StaffMembers] ALTER COLUMN [WorkingDays] nvarchar(max) NOT NULL;
ALTER TABLE [StaffMembers] ADD DEFAULT N'' FOR [WorkingDays];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'Position');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var3 + '];');
UPDATE [StaffMembers] SET [Position] = N'' WHERE [Position] IS NULL;
ALTER TABLE [StaffMembers] ALTER COLUMN [Position] nvarchar(max) NOT NULL;
ALTER TABLE [StaffMembers] ADD DEFAULT N'' FOR [Position];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'Department');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var4 + '];');
UPDATE [StaffMembers] SET [Department] = N'' WHERE [Department] IS NULL;
ALTER TABLE [StaffMembers] ALTER COLUMN [Department] nvarchar(max) NOT NULL;
ALTER TABLE [StaffMembers] ADD DEFAULT N'' FOR [Department];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[StaffMembers]') AND [c].[name] = N'ContactNumber');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [StaffMembers] DROP CONSTRAINT [' + @var5 + '];');
UPDATE [StaffMembers] SET [ContactNumber] = N'' WHERE [ContactNumber] IS NULL;
ALTER TABLE [StaffMembers] ALTER COLUMN [ContactNumber] nvarchar(max) NOT NULL;
ALTER TABLE [StaffMembers] ADD DEFAULT N'' FOR [ContactNumber];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'UpdatedAt');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [UpdatedAt] datetime2 NULL;
GO

ALTER TABLE [NCDRiskAssessments] ADD [AlcoholConsumption] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [SmokingStatus] nvarchar(max) NOT NULL DEFAULT N'';
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HEEADSSSAssessments]') AND [c].[name] = N'UpdatedAt');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [HEEADSSSAssessments] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [HEEADSSSAssessments] ALTER COLUMN [UpdatedAt] datetime2 NULL;
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationEmployment] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeEnvironment] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [RolePermissions] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
GO

CREATE INDEX [IX_RolePermissions_RoleId] ON [RolePermissions] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250526152431_RenameFullNameToName', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250526152737_UpdateStaffMemberNameColumn', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [GuardianInformation] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [ResidencyProofPath] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_GuardianInformation] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GuardianInformation_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE UNIQUE INDEX [IX_GuardianInformation_UserId] ON [GuardianInformation] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250526165931_AddGuardianInformation', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'FilePath');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [FilePath] nvarchar(512) NOT NULL;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'ApprovedBy');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [ApprovedBy] nvarchar(450) NULL;
GO

CREATE INDEX [IX_UserDocuments_ApprovedBy] ON [UserDocuments] ([ApprovedBy]);
GO

ALTER TABLE [UserDocuments] ADD CONSTRAINT [FK_UserDocuments_AspNetUsers_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [AspNetUsers] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250527035830_UpdateUserDocumentsSchema', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE INDEX [IX_UserDocuments_Status] ON [UserDocuments] ([Status]);
GO

CREATE INDEX [IX_UserDocuments_UploadDate] ON [UserDocuments] ([UploadDate]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250527040849_UpdateUserDocumentsWithNoAction', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [AssessmentNotes] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [AttendanceIssues] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [CareerPlans] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ContactNumber] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [CopingMechanisms] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DatingRelationships] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DietDescription] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EatingDisorderSymptoms] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ExperiencedBullying] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FamilyRelationship] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FeelsSafeAtHome] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FeelsSafeAtSchool] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FollowUpPlan] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Gender] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Hobbies] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [MoodChanges] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [PersonalStrengths] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [PhysicalActivity] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [RecommendedActions] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SchoolPerformance] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ScreenTime] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SelfHarmBehavior] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualActivity] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualOrientation] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SubstanceType] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SubstanceUse] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SuicidalThoughts] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SupportSystems] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [WeightConcerns] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250528115030_FixFullNameColumn', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_UserDocuments_Status] ON [UserDocuments];
DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'Status');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [Status] nvarchar(450) NOT NULL;
CREATE INDEX [IX_UserDocuments_Status] ON [UserDocuments] ([Status]);
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'FilePath');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [FilePath] nvarchar(max) NOT NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'FileName');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [FileName] nvarchar(max) NOT NULL;
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserDocuments]') AND [c].[name] = N'ContentType');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [UserDocuments] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [UserDocuments] ALTER COLUMN [ContentType] nvarchar(max) NOT NULL;
GO

ALTER TABLE [UserDocuments] ADD [FileType] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [UserType] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [HealthReports] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [CheckupDate] datetime2 NOT NULL,
    [BloodPressure] nvarchar(20) NOT NULL,
    [HeartRate] int NULL,
    [BloodSugar] decimal(5,1) NULL,
    [Weight] decimal(5,1) NULL,
    [Temperature] decimal(4,1) NULL,
    [PhysicalActivity] nvarchar(100) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_HealthReports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HealthReports_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_HealthReports_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE INDEX [IX_HealthReports_DoctorId] ON [HealthReports] ([DoctorId]);
GO

CREATE INDEX [IX_HealthReports_UserId] ON [HealthReports] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250528135946_AddUserTypeAndUpdatedAtColumns', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'ChestPainLocation');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [ChestPainLocation];
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'ChestPainWhenWalking');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [ChestPainWhenWalking];
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'DrinksAlcohol');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [DrinksAlcohol];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'DrinksBeer');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [DrinksBeer];
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'DrinksHardLiquor');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [DrinksHardLiquor];
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'DrinksWine');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [DrinksWine];
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsFattyFood');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsFattyFood];
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsFish');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsFish];
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsFruits');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsFruits];
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsMeat');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsMeat];
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsProcessedFood');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsProcessedFood];
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsSaltyFood');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsSaltyFood];
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsSweetFood');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsSweetFood];
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'EatsVegetables');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [EatsVegetables];
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HasChestPain');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HasChestPain];
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HasHighSaltIntake');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HasHighSaltIntake];
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HasNoRegularExercise');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HasNoRegularExercise];
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HasNumbness');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HasNumbness];
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HasRegularExercise');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HasRegularExercise];
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'HealthFacility');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [HealthFacility];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'IsBingeDrinker');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [IsBingeDrinker];
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'PainGoneIn10Min');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [PainGoneIn10Min];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'PainMoreThan30Min');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [PainMoreThan30Min];
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'PainRelievedWithRest');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [PainRelievedWithRest];
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'StopsWhenPain');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [StopsWhenPain];
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'UserId');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [UserId] nvarchar(450) NOT NULL;
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'UpdatedAt');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var40 + '];');
UPDATE [NCDRiskAssessments] SET [UpdatedAt] = '0001-01-01T00:00:00.0000000' WHERE [UpdatedAt] IS NULL;
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;
ALTER TABLE [NCDRiskAssessments] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [UpdatedAt];
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Telepono');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Telepono] nvarchar(20) NOT NULL;
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'SmokingStatus');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [SmokingStatus] nvarchar(20) NOT NULL;
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'RiskStatus');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [RiskStatus] nvarchar(20) NOT NULL;
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Relihiyon');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Relihiyon] nvarchar(50) NOT NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Kasarian');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Kasarian] nvarchar(10) NOT NULL;
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'FamilyOtherDiseaseDetails');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [FamilyOtherDiseaseDetails] nvarchar(200) NOT NULL;
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'FamilyNo');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [FamilyNo] nvarchar(20) NOT NULL;
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'ExerciseDuration');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [ExerciseDuration] nvarchar(50) NOT NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Edad');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var49 + '];');
UPDATE [NCDRiskAssessments] SET [Edad] = 0 WHERE [Edad] IS NULL;
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Edad] int NOT NULL;
ALTER TABLE [NCDRiskAssessments] ADD DEFAULT 0 FOR [Edad];
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'CancerType');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [CancerType] nvarchar(100) NOT NULL;
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Birthday');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var51 + '];');
UPDATE [NCDRiskAssessments] SET [Birthday] = '0001-01-01T00:00:00.0000000' WHERE [Birthday] IS NULL;
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Birthday] datetime2 NOT NULL;
ALTER TABLE [NCDRiskAssessments] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [Birthday];
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Barangay');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Barangay] nvarchar(50) NOT NULL;
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentId');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NULL;
GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AlcoholFrequency');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AlcoholFrequency] nvarchar(50) NOT NULL;
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AlcoholConsumption');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AlcoholConsumption] nvarchar(50) NOT NULL;
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Address');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Address] nvarchar(100) NOT NULL;
GO

ALTER TABLE [NCDRiskAssessments] ADD [AppointmentType] nvarchar(50) NOT NULL DEFAULT N'';
GO

CREATE UNIQUE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments] ([AppointmentId]) WHERE [AppointmentId] IS NOT NULL;
GO

CREATE INDEX [IX_NCDRiskAssessments_UserId] ON [NCDRiskAssessments] ([UserId]);
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250528174550_AddNCDRiskAssessmentTable', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [DoctorAvailabilities] (
    [Id] int NOT NULL IDENTITY,
    [DoctorId] nvarchar(450) NOT NULL,
    [IsAvailable] bit NOT NULL,
    [Monday] bit NOT NULL,
    [Tuesday] bit NOT NULL,
    [Wednesday] bit NOT NULL,
    [Thursday] bit NOT NULL,
    [Friday] bit NOT NULL,
    [Saturday] bit NOT NULL,
    [Sunday] bit NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_DoctorAvailabilities] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DoctorAvailabilities_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_DoctorAvailabilities_DoctorId] ON [DoctorAvailabilities] ([DoctorId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250528180838_AddDoctorAvailabilityTable', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Doctors] DROP CONSTRAINT [FK_Doctors_AspNetUsers_UserId];
GO

ALTER TABLE [Patients] DROP CONSTRAINT [FK_Patients_AspNetUsers_UserId];
GO

ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId1];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId];
GO

ALTER TABLE [Prescriptions] DROP CONSTRAINT [FK_Prescriptions_Patients_PatientId];
GO

DROP INDEX [IX_PrescriptionMedications_PrescriptionId1] ON [PrescriptionMedications];
GO

DROP INDEX [IX_Doctors_UserId] ON [Doctors];
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedications]') AND [c].[name] = N'PrescriptionId1');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedications] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [PrescriptionMedications] DROP COLUMN [PrescriptionId1];
GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prescriptions]') AND [c].[name] = N'UpdatedAt');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [Prescriptions] DROP CONSTRAINT [' + @var58 + '];');
UPDATE [Prescriptions] SET [UpdatedAt] = '0001-01-01T00:00:00.0000000' WHERE [UpdatedAt] IS NULL;
ALTER TABLE [Prescriptions] ALTER COLUMN [UpdatedAt] datetime2 NOT NULL;
ALTER TABLE [Prescriptions] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [UpdatedAt];
GO

ALTER TABLE [Prescriptions] ADD [CancelledAt] datetime2 NULL;
GO

ALTER TABLE [Prescriptions] ADD [CompletedAt] datetime2 NULL;
GO

ALTER TABLE [Prescriptions] ADD [Diagnosis] nvarchar(1000) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Prescriptions] ADD [Duration] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [PrescriptionMedicines] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationName] nvarchar(200) NOT NULL,
    [Dosage] decimal(18,2) NOT NULL,
    [Unit] nvarchar(20) NOT NULL,
    [Frequency] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedicines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedicines_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_PrescriptionMedicines_PrescriptionId] ON [PrescriptionMedicines] ([PrescriptionId]);
GO

ALTER TABLE [Doctors] ADD CONSTRAINT [FK_Doctors_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Patients] ADD CONSTRAINT [FK_Patients_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Prescriptions] ADD CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250528192216_AddPrescriptionDurationAndDates', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Appointments] DROP CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId];
GO

ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId];
GO

DROP INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments];
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentType');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var59 + '];');
ALTER TABLE [NCDRiskAssessments] DROP COLUMN [AppointmentType];
GO

ALTER TABLE [UserPermissions] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [UserPermissions] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PrescriptionMedicines]') AND [c].[name] = N'Dosage');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [PrescriptionMedicines] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [PrescriptionMedicines] ALTER COLUMN [Dosage] decimal(10,2) NOT NULL;
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'FamilyOtherDiseaseDetails');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [FamilyOtherDiseaseDetails] nvarchar(max) NOT NULL;
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentId');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var62 + '];');
UPDATE [NCDRiskAssessments] SET [AppointmentId] = 0 WHERE [AppointmentId] IS NULL;
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NOT NULL;
ALTER TABLE [NCDRiskAssessments] ADD DEFAULT 0 FOR [AppointmentId];
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Birthday] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'LastName');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var63 + '];');
UPDATE [AspNetUsers] SET [LastName] = N'' WHERE [LastName] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [LastName] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [LastName];
GO

CREATE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments] ([AppointmentId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE SET NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529035759_AddTimestampsToUserPermissions', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [HEEADSSSAssessments] DROP CONSTRAINT [FK_HEEADSSSAssessments_ApplicationUser_UserId];
GO

ALTER TABLE [HEEADSSSAssessments] DROP CONSTRAINT [FK_HEEADSSSAssessments_Appointment_AppointmentId];
GO

ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_ApplicationUser_UserId];
GO

ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_Appointment_AppointmentId];
GO

DROP INDEX [IX_HEEADSSSAssessments_AppointmentId] ON [HEEADSSSAssessments];
GO

DROP INDEX [IX_HEEADSSSAssessments_UserId] ON [HEEADSSSAssessments];
GO

ALTER TABLE [Appointment] DROP CONSTRAINT [AK_Appointment_TempId];
GO

ALTER TABLE [Appointment] DROP CONSTRAINT [AK_Appointment_TempId1];
GO

ALTER TABLE [ApplicationUser] DROP CONSTRAINT [AK_ApplicationUser_TempId1];
GO

ALTER TABLE [ApplicationUser] DROP CONSTRAINT [AK_ApplicationUser_TempId2];
GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ApplicationUser]') AND [c].[name] = N'TempId1');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [ApplicationUser] DROP CONSTRAINT [' + @var64 + '];');
ALTER TABLE [ApplicationUser] DROP COLUMN [TempId1];
GO

EXEC sp_rename N'[Appointment]', N'Appointments';
GO

EXEC sp_rename N'[ApplicationUser]', N'AspNetUsers';
GO

EXEC sp_rename N'[Appointments].[TempId1]', N'Status', N'COLUMN';
GO

EXEC sp_rename N'[Appointments].[TempId]', N'AgeValue', N'COLUMN';
GO

EXEC sp_rename N'[AspNetUsers].[TempId2]', N'Id', N'COLUMN';
GO

DROP INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments];
DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentId');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var65 + '];');
UPDATE [NCDRiskAssessments] SET [AppointmentId] = 0 WHERE [AppointmentId] IS NULL;
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NOT NULL;
ALTER TABLE [NCDRiskAssessments] ADD DEFAULT 0 FOR [AppointmentId];
CREATE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments] ([AppointmentId]);
GO

ALTER TABLE [NCDRiskAssessments] ADD [Address] nvarchar(100) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [AlcoholConsumption] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [AlcoholFrequency] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [Barangay] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [Birthday] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [NCDRiskAssessments] ADD [CancerType] nvarchar(100) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [NCDRiskAssessments] ADD [Edad] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [NCDRiskAssessments] ADD [ExerciseDuration] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasCancer] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasDiabetes] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasHeartDisease] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasHypertension] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasKidneyDisease] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasOtherDisease] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyHasStroke] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyNo] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [FamilyOtherDiseaseDetails] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasCOPD] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasCancer] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasDiabetes] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasEyeDisease] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasHypertension] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [HasLungDisease] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [NCDRiskAssessments] ADD [Kasarian] nvarchar(10) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [Relihiyon] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [RiskStatus] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [SmokingStatus] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [Telepono] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [NCDRiskAssessments] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HEEADSSSAssessments]') AND [c].[name] = N'UserId');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [HEEADSSSAssessments] DROP CONSTRAINT [' + @var66 + '];');
ALTER TABLE [HEEADSSSAssessments] ALTER COLUMN [UserId] nvarchar(max) NOT NULL;
GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HEEADSSSAssessments]') AND [c].[name] = N'AppointmentId');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [HEEADSSSAssessments] DROP CONSTRAINT [' + @var67 + '];');
UPDATE [HEEADSSSAssessments] SET [AppointmentId] = 0 WHERE [AppointmentId] IS NULL;
ALTER TABLE [HEEADSSSAssessments] ALTER COLUMN [AppointmentId] int NOT NULL;
ALTER TABLE [HEEADSSSAssessments] ADD DEFAULT 0 FOR [AppointmentId];
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ActivitiesParticipation] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ActivitiesRegularExercise] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ActivitiesScreenTime] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Age] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [HEEADSSSAssessments] ADD [AssessedBy] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [AssessmentNotes] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [AttendanceIssues] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [CareerPlans] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ContactNumber] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [CopingMechanisms] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DatingRelationships] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DietDescription] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DrugsAlcoholUse] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DrugsIllicitDrugUse] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [DrugsTobaccoUse] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EatingBodyImageSatisfaction] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EatingDisorderSymptoms] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EatingDisorderedEatingBehaviors] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EatingWeightComments] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationBullying] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationCurrentlyStudying] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationEmployment] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationSchoolWorkProblems] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [EducationWorking] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ExperiencedBullying] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FamilyNo] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FamilyRelationship] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FeelsSafeAtHome] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FeelsSafeAtSchool] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FollowUpPlan] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [FullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Gender] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HealthFacility] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Hobbies] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeEnvironment] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeFamilyChanges] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeFamilyProblems] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeParentalBlame] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [HomeParentalListening] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [MoodChanges] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [Notes] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [PersonalStrengths] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [PhysicalActivity] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [RecommendedActions] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SafetyGunsAtHome] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SafetyPhysicalAbuse] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SafetyProtectiveGear] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SafetyRelationshipViolence] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SchoolPerformance] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [ScreenTime] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SelfHarmBehavior] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualActivity] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualOrientation] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualityBodyConcerns] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualityIntimateRelationships] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualityPartners] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualityPregnancy] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualityProtection] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualitySTI] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SexualitySexualOrientation] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SubstanceType] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SubstanceUse] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SuicidalThoughts] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SuicideDepressionFeelings] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SuicideFamilyHistory] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SuicideSelfHarmThoughts] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [SupportSystems] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [HEEADSSSAssessments] ADD [UpdatedAt] datetime2 NULL;
GO

ALTER TABLE [HEEADSSSAssessments] ADD [WeightConcerns] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Appointments] ADD [Id] int NOT NULL IDENTITY;
GO

ALTER TABLE [Appointments] ADD [Address] nvarchar(200) NULL;
GO

ALTER TABLE [Appointments] ADD [Allergies] nvarchar(500) NULL;
GO

ALTER TABLE [Appointments] ADD [ApplicationUserId] nvarchar(450) NULL;
GO

ALTER TABLE [Appointments] ADD [AppointmentDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Appointments] ADD [AppointmentTime] time NOT NULL DEFAULT '00:00:00';
GO

ALTER TABLE [Appointments] ADD [AppointmentTimeInput] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [AttachmentPath] nvarchar(500) NULL;
GO

ALTER TABLE [Appointments] ADD [AttachmentsData] nvarchar(max) NULL;
GO

ALTER TABLE [Appointments] ADD [ContactNumber] nvarchar(20) NULL;
GO

ALTER TABLE [Appointments] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Appointments] ADD [CurrentMedications] nvarchar(500) NULL;
GO

ALTER TABLE [Appointments] ADD [DateOfBirth] datetime2 NULL;
GO

ALTER TABLE [Appointments] ADD [DependentAge] int NULL;
GO

ALTER TABLE [Appointments] ADD [DependentFullName] nvarchar(100) NULL;
GO

ALTER TABLE [Appointments] ADD [Description] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [DoctorId] nvarchar(450) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [EmergencyContact] nvarchar(100) NULL;
GO

ALTER TABLE [Appointments] ADD [EmergencyContactNumber] nvarchar(20) NULL;
GO

ALTER TABLE [Appointments] ADD [Gender] nvarchar(10) NULL;
GO

ALTER TABLE [Appointments] ADD [Instructions] nvarchar(1000) NULL;
GO

ALTER TABLE [Appointments] ADD [MedicalHistory] nvarchar(1000) NULL;
GO

ALTER TABLE [Appointments] ADD [PatientId] nvarchar(450) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [PatientName] nvarchar(100) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [PatientUserId] nvarchar(450) NULL;
GO

ALTER TABLE [Appointments] ADD [Prescription] nvarchar(1000) NULL;
GO

ALTER TABLE [Appointments] ADD [ReasonForVisit] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD [RelationshipToDependent] nvarchar(50) NULL;
GO

ALTER TABLE [Appointments] ADD [Type] nvarchar(50) NULL;
GO

ALTER TABLE [Appointments] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [AccessFailedCount] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [AspNetUsers] ADD [Address] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [AgreedAt] datetime2 NULL;
GO

ALTER TABLE [AspNetUsers] ADD [BirthDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [ConcurrencyStamp] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [Email] nvarchar(256) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [EmailConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [EncryptedFullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [EncryptedStatus] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [FirstName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Gender] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [HasAgreedToTerms] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [JoinDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [LastActive] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [LastName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [LockoutEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [LockoutEnd] datetimeoffset NULL;
GO

ALTER TABLE [AspNetUsers] ADD [MaxDailyPatients] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [AspNetUsers] ADD [MiddleName] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Name] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [NormalizedEmail] nvarchar(256) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [NormalizedUserName] nvarchar(256) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [PasswordHash] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [PhilHealthId] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [PhoneNumber] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [PhoneNumberConfirmed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [ProfileImage] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [ProfilePicture] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [SecurityStamp] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [Specialization] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Status] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [Suffix] nvarchar(max) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [TwoFactorEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AspNetUsers] ADD [UpdatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AspNetUsers] ADD [UserName] nvarchar(256) NULL;
GO

ALTER TABLE [AspNetUsers] ADD [UserType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [AspNetUsers] ADD [WorkingDays] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AspNetUsers] ADD [WorkingHours] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [PK_Appointments] PRIMARY KEY ([Id]);
GO

ALTER TABLE [AspNetUsers] ADD CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]);
GO

CREATE TABLE [AppointmentAttachments] (
    [Id] int NOT NULL IDENTITY,
    [AppointmentId] int NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [OriginalFileName] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [ApplicationUserId] nvarchar(450) NULL,
    [AttachmentsData] varbinary(max) NULL,
    CONSTRAINT [PK_AppointmentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]),
    CONSTRAINT [FK_AppointmentAttachments_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id])
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
    CONSTRAINT [FK_AppointmentFiles_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id])
);
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [Assessments] (
    [Id] int NOT NULL IDENTITY,
    [FamilyNumber] nvarchar(max) NOT NULL,
    [ReasonForVisit] nvarchar(max) NOT NULL,
    [Symptoms] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Assessments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ConsultationTimeSlots] (
    [Id] int NOT NULL IDENTITY,
    [ConsultationType] nvarchar(max) NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [IsBooked] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ConsultationTimeSlots] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DoctorAvailabilities] (
    [Id] int NOT NULL IDENTITY,
    [DoctorId] nvarchar(450) NOT NULL,
    [IsAvailable] bit NOT NULL,
    [Monday] bit NOT NULL,
    [Tuesday] bit NOT NULL,
    [Wednesday] bit NOT NULL,
    [Thursday] bit NOT NULL,
    [Friday] bit NOT NULL,
    [Saturday] bit NOT NULL,
    [Sunday] bit NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [LastUpdated] datetime2 NOT NULL,
    CONSTRAINT [PK_DoctorAvailabilities] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DoctorAvailabilities_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
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

CREATE TABLE [FamilyRecords] (
    [Id] int NOT NULL IDENTITY,
    [FamilyNumber] nvarchar(max) NOT NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_FamilyRecords] PRIMARY KEY ([Id])
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

CREATE TABLE [GuardianInformation] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(50) NOT NULL,
    [LastName] nvarchar(50) NOT NULL,
    [ResidencyProofPath] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_GuardianInformation] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GuardianInformation_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
);
GO

CREATE TABLE [HealthReports] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [CheckupDate] datetime2 NOT NULL,
    [BloodPressure] nvarchar(20) NOT NULL,
    [HeartRate] int NULL,
    [BloodSugar] decimal(5,1) NULL,
    [Weight] decimal(5,1) NULL,
    [Temperature] decimal(4,1) NULL,
    [PhysicalActivity] nvarchar(100) NOT NULL,
    [Notes] nvarchar(max) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_HealthReports] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HealthReports_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_HealthReports_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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
    [Title] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [Link] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    [RecipientId] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ReadAt] datetime2 NULL,
    [IsRead] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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

CREATE TABLE [Permissions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StaffMembers] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Department] nvarchar(max) NOT NULL,
    [Position] nvarchar(max) NOT NULL,
    [Specialization] nvarchar(max) NULL,
    [LicenseNumber] nvarchar(max) NULL,
    [ContactNumber] nvarchar(max) NOT NULL,
    [WorkingDays] nvarchar(max) NOT NULL,
    [WorkingHours] nvarchar(max) NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [MaxDailyPatients] int NOT NULL,
    [IsActive] bit NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_StaffMembers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffMembers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [StaffPositions] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_StaffPositions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [UserDocuments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [ContentType] nvarchar(max) NOT NULL,
    [FileType] nvarchar(max) NOT NULL,
    [FileSize] bigint NOT NULL,
    [Status] nvarchar(450) NOT NULL,
    [UploadDate] datetime2 NOT NULL,
    [ApprovedBy] nvarchar(450) NULL,
    [ApprovedAt] datetime2 NULL,
    CONSTRAINT [PK_UserDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserDocuments_AspNetUsers_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
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
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id])
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id])
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
    [FamilyNumber] nvarchar(50) NOT NULL,
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
    [ApplicationUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_MedicalRecords_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_MedicalRecords_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [Prescriptions] (
    [Id] int NOT NULL IDENTITY,
    [PatientId] nvarchar(450) NOT NULL,
    [DoctorId] nvarchar(450) NOT NULL,
    [Diagnosis] nvarchar(1000) NOT NULL,
    [Duration] int NOT NULL,
    [Notes] nvarchar(1000) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [PrescriptionDate] datetime2 NOT NULL,
    [CompletedAt] datetime2 NULL,
    [CancelledAt] datetime2 NULL,
    [ApplicationUserId] nvarchar(450) NULL,
    [PatientUserId] nvarchar(450) NULL,
    CONSTRAINT [PK_Prescriptions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_Prescriptions_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Prescriptions_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId])
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
    CONSTRAINT [FK_VitalSigns_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId])
);
GO

CREATE TABLE [RolePermissions] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserPermissions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [PermissionId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]),
    CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id])
);
GO

CREATE TABLE [StaffPermissions] (
    [Id] int NOT NULL IDENTITY,
    [StaffMemberId] int NOT NULL,
    [PermissionId] int NOT NULL,
    [GrantedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StaffPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StaffPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]),
    CONSTRAINT [FK_StaffPermissions_StaffMembers_StaffMemberId] FOREIGN KEY ([StaffMemberId]) REFERENCES [StaffMembers] ([Id])
);
GO

CREATE TABLE [StaffPositionPermission] (
    [PermissionId] int NOT NULL,
    [StaffPositionId] int NOT NULL,
    CONSTRAINT [PK_StaffPositionPermission] PRIMARY KEY ([PermissionId], [StaffPositionId]),
    CONSTRAINT [FK_StaffPositionPermission_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]),
    CONSTRAINT [FK_StaffPositionPermission_StaffPositions_StaffPositionId] FOREIGN KEY ([StaffPositionId]) REFERENCES [StaffPositions] ([Id])
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
    CONSTRAINT [FK_PrescriptionMedications_MedicalRecords_MedicalRecordId] FOREIGN KEY ([MedicalRecordId]) REFERENCES [MedicalRecords] ([Id]),
    CONSTRAINT [FK_PrescriptionMedications_Medications_MedicationId] FOREIGN KEY ([MedicationId]) REFERENCES [Medications] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PrescriptionMedications_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id])
);
GO

CREATE TABLE [PrescriptionMedicines] (
    [Id] int NOT NULL IDENTITY,
    [PrescriptionId] int NOT NULL,
    [MedicationName] nvarchar(200) NOT NULL,
    [Dosage] decimal(10,2) NOT NULL,
    [Unit] nvarchar(20) NOT NULL,
    [Frequency] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_PrescriptionMedicines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PrescriptionMedicines_Prescriptions_PrescriptionId] FOREIGN KEY ([PrescriptionId]) REFERENCES [Prescriptions] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Appointments_ApplicationUserId] ON [Appointments] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Appointments_DoctorId] ON [Appointments] ([DoctorId]);
GO

CREATE INDEX [IX_Appointments_PatientId] ON [Appointments] ([PatientId]);
GO

CREATE INDEX [IX_Appointments_PatientUserId] ON [Appointments] ([PatientUserId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_AppointmentAttachments_ApplicationUserId] ON [AppointmentAttachments] ([ApplicationUserId]);
GO

CREATE INDEX [IX_AppointmentAttachments_AppointmentId] ON [AppointmentAttachments] ([AppointmentId]);
GO

CREATE INDEX [IX_AppointmentFiles_AppointmentId] ON [AppointmentFiles] ([AppointmentId]);
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

CREATE INDEX [IX_DoctorAvailabilities_DoctorId] ON [DoctorAvailabilities] ([DoctorId]);
GO

CREATE UNIQUE INDEX [IX_Doctors_UserId] ON [Doctors] ([UserId]);
GO

CREATE INDEX [IX_FamilyMembers_PatientId] ON [FamilyMembers] ([PatientId]);
GO

CREATE INDEX [IX_FamilyMembers_UserId] ON [FamilyMembers] ([UserId]);
GO

CREATE INDEX [IX_Feedbacks_UserId] ON [Feedbacks] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_GuardianInformation_UserId] ON [GuardianInformation] ([UserId]);
GO

CREATE INDEX [IX_HealthReports_DoctorId] ON [HealthReports] ([DoctorId]);
GO

CREATE INDEX [IX_HealthReports_UserId] ON [HealthReports] ([UserId]);
GO

CREATE INDEX [IX_MedicalRecords_ApplicationUserId] ON [MedicalRecords] ([ApplicationUserId]);
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

CREATE INDEX [IX_PrescriptionMedicines_PrescriptionId] ON [PrescriptionMedicines] ([PrescriptionId]);
GO

CREATE INDEX [IX_Prescriptions_ApplicationUserId] ON [Prescriptions] ([ApplicationUserId]);
GO

CREATE INDEX [IX_Prescriptions_DoctorId] ON [Prescriptions] ([DoctorId]);
GO

CREATE INDEX [IX_Prescriptions_PatientId] ON [Prescriptions] ([PatientId]);
GO

CREATE INDEX [IX_Prescriptions_PatientUserId] ON [Prescriptions] ([PatientUserId]);
GO

CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
GO

CREATE INDEX [IX_RolePermissions_RoleId] ON [RolePermissions] ([RoleId]);
GO

CREATE INDEX [IX_StaffMembers_UserId] ON [StaffMembers] ([UserId]);
GO

CREATE INDEX [IX_StaffPermissions_PermissionId] ON [StaffPermissions] ([PermissionId]);
GO

CREATE INDEX [IX_StaffPermissions_StaffMemberId] ON [StaffPermissions] ([StaffMemberId]);
GO

CREATE INDEX [IX_StaffPositionPermission_StaffPositionId] ON [StaffPositionPermission] ([StaffPositionId]);
GO

CREATE INDEX [IX_UserDocuments_ApprovedBy] ON [UserDocuments] ([ApprovedBy]);
GO

CREATE INDEX [IX_UserDocuments_Status] ON [UserDocuments] ([Status]);
GO

CREATE INDEX [IX_UserDocuments_UploadDate] ON [UserDocuments] ([UploadDate]);
GO

CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);
GO

CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions] ([PermissionId]);
GO

CREATE INDEX [IX_UserPermissions_UserId] ON [UserPermissions] ([UserId]);
GO

CREATE INDEX [IX_VitalSigns_PatientId] ON [VitalSigns] ([PatientId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_ApplicationUserId] FOREIGN KEY ([ApplicationUserId]) REFERENCES [AspNetUsers] ([Id]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [Patients] ([UserId]);
GO

ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_Patients_PatientUserId] FOREIGN KEY ([PatientUserId]) REFERENCES [Patients] ([UserId]);
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE SET NULL;
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529035948_AddCreatedAndUpdatedAtToUserPermissions', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'UpdatedAt');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [UpdatedAt] datetime2 NULL;
GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Telepono');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var69 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Telepono] nvarchar(max) NOT NULL;
GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'FamilyNo');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var70 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [FamilyNo] nvarchar(max) NOT NULL;
GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Barangay');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Barangay] nvarchar(max) NOT NULL;
GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'Address');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [Address] nvarchar(max) NOT NULL;
GO

ALTER TABLE [NCDRiskAssessments] ADD [HealthFacility] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529051202_AddHealthFacilityAndFamilyNoFields', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [NCDRiskAssessments] ADD [HighSaltIntake] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529051329_AddHighSaltIntakeField', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [IntegratedAssessments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [FamilyNo] nvarchar(50) NOT NULL,
    [HealthFacility] nvarchar(200) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Barangay] nvarchar(max) NOT NULL,
    [Birthday] datetime2 NOT NULL,
    [Telepono] nvarchar(max) NOT NULL,
    [Edad] int NOT NULL,
    [Kasarian] nvarchar(max) NOT NULL,
    [Relihiyon] nvarchar(max) NOT NULL,
    [HasDiabetes] bit NOT NULL,
    [HasHypertension] bit NOT NULL,
    [HasCancer] bit NOT NULL,
    [HasCOPD] bit NOT NULL,
    [HasLungDisease] bit NOT NULL,
    [HasEyeDisease] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_IntegratedAssessments] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529063000_FixNCDRiskAssessmentForeignKey', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId];
GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentId');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NULL;
GO

ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529071630_AddUserPermissionsTimestamps', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [GuardianInformation] DROP CONSTRAINT [FK_GuardianInformation_AspNetUsers_UserId];
GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuardianInformation]') AND [c].[name] = N'FirstName');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [GuardianInformation] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [GuardianInformation] DROP COLUMN [FirstName];
GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuardianInformation]') AND [c].[name] = N'LastName');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [GuardianInformation] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [GuardianInformation] DROP COLUMN [LastName];
GO

EXEC sp_rename N'[GuardianInformation].[ResidencyProofPath]', N'ResidencyProof', N'COLUMN';
GO

EXEC sp_rename N'[GuardianInformation].[Id]', N'GuardianId', N'COLUMN';
GO

ALTER TABLE [GuardianInformation] ADD [GuardianFirstName] nvarchar(100) NOT NULL DEFAULT N'';
GO

ALTER TABLE [GuardianInformation] ADD [GuardianLastName] nvarchar(100) NOT NULL DEFAULT N'';
GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AspNetUsers]') AND [c].[name] = N'Suffix');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [AspNetUsers] DROP CONSTRAINT [' + @var76 + '];');
UPDATE [AspNetUsers] SET [Suffix] = N'' WHERE [Suffix] IS NULL;
ALTER TABLE [AspNetUsers] ALTER COLUMN [Suffix] nvarchar(max) NOT NULL;
ALTER TABLE [AspNetUsers] ADD DEFAULT N'' FOR [Suffix];
GO

ALTER TABLE [GuardianInformation] ADD CONSTRAINT [FK_GuardianInformation_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529075337_AddGuardianConsentFeatures', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [IntegratedAssessments] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(max) NOT NULL,
    [FamilyNo] nvarchar(50) NOT NULL,
    [HealthFacility] nvarchar(200) NOT NULL,
    [Address] nvarchar(max) NOT NULL,
    [Barangay] nvarchar(max) NOT NULL,
    [Birthday] datetime2 NOT NULL,
    [Telepono] nvarchar(max) NOT NULL,
    [Edad] int NOT NULL,
    [Kasarian] nvarchar(max) NOT NULL,
    [Relihiyon] nvarchar(max) NOT NULL,
    [HasDiabetes] bit NOT NULL,
    [HasHypertension] bit NOT NULL,
    [HasCancer] bit NOT NULL,
    [HasCOPD] bit NOT NULL,
    [HasLungDisease] bit NOT NULL,
    [HasEyeDisease] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_IntegratedAssessments] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529084506_FixNCDRiskAssessmentAppointmentId', N'8.0.2');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[NCDRiskAssessments]') AND [c].[name] = N'AppointmentId');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250529085417_SyncNCDRiskAssessmentSchema', N'8.0.2');
GO

COMMIT;
GO

