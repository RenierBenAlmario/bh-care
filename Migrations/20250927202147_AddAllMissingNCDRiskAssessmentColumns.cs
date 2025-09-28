using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class AddAllMissingNCDRiskAssessmentColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add all missing columns to NCDRiskAssessments table
            migrationBuilder.AddColumn<string>(
                name: "AlcoholAmount1Bottle320ml",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholAmount2Bottle640ml",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholAmount3to4WineGlasses300ml",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholAmountLessThan3Shot45ml",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholAmountMoreThan4Shots75ml",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholFrequency1to3TimesPerWeek",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlcoholFrequencyMoreThan4TimesPerWeek",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssessmentDate",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BMI",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BMIStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BPStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BaselineBP",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodSugarStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BreastCancerScreened",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancerScreeningStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CervicalCancerScreened",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChestPainSpreadsToArm",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CholesterolResult",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CholesterolStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CombinationExercise",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfAssessment",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "NCDRiskAssessments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorName",
                table: "NCDRiskAssessments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinksAlcohol",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinksBeer",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinksWhiskyGinBrandy",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DrinksWine",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsFattyFoodMoreThan2TimesPerWeek",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsFishDaily",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsFruitsDaily",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsMeatDaily",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsOilyFoodMoreThan2TimesPerWeek",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsSweetFoodMoreThan2TimesPerWeek",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EatsVegetablesDaily",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FastingBloodSugar",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormerSmoker",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasChestPain",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasHighSaltIntake",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasHistoryOfSmoking",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasPolydipsia",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasPolyphagia",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasPolyuria",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasStress",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasUnhealthyDiet",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasUrineKetones",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasUrineProtein",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HasWeightLoss",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Height",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hip",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IDNumber",
                table: "NCDRiskAssessments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsufficientPhysicalActivity",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InterviewedBy",
                table: "NCDRiskAssessments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IsBingeDrinker",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LeftArmMeanBP",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LossOfConsciousnessLessThan10Min",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModerateIntensityExercise",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NeverSmokedButExposedToSmoke",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumbnessWhenWalkingFast",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PainLastsMoreThan30Min",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PainRelievedWithRest",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PatientSignature",
                table: "NCDRiskAssessments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RandomBloodSugar",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RightArmMeanBP",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskPercentage",
                table: "NCDRiskAssessments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SeeDoctorIfYes",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrineKetones",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrineProtein",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VigorousIntensityExercise",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WHRatio",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WHStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Waist",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Weight",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove all the added columns
            migrationBuilder.DropColumn(name: "AlcoholAmount1Bottle320ml", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholAmount2Bottle640ml", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholAmount3to4WineGlasses300ml", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholAmountLessThan3Shot45ml", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholAmountMoreThan4Shots75ml", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholFrequency1to3TimesPerWeek", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AlcoholFrequencyMoreThan4TimesPerWeek", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "AssessmentDate", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BMI", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BMIStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BPStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BaselineBP", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BloodSugarStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "BreastCancerScreened", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "CancerScreeningStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "CervicalCancerScreened", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "ChestPainSpreadsToArm", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "CholesterolResult", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "CholesterolStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "CombinationExercise", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DateOfAssessment", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "Designation", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DoctorName", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DrinksAlcohol", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DrinksBeer", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DrinksWhiskyGinBrandy", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "DrinksWine", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsFattyFoodMoreThan2TimesPerWeek", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsFishDaily", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsFruitsDaily", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsMeatDaily", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsOilyFoodMoreThan2TimesPerWeek", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsSweetFoodMoreThan2TimesPerWeek", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "EatsVegetablesDaily", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "FastingBloodSugar", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "FormerSmoker", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasChestPain", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasHighSaltIntake", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasHistoryOfSmoking", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasPolydipsia", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasPolyphagia", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasPolyuria", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasStress", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasUnhealthyDiet", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasUrineKetones", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasUrineProtein", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "HasWeightLoss", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "Height", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "Hip", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "IDNumber", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "InsufficientPhysicalActivity", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "InterviewedBy", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "IsBingeDrinker", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "LeftArmMeanBP", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "LossOfConsciousnessLessThan10Min", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "ModerateIntensityExercise", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "NeverSmokedButExposedToSmoke", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "NumbnessWhenWalkingFast", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "PainLastsMoreThan30Min", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "PainRelievedWithRest", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "PatientSignature", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "RandomBloodSugar", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "RightArmMeanBP", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "RiskPercentage", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "SeeDoctorIfYes", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "UrineKetones", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "UrineProtein", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "VigorousIntensityExercise", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "WHRatio", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "WHStatus", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "Waist", table: "NCDRiskAssessments");
            migrationBuilder.DropColumn(name: "Weight", table: "NCDRiskAssessments");
        }
    }
}





