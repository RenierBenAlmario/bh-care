-- Comprehensive fix for NCDRiskAssessments table
-- This script adds all missing columns mentioned in the error message

USE [Barangay]
GO

-- Add missing columns from the error message
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CancerMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerMedication NVARCHAR(MAX) NULL;
    PRINT 'Added CancerMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CancerYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerYear NVARCHAR(4) NULL;
    PRINT 'Added CancerYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CivilStatus' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CivilStatus NVARCHAR(50) NULL;
    PRINT 'Added CivilStatus column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'DiabetesMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesMedication NVARCHAR(MAX) NULL;
    PRINT 'Added DiabetesMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'DiabetesYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesYear NVARCHAR(4) NULL;
    PRINT 'Added DiabetesYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryCancerFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryCancerMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryCancerSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryDiabetesFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryDiabetesMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryDiabetesSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryHeartDiseaseFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryHeartDiseaseMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryHeartDiseaseSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryLungDiseaseFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryLungDiseaseMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryLungDiseaseSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOther' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOther NVARCHAR(100) NULL;
    PRINT 'Added FamilyHistoryOther column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryOtherFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryOtherMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryOtherSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeFather BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryStrokeFather column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeMother BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryStrokeMother column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeSibling BIT NOT NULL DEFAULT 0;
    PRINT 'Added FamilyHistoryStrokeSibling column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FirstName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FirstName NVARCHAR(50) NULL;
    PRINT 'Added FirstName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HypertensionMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionMedication NVARCHAR(MAX) NULL;
    PRINT 'Added HypertensionMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HypertensionYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionYear NVARCHAR(4) NULL;
    PRINT 'Added HypertensionYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LastName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LastName NVARCHAR(50) NULL;
    PRINT 'Added LastName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LungDiseaseMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseMedication NVARCHAR(MAX) NULL;
    PRINT 'Added LungDiseaseMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LungDiseaseYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseYear NVARCHAR(4) NULL;
    PRINT 'Added LungDiseaseYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'MiddleName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD MiddleName NVARCHAR(50) NULL;
    PRINT 'Added MiddleName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Occupation' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Occupation NVARCHAR(100) NULL;
    PRINT 'Added Occupation column';
END

-- Additional columns that might be missing based on the model
IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HealthFacility' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HealthFacility NVARCHAR(100) NULL;
    PRINT 'Added HealthFacility column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HasAsthma' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasAsthma BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasAsthma column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HasDifficultyBreathing' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasDifficultyBreathing BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasDifficultyBreathing column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HasNoRegularExercise' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasNoRegularExercise BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasNoRegularExercise column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HighSaltIntake' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HighSaltIntake BIT NOT NULL DEFAULT 0;
    PRINT 'Added HighSaltIntake column';
END

-- Verify the columns were added successfully
SELECT 
    COLUMN_NAME, 
    DATA_TYPE,
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'NCDRiskAssessments'
ORDER BY 
    COLUMN_NAME;