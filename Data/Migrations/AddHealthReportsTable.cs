using Microsoft.EntityFrameworkCore.Migrations;

namespace Barangay.Data.Migrations
{
    public partial class AddHealthReportsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HealthReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CheckupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BloodPressure = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    HeartRate = table.Column<int>(type: "int", nullable: true),
                    BloodSugar = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(5,1)", nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(4,1)", nullable: true),
                    PhysicalActivity = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoctorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthReports_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HealthReports_AspNetUsers_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthReports_UserId",
                table: "HealthReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthReports_DoctorId",
                table: "HealthReports",
                column: "DoctorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthReports");
        }
    }
} 