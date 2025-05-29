using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class AddGuardianInformationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "GuardianInformation");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "GuardianInformation");

            migrationBuilder.DropColumn(
                name: "ResidencyProofPath",
                table: "GuardianInformation");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GuardianInformation",
                newName: "GuardianId");

            migrationBuilder.AddColumn<string>(
                name: "GuardianFirstName",
                table: "GuardianInformation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuardianLastName",
                table: "GuardianInformation",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "ResidencyProof",
                table: "GuardianInformation",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "GuardianFirstName",
                table: "GuardianInformation");

            migrationBuilder.DropColumn(
                name: "GuardianLastName",
                table: "GuardianInformation");

            migrationBuilder.DropColumn(
                name: "ResidencyProof",
                table: "GuardianInformation");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "GuardianId",
                table: "GuardianInformation",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "GuardianInformation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "GuardianInformation",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResidencyProofPath",
                table: "GuardianInformation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}
