using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Barangay.Migrations
{
    public partial class AddEmailVerificationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if table already exists before creating it
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EmailVerifications')
                BEGIN
                    CREATE TABLE [EmailVerifications] (
                        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Email] NVARCHAR(255) NOT NULL,
                        [VerificationCode] NVARCHAR(10) NOT NULL,
                        [ExpiryTime] DATETIME2 NOT NULL,
                        [IsVerified] BIT NOT NULL DEFAULT(0),
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
                        [VerifiedAt] DATETIME2 NULL
                    );
                END
            ");

            // Create index if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmailVerifications_Email')
                BEGIN
                    CREATE UNIQUE INDEX [IX_EmailVerifications_Email] ON [EmailVerifications]([Email]);
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerifications");
        }
    }
} 