using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class AddNCDRiskAssessmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChestPainLocation",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "ChestPainWhenWalking",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DrinksAlcohol",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DrinksBeer",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DrinksHardLiquor",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "DrinksWine",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsFattyFood",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsFish",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsFruits",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsMeat",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsProcessedFood",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsSaltyFood",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsSweetFood",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "EatsVegetables",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HasChestPain",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HasHighSaltIntake",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HasNoRegularExercise",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HasNumbness",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HasRegularExercise",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HealthFacility",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "IsBingeDrinker",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "PainGoneIn10Min",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "PainMoreThan30Min",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "PainRelievedWithRest",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "StopsWhenPain",
                table: "NCDRiskAssessments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "NCDRiskAssessments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "NCDRiskAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telepono",
                table: "NCDRiskAssessments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "SmokingStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RiskStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Relihiyon",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Kasarian",
                table: "NCDRiskAssessments",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyOtherDiseaseDetails",
                table: "NCDRiskAssessments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FamilyNo",
                table: "NCDRiskAssessments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ExerciseDuration",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Edad",
                table: "NCDRiskAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancerType",
                table: "NCDRiskAssessments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "NCDRiskAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Barangay",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "NCDRiskAssessments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AlcoholFrequency",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AlcoholConsumption",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "NCDRiskAssessments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "AppointmentType",
                table: "NCDRiskAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NCDRiskAssessments_UserId",
                table: "NCDRiskAssessments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NCDRiskAssessments_AspNetUsers_UserId",
                table: "NCDRiskAssessments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_NCDRiskAssessments_AspNetUsers_UserId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropIndex(
                name: "IX_NCDRiskAssessments_UserId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "AppointmentType",
                table: "NCDRiskAssessments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "NCDRiskAssessments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Telepono",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SmokingStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "RiskStatus",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Relihiyon",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Kasarian",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FamilyOtherDiseaseDetails",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FamilyNo",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ExerciseDuration",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Edad",
                table: "NCDRiskAssessments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CancerType",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "NCDRiskAssessments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Barangay",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "NCDRiskAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AlcoholFrequency",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AlcoholConsumption",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<bool>(
                name: "ChestPainLocation",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ChestPainWhenWalking",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DrinksAlcohol",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DrinksBeer",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DrinksHardLiquor",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DrinksWine",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsFattyFood",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsFish",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsFruits",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsMeat",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsProcessedFood",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsSaltyFood",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsSweetFood",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EatsVegetables",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasChestPain",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasHighSaltIntake",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNoRegularExercise",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasNumbness",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRegularExercise",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HealthFacility",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsBingeDrinker",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PainGoneIn10Min",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PainMoreThan30Min",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PainRelievedWithRest",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StopsWhenPain",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
