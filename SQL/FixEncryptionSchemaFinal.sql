-- Fix NCDRiskAssessments table schema to support encrypted fields
-- This script drops default constraints, changes data types, and recreates constraints

USE [Barangay];
GO

-- Start transaction
BEGIN TRANSACTION;

BEGIN TRY
    -- Drop all default constraints on BIT columns first
    DECLARE @sql NVARCHAR(MAX) = '';
    
    SELECT @sql = @sql + 'ALTER TABLE [dbo].[NCDRiskAssessments] DROP CONSTRAINT ' + CONSTRAINT_NAME + ';' + CHAR(13)
    FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE 
    WHERE TABLE_NAME = 'NCDRiskAssessments' 
    AND CONSTRAINT_NAME LIKE 'DF__NCDRiskAs__%';
    
    IF @sql IS NOT NULL AND @sql != ''
    BEGIN
        EXEC sp_executesql @sql;
        PRINT 'Dropped default constraints successfully.';
    END
    
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

