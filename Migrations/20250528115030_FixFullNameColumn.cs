using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class FixFullNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AssessmentNotes",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AttendanceIssues",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CareerPlans",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CopingMechanisms",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DatingRelationships",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DietDescription",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EatingDisorderSymptoms",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExperiencedBullying",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FamilyRelationship",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "FeelsSafeAtHome",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FeelsSafeAtSchool",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpPlan",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Hobbies",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "MoodChanges",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PersonalStrengths",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhysicalActivity",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedActions",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SchoolPerformance",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ScreenTime",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SelfHarmBehavior",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SexualActivity",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SexualOrientation",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubstanceType",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "SubstanceUse",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SuicidalThoughts",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SupportSystems",
                table: "HEEADSSSAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "WeightConcerns",
                table: "HEEADSSSAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "AssessmentNotes",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "AttendanceIssues",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "CareerPlans",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "CopingMechanisms",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "DatingRelationships",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "DietDescription",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "EatingDisorderSymptoms",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "ExperiencedBullying",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "FamilyRelationship",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "FeelsSafeAtHome",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "FeelsSafeAtSchool",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "FollowUpPlan",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "Hobbies",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "MoodChanges",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "PersonalStrengths",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "PhysicalActivity",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "RecommendedActions",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SchoolPerformance",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "ScreenTime",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SelfHarmBehavior",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SexualActivity",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SexualOrientation",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SubstanceType",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SubstanceUse",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SuicidalThoughts",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "SupportSystems",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "WeightConcerns",
                table: "HEEADSSSAssessments");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");
        }
    }
}
