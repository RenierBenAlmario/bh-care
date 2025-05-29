using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Barangay.Models;
using Barangay.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Barangay.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IEncryptionService encryptionService)
        {
            // Create roles if they don't exist
            string[] roles = new string[] { "Admin", "Doctor", "Patient", "Staff" };
            
            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Continue with the rest of your initialization
            if (!context.StaffMembers.Any(s => s.Role == "Doctor"))
            {
                // Create a doctor user
                var doctorUser = new ApplicationUser
                {
                    UserName = "john.smith@example.com",
                    Email = "john.smith@example.com",
                    FullName = "Dr. John Smith",
                    Specialization = "General Practice",
                    IsActive = true,
                    WorkingDays = "Monday,Tuesday,Wednesday,Thursday,Friday",
                    WorkingHours = "9:00 AM - 5:00 PM",
                    MaxDailyPatients = 20,
                    EmailConfirmed = true
                };

                // Now this should work because the role exists
                await userManager.CreateAsync(doctorUser, "Password123!");
                await userManager.AddToRoleAsync(doctorUser, "Doctor");
                
                // Create staff member
                var doctor = new StaffMember
                {
                    UserId = doctorUser.Id,
                    Name = "Dr. John Smith",
                    Email = "john.smith@example.com",
                    Role = "Doctor",
                    Specialization = "General Practice",
                    IsActive = true,
                    WorkingDays = "Monday,Tuesday,Wednesday,Thursday,Friday",
                    WorkingHours = "9:00 AM - 5:00 PM",
                    MaxDailyPatients = 20
                };

                context.StaffMembers.Add(doctor);
                await context.SaveChangesAsync();
            }
        }

        public static async Task Initialize(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Make sure database is created
                    await context.Database.EnsureCreatedAsync();
                    
                    logger.LogInformation("Creating Permissions and UserPermissions tables if not exists...");
                    
                    // Create Permissions table with SQL if it doesn't exist
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
                        BEGIN
                            CREATE TABLE [Permissions] (
                                [Id] INT IDENTITY(1,1) NOT NULL,
                                [Name] NVARCHAR(50) NOT NULL,
                                [Description] NVARCHAR(255) NOT NULL,
                                [Category] NVARCHAR(50) NOT NULL,
                                CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
                            );
                        END");
                    
                    // Create UserPermissions table if it doesn't exist
                    await context.Database.ExecuteSqlRawAsync(@"
                        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
                        BEGIN
                            CREATE TABLE [UserPermissions] (
                                [Id] INT IDENTITY(1,1) NOT NULL,
                                [UserId] NVARCHAR(450) NOT NULL,
                                [PermissionId] INT NOT NULL,
                                CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id])
                            );
                            
                            -- Add foreign key constraints if the referenced tables exist
                            IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
                            BEGIN
                                ALTER TABLE [UserPermissions] 
                                ADD CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] 
                                FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id])
                                ON DELETE CASCADE;
                            END
                            
                            -- Add PermissionId foreign key
                            ALTER TABLE [UserPermissions] 
                            ADD CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] 
                            FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id])
                            ON DELETE CASCADE;
                        END");
                    
                    // Seed default permissions
                    await context.Database.ExecuteSqlRawAsync(@"
                        -- Insert permissions only if they don't exist
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ApproveUsers')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('ApproveUsers', 'Ability to approve user registrations', 'User Management');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'DeleteUsers')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('DeleteUsers', 'Ability to delete users from the system', 'User Management');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ViewReports')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('ViewReports', 'Ability to view system reports', 'Reports');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ManageUsers')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('ManageUsers', 'Ability to manage user accounts', 'User Management');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ManageAppointments')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('ManageAppointments', 'Ability to manage appointments', 'Appointments');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ManageMedicalRecords')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('ManageMedicalRecords', 'Ability to manage medical records', 'Medical Records');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'AccessAdminDashboard')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('AccessAdminDashboard', 'Ability to access the admin dashboard', 'Dashboard Access');
                        
                        IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'AccessDoctorDashboard')
                            INSERT INTO [Permissions] ([Name], [Description], [Category])
                            VALUES ('AccessDoctorDashboard', 'Ability to access the doctor dashboard', 'Dashboard Access');
                    ");
                    
                    // Grant all permissions to admin users
                    await context.Database.ExecuteSqlRawAsync(@"
                        -- Find admin users
                        DECLARE @AdminUsers TABLE (UserId NVARCHAR(450));
                        
                        -- Insert all users with Admin role
                        INSERT INTO @AdminUsers (UserId)
                        SELECT ur.UserId
                        FROM AspNetUserRoles ur
                        JOIN AspNetRoles r ON ur.RoleId = r.Id
                        WHERE r.Name = 'Admin';
                        
                        -- Grant all permissions to admin users
                        INSERT INTO UserPermissions (UserId, PermissionId)
                        SELECT au.UserId, p.Id
                        FROM @AdminUsers au
                        CROSS JOIN Permissions p
                        WHERE NOT EXISTS (
                            SELECT 1 FROM UserPermissions up 
                            WHERE up.UserId = au.UserId AND up.PermissionId = p.Id
                        );
                    ");
                    
                    logger.LogInformation("Permissions and UserPermissions tables created and seeded successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database: {Message}", ex.Message);
            }
        }
    }
}