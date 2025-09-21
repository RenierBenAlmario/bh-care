-- Database Schema Update for NCD Risk Assessment Encryption
-- This script updates column sizes to accommodate encrypted data

-- Personal Information Fields
ALTER TABLE NCDRiskAssessments ALTER COLUMN FirstName nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN LastName nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN MiddleName nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN Kasarian nvarchar(50);
ALTER TABLE NCDRiskAssessments ALTER COLUMN Relihiyon nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN CivilStatus nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN Occupation nvarchar(200);

-- Medical Information Fields
ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerType nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerMedication nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN CancerYear nvarchar(50);
ALTER TABLE NCDRiskAssessments ALTER COLUMN DiabetesMedication nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN DiabetesYear nvarchar(50);
ALTER TABLE NCDRiskAssessments ALTER COLUMN HypertensionMedication nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN HypertensionYear nvarchar(50);
ALTER TABLE NCDRiskAssessments ALTER COLUMN LungDiseaseMedication nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN LungDiseaseYear nvarchar(50);

-- Lifestyle and Health Status Fields
ALTER TABLE NCDRiskAssessments ALTER COLUMN SmokingStatus nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN AlcoholFrequency nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN AlcoholConsumption nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN ExerciseDuration nvarchar(100);
ALTER TABLE NCDRiskAssessments ALTER COLUMN RiskStatus nvarchar(100);

-- Chest Pain and Other Medical Fields
ALTER TABLE NCDRiskAssessments ALTER COLUMN ChestPain nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN ChestPainLocation nvarchar(200);

-- Appointment and Family History Fields
ALTER TABLE NCDRiskAssessments ALTER COLUMN AppointmentType nvarchar(200);
ALTER TABLE NCDRiskAssessments ALTER COLUMN FamilyHistoryOther nvarchar(200);

-- Verify the changes
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'NCDRiskAssessments' 
AND COLUMN_NAME IN (
    'FirstName', 'LastName', 'MiddleName', 'Kasarian', 'Relihiyon', 'CivilStatus', 'Occupation',
    'CancerType', 'CancerMedication', 'CancerYear', 'DiabetesMedication', 'DiabetesYear',
    'HypertensionMedication', 'HypertensionYear', 'LungDiseaseMedication', 'LungDiseaseYear',
    'SmokingStatus', 'AlcoholFrequency', 'AlcoholConsumption', 'ExerciseDuration', 'RiskStatus',
    'ChestPain', 'ChestPainLocation', 'AppointmentType', 'FamilyHistoryOther',
    'Telepono', 'Address', 'FamilyNo', 'FamilyOtherDiseaseDetails'
)
ORDER BY COLUMN_NAME;
