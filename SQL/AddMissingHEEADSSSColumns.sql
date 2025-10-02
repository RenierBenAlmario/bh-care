-- Add missing columns to HEEADSSSAssessments table
-- These columns are referenced in the model but missing from the database

-- PhilHealth and program fields
ALTER TABLE HEEADSSSAssessments ADD IsNHPTS NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD Is4Ps NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD IsPhilHealthBeneficiaryOnly NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD IsOwnPhilHealth NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD PhilHealthPIN NVARCHAR(4000);

-- Sexuality fields
ALTER TABLE HEEADSSSAssessments ADD SexualityHealthConcerns NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD SexualityPartnersCount NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD SexualityPregnancyExperience NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD SexualityProtectionUse NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD SexualitySTIExperience NVARCHAR(4000);
ALTER TABLE HEEADSSSAssessments ADD SexualityHarassment NVARCHAR(4000);
