
-- Add missing columns to NCDRiskAssessments table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CancerMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerMedication NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CancerYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerYear NVARCHAR(4) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'CivilStatus' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CivilStatus NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'DiabetesMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesMedication NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'DiabetesYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesYear NVARCHAR(4) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryCancerSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryCancerSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryDiabetesSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryDiabetesSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryHeartDiseaseSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryHeartDiseaseSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryLungDiseaseSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryLungDiseaseSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOther' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOther NVARCHAR(100) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryOtherSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryOtherSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeFather' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeFather BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeMother' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeMother BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FamilyHistoryStrokeSibling' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FamilyHistoryStrokeSibling BIT NOT NULL DEFAULT 0;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'FirstName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FirstName NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HypertensionMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionMedication NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'HypertensionYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionYear NVARCHAR(4) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LastName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LastName NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LungDiseaseMedication' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseMedication NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'LungDiseaseYear' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseYear NVARCHAR(4) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'MiddleName' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD MiddleName NVARCHAR(50) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE Name = N'Occupation' AND Object_ID = Object_ID(N'NCDRiskAssessments'))
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Occupation NVARCHAR(100) NULL;
END
