-- Create Appointments table
CREATE TABLE [dbo].[Appointments] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [PatientId] NVARCHAR(450) NOT NULL,
    [DoctorId] NVARCHAR(450) NOT NULL,
    [PatientName] NVARCHAR(100) NOT NULL,
    [AppointmentDate] DATE NOT NULL,
    [AppointmentTime] TIME NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Status] NVARCHAR(50) NOT NULL,
    [Type] NVARCHAR(50) NULL,
    [AgeValue] INT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    [DependentFullName] NVARCHAR(100) NULL,
    [DependentAge] INT NULL,
    [RelationshipToDependent] NVARCHAR(50) NULL,
    [ContactNumber] NVARCHAR(20) NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Appointments_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

-- Create indexes for Appointments table
CREATE INDEX [IX_Appointments_PatientId] ON [dbo].[Appointments]([PatientId]);
CREATE INDEX [IX_Appointments_DoctorId] ON [dbo].[Appointments]([DoctorId]);
CREATE INDEX [IX_Appointments_AppointmentDate] ON [dbo].[Appointments]([AppointmentDate]);
CREATE INDEX [IX_Appointments_Status] ON [dbo].[Appointments]([Status]);

-- Create NCDRiskAssessment table
CREATE TABLE [dbo].[NCDRiskAssessments] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [UserId] NVARCHAR(450) NOT NULL,
    [AppointmentId] INT NOT NULL,
    [AppointmentType] NVARCHAR(50) NULL,
    [Address] NVARCHAR(200) NULL,
    [Barangay] NVARCHAR(100) NULL,
    [Birthday] DATE NOT NULL,
    [Telepono] NVARCHAR(20) NULL,
    [Edad] INT NOT NULL,
    [Kasarian] NVARCHAR(10) NULL,
    [Relihiyon] NVARCHAR(50) NULL,
    [FamilyNo] NVARCHAR(50) NULL,
    -- Medical History
    [HasDiabetes] BIT NOT NULL DEFAULT 0,
    [HasHypertension] BIT NOT NULL DEFAULT 0,
    [HasCancer] BIT NOT NULL DEFAULT 0,
    [HasCOPD] BIT NOT NULL DEFAULT 0,
    [HasLungDisease] BIT NOT NULL DEFAULT 0,
    [HasEyeDisease] BIT NOT NULL DEFAULT 0,
    [CancerType] NVARCHAR(100) NULL,
    -- Chest Pain
    [HasChestPain] BIT NOT NULL DEFAULT 0,
    [ChestPainLocation] BIT NOT NULL DEFAULT 0,
    [ChestPainWhenWalking] BIT NOT NULL DEFAULT 0,
    [StopsWhenPain] BIT NOT NULL DEFAULT 0,
    [PainRelievedWithRest] BIT NOT NULL DEFAULT 0,
    [PainGoneIn10Min] BIT NOT NULL DEFAULT 0,
    [PainMoreThan30Min] BIT NOT NULL DEFAULT 0,
    [HasNumbness] BIT NOT NULL DEFAULT 0,
    -- Family History
    [FamilyHasHypertension] BIT NOT NULL DEFAULT 0,
    [FamilyHasHeartDisease] BIT NOT NULL DEFAULT 0,
    [FamilyHasStroke] BIT NOT NULL DEFAULT 0,
    [FamilyHasDiabetes] BIT NOT NULL DEFAULT 0,
    [FamilyHasCancer] BIT NOT NULL DEFAULT 0,
    [FamilyHasKidneyDisease] BIT NOT NULL DEFAULT 0,
    [FamilyHasOtherDisease] BIT NOT NULL DEFAULT 0,
    [FamilyOtherDiseaseDetails] NVARCHAR(200) NULL,
    -- Lifestyle
    [EatsVegetables] BIT NOT NULL DEFAULT 0,
    [EatsFruits] BIT NOT NULL DEFAULT 0,
    [EatsFish] BIT NOT NULL DEFAULT 0,
    [EatsMeat] BIT NOT NULL DEFAULT 0,
    [EatsProcessedFood] BIT NOT NULL DEFAULT 0,
    [HasHighSaltIntake] BIT NOT NULL DEFAULT 0,
    [EatsSaltyFood] BIT NOT NULL DEFAULT 0,
    [EatsSweetFood] BIT NOT NULL DEFAULT 0,
    [EatsFattyFood] BIT NOT NULL DEFAULT 0,
    -- Alcohol
    [DrinksAlcohol] BIT NOT NULL DEFAULT 0,
    [DrinksBeer] BIT NOT NULL DEFAULT 0,
    [DrinksWine] BIT NOT NULL DEFAULT 0,
    [DrinksHardLiquor] BIT NOT NULL DEFAULT 0,
    [IsBingeDrinker] BIT NOT NULL DEFAULT 0,
    [AlcoholFrequency] NVARCHAR(20) NULL,
    -- Exercise
    [HasRegularExercise] BIT NOT NULL DEFAULT 0,
    [HasNoRegularExercise] BIT NOT NULL DEFAULT 0,
    [ExerciseDuration] NVARCHAR(50) NULL,
    -- Assessment
    [RiskStatus] NVARCHAR(20) NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [UpdatedAt] DATETIME2 NOT NULL,
    CONSTRAINT [PK_NCDRiskAssessments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NCDRiskAssessments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] FOREIGN KEY ([AppointmentId]) REFERENCES [dbo].[Appointments] ([Id]) ON DELETE CASCADE
);

-- Create indexes for NCDRiskAssessment table
CREATE INDEX [IX_NCDRiskAssessments_UserId] ON [dbo].[NCDRiskAssessments]([UserId]);
CREATE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [dbo].[NCDRiskAssessments]([AppointmentId]);
CREATE INDEX [IX_NCDRiskAssessments_FamilyNo] ON [dbo].[NCDRiskAssessments]([FamilyNo]); 