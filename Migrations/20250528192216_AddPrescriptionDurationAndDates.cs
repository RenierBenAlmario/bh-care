using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class AddPrescriptionDurationAndDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionMedications_Prescriptions_PrescriptionId1",
                table: "PrescriptionMedications");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_AspNetUsers_DoctorId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionMedications_PrescriptionId1",
                table: "PrescriptionMedications");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "PrescriptionId1",
                table: "PrescriptionMedications");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Prescriptions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Prescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PrescriptionMedicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dosage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionMedicines_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMedicines_PrescriptionId",
                table: "PrescriptionMedicines",
                column: "PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_AspNetUsers_DoctorId",
                table: "Prescriptions",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_AspNetUsers_DoctorId",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions");

            migrationBuilder.DropTable(
                name: "PrescriptionMedicines");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "CancelledAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Prescriptions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "PrescriptionId1",
                table: "PrescriptionMedications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMedications_PrescriptionId1",
                table: "PrescriptionMedications",
                column: "PrescriptionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_UserId",
                table: "Doctors",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_UserId",
                table: "Doctors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionMedications_Prescriptions_PrescriptionId1",
                table: "PrescriptionMedications",
                column: "PrescriptionId1",
                principalTable: "Prescriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_AspNetUsers_DoctorId",
                table: "Prescriptions",
                column: "DoctorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Patients_PatientId",
                table: "Prescriptions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "UserId");
        }
    }
}
