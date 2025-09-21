using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using Barangay.Models;
using System.Linq;

namespace Barangay.Data
{
    public static class MigrationManager
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Check if database exists and can connect
                    bool canConnect = await context.Database.CanConnectAsync();
                    if (!canConnect)
                    {
                        Console.WriteLine("Database doesn't exist, creating with migrations...");
                        await context.Database.MigrateAsync();
                    }
                    else
                    {
                        // Database exists, check if we need to run migrations safely
                        // If migrations history table doesn't exist but tables do, we're in a special case
                        // where the database was created outside of EF Core
                        bool migrationTableExists = false;
                        try
                        {
                            // Check if __EFMigrationsHistory table exists
                            migrationTableExists = await context.Database.ExecuteSqlRawAsync(
                                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory'") > 0;
                        }
                        catch
                        {
                            migrationTableExists = false;
                        }
                        
                        if (!migrationTableExists)
                        {
                            Console.WriteLine("Database exists but wasn't created with EF Core migrations.");
                            Console.WriteLine("Skipping automatic migrations and continuing with other initialization tasks.");
                        }
                        else
                        {
                            // Check if AspNetRoles table already exists (an indicator of Identity tables being present)
                            bool identityTablesExist = false;
                            try
                            {
                                identityTablesExist = await context.Database.ExecuteSqlRawAsync(
                                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetRoles'") > 0;
                            }
                            catch
                            {
                                identityTablesExist = false;
                            }
                            
                            if (identityTablesExist)
                            {
                                Console.WriteLine("Identity tables already exist. Skipping migrations that create these tables.");
                                // Only apply specific migrations that don't create Identity tables
                                // This is a workaround - in a real scenario, you might want to script migrations
                                // For now, just skip migrations as we're handling tables in other ways
                            }
                            else
                            {
                                // Apply migrations safely as normal
                                Console.WriteLine("Applying pending migrations...");
                                await context.Database.MigrateAsync();
                            }
                        }
                    }
                    
                    // Initialize roles
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    await SeedRolesAsync(roleManager);
                    
                    // Ensure the UserDocuments table structure is correct
                    await EnsureUserDocumentsTableAsync(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during migration: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                // Continue execution despite migration errors - table setup handled separately
            }
        }
        
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Doctor", "Patient" };
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Role '{role}' created.");
                }
            }
        }
        
        private static async Task EnsureUserDocumentsTableAsync(ApplicationDbContext context)
        {
            try
            {
                Console.WriteLine("Checking UserDocuments table structure...");
                
                // First, try to create the table if it doesn't exist
                await context.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
                BEGIN
                    CREATE TABLE [UserDocuments](
                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                        [UserId] NVARCHAR(450) NOT NULL,
                        [FileName] NVARCHAR(256) NOT NULL DEFAULT (''),
                        [FilePath] NVARCHAR(256) NOT NULL DEFAULT (''),
                        [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending'),
                        [ApprovedAt] DATETIME2 NULL,
                        [ApprovedBy] NVARCHAR(450) NULL,
                        [FileSize] BIGINT NOT NULL DEFAULT (0),
                        [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream'),
                        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_UserDocuments_AspNetUsers_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [AspNetUsers] ([Id])
                    );
                    
                    PRINT 'UserDocuments table created';
                END
                ELSE
                BEGIN
                    PRINT 'UserDocuments table already exists, checking columns...';
                    
                    -- Check if UploadDate column exists
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                                   WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'UploadDate')
                    BEGIN
                        ALTER TABLE [UserDocuments] ADD [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE();
                        PRINT 'UploadDate column added';
                    END
                    
                    -- Fix NULL values in all columns
                    UPDATE [UserDocuments] SET [FileName] = '' WHERE [FileName] IS NULL;
                    UPDATE [UserDocuments] SET [FilePath] = '' WHERE [FilePath] IS NULL;
                    UPDATE [UserDocuments] SET [Status] = 'Pending' WHERE [Status] IS NULL;
                    UPDATE [UserDocuments] SET [ContentType] = 'application/octet-stream' WHERE [ContentType] IS NULL;
                    UPDATE [UserDocuments] SET [FileSize] = 0 WHERE [FileSize] IS NULL;
                    PRINT 'NULL values fixed in UserDocuments table';
                END
                ");
                
                Console.WriteLine("UserDocuments table structure check completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring UserDocuments table: {ex.Message}");
                // Don't throw - allow application to continue
            }
        }
    }
} 