-- Add missing columns to NCDRiskAssessments table
-- This script adds columns that are defined in the NCDRiskAssessment model but missing from the database

USE [Barangay]
GO

-- Check if columns exist before adding them
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'Pangalan')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pangalan NVARCHAR(100) NULL;
    PRINT 'Added Pangalan column';
END
ELSE
BEGIN
    PRINT 'Pangalan column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'EatsProcessedFood')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsProcessedFood BIT NOT NULL DEFAULT 0;
    PRINT 'Added EatsProcessedFood column';
END
ELSE
BEGIN
    PRINT 'EatsProcessedFood column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HighSaltIntake')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HighSaltIntake BIT NOT NULL DEFAULT 0;
    PRINT 'Added HighSaltIntake column';
END
ELSE
BEGIN
    PRINT 'HighSaltIntake column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'AlcoholFrequency')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholFrequency NVARCHAR(50) NULL;
    PRINT 'Added AlcoholFrequency column';
END
ELSE
BEGIN
    PRINT 'AlcoholFrequency column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'AlcoholConsumption')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholConsumption NVARCHAR(50) NULL;
    PRINT 'Added AlcoholConsumption column';
END
ELSE
BEGIN
    PRINT 'AlcoholConsumption column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ExerciseDuration')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ExerciseDuration NVARCHAR(50) NULL;
    PRINT 'Added ExerciseDuration column';
END
ELSE
BEGIN
    PRINT 'ExerciseDuration column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'SmokingStatus')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD SmokingStatus NVARCHAR(50) NULL;
    PRINT 'Added SmokingStatus column';
END
ELSE
BEGIN
    PRINT 'SmokingStatus column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'FamilyOtherDiseaseDetails')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyOtherDiseaseDetails NVARCHAR(MAX) NULL;
    PRINT 'Added FamilyOtherDiseaseDetails column';
END
ELSE
BEGIN
    PRINT 'FamilyOtherDiseaseDetails column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'RiskStatus')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD RiskStatus NVARCHAR(50) NULL;
    PRINT 'Added RiskStatus column';
END
ELSE
BEGIN
    PRINT 'RiskStatus column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPain')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ChestPain NVARCHAR(100) NULL;
    PRINT 'Added ChestPain column';
END
ELSE
BEGIN
    PRINT 'ChestPain column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPainLocation')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ChestPainLocation NVARCHAR(100) NULL;
    PRINT 'Added ChestPainLocation column';
END
ELSE
BEGIN
    PRINT 'ChestPainLocation column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'ChestPainValue')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ChestPainValue INT NULL;
    PRINT 'Added ChestPainValue column';
END
ELSE
BEGIN
    PRINT 'ChestPainValue column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasDifficultyBreathing')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasDifficultyBreathing BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasDifficultyBreathing column';
END
ELSE
BEGIN
    PRINT 'HasDifficultyBreathing column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasAsthma')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasAsthma BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasAsthma column';
END
ELSE
BEGIN
    PRINT 'HasAsthma column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'HasNoRegularExercise')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasNoRegularExercise BIT NOT NULL DEFAULT 0;
    PRINT 'Added HasNoRegularExercise column';
END
ELSE
BEGIN
    PRINT 'HasNoRegularExercise column already exists';
END

PRINT 'Database schema update completed successfully!';
GO
