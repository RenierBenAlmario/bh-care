using System;
using System.Threading.Tasks;
using System.Linq;
using Barangay.Data;
using Barangay.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Barangay
{
    public class TestRegistration
    {
        public static async Task TestDatabaseAccess(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Testing UserDocuments table existence...");
            
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                try
                {
                    // Check if UserDocuments table exists
                    bool tableExists = false;
                    try
                    {
                        // Try to get the first document just to test if the table exists
                        var doc = await dbContext.UserDocuments.FirstOrDefaultAsync();
                        tableExists = true;
                        Console.WriteLine("UserDocuments table exists");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error accessing UserDocuments table: {ex.Message}");
                        tableExists = false;
                    }
                    
                    if (!tableExists)
                    {
                        Console.WriteLine("Creating UserDocuments table...");
                        try 
                        {
                            // You might need to run this manually if migrations aren't working
                            await dbContext.Database.ExecuteSqlRawAsync(@"
                                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
                                BEGIN
                                    CREATE TABLE [UserDocuments](
                                        [Id] INT IDENTITY(1,1) PRIMARY KEY,
                                        [UserId] NVARCHAR(450) NOT NULL,
                                        [FilePath] NVARCHAR(256) NOT NULL DEFAULT (''),
                                        [FileName] NVARCHAR(255) NOT NULL DEFAULT (''),
                                        [FileSize] BIGINT NOT NULL DEFAULT (0),
                                        [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream'),
                                        [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending'),
                                        [ApprovedAt] DATETIME2 NULL,
                                        [ApprovedBy] NVARCHAR(256) NULL DEFAULT (''),
                                        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
                                        CONSTRAINT [FK_UserDocuments_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
                                    );
                                END
                            ");
                            Console.WriteLine("UserDocuments table created successfully");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error creating UserDocuments table: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Test failed: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }
    }
} 