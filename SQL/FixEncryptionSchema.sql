-- Fix NCDRiskAssessments table schema to support encrypted fields
-- This script changes data types from specific types to NVARCHAR to accommodate encrypted strings

USE [Barangay];
GO

-- Start transaction
BEGIN TRANSACTION;

BEGIN TRY
    -- Change Birthday from DATE to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [Birthday] NVARCHAR(4000) NULL;
    
    -- Change Edad from INT to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [Edad] NVARCHAR(4000) NULL;
    
    -- Change all BIT fields to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasDiabetes] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasHypertension] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasCancer] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasCOPD] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasLungDisease] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasEyeDisease] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasChestPain] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [ChestPainLocation] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [ChestPainWhenWalking] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [StopsWhenPain] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [PainRelievedWithRest] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [PainGoneIn10Min] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [PainMoreThan30Min] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasNumbness] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasHypertension] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasHeartDisease] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasStroke] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasDiabetes] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasCancer] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasKidneyDisease] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHasOtherDisease] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsVegetables] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsFruits] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsFish] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsMeat] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsProcessedFood] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasHighSaltIntake] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsSaltyFood] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsSweetFood] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [EatsFattyFood] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [DrinksAlcohol] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [DrinksBeer] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [DrinksWine] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [DrinksHardLiquor] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [IsBingeDrinker] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasRegularExercise] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasNoRegularExercise] NVARCHAR(4000) NULL;
    
    -- Change CreatedAt and UpdatedAt from DATETIME2 to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [CreatedAt] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [UpdatedAt] NVARCHAR(4000) NULL;
    
    -- Add new columns for the current model (these might not exist in the old schema)
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'HealthFacility')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HealthFacility] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FirstName')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FirstName] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'MiddleName')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [MiddleName] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'LastName')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [LastName] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'CivilStatus')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [CivilStatus] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'Occupation')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [Occupation] NVARCHAR(4000) NULL;
    END
    
    -- Add medication and year fields
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'DiabetesMedication')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [DiabetesMedication] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'DiabetesYear')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [DiabetesYear] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'HypertensionMedication')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HypertensionMedication] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'HypertensionYear')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HypertensionYear] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'CancerMedication')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [CancerMedication] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'CancerYear')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [CancerYear] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'LungDiseaseMedication')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [LungDiseaseMedication] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'LungDiseaseYear')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [LungDiseaseYear] NVARCHAR(4000) NULL;
    END
    
    -- Add family history detail fields
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryCancerFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryCancerFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryCancerMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryCancerMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryCancerSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryCancerSibling] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryDiabetesFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryDiabetesFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryDiabetesMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryDiabetesMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryDiabetesSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryDiabetesSibling] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryHeartDiseaseFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryHeartDiseaseFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryHeartDiseaseMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryHeartDiseaseMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryHeartDiseaseSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryHeartDiseaseSibling] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryLungDiseaseFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryLungDiseaseFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryLungDiseaseMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryLungDiseaseMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryLungDiseaseSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryLungDiseaseSibling] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryOther')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryOther] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryOtherFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryOtherFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryOtherMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryOtherMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryOtherSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryOtherSibling] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryStrokeFather')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryStrokeFather] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryStrokeMother')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryStrokeMother] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'FamilyHistoryStrokeSibling')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [FamilyHistoryStrokeSibling] NVARCHAR(4000) NULL;
    END
    
    -- Add additional fields that might be missing
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'SmokingStatus')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [SmokingStatus] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'AlcoholConsumption')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [AlcoholConsumption] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'ChestPain')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [ChestPain] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'ChestPainValue')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [ChestPainValue] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'HasDifficultyBreathing')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HasDifficultyBreathing] NVARCHAR(4000) NULL;
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[NCDRiskAssessments]') AND name = 'HasAsthma')
    BEGIN
        ALTER TABLE [dbo].[NCDRiskAssessments] ADD [HasAsthma] NVARCHAR(4000) NULL;
    END
    
    PRINT 'Schema update completed successfully!';
    
    -- Commit transaction
    COMMIT TRANSACTION;
    
END TRY
BEGIN CATCH
    -- Rollback transaction on error
    ROLLBACK TRANSACTION;
    
    -- Print error information
    PRINT 'Error occurred during schema update:';
    PRINT ERROR_MESSAGE();
    PRINT 'Transaction rolled back.';
    
    -- Re-throw the error
    THROW;
END CATCH
GO

