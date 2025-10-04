-- Fix NCD Risk Assessment Missing Columns
-- This script adds all the missing columns that are collected in the form but not saved to the database

-- Check if columns exist before adding them
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Smoked100Sticks')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Smoked100Sticks BIT DEFAULT 0;
    PRINT 'Added Smoked100Sticks column';
END
ELSE
BEGIN
    PRINT 'Smoked100Sticks column already exists';
END

-- Alcohol Amount Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholAmount1Bottle320ml')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholAmount1Bottle320ml BIT DEFAULT 0;
    PRINT 'Added AlcoholAmount1Bottle320ml column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholAmount2Bottle640ml')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholAmount2Bottle640ml BIT DEFAULT 0;
    PRINT 'Added AlcoholAmount2Bottle640ml column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholAmount3to4WineGlasses300ml')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholAmount3to4WineGlasses300ml BIT DEFAULT 0;
    PRINT 'Added AlcoholAmount3to4WineGlasses300ml column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholAmountLessThan3Shot45ml')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholAmountLessThan3Shot45ml BIT DEFAULT 0;
    PRINT 'Added AlcoholAmountLessThan3Shot45ml column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholAmountMoreThan4Shots75ml')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholAmountMoreThan4Shots75ml BIT DEFAULT 0;
    PRINT 'Added AlcoholAmountMoreThan4Shots75ml column';
END

-- Alcohol Frequency Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholFrequency1to3TimesPerWeek')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholFrequency1to3TimesPerWeek BIT DEFAULT 0;
    PRINT 'Added AlcoholFrequency1to3TimesPerWeek column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholFrequencyMoreThan4TimesPerWeek')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholFrequencyMoreThan4TimesPerWeek BIT DEFAULT 0;
    PRINT 'Added AlcoholFrequencyMoreThan4TimesPerWeek column';
END

-- Exercise Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'ModerateIntensityExercise')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ModerateIntensityExercise BIT DEFAULT 0;
    PRINT 'Added ModerateIntensityExercise column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'VigorousIntensityExercise')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD VigorousIntensityExercise BIT DEFAULT 0;
    PRINT 'Added VigorousIntensityExercise column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'InsufficientPhysicalActivity')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD InsufficientPhysicalActivity BIT DEFAULT 0;
    PRINT 'Added InsufficientPhysicalActivity column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'CombinationExercise')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CombinationExercise BIT DEFAULT 0;
    PRINT 'Added CombinationExercise column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HasEnoughExercise')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasEnoughExercise BIT DEFAULT 0;
    PRINT 'Added HasEnoughExercise column';
END

-- Smoking Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'FormerSmoker')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FormerSmoker BIT DEFAULT 0;
    PRINT 'Added FormerSmoker column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'NeverSmokedButExposedToSmoke')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD NeverSmokedButExposedToSmoke BIT DEFAULT 0;
    PRINT 'Added NeverSmokedButExposedToSmoke column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HasHistoryOfSmoking')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasHistoryOfSmoking BIT DEFAULT 0;
    PRINT 'Added HasHistoryOfSmoking column';
END

-- Chest Pain Question Columns (Q2.1-2.8)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit21')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit21 BIT DEFAULT 0;
    PRINT 'Added Pananakit21 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit22')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit22 BIT DEFAULT 0;
    PRINT 'Added Pananakit22 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit23')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit23 BIT DEFAULT 0;
    PRINT 'Added Pananakit23 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit24')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit24 BIT DEFAULT 0;
    PRINT 'Added Pananakit24 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit25')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit25 BIT DEFAULT 0;
    PRINT 'Added Pananakit25 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit26')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit26 BIT DEFAULT 0;
    PRINT 'Added Pananakit26 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit27')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit27 BIT DEFAULT 0;
    PRINT 'Added Pananakit27 column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Pananakit28')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Pananakit28 BIT DEFAULT 0;
    PRINT 'Added Pananakit28 column';
END

-- Additional Chest Pain Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'ChestPainSpreadsToArm')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD ChestPainSpreadsToArm BIT DEFAULT 0;
    PRINT 'Added ChestPainSpreadsToArm column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'NumbnessWhenWalkingFast')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD NumbnessWhenWalkingFast BIT DEFAULT 0;
    PRINT 'Added NumbnessWhenWalkingFast column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'LossOfConsciousnessLessThan10Min')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LossOfConsciousnessLessThan10Min BIT DEFAULT 0;
    PRINT 'Added LossOfConsciousnessLessThan10Min column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'PainLastsMoreThan30Min')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD PainLastsMoreThan30Min BIT DEFAULT 0;
    PRINT 'Added PainLastsMoreThan30Min column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'SeeDoctorIfYes')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD SeeDoctorIfYes BIT DEFAULT 0;
    PRINT 'Added SeeDoctorIfYes column';
END

-- Nutrition Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsVegetablesDaily')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsVegetablesDaily BIT DEFAULT 0;
    PRINT 'Added EatsVegetablesDaily column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsFruitsDaily')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsFruitsDaily BIT DEFAULT 0;
    PRINT 'Added EatsFruitsDaily column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsFishDaily')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsFishDaily BIT DEFAULT 0;
    PRINT 'Added EatsFishDaily column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsMeatDaily')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsMeatDaily BIT DEFAULT 0;
    PRINT 'Added EatsMeatDaily column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HasUnhealthyDiet')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasUnhealthyDiet BIT DEFAULT 0;
    PRINT 'Added HasUnhealthyDiet column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsFattyFoodMoreThan2TimesPerWeek')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsFattyFoodMoreThan2TimesPerWeek BIT DEFAULT 0;
    PRINT 'Added EatsFattyFoodMoreThan2TimesPerWeek column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsSweetFoodMoreThan2TimesPerWeek')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsSweetFoodMoreThan2TimesPerWeek BIT DEFAULT 0;
    PRINT 'Added EatsSweetFoodMoreThan2TimesPerWeek column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EatsOilyFoodMoreThan2TimesPerWeek')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EatsOilyFoodMoreThan2TimesPerWeek BIT DEFAULT 0;
    PRINT 'Added EatsOilyFoodMoreThan2TimesPerWeek column';
END

-- Additional Alcohol Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DrinksAlcohol')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DrinksAlcohol BIT DEFAULT 0;
    PRINT 'Added DrinksAlcohol column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DrinksBeer')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DrinksBeer BIT DEFAULT 0;
    PRINT 'Added DrinksBeer column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DrinksWine')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DrinksWine BIT DEFAULT 0;
    PRINT 'Added DrinksWine column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DrinksWhiskyGinBrandy')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DrinksWhiskyGinBrandy BIT DEFAULT 0;
    PRINT 'Added DrinksWhiskyGinBrandy column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholPerOccasion')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholPerOccasion NVARCHAR(50) NULL;
    PRINT 'Added AlcoholPerOccasion column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'AlcoholStoppedDuration')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD AlcoholStoppedDuration NVARCHAR(50) NULL;
    PRINT 'Added AlcoholStoppedDuration column';
END

-- Additional Health Condition Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HasAsthma')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasAsthma BIT DEFAULT 0;
    PRINT 'Added HasAsthma column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HasDifficultyBreathing')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HasDifficultyBreathing BIT DEFAULT 0;
    PRINT 'Added HasDifficultyBreathing column';
END

-- Additional Personal Information Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'FirstName')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD FirstName NVARCHAR(100) NULL;
    PRINT 'Added FirstName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'MiddleName')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD MiddleName NVARCHAR(100) NULL;
    PRINT 'Added MiddleName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'LastName')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LastName NVARCHAR(100) NULL;
    PRINT 'Added LastName column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'Occupation')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD Occupation NVARCHAR(100) NULL;
    PRINT 'Added Occupation column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'CivilStatus')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CivilStatus NVARCHAR(50) NULL;
    PRINT 'Added CivilStatus column';
END

-- Medication and Year Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DiabetesYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesYear NVARCHAR(10) NULL;
    PRINT 'Added DiabetesYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DiabetesMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DiabetesMedication NVARCHAR(100) NULL;
    PRINT 'Added DiabetesMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HypertensionYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionYear NVARCHAR(10) NULL;
    PRINT 'Added HypertensionYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HypertensionMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HypertensionMedication NVARCHAR(100) NULL;
    PRINT 'Added HypertensionMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'CancerYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerYear NVARCHAR(10) NULL;
    PRINT 'Added CancerYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'CancerMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD CancerMedication NVARCHAR(100) NULL;
    PRINT 'Added CancerMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'LungDiseaseYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseYear NVARCHAR(10) NULL;
    PRINT 'Added LungDiseaseYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'LungDiseaseMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD LungDiseaseMedication NVARCHAR(100) NULL;
    PRINT 'Added LungDiseaseMedication column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EyeDiseaseYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EyeDiseaseYear NVARCHAR(10) NULL;
    PRINT 'Added EyeDiseaseYear column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'EyeDiseaseMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD EyeDiseaseMedication NVARCHAR(100) NULL;
    PRINT 'Added EyeDiseaseMedication column';
END

-- Additional System Columns
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'HealthFacility')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD HealthFacility NVARCHAR(255) NULL;
    PRINT 'Added HealthFacility column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'DateOfAssessment')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD DateOfAssessment NVARCHAR(50) NULL;
    PRINT 'Added DateOfAssessment column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'IDNumber')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD IDNumber NVARCHAR(50) NULL;
    PRINT 'Added IDNumber column';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('NCDRiskAssessments') AND name = 'IDNo')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD IDNo NVARCHAR(50) NULL;
    PRINT 'Added IDNo column';
END

PRINT 'All missing columns have been added to NCDRiskAssessments table';
PRINT 'Please update your Entity Framework model to include these new columns';
