using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDocumentsWithNoAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_Status",
                table: "UserDocuments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserDocuments_UploadDate",
                table: "UserDocuments",
                column: "UploadDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserDocuments_Status",
                table: "UserDocuments");

            migrationBuilder.DropIndex(
                name: "IX_UserDocuments_UploadDate",
                table: "UserDocuments");
        }
    }
}
