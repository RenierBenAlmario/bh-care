-- Updated NCD Risk Assessment Database Structure
-- Based on the actual NCD form images provided

-- Drop existing table if it exists (for migration)
-- DROP TABLE IF EXISTS NCDRiskAssessments;

CREATE TABLE NCDRiskAssessments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId NVARCHAR(450) NOT NULL,
    AppointmentId INT NOT NULL,
    
    -- Health Facility Information
    HealthFacility NVARCHAR(100) DEFAULT 'Barangay Health Center',
    DateOfAssessment DATE DEFAULT GETDATE(),
    
    -- Part I: Demographic-Socio-Economic Profile
    FamilyNo NVARCHAR(50),
    IDNo NVARCHAR(50),
    FirstName NVARCHAR(50),
    MiddleName NVARCHAR(50),
    LastName NVARCHAR(50),
    Address NVARCHAR(255),
    Barangay NVARCHAR(100),
    Telepono NVARCHAR(20),
    Birthday DATE,
    Edad INT,
    Kasarian NVARCHAR(10), -- P (Panginoon/Female), L (Lalaki/Male)
    Relihiyon NVARCHAR(50),
    CivilStatus NVARCHAR(50), -- W (Widow), M (Married), S (Single), D (Divorced)
    Occupation NVARCHAR(100),
    
    -- Part II: Past Medical History
    -- 1. Ikaw ba ay may sumusunod nakaramdaman?
    HasDiabetes BIT DEFAULT 0,
    DiabetesYear NVARCHAR(10), -- Taon Nalaman ang Sakit
    DiabetesMedication NVARCHAR(100), -- Iniinom na Gamot
    
    HasHypertension BIT DEFAULT 0,
    HypertensionYear NVARCHAR(10),
    HypertensionMedication NVARCHAR(100),
    
    HasCancer BIT DEFAULT 0,
    CancerType NVARCHAR(100),
    CancerYear NVARCHAR(10),
    CancerMedication NVARCHAR(100),
    
    HasLungDisease BIT DEFAULT 0, -- Sakit sa baga na hindi nakakahawa
    LungDiseaseYear NVARCHAR(10),
    LungDiseaseMedication NVARCHAR(100),
    
    HasCOPD BIT DEFAULT 0,
    COPDYear NVARCHAR(10),
    COPDMedication NVARCHAR(100),
    
    HasEyeDisease BIT DEFAULT 0,
    EyeDiseaseYear NVARCHAR(10),
    EyeDiseaseMedication NVARCHAR(100),
    
    -- Part III: Lifestyle Factors
    -- B.1 Diet
    EatsSaltyFood BIT DEFAULT 0, -- Maalat na pagkain
    EatsSweetFood BIT DEFAULT 0, -- Matatamis na pagkain
    EatsFattyFood BIT DEFAULT 0, -- Mamantikang pagkain
    HasHighSaltIntake BIT DEFAULT 0,
    
    -- B.2 Alcohol
    DrinksAlcohol BIT DEFAULT 0, -- Umiinom ka ba ng alak?
    AlcoholStoppedDuration NVARCHAR(50), -- Kung hindi, gaano katagal ka natumigil sa pag-inom?
    DrinksBeer BIT DEFAULT 0,
    DrinksWine BIT DEFAULT 0,
    DrinksHardLiquor BIT DEFAULT 0, -- Whisky/Gin/Brandy
    
    -- Alcohol consumption amounts
    BeerAmount NVARCHAR(50), -- 1 bote (320 ml), 2 bote (640 ml), > 3 bote
    WineAmount NVARCHAR(50), -- 3 wine glasses (300 ml), > 4 wine glasses
    HardLiquorAmount NVARCHAR(50), -- <3 shots/jigger (75 ml), > 4 shots/jigger
    
    AlcoholFrequency NVARCHAR(50), -- Gaanokadalas ka umiinom ng alaksaisanglinggo?
    AlcoholPerOccasion NVARCHAR(50), -- Sa isang okasyon, ilang bote ng alak ang naiinom mo?
    IsBingeDrinker BIT DEFAULT 0,
    
    -- B.3 Exercise
    HasRegularExercise BIT DEFAULT 0, -- May sapat ka bang ehersisyo?
    ExerciseType NVARCHAR(50), -- Moderate Intensity, Vigorous Intensity, Kombinasyon ng dalawa
    ExerciseDuration NVARCHAR(50),
    
    -- B.4 Smoking
    IsSmoker BIT DEFAULT 0, -- Ikaw ba ay naninigarilyo?
    SmokingSticksPerDay NVARCHAR(50),
    SmokingDuration NVARCHAR(50), -- Gaano katagal ka naninigarilyo?
    SmokingQuitDuration NVARCHAR(50), -- Kung tumigil ka, gaano katagal ka natumigil?
    ExposedToSmoke BIT DEFAULT 0, -- Nakakalanghap ka ba ng usok ng sigarilyo?
    Smoked100Sticks BIT DEFAULT 0, -- Ikaw ba ay naka-100 sticks o higit pa sa panahon ng paninigarilyo?
    
    -- B.5 Stress
    IsStressed BIT DEFAULT 0, -- Ikaw ba ay stressed?
    StressCauses NVARCHAR(255), -- Ano ang dahilan ng stress?
    StressAffectsDailyLife BIT DEFAULT 0, -- Naaapektuhan ba ang iyong araw-araw na pamumuhay o paggalaw dahil dito?
    
    -- Part IV: Risk Screening
    -- 4.1 Anthropometric Measurement
    Weight DECIMAL(5,2), -- Weight in kg
    Height DECIMAL(5,2), -- Height in cm
    BMI DECIMAL(4,1), -- BMI calculation
    BMIStatus NVARCHAR(50), -- Underweight (<18.5), Normal (18.5-22.9), Overweight (23-24.9), Obese (≥25)
    
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
    UrineProtein NVARCHAR(10), -- (+) urine protein, (-) urine protein
    UrineKetones NVARCHAR(10), -- (+) urine ketones, (-) urine ketones
    
    -- 4.6 Risk Profile For Doctors Only
    RiskPercentage NVARCHAR(10), -- <5%, 5%-10%, 10%-20%, 20-<30%, >30% risk
    
    -- 4.7 Cancer Screening (Women 30 years old and above)
    BreastCancerScreened BIT DEFAULT 0, -- Ikaw ba ay nai-screen na sa Breast/Cervical Cancer?
    CervicalCancerScreened BIT DEFAULT 0,
    CancerScreeningStatus NVARCHAR(50), -- Oo (Ipaalala ang next visit), Hindi (i-refer)
    
    -- Assessment Information
    InterviewedBy NVARCHAR(100), -- Interviewed/Assessed by
    Designation NVARCHAR(100),
    AssessmentDate DATE,
    PatientSignature NVARCHAR(100), -- Signature of Patient
    
    -- Risk Status Summary
    RiskStatus NVARCHAR(50),
    RiskFactors NVARCHAR(MAX), -- Summary of identified risk factors
    
    -- System Fields
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    
    -- Foreign Keys
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id)
);

-- Create indexes for better performance
CREATE INDEX IX_NCDRiskAssessments_UserId ON NCDRiskAssessments(UserId);
CREATE INDEX IX_NCDRiskAssessments_AppointmentId ON NCDRiskAssessments(AppointmentId);
CREATE INDEX IX_NCDRiskAssessments_AssessmentDate ON NCDRiskAssessments(DateOfAssessment);
CREATE INDEX IX_NCDRiskAssessments_RiskStatus ON NCDRiskAssessments(RiskStatus);
