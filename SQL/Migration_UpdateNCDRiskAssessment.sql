-- Migration Script to Update NCD Risk Assessment Table
-- This script adds new fields to match the actual NCD form structure

-- Add new columns to existing NCDRiskAssessments table
ALTER TABLE NCDRiskAssessments 
ADD 
    -- Health Facility Information
    DateOfAssessment DATE DEFAULT GETDATE(),
    
    -- Part I: Demographic-Socio-Economic Profile
    IDNo NVARCHAR(50),
    
    -- Part II: Past Medical History Details
    DiabetesYear NVARCHAR(10),
    DiabetesMedication NVARCHAR(100),
    HypertensionYear NVARCHAR(10),
    HypertensionMedication NVARCHAR(100),
    CancerYear NVARCHAR(10),
    CancerMedication NVARCHAR(100),
    LungDiseaseYear NVARCHAR(10),
    LungDiseaseMedication NVARCHAR(100),
    COPDYear NVARCHAR(10),
    COPDMedication NVARCHAR(100),
    EyeDiseaseYear NVARCHAR(10),
    EyeDiseaseMedication NVARCHAR(100),
    
    -- Part III: Lifestyle Factors
    -- B.1 Diet
    EatsSaltyFood BIT DEFAULT 0,
    EatsSweetFood BIT DEFAULT 0,
    EatsFattyFood BIT DEFAULT 0,
    
    -- B.2 Alcohol
    DrinksAlcohol BIT DEFAULT 0,
    AlcoholStoppedDuration NVARCHAR(50),
    DrinksBeer BIT DEFAULT 0,
    DrinksWine BIT DEFAULT 0,
    DrinksHardLiquor BIT DEFAULT 0,
    BeerAmount NVARCHAR(50),
    WineAmount NVARCHAR(50),
    HardLiquorAmount NVARCHAR(50),
    AlcoholPerOccasion NVARCHAR(50),
    IsBingeDrinker BIT DEFAULT 0,
    
    -- B.3 Exercise
    ExerciseType NVARCHAR(50),
    
    -- B.4 Smoking
    IsSmoker BIT DEFAULT 0,
    SmokingSticksPerDay NVARCHAR(50),
    SmokingDuration NVARCHAR(50),
    SmokingQuitDuration NVARCHAR(50),
    ExposedToSmoke BIT DEFAULT 0,
    Smoked100Sticks BIT DEFAULT 0,
    
    -- B.5 Stress
    IsStressed BIT DEFAULT 0,
    StressCauses NVARCHAR(255),
    StressAffectsDailyLife BIT DEFAULT 0,
    
    -- Part IV: Risk Screening
    -- 4.1 Anthropometric Measurement
    Weight DECIMAL(5,2),
    Height DECIMAL(5,2),
    BMI DECIMAL(4,1),
    BMIStatus NVARCHAR(50),
    
    -- 4.2 Blood Pressure
    SystolicBP INT,
    DiastolicBP INT,
    BPStatus NVARCHAR(50),
    
    -- 4.3 Blood Sugar
    FastingBloodSugar DECIMAL(5,2),
    RandomBloodSugar DECIMAL(5,2),
    BloodSugarStatus NVARCHAR(50),
    
    -- 4.4 Cholesterol
    TotalCholesterol DECIMAL(5,2),
    HDLCholesterol DECIMAL(5,2),
    LDLCholesterol DECIMAL(5,2),
    Triglycerides DECIMAL(5,2),
    CholesterolStatus NVARCHAR(50),
    
    -- 4.5 Urine Dipstick Test
    UrineProtein NVARCHAR(10),
    UrineKetones NVARCHAR(10),
    
    -- 4.6 Risk Profile For Doctors Only
    RiskPercentage NVARCHAR(10),
    
    -- 4.7 Cancer Screening (Women 30 years old and above)
    BreastCancerScreened BIT DEFAULT 0,
    CervicalCancerScreened BIT DEFAULT 0,
    CancerScreeningStatus NVARCHAR(50),
    
    -- Assessment Information
    InterviewedBy NVARCHAR(100),
    Designation NVARCHAR(100),
    AssessmentDate DATE,
    PatientSignature NVARCHAR(100),
    
    -- Risk Status Summary
    RiskFactors NVARCHAR(MAX);

-- Update existing records with default values
UPDATE NCDRiskAssessments 
SET 
    DateOfAssessment = ISNULL(DateOfAssessment, CreatedAt),
    HealthFacility = ISNULL(HealthFacility, 'Barangay Health Center'),
    SmokingStatus = ISNULL(SmokingStatus, 'Non-smoker'),
    RiskStatus = ISNULL(RiskStatus, 'Low Risk');

-- Create indexes for better performance
CREATE INDEX IX_NCDRiskAssessments_DateOfAssessment ON NCDRiskAssessments(DateOfAssessment);
CREATE INDEX IX_NCDRiskAssessments_BMIStatus ON NCDRiskAssessments(BMIStatus);
CREATE INDEX IX_NCDRiskAssessments_RiskPercentage ON NCDRiskAssessments(RiskPercentage);

-- Add constraints for data validation
ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT CK_NCDRiskAssessments_BMI 
    CHECK (BMI IS NULL OR (BMI >= 10 AND BMI <= 100));

ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT CK_NCDRiskAssessments_Weight 
    CHECK (Weight IS NULL OR (Weight >= 20 AND Weight <= 500));

ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT CK_NCDRiskAssessments_Height 
    CHECK (Height IS NULL OR (Height >= 100 AND Height <= 250));

ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT CK_NCDRiskAssessments_SystolicBP 
    CHECK (SystolicBP IS NULL OR (SystolicBP >= 70 AND SystolicBP <= 250));

ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT CK_NCDRiskAssessments_DiastolicBP 
    CHECK (DiastolicBP IS NULL OR (DiastolicBP >= 40 AND DiastolicBP <= 150));

-- Add comments for documentation
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Updated NCD Risk Assessment table matching the actual form structure', 
    @level0type = N'SCHEMA', @level0name = N'dbo', 
    @level1type = N'TABLE', @level1name = N'NCDRiskAssessments';

PRINT 'NCD Risk Assessment table has been successfully updated with new fields matching the form structure.';
