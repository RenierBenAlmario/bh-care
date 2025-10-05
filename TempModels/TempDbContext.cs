using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Barangay.TempModels;

public partial class TempDbContext : DbContext
{
    public TempDbContext(DbContextOptions<TempDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<NcdriskAssessment> NcdriskAssessments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NcdriskAssessment>(entity =>
        {
            entity.ToTable("NCDRiskAssessments");

            entity.HasIndex(e => e.AppointmentId, "IX_NCDRiskAssessments_AppointmentId");

            entity.HasIndex(e => e.UserId, "IX_NCDRiskAssessments_UserId");

            entity.Property(e => e.Address).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeBeer).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeWhisky).HasMaxLength(4000);
            entity.Property(e => e.AlchoholTypeWine).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount1Bottle320ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount2Bottle640ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmount3to4WineGlasses300ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmountLessThan3Shot45ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholAmountMoreThan4Shots75ml).HasMaxLength(4000);
            entity.Property(e => e.AlcoholConsumption).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequency).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequency1to3TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.AlcoholFrequencyMoreThan4TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.AlcoholInom).HasMaxLength(4000);
            entity.Property(e => e.AlcoholOkasyon).HasMaxLength(4000);
            entity.Property(e => e.AlcoholPerOccasion).HasMaxLength(4000);
            entity.Property(e => e.AlcoholStoppedDuration).HasMaxLength(4000);
            entity.Property(e => e.AppointmentType).HasMaxLength(4000);
            entity.Property(e => e.AssessmentDate).HasMaxLength(4000);
            entity.Property(e => e.Barangay).HasMaxLength(4000);
            entity.Property(e => e.BaselineBp)
                .HasMaxLength(4000)
                .HasColumnName("BaselineBP");
            entity.Property(e => e.BeerConsumption1).HasMaxLength(4000);
            entity.Property(e => e.BeerConsumption2).HasMaxLength(4000);
            entity.Property(e => e.BeerConsumption3).HasMaxLength(4000);
            entity.Property(e => e.Birthday).HasMaxLength(4000);
            entity.Property(e => e.BloodSugarStatus).HasMaxLength(4000);
            entity.Property(e => e.Bmi)
                .HasMaxLength(4000)
                .HasColumnName("BMI");
            entity.Property(e => e.Bmistatus)
                .HasMaxLength(4000)
                .HasColumnName("BMIStatus");
            entity.Property(e => e.Bpstatus)
                .HasMaxLength(4000)
                .HasColumnName("BPStatus");
            entity.Property(e => e.BreastCancerScreened).HasMaxLength(4000);
            entity.Property(e => e.CancerMedication).HasMaxLength(4000);
            entity.Property(e => e.CancerScreeningStatus).HasMaxLength(4000);
            entity.Property(e => e.CancerSite).HasMaxLength(200);
            entity.Property(e => e.CancerType).HasMaxLength(4000);
            entity.Property(e => e.CancerYear).HasMaxLength(4000);
            entity.Property(e => e.CervicalCancerScreened).HasMaxLength(4000);
            entity.Property(e => e.ChestPain).HasMaxLength(4000);
            entity.Property(e => e.ChestPainLocation).HasMaxLength(4000);
            entity.Property(e => e.ChestPainSpreadsToArm).HasMaxLength(4000);
            entity.Property(e => e.ChestPainValue).HasMaxLength(4000);
            entity.Property(e => e.CholesterolResult).HasMaxLength(4000);
            entity.Property(e => e.CholesterolStatus).HasMaxLength(4000);
            entity.Property(e => e.CivilStatus).HasMaxLength(4000);
            entity.Property(e => e.CombinationExercise).HasMaxLength(4000);
            entity.Property(e => e.CreatedAt)
                .HasMaxLength(4000)
                .HasDefaultValue("");
            entity.Property(e => e.DateAssessment).HasMaxLength(4000);
            entity.Property(e => e.DateOfAssessment).HasMaxLength(4000);
            entity.Property(e => e.Designation).HasMaxLength(4000);
            entity.Property(e => e.DiabetesMedication).HasMaxLength(4000);
            entity.Property(e => e.DiabetesYear).HasMaxLength(4000);
            entity.Property(e => e.DoctorName).HasMaxLength(4000);
            entity.Property(e => e.DrinksAlcohol).HasMaxLength(4000);
            entity.Property(e => e.DrinksBeer).HasMaxLength(4000);
            entity.Property(e => e.DrinksWhiskyGinBrandy).HasMaxLength(4000);
            entity.Property(e => e.DrinksWine).HasMaxLength(4000);
            entity.Property(e => e.EatsFattyFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsFishDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsFruitsDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsMeatDaily).HasMaxLength(4000);
            entity.Property(e => e.EatsOilyFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsSweetFoodMoreThan2TimesPerWeek).HasMaxLength(4000);
            entity.Property(e => e.EatsVegetablesDaily).HasMaxLength(4000);
            entity.Property(e => e.Edad).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoDuration).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoRegular).HasMaxLength(4000);
            entity.Property(e => e.EhersisyoType).HasMaxLength(4000);
            entity.Property(e => e.ExerciseDuration).HasMaxLength(4000);
            entity.Property(e => e.EyeDiseaseMedication).HasMaxLength(4000);
            entity.Property(e => e.EyeDiseaseYear).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasCancer).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasDiabetes).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasHeartDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasHypertension).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasKidneyDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasOtherDisease).HasMaxLength(4000);
            entity.Property(e => e.FamilyHasStroke).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryCancerSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryDiabetesSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryEyeDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryHeartDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryKidneyDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryLungDiseaseSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOther).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryOtherSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeFather).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeMother).HasMaxLength(4000);
            entity.Property(e => e.FamilyHistoryStrokeSibling).HasMaxLength(4000);
            entity.Property(e => e.FamilyNo).HasMaxLength(4000);
            entity.Property(e => e.FamilyOtherDiseaseDetails).HasMaxLength(4000);
            entity.Property(e => e.FastingBloodSugar).HasMaxLength(4000);
            entity.Property(e => e.FirstName).HasMaxLength(4000);
            entity.Property(e => e.FormerSmoker).HasMaxLength(4000);
            entity.Property(e => e.HasAsthma).HasMaxLength(4000);
            entity.Property(e => e.HasCancer).HasMaxLength(4000);
            entity.Property(e => e.HasChestPain).HasMaxLength(4000);
            entity.Property(e => e.HasCopd)
                .HasMaxLength(4000)
                .HasColumnName("HasCOPD");
            entity.Property(e => e.HasDiabetes).HasMaxLength(4000);
            entity.Property(e => e.HasDifficultyBreathing).HasMaxLength(4000);
            entity.Property(e => e.HasEnoughExercise).HasMaxLength(4000);
            entity.Property(e => e.HasEyeDisease).HasMaxLength(4000);
            entity.Property(e => e.HasEyeDiseaseCondition).HasMaxLength(4000);
            entity.Property(e => e.HasHighSaltIntake).HasMaxLength(4000);
            entity.Property(e => e.HasHistoryOfSmoking).HasMaxLength(4000);
            entity.Property(e => e.HasHypertension).HasMaxLength(4000);
            entity.Property(e => e.HasLungDisease).HasMaxLength(4000);
            entity.Property(e => e.HasLungDiseaseNonInfectious).HasMaxLength(4000);
            entity.Property(e => e.HasNoRegularExercise).HasMaxLength(4000);
            entity.Property(e => e.HasPolydipsia).HasMaxLength(4000);
            entity.Property(e => e.HasPolyphagia).HasMaxLength(4000);
            entity.Property(e => e.HasPolyuria).HasMaxLength(4000);
            entity.Property(e => e.HasStress).HasMaxLength(4000);
            entity.Property(e => e.HasStrokeSymptoms).HasMaxLength(4000);
            entity.Property(e => e.HasUnhealthyDiet).HasMaxLength(4000);
            entity.Property(e => e.HasUrineKetones).HasMaxLength(4000);
            entity.Property(e => e.HasUrineProtein).HasMaxLength(4000);
            entity.Property(e => e.HasWeightLoss).HasMaxLength(4000);
            entity.Property(e => e.HealthFacility).HasMaxLength(4000);
            entity.Property(e => e.HealthFacilityName).HasMaxLength(4000);
            entity.Property(e => e.Height).HasMaxLength(4000);
            entity.Property(e => e.HighSaltIntake).HasMaxLength(4000);
            entity.Property(e => e.Hip).HasMaxLength(4000);
            entity.Property(e => e.HypertensionMedication).HasMaxLength(4000);
            entity.Property(e => e.HypertensionYear).HasMaxLength(4000);
            entity.Property(e => e.Idno)
                .HasMaxLength(4000)
                .HasColumnName("IDNo");
            entity.Property(e => e.Idnumber)
                .HasMaxLength(4000)
                .HasColumnName("IDNumber");
            entity.Property(e => e.InsufficientPhysicalActivity).HasMaxLength(4000);
            entity.Property(e => e.InterviewedBy).HasMaxLength(4000);
            entity.Property(e => e.IsBingeDrinker).HasMaxLength(4000);
            entity.Property(e => e.Kasarian).HasMaxLength(4000);
            entity.Property(e => e.LastName).HasMaxLength(4000);
            entity.Property(e => e.LeftArmMeanBp)
                .HasMaxLength(4000)
                .HasColumnName("LeftArmMeanBP");
            entity.Property(e => e.LossOfConsciousnessLessThan10Min).HasMaxLength(4000);
            entity.Property(e => e.LungDiseaseMedication).HasMaxLength(4000);
            entity.Property(e => e.LungDiseaseYear).HasMaxLength(4000);
            entity.Property(e => e.MiddleName).HasMaxLength(4000);
            entity.Property(e => e.ModerateIntensityExercise).HasMaxLength(4000);
            entity.Property(e => e.NeverSmokedButExposedToSmoke).HasMaxLength(4000);
            entity.Property(e => e.NumbnessWhenWalkingFast).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonKumakainMamantika).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonKumakainMatatamis).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasGulay).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasIsda).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasKarne).HasMaxLength(4000);
            entity.Property(e => e.NutrisyonMadalasPratas).HasMaxLength(4000);
            entity.Property(e => e.Occupation).HasMaxLength(4000);
            entity.Property(e => e.PainLastsMoreThan30Min).HasMaxLength(4000);
            entity.Property(e => e.PainRelievedWithRest).HasMaxLength(4000);
            entity.Property(e => e.Pananakit21).HasMaxLength(4000);
            entity.Property(e => e.Pananakit22).HasMaxLength(4000);
            entity.Property(e => e.Pananakit23).HasMaxLength(4000);
            entity.Property(e => e.Pananakit24).HasMaxLength(4000);
            entity.Property(e => e.Pananakit25).HasMaxLength(4000);
            entity.Property(e => e.Pananakit26).HasMaxLength(4000);
            entity.Property(e => e.Pananakit27).HasMaxLength(4000);
            entity.Property(e => e.Pananakit28).HasMaxLength(4000);
            entity.Property(e => e.PatientSignature).HasMaxLength(4000);
            entity.Property(e => e.RandomBloodSugar).HasMaxLength(4000);
            entity.Property(e => e.Relihiyon).HasMaxLength(4000);
            entity.Property(e => e.RightArmMeanBp)
                .HasMaxLength(4000)
                .HasColumnName("RightArmMeanBP");
            entity.Property(e => e.RiskPercentage).HasMaxLength(4000);
            entity.Property(e => e.RiskStatus).HasMaxLength(4000);
            entity.Property(e => e.SeeDoctorIfYes).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoKadami).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoSticks).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoTumigil).HasMaxLength(4000);
            entity.Property(e => e.SigarilyoUsok).HasMaxLength(4000);
            entity.Property(e => e.Smoked100Sticks).HasMaxLength(4000);
            entity.Property(e => e.SmokingQuitDuration).HasMaxLength(4000);
            entity.Property(e => e.SmokingStatus).HasMaxLength(4000);
            entity.Property(e => e.StressEpekto).HasMaxLength(4000);
            entity.Property(e => e.StressMadalas).HasMaxLength(4000);
            entity.Property(e => e.StressSino).HasMaxLength(4000);
            entity.Property(e => e.Telepono).HasMaxLength(4000);
            entity.Property(e => e.UpdatedAt)
                .HasMaxLength(4000)
                .HasDefaultValue("");
            entity.Property(e => e.UrineKetones).HasMaxLength(4000);
            entity.Property(e => e.UrineProtein).HasMaxLength(4000);
            entity.Property(e => e.VigorousIntensityExercise).HasMaxLength(4000);
            entity.Property(e => e.Waist).HasMaxLength(4000);
            entity.Property(e => e.Weight).HasMaxLength(4000);
            entity.Property(e => e.WhiskyConsumption1).HasMaxLength(4000);
            entity.Property(e => e.WhiskyConsumption2).HasMaxLength(4000);
            entity.Property(e => e.Whratio)
                .HasMaxLength(4000)
                .HasColumnName("WHRatio");
            entity.Property(e => e.Whstatus)
                .HasMaxLength(4000)
                .HasColumnName("WHStatus");
            entity.Property(e => e.WineConsumption1).HasMaxLength(4000);
            entity.Property(e => e.WineConsumption2).HasMaxLength(4000);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
