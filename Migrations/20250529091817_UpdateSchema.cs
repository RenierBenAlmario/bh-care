using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "AppointmentType",
                table: "NCDRiskAssessments");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserPermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserPermissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<decimal>(
                name: "Dosage",
                table: "PrescriptionMedicines",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

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
                name: "Barangay",
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

            migrationBuilder.AddColumn<string>(
                name: "HealthFacility",
                table: "NCDRiskAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "HighSaltIntake",
                table: "NCDRiskAssessments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Birthday",
                table: "HEEADSSSAssessments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "IntegratedAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HealthFacility = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Barangay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Telepono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Kasarian = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Relihiyon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HasDiabetes = table.Column<bool>(type: "bit", nullable: false),
                    HasHypertension = table.Column<bool>(type: "bit", nullable: false),
                    HasCancer = table.Column<bool>(type: "bit", nullable: false),
                    HasCOPD = table.Column<bool>(type: "bit", nullable: false),
                    HasLungDisease = table.Column<bool>(type: "bit", nullable: false),
                    HasEyeDisease = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntegratedAssessments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropTable(
                name: "IntegratedAssessments");

            migrationBuilder.DropIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "HealthFacility",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "HighSaltIntake",
                table: "NCDRiskAssessments");

            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "HEEADSSSAssessments");

            migrationBuilder.AlterColumn<decimal>(
                name: "Dosage",
                table: "PrescriptionMedicines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

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
                name: "Barangay",
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

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_NCDRiskAssessments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_DoctorId",
                table: "Appointments",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NCDRiskAssessments_Appointments_AppointmentId",
                table: "NCDRiskAssessments",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
