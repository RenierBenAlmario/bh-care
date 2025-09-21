-- Script to create the HEEADSSSAssessments table with all fields nullable
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HEEADSSSAssessments')
BEGIN
    PRINT 'Creating HEEADSSSAssessments table...';
    
    CREATE TABLE [dbo].[HEEADSSSAssessments](
        [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] [nvarchar](450) NULL,
        [AppointmentId] [int] NULL,
        [HealthFacility] [nvarchar](255) NULL,
        [FamilyNo] [nvarchar](50) NULL,
        [FullName] [nvarchar](255) NULL,
        [Birthday] [datetime2] NULL,
        [Age] [int] NULL,
        [Gender] [nvarchar](20) NULL,
        [Address] [nvarchar](255) NULL,
        [ContactNumber] [nvarchar](20) NULL,
        [HomeEnvironment] [nvarchar](max) NULL,
        [FamilyRelationship] [nvarchar](max) NULL,
        [HomeFamilyProblems] [nvarchar](max) NULL,
        [HomeParentalListening] [nvarchar](max) NULL,
        [HomeParentalBlame] [nvarchar](max) NULL,
        [HomeFamilyChanges] [nvarchar](max) NULL,
        [SchoolPerformance] [nvarchar](max) NULL,
        [AttendanceIssues] [bit] NULL DEFAULT(0),
        [CareerPlans] [nvarchar](max) NULL,
        [EducationCurrentlyStudying] [nvarchar](max) NULL,
        [EducationWorking] [nvarchar](max) NULL,
        [EducationSchoolWorkProblems] [nvarchar](max) NULL,
        [EducationBullying] [nvarchar](max) NULL,
        [EducationEmployment] [nvarchar](max) NULL,
        [DietDescription] [nvarchar](max) NULL,
        [WeightConcerns] [bit] NULL DEFAULT(0),
        [EatingDisorderSymptoms] [bit] NULL DEFAULT(0),
        [EatingBodyImageSatisfaction] [nvarchar](max) NULL,
        [EatingDisorderedEatingBehaviors] [nvarchar](max) NULL,
        [EatingWeightComments] [nvarchar](max) NULL,
        [Hobbies] [nvarchar](max) NULL,
        [PhysicalActivity] [nvarchar](max) NULL,
        [ScreenTime] [nvarchar](max) NULL,
        [ActivitiesParticipation] [nvarchar](max) NULL,
        [ActivitiesRegularExercise] [nvarchar](max) NULL,
        [ActivitiesScreenTime] [nvarchar](max) NULL,
        [SubstanceUse] [bit] NULL DEFAULT(0),
        [SubstanceType] [nvarchar](max) NULL,
        [DrugsTobaccoUse] [nvarchar](max) NULL,
        [DrugsAlcoholUse] [nvarchar](max) NULL,
        [DrugsIllicitDrugUse] [nvarchar](max) NULL,
        [DatingRelationships] [nvarchar](max) NULL,
        [SexualActivity] [bit] NULL DEFAULT(0),
        [SexualOrientation] [nvarchar](max) NULL,
        [SexualityBodyConcerns] [nvarchar](max) NULL,
        [SexualityIntimateRelationships] [nvarchar](max) NULL,
        [SexualityPartners] [nvarchar](max) NULL,
        [SexualitySexualOrientation] [nvarchar](max) NULL,
        [SexualityPregnancy] [nvarchar](max) NULL,
        [SexualitySTI] [nvarchar](max) NULL,
        [SexualityProtection] [nvarchar](max) NULL,
        [MoodChanges] [bit] NULL DEFAULT(0),
        [SuicidalThoughts] [bit] NULL DEFAULT(0),
        [SelfHarmBehavior] [bit] NULL DEFAULT(0),
        [FeelsSafeAtHome] [bit] NULL DEFAULT(1),
        [FeelsSafeAtSchool] [bit] NULL DEFAULT(1),
        [ExperiencedBullying] [bit] NULL DEFAULT(0),
        [PersonalStrengths] [nvarchar](max) NULL,
        [SupportSystems] [nvarchar](max) NULL,
        [CopingMechanisms] [nvarchar](max) NULL,
        [SafetyPhysicalAbuse] [nvarchar](max) NULL,
        [SafetyRelationshipViolence] [nvarchar](max) NULL,
        [SafetyProtectiveGear] [nvarchar](max) NULL,
        [SafetyGunsAtHome] [nvarchar](max) NULL,
        [SuicideDepressionFeelings] [nvarchar](max) NULL,
        [SuicideSelfHarmThoughts] [nvarchar](max) NULL,
        [SuicideFamilyHistory] [nvarchar](max) NULL,
        [AssessmentNotes] [nvarchar](max) NULL,
        [RecommendedActions] [nvarchar](max) NULL,
        [FollowUpPlan] [nvarchar](max) NULL,
        [Notes] [nvarchar](max) NULL,
        [AssessedBy] [nvarchar](255) NULL,
        [CreatedAt] [datetime2] NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] [datetime2] NULL
    );
    
    PRINT 'HEEADSSSAssessments table created successfully.';
END
ELSE
BEGIN
    PRINT 'HEEADSSSAssessments table already exists.';
END

-- Script to create the NCDRiskAssessments table with all fields nullable
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'NCDRiskAssessments')
BEGIN
    PRINT 'Creating NCDRiskAssessments table...';
    
    CREATE TABLE [dbo].[NCDRiskAssessments](
        [Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [UserId] [nvarchar](450) NULL,
        [AppointmentId] [int] NULL,
        [HealthFacility] [nvarchar](255) NULL,
        [FamilyNo] [nvarchar](50) NULL,
        [Address] [nvarchar](255) NULL,
        [Barangay] [nvarchar](100) NULL,
        [Birthday] [datetime2] NULL,
        [Telepono] [nvarchar](20) NULL,
        [Edad] [int] NULL,
        [Kasarian] [nvarchar](10) NULL,
        [Relihiyon] [nvarchar](50) NULL,
        [HasDiabetes] [bit] NOT NULL DEFAULT(0),
        [HasHypertension] [bit] NOT NULL DEFAULT(0),
        [HasCancer] [bit] NOT NULL DEFAULT(0),
        [HasCOPD] [bit] NOT NULL DEFAULT(0),
        [HasLungDisease] [bit] NOT NULL DEFAULT(0),
        [HasEyeDisease] [bit] NOT NULL DEFAULT(0),
        [CancerType] [nvarchar](100) NULL,
        [FamilyHasHypertension] [bit] NOT NULL DEFAULT(0),
        [FamilyHasHeartDisease] [bit] NOT NULL DEFAULT(0),
        [FamilyHasStroke] [bit] NOT NULL DEFAULT(0),
        [FamilyHasDiabetes] [bit] NOT NULL DEFAULT(0),
        [FamilyHasCancer] [bit] NOT NULL DEFAULT(0),
        [FamilyHasKidneyDisease] [bit] NOT NULL DEFAULT(0),
        [FamilyHasOtherDisease] [bit] NOT NULL DEFAULT(0),
        [FamilyOtherDiseaseDetails] [nvarchar](max) NULL,
        [SmokingStatus] [nvarchar](20) NULL DEFAULT('Non-smoker'),
        [HighSaltIntake] [bit] NOT NULL DEFAULT(0),
        [AlcoholFrequency] [nvarchar](50) NULL,
        [AlcoholConsumption] [nvarchar](50) NULL,
        [ExerciseDuration] [nvarchar](50) NULL,
        [RiskStatus] [nvarchar](20) NULL DEFAULT('Low Risk'),
        [CreatedAt] [datetime2] NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] [datetime2] NULL
    );
    
    PRINT 'NCDRiskAssessments table created successfully.';
END
ELSE
BEGIN
    PRINT 'NCDRiskAssessments table already exists.';
END 