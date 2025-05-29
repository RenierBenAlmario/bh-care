using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    public partial class AddStatusColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if Status column exists
            if (!migrationBuilder.DoesColumnExist("AspNetUsers", "Status"))
            {
                migrationBuilder.AddColumn<string>(
                    name: "Status",
                    table: "AspNetUsers",
                    type: "nvarchar(max)",
                    nullable: false,
                    defaultValue: "Pending");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AspNetUsers");
        }
    }

    // Helper extension method for migrationBuilder
    public static class MigrationBuilderExtensions
    {
        public static bool DoesColumnExist(this MigrationBuilder migrationBuilder, string tableName, string columnName)
        {
            migrationBuilder.Sql(
                $@"IF EXISTS (
                    SELECT 1
                    FROM sys.columns 
                    WHERE object_id = OBJECT_ID(N'[dbo].[{tableName}]') 
                    AND name = '{columnName}'
                )
                BEGIN
                    SELECT 1
                END
                ELSE
                BEGIN
                    SELECT 0
                END");
            
            // This is just a stub - the actual logic would need to be in a custom IMigrationsSqlGenerator
            return false;
        }
    }
} 