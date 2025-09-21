-- SQL Script to make NCDRiskAssessment fields nullable
-- This script should be run directly against the database

-- Make string columns nullable
ALTER TABLE NCDRiskAssessments ALTER COLUMN UserId NVARCHAR(450) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN HealthFacility NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN FamilyNo NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Address NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Barangay NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Telepono NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Kasarian NVARCHAR(10) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Relihiyon NVARCHAR(50) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType NVARCHAR(100) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN FamilyOtherDiseaseDetails NVARCHAR(MAX) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN SmokingStatus NVARCHAR(20) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN AlcoholFrequency NVARCHAR(50) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN AlcoholConsumption NVARCHAR(50) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN ExerciseDuration NVARCHAR(50) NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN RiskStatus NVARCHAR(20) NULL;

-- Make integer and date columns nullable
ALTER TABLE NCDRiskAssessments ALTER COLUMN AppointmentId INT NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Edad INT NULL;
ALTER TABLE NCDRiskAssessments ALTER COLUMN Birthday DATETIME NULL;

-- Make boolean columns nullable 
-- Note: SQL Server doesn't directly support nullable bit columns through ALTER COLUMN
-- You need to drop and recreate the column
-- First add new nullable columns
ALTER TABLE NCDRiskAssessments ADD HasDiabetesNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasHypertensionNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasCancerNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasCOPDNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasLungDiseaseNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HasEyeDiseaseNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasHypertensionNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasHeartDiseaseNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasStrokeNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasDiabetesNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasCancerNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasKidneyDiseaseNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD FamilyHasOtherDiseaseNew BIT NULL;
ALTER TABLE NCDRiskAssessments ADD HighSaltIntakeNew BIT NULL;

-- Copy data from old to new columns
UPDATE NCDRiskAssessments SET 
    HasDiabetesNew = HasDiabetes,
    HasHypertensionNew = HasHypertension,
    HasCancerNew = HasCancer,
    HasCOPDNew = HasCOPD,
    HasLungDiseaseNew = HasLungDisease,
    HasEyeDiseaseNew = HasEyeDisease,
    FamilyHasHypertensionNew = FamilyHasHypertension,
    FamilyHasHeartDiseaseNew = FamilyHasHeartDisease,
    FamilyHasStrokeNew = FamilyHasStroke,
    FamilyHasDiabetesNew = FamilyHasDiabetes,
    FamilyHasCancerNew = FamilyHasCancer,
    FamilyHasKidneyDiseaseNew = FamilyHasKidneyDisease,
    FamilyHasOtherDiseaseNew = FamilyHasOtherDisease,
    HighSaltIntakeNew = HighSaltIntake;

-- Drop old columns
ALTER TABLE NCDRiskAssessments DROP COLUMN HasDiabetes;
ALTER TABLE NCDRiskAssessments DROP COLUMN HasHypertension;
ALTER TABLE NCDRiskAssessments DROP COLUMN HasCancer;
ALTER TABLE NCDRiskAssessments DROP COLUMN HasCOPD;
ALTER TABLE NCDRiskAssessments DROP COLUMN HasLungDisease;
ALTER TABLE NCDRiskAssessments DROP COLUMN HasEyeDisease;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasHypertension;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasHeartDisease;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasStroke;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasDiabetes;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasCancer;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasKidneyDisease;
ALTER TABLE NCDRiskAssessments DROP COLUMN FamilyHasOtherDisease;
ALTER TABLE NCDRiskAssessments DROP COLUMN HighSaltIntake;

-- Rename new columns to original names
EXEC sp_rename 'NCDRiskAssessments.HasDiabetesNew', 'HasDiabetes', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HasHypertensionNew', 'HasHypertension', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HasCancerNew', 'HasCancer', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HasCOPDNew', 'HasCOPD', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HasLungDiseaseNew', 'HasLungDisease', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HasEyeDiseaseNew', 'HasEyeDisease', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasHypertensionNew', 'FamilyHasHypertension', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasHeartDiseaseNew', 'FamilyHasHeartDisease', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasStrokeNew', 'FamilyHasStroke', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasDiabetesNew', 'FamilyHasDiabetes', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasCancerNew', 'FamilyHasCancer', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasKidneyDiseaseNew', 'FamilyHasKidneyDisease', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.FamilyHasOtherDiseaseNew', 'FamilyHasOtherDisease', 'COLUMN';
EXEC sp_rename 'NCDRiskAssessments.HighSaltIntakeNew', 'HighSaltIntake', 'COLUMN';

-- Make UpdatedAt nullable (CreatedAt should remain non-nullable with default)
ALTER TABLE NCDRiskAssessments ALTER COLUMN UpdatedAt DATETIME NULL; 