using System;
using System.Collections.Generic;

namespace Barangay.TempModels;

public partial class NcdriskAssessment
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public int? AppointmentId { get; set; }

    public string? HealthFacility { get; set; }

    public string? FamilyNo { get; set; }

    public string? Address { get; set; }

    public string? Barangay { get; set; }

    public string? Birthday { get; set; }

    public string? Telepono { get; set; }

    public string? Edad { get; set; }

    public string? Kasarian { get; set; }

    public string? Relihiyon { get; set; }

    public string? HasDiabetes { get; set; }

    public string? HasHypertension { get; set; }

    public string? HasCancer { get; set; }

    public string? HasCopd { get; set; }

    public string? HasLungDisease { get; set; }

    public string? HasEyeDisease { get; set; }

    public string? CancerType { get; set; }

    public string? FamilyHasHypertension { get; set; }

    public string? FamilyHasHeartDisease { get; set; }

    public string? FamilyHasStroke { get; set; }

    public string? FamilyHasDiabetes { get; set; }

    public string? FamilyHasCancer { get; set; }

    public string? FamilyHasKidneyDisease { get; set; }

    public string? FamilyHasOtherDisease { get; set; }

    public string? FamilyOtherDiseaseDetails { get; set; }

    public string? SmokingStatus { get; set; }

    public string? HighSaltIntake { get; set; }

    public string? AlcoholFrequency { get; set; }

    public string? AlcoholConsumption { get; set; }

    public string? ExerciseDuration { get; set; }

    public string? RiskStatus { get; set; }

    public string? ChestPain { get; set; }

    public string? ChestPainLocation { get; set; }

    public string? ChestPainValue { get; set; }

    public string? HasDifficultyBreathing { get; set; }

    public string? HasAsthma { get; set; }

    public string? HasNoRegularExercise { get; set; }

    public string CreatedAt { get; set; } = null!;

    public string UpdatedAt { get; set; } = null!;

    public string? AppointmentType { get; set; }

    public string? CancerMedication { get; set; }

    public string? CancerYear { get; set; }

    public string? CivilStatus { get; set; }

    public string? DiabetesMedication { get; set; }

    public string? DiabetesYear { get; set; }

    public string? FamilyHistoryCancerFather { get; set; }

    public string? FamilyHistoryCancerMother { get; set; }

    public string? FamilyHistoryCancerSibling { get; set; }

    public string? FamilyHistoryDiabetesFather { get; set; }

    public string? FamilyHistoryDiabetesMother { get; set; }

    public string? FamilyHistoryDiabetesSibling { get; set; }

    public string? FamilyHistoryHeartDiseaseFather { get; set; }

    public string? FamilyHistoryHeartDiseaseMother { get; set; }

    public string? FamilyHistoryHeartDiseaseSibling { get; set; }

    public string? FamilyHistoryLungDiseaseFather { get; set; }

    public string? FamilyHistoryLungDiseaseMother { get; set; }

    public string? FamilyHistoryLungDiseaseSibling { get; set; }

    public string? FamilyHistoryOther { get; set; }

    public string? FamilyHistoryOtherFather { get; set; }

    public string? FamilyHistoryOtherMother { get; set; }

    public string? FamilyHistoryOtherSibling { get; set; }

    public string? FamilyHistoryStrokeFather { get; set; }

    public string? FamilyHistoryStrokeMother { get; set; }

    public string? FamilyHistoryStrokeSibling { get; set; }

    public string? FirstName { get; set; }

    public string? HypertensionMedication { get; set; }

    public string? HypertensionYear { get; set; }

    public string? LastName { get; set; }

    public string? LungDiseaseMedication { get; set; }

    public string? LungDiseaseYear { get; set; }

    public string? MiddleName { get; set; }

    public string? Occupation { get; set; }

    public string? AlcoholAmount1Bottle320ml { get; set; }

    public string? AlcoholAmount2Bottle640ml { get; set; }

    public string? AlcoholAmount3to4WineGlasses300ml { get; set; }

    public string? AlcoholAmountLessThan3Shot45ml { get; set; }

    public string? AlcoholAmountMoreThan4Shots75ml { get; set; }

    public string? AlcoholFrequency1to3TimesPerWeek { get; set; }

    public string? AlcoholFrequencyMoreThan4TimesPerWeek { get; set; }

    public string? AssessmentDate { get; set; }

    public string? Bmi { get; set; }

    public string? Bmistatus { get; set; }

    public string? Bpstatus { get; set; }

    public string? BaselineBp { get; set; }

    public string? BloodSugarStatus { get; set; }

    public string? BreastCancerScreened { get; set; }

    public string? CancerScreeningStatus { get; set; }

    public string? CervicalCancerScreened { get; set; }

    public string? ChestPainSpreadsToArm { get; set; }

    public string? CholesterolResult { get; set; }

    public string? CholesterolStatus { get; set; }

    public string? CombinationExercise { get; set; }

    public string? DateOfAssessment { get; set; }

    public string? Designation { get; set; }

    public string? DoctorName { get; set; }

    public string? DrinksAlcohol { get; set; }

    public string? DrinksBeer { get; set; }

    public string? DrinksWhiskyGinBrandy { get; set; }

    public string? DrinksWine { get; set; }

    public string? EatsFattyFoodMoreThan2TimesPerWeek { get; set; }

    public string? EatsFishDaily { get; set; }

    public string? EatsFruitsDaily { get; set; }

    public string? EatsMeatDaily { get; set; }

    public string? EatsOilyFoodMoreThan2TimesPerWeek { get; set; }

    public string? EatsSweetFoodMoreThan2TimesPerWeek { get; set; }

    public string? EatsVegetablesDaily { get; set; }

    public string? FastingBloodSugar { get; set; }

    public string? FormerSmoker { get; set; }

    public string? HasChestPain { get; set; }

    public string? HasHighSaltIntake { get; set; }

    public string? HasHistoryOfSmoking { get; set; }

    public string? HasPolydipsia { get; set; }

    public string? HasPolyphagia { get; set; }

    public string? HasPolyuria { get; set; }

    public string? HasStress { get; set; }

    public string? HasUnhealthyDiet { get; set; }

    public string? HasUrineKetones { get; set; }

    public string? HasUrineProtein { get; set; }

    public string? HasWeightLoss { get; set; }

    public string? Height { get; set; }

    public string? Hip { get; set; }

    public string? Idnumber { get; set; }

    public string? InsufficientPhysicalActivity { get; set; }

    public string? InterviewedBy { get; set; }

    public string? IsBingeDrinker { get; set; }

    public string? LeftArmMeanBp { get; set; }

    public string? LossOfConsciousnessLessThan10Min { get; set; }

    public string? ModerateIntensityExercise { get; set; }

    public string? NeverSmokedButExposedToSmoke { get; set; }

    public string? NumbnessWhenWalkingFast { get; set; }

    public string? PainLastsMoreThan30Min { get; set; }

    public string? PainRelievedWithRest { get; set; }

    public string? PatientSignature { get; set; }

    public string? RandomBloodSugar { get; set; }

    public string? RightArmMeanBp { get; set; }

    public string? RiskPercentage { get; set; }

    public string? SeeDoctorIfYes { get; set; }

    public string? UrineKetones { get; set; }

    public string? UrineProtein { get; set; }

    public string? VigorousIntensityExercise { get; set; }

    public string? Whratio { get; set; }

    public string? Whstatus { get; set; }

    public string? Waist { get; set; }

    public string? Weight { get; set; }

    public string? AlcoholStoppedDuration { get; set; }

    public string? EyeDiseaseMedication { get; set; }

    public string? EyeDiseaseYear { get; set; }

    public string? FamilyHistoryEyeDiseaseFather { get; set; }

    public string? FamilyHistoryEyeDiseaseMother { get; set; }

    public string? FamilyHistoryEyeDiseaseSibling { get; set; }

    public string? FamilyHistoryKidneyDiseaseFather { get; set; }

    public string? FamilyHistoryKidneyDiseaseMother { get; set; }

    public string? FamilyHistoryKidneyDiseaseSibling { get; set; }

    public string? HasEnoughExercise { get; set; }

    public string? Idno { get; set; }

    public string? Smoked100Sticks { get; set; }

    public string? AlcoholPerOccasion { get; set; }

    public string? AlchoholTypeBeer { get; set; }

    public string? AlchoholTypeWhisky { get; set; }

    public string? AlchoholTypeWine { get; set; }

    public string? AlcoholInom { get; set; }

    public string? AlcoholOkasyon { get; set; }

    public string? BeerConsumption1 { get; set; }

    public string? BeerConsumption2 { get; set; }

    public string? BeerConsumption3 { get; set; }

    public string? DateAssessment { get; set; }

    public string? EhersisyoDuration { get; set; }

    public string? EhersisyoRegular { get; set; }

    public string? EhersisyoType { get; set; }

    public string? HasEyeDiseaseCondition { get; set; }

    public string? HasLungDiseaseNonInfectious { get; set; }

    public string? HealthFacilityName { get; set; }

    public string? NutrisyonKumakainMamantika { get; set; }

    public string? NutrisyonKumakainMatatamis { get; set; }

    public string? NutrisyonMadalasGulay { get; set; }

    public string? NutrisyonMadalasIsda { get; set; }

    public string? NutrisyonMadalasKarne { get; set; }

    public string? NutrisyonMadalasPratas { get; set; }

    public string? Pananakit21 { get; set; }

    public string? Pananakit22 { get; set; }

    public string? Pananakit23 { get; set; }

    public string? Pananakit24 { get; set; }

    public string? Pananakit25 { get; set; }

    public string? Pananakit26 { get; set; }

    public string? Pananakit27 { get; set; }

    public string? Pananakit28 { get; set; }

    public string? SigarilyoKadami { get; set; }

    public string? SigarilyoSticks { get; set; }

    public string? SigarilyoTumigil { get; set; }

    public string? SigarilyoUsok { get; set; }

    public string? StressEpekto { get; set; }

    public string? StressMadalas { get; set; }

    public string? StressSino { get; set; }

    public string? WhiskyConsumption1 { get; set; }

    public string? WhiskyConsumption2 { get; set; }

    public string? WineConsumption1 { get; set; }

    public string? WineConsumption2 { get; set; }

    public string? HasStrokeSymptoms { get; set; }

    public string? CancerSite { get; set; }

    public string? SmokingQuitDuration { get; set; }
}
