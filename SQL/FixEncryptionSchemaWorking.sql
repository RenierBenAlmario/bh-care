-- Fix NCDRiskAssessments table schema to support encrypted fields
-- This script drops default constraints, changes data types

USE [Barangay];
GO

-- Start transaction
BEGIN TRANSACTION;

BEGIN TRY
    -- Drop all default constraints
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__40F9A68C];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__41EDCAC5];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__42E1EEFE];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__43D61337];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__44CA3770];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__45BE5BA9];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__46B27FE2];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__47A6A41B];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__489AC854];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__498EEC8D];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4A8310C6];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4B7734FF];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4C6B5938];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4D5F7D71];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4E53A1AA];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__4F47C5E3];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__503BEA1C];
    ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT [DF__NCDRiskAs__Famil__51300E55];
    
    PRINT 'Dropped all default constraints successfully.';
    
    -- Change Birthday from datetime2 to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [Birthday] NVARCHAR(4000) NULL;
    
    -- Change Edad from int to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [Edad] NVARCHAR(4000) NULL;
    
    -- Change ChestPainValue from int to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [ChestPainValue] NVARCHAR(4000) NULL;
    
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
    ALTER COLUMN [HighSaltIntake] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasDifficultyBreathing] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasAsthma] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [HasNoRegularExercise] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryCancerFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryCancerMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryCancerSibling] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryDiabetesFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryDiabetesMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryDiabetesSibling] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryHeartDiseaseFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryHeartDiseaseMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryHeartDiseaseSibling] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryLungDiseaseFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryLungDiseaseMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryLungDiseaseSibling] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryOtherFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryOtherMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryOtherSibling] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryStrokeFather] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryStrokeMother] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [FamilyHistoryStrokeSibling] NVARCHAR(4000) NULL;
    
    -- Change CreatedAt and UpdatedAt from datetime2 to NVARCHAR(4000) to support encrypted strings
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [CreatedAt] NVARCHAR(4000) NULL;
    
    ALTER TABLE [dbo].[NCDRiskAssessments] 
    ALTER COLUMN [UpdatedAt] NVARCHAR(4000) NULL;
    
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

