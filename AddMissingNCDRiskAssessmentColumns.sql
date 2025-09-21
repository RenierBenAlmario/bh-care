-- SQL Script to add missing columns to NCDRiskAssessments table
-- This script should be run directly against the database

-- Add missing columns from the error message
ALTER TABLE NCDRiskAssessments ADD ChestPain NVARCHAR(100) NULL;
ALTER TABLE NCDRiskAssessments ADD ChestPainLocation NVARCHAR(100) NULL;
ALTER TABLE NCDRiskAssessments ADD ChestPainValue INT NULL;
ALTER TABLE NCDRiskAssessments ADD HasAsthma BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasDifficultyBreathing BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasNoRegularExercise BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HealthFacility NVARCHAR(100) NULL;
ALTER TABLE NCDRiskAssessments ADD HighSaltIntake BIT NULL;

-- Verify columns were added
SELECT 
    COLUMN_NAME, 
    DATA_TYPE,
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'NCDRiskAssessments'
    AND COLUMN_NAME IN (
        'ChestPain', 
        'ChestPainLocation', 
        'ChestPainValue',
        'HasAsthma',
        'HasDifficultyBreathing',
        'HasNoRegularExercise',
        'HealthFacility',
        'HighSaltIntake'
    ); 