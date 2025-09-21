# NCD Risk Assessment Database Structure

## Overview
This document describes the updated NCD (Non-Communicable Disease) Risk Assessment database structure that matches the actual NCD form used in the Barangay Health Care System.

## Database Table: NCDRiskAssessments

### Health Facility Information
- `HealthFacility` (NVARCHAR(100)): Name of the health facility (default: "Barangay Health Center")
- `DateOfAssessment` (DATE): Date when the assessment was conducted (default: current date)

### Part I: Demographic-Socio-Economic Profile
- `FamilyNo` (NVARCHAR(50)): Family number
- `IDNo` (NVARCHAR(50)): Patient ID number
- `FirstName` (NVARCHAR(50)): Patient's first name
- `MiddleName` (NVARCHAR(50)): Patient's middle name
- `LastName` (NVARCHAR(50)): Patient's last name
- `Address` (NVARCHAR(255)): Patient's address
- `Barangay` (NVARCHAR(100)): Barangay name
- `Telepono` (NVARCHAR(20)): Contact number
- `Birthday` (DATE): Date of birth
- `Edad` (INT): Age (20-120 years)
- `Kasarian` (NVARCHAR(10)): Gender (P = Female, L = Male)
- `Relihiyon` (NVARCHAR(50)): Religion
- `CivilStatus` (NVARCHAR(50)): Civil status (W = Widow, M = Married, S = Single, D = Divorced)
- `Occupation` (NVARCHAR(100)): Occupation

### Part II: Past Medical History
#### Diabetes
- `HasDiabetes` (BIT): Whether patient has diabetes
- `DiabetesYear` (NVARCHAR(10)): Year when diabetes was diagnosed
- `DiabetesMedication` (NVARCHAR(100)): Current diabetes medication

#### Hypertension
- `HasHypertension` (BIT): Whether patient has hypertension
- `HypertensionYear` (NVARCHAR(10)): Year when hypertension was diagnosed
- `HypertensionMedication` (NVARCHAR(100)): Current hypertension medication

#### Cancer
- `HasCancer` (BIT): Whether patient has cancer
- `CancerType` (NVARCHAR(100)): Type of cancer
- `CancerYear` (NVARCHAR(10)): Year when cancer was diagnosed
- `CancerMedication` (NVARCHAR(100)): Current cancer medication

#### Lung Disease
- `HasLungDisease` (BIT): Whether patient has lung disease (non-communicable)
- `LungDiseaseYear` (NVARCHAR(10)): Year when lung disease was diagnosed
- `LungDiseaseMedication` (NVARCHAR(100)): Current lung disease medication

#### COPD
- `HasCOPD` (BIT): Whether patient has COPD
- `COPDYear` (NVARCHAR(10)): Year when COPD was diagnosed
- `COPDMedication` (NVARCHAR(100)): Current COPD medication

#### Eye Disease
- `HasEyeDisease` (BIT): Whether patient has eye disease
- `EyeDiseaseYear` (NVARCHAR(10)): Year when eye disease was diagnosed
- `EyeDiseaseMedication` (NVARCHAR(100)): Current eye disease medication

### Part III: Lifestyle Factors

#### B.1 Diet
- `EatsSaltyFood` (BIT): Eats salty food more than 2 times per week
- `EatsSweetFood` (BIT): Eats sweet food more than 2 times per week
- `EatsFattyFood` (BIT): Eats fatty food more than 2 times per week
- `HasHighSaltIntake` (BIT): Has high salt intake

#### B.2 Alcohol
- `DrinksAlcohol` (BIT): Whether patient drinks alcohol
- `AlcoholStoppedDuration` (NVARCHAR(50)): Duration since stopped drinking (if applicable)
- `DrinksBeer` (BIT): Drinks beer
- `DrinksWine` (BIT): Drinks wine
- `DrinksHardLiquor` (BIT): Drinks whisky/gin/brandy
- `BeerAmount` (NVARCHAR(50)): Amount of beer consumed (1 bote, 2 bote, >3 bote)
- `WineAmount` (NVARCHAR(50)): Amount of wine consumed (3 glasses, >4 glasses)
- `HardLiquorAmount` (NVARCHAR(50)): Amount of hard liquor consumed (<3 shots, >4 shots)
- `AlcoholFrequency` (NVARCHAR(50)): How often drinks alcohol per week
- `AlcoholPerOccasion` (NVARCHAR(50)): Amount consumed per occasion
- `IsBingeDrinker` (BIT): Whether patient is a binge drinker

#### B.3 Exercise
- `HasRegularExercise` (BIT): Whether patient has regular exercise
- `ExerciseType` (NVARCHAR(50)): Type of exercise (Moderate, Vigorous, Combination)
- `ExerciseDuration` (NVARCHAR(50)): Duration of exercise

#### B.4 Smoking
- `IsSmoker` (BIT): Whether patient smokes
- `SmokingSticksPerDay` (NVARCHAR(50)): Number of cigarettes per day
- `SmokingDuration` (NVARCHAR(50)): Duration of smoking
- `SmokingQuitDuration` (NVARCHAR(50)): Duration since quit smoking
- `ExposedToSmoke` (BIT): Exposed to secondhand smoke
- `Smoked100Sticks` (BIT): Smoked more than 100 sticks in lifetime

#### B.5 Stress
- `IsStressed` (BIT): Whether patient is stressed
- `StressCauses` (NVARCHAR(255)): Causes of stress
- `StressAffectsDailyLife` (BIT): Whether stress affects daily life

### Part IV: Risk Screening

#### 4.1 Anthropometric Measurement
- `Weight` (DECIMAL(5,2)): Weight in kg
- `Height` (DECIMAL(5,2)): Height in cm
- `BMI` (DECIMAL(4,1)): Body Mass Index
- `BMIStatus` (NVARCHAR(50)): BMI category (Underweight, Normal, Overweight, Obese)

#### 4.2 Blood Pressure
- `SystolicBP` (INT): Systolic blood pressure
- `DiastolicBP` (INT): Diastolic blood pressure
- `BPStatus` (NVARCHAR(50)): Blood pressure status

#### 4.3 Blood Sugar
- `FastingBloodSugar` (DECIMAL(5,2)): Fasting blood sugar level
- `RandomBloodSugar` (DECIMAL(5,2)): Random blood sugar level
- `BloodSugarStatus` (NVARCHAR(50)): Blood sugar status

#### 4.4 Cholesterol
- `TotalCholesterol` (DECIMAL(5,2)): Total cholesterol level
- `HDLCholesterol` (DECIMAL(5,2)): HDL cholesterol level
- `LDLCholesterol` (DECIMAL(5,2)): LDL cholesterol level
- `Triglycerides` (DECIMAL(5,2)): Triglycerides level
- `CholesterolStatus` (NVARCHAR(50)): Cholesterol status

#### 4.5 Urine Dipstick Test
- `UrineProtein` (NVARCHAR(10)): Urine protein test result (+ or -)
- `UrineKetones` (NVARCHAR(10)): Urine ketones test result (+ or -)

#### 4.6 Risk Profile (For Doctors Only)
- `RiskPercentage` (NVARCHAR(10)): Risk percentage (<5%, 5%-10%, 10%-20%, 20-<30%, >30%)

#### 4.7 Cancer Screening (Women 30+ years old)
- `BreastCancerScreened` (BIT): Whether breast cancer screened
- `CervicalCancerScreened` (BIT): Whether cervical cancer screened
- `CancerScreeningStatus` (NVARCHAR(50)): Screening status (Yes - remind next visit, No - refer)

### Assessment Information
- `InterviewedBy` (NVARCHAR(100)): Name of person who conducted the assessment
- `Designation` (NVARCHAR(100)): Designation of interviewer
- `AssessmentDate` (DATE): Date of assessment
- `PatientSignature` (NVARCHAR(100)): Patient's signature

### Risk Status Summary
- `RiskStatus` (NVARCHAR(50)): Overall risk status
- `RiskFactors` (NVARCHAR(MAX)): Summary of identified risk factors

### System Fields
- `CreatedAt` (DATETIME): Record creation timestamp
- `UpdatedAt` (DATETIME): Record last update timestamp
- `AppointmentId` (INT): Foreign key to Appointments table
- `UserId` (NVARCHAR(450)): Foreign key to AspNetUsers table

## Data Validation Rules

### Constraints
- BMI: Must be between 10 and 100
- Weight: Must be between 20 and 500 kg
- Height: Must be between 100 and 250 cm
- Systolic BP: Must be between 70 and 250 mmHg
- Diastolic BP: Must be between 40 and 150 mmHg
- Age: Must be between 20 and 120 years

### Indexes
- `IX_NCDRiskAssessments_UserId`: For user-based queries
- `IX_NCDRiskAssessments_AppointmentId`: For appointment-based queries
- `IX_NCDRiskAssessments_DateOfAssessment`: For date-based queries
- `IX_NCDRiskAssessments_BMIStatus`: For BMI status queries
- `IX_NCDRiskAssessments_RiskPercentage`: For risk percentage queries

## Migration Notes

To update an existing database:
1. Run the migration script `SQL/Migration_UpdateNCDRiskAssessment.sql`
2. Update the Entity Framework model in `Models/NCDRiskAssessment.cs`
3. Update the ViewModel in `Models/NCDRiskAssessmentViewModel.cs`
4. Update the form views to include new fields
5. Test the updated functionality

## Form Mapping

The database structure now exactly matches the NCD Risk Assessment form fields:
- **Page 1**: Demographic information, medical history, lifestyle factors
- **Page 2**: Risk screening, laboratory results, cancer screening, assessment information

This ensures data integrity and accurate representation of the actual form data in the system.
