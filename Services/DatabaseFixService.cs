using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Barangay.Data;
using Barangay.Models;

namespace Barangay.Services
{
    public class DatabaseFixService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseFixService> _logger;
        private readonly IConfiguration _configuration;

        public DatabaseFixService(
            IServiceProvider serviceProvider,
            ILogger<DatabaseFixService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting database fix service...");
            
            try
            {
                // Run in a scope to get database context
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                    // Check if database exists, if not don't try to run fixes
                    if (!await dbContext.Database.CanConnectAsync(cancellationToken))
                    {
                        _logger.LogWarning("Cannot connect to database. Skipping database fixes.");
            return;
        }

                    _logger.LogInformation("Connected to database. Running fixes...");

                    // 1. Fix Guardian Information
                    await FixGuardianInformationAsync(dbContext, cancellationToken);

                    // 2. Fix Role Permissions
                    await FixRolePermissionsAsync(dbContext, cancellationToken);

                    // 3. Refresh all permission caches
                    var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();
                    await permissionService.RefreshAllPermissionCachesAsync();

                    _logger.LogInformation("Database fixes completed successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error running database fixes.");
            }
        }

        private async Task FixGuardianInformationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fixing GuardianInformation table...");

            try
            {
                // Check if GuardianInformation table exists
                bool tableExists = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF OBJECT_ID(N'dbo.GuardianInformation', N'U') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;

                if (!tableExists)
                {
                    _logger.LogInformation("GuardianInformation table does not exist. No fixes needed.");
                    return;
                }

                // Check if GuardianFirstName and FirstName columns exist
                bool hasFirstName = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'FirstName') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;
                bool hasGuardianFirstName = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'GuardianFirstName') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;

                // If FirstName exists but not GuardianFirstName, add it
                if (hasFirstName && !hasGuardianFirstName)
                {
                    _logger.LogInformation("Adding GuardianFirstName column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD GuardianFirstName NVARCHAR(100) NULL");
                    
                    // Copy data from FirstName to GuardianFirstName
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET GuardianFirstName = FirstName WHERE GuardianFirstName IS NULL AND FirstName IS NOT NULL");
                }
                // If GuardianFirstName exists but not FirstName, add it
                else if (!hasFirstName && hasGuardianFirstName)
                {
                    _logger.LogInformation("Adding FirstName column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD FirstName NVARCHAR(100) NULL");
                    
                    // Copy data from GuardianFirstName to FirstName
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET FirstName = GuardianFirstName WHERE FirstName IS NULL AND GuardianFirstName IS NOT NULL");
                }
                // If both columns exist, synchronize data
                else if (hasFirstName && hasGuardianFirstName)
                {
                    _logger.LogInformation("Synchronizing FirstName and GuardianFirstName columns...");
                    
                    // Copy from FirstName to GuardianFirstName where GuardianFirstName is null
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET GuardianFirstName = FirstName WHERE GuardianFirstName IS NULL AND FirstName IS NOT NULL");
                    
                    // Copy from GuardianFirstName to FirstName where FirstName is null
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET FirstName = GuardianFirstName WHERE FirstName IS NULL AND GuardianFirstName IS NOT NULL");
                }

                // Do same for LastName and GuardianLastName
                bool hasLastName = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'LastName') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;
                bool hasGuardianLastName = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'GuardianLastName') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;

                // If LastName exists but not GuardianLastName, add it
                if (hasLastName && !hasGuardianLastName)
                {
                    _logger.LogInformation("Adding GuardianLastName column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD GuardianLastName NVARCHAR(100) NULL");
                    
                    // Copy data from LastName to GuardianLastName
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET GuardianLastName = LastName WHERE GuardianLastName IS NULL AND LastName IS NOT NULL");
                }
                // If GuardianLastName exists but not LastName, add it
                else if (!hasLastName && hasGuardianLastName)
                {
                    _logger.LogInformation("Adding LastName column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD LastName NVARCHAR(100) NULL");
                    
                    // Copy data from GuardianLastName to LastName
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET LastName = GuardianLastName WHERE LastName IS NULL AND GuardianLastName IS NOT NULL");
                }
                // If both columns exist, synchronize data
                else if (hasLastName && hasGuardianLastName)
                {
                    _logger.LogInformation("Synchronizing LastName and GuardianLastName columns...");
                    
                    // Copy from LastName to GuardianLastName where GuardianLastName is null
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET GuardianLastName = LastName WHERE GuardianLastName IS NULL AND LastName IS NOT NULL");
                    
                    // Copy from GuardianLastName to LastName where LastName is null
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "UPDATE dbo.GuardianInformation SET LastName = GuardianLastName WHERE LastName IS NULL AND GuardianLastName IS NOT NULL");
                }

                // Ensure other required columns exist
                bool hasResidencyProofPath = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'ResidencyProofPath') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;
                if (!hasResidencyProofPath)
                {
                    _logger.LogInformation("Adding ResidencyProofPath column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD ResidencyProofPath NVARCHAR(MAX) NULL");
                }

                bool hasConsentStatus = await dbContext.Database.ExecuteSqlRawAsync(
                    "IF COL_LENGTH('dbo.GuardianInformation', 'ConsentStatus') IS NOT NULL SELECT 1 ELSE SELECT 0") > 0;
                if (!hasConsentStatus)
                {
                    _logger.LogInformation("Adding ConsentStatus column...");
                    await dbContext.Database.ExecuteSqlRawAsync(
                        "ALTER TABLE dbo.GuardianInformation ADD ConsentStatus NVARCHAR(50) DEFAULT 'Pending'");
                }
                
                _logger.LogInformation("GuardianInformation table fix completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing GuardianInformation table.");
                throw;
            }
        }

        private async Task FixRolePermissionsAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fixing role permissions...");

            try
            {
                // Get Nurse and Doctor role IDs
                var nurseRoleId = await dbContext.Roles
                    .Where(r => r.NormalizedName == "NURSE")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                var doctorRoleId = await dbContext.Roles
                    .Where(r => r.NormalizedName == "DOCTOR")
                    .Select(r => r.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrEmpty(nurseRoleId) && string.IsNullOrEmpty(doctorRoleId))
                {
                    _logger.LogInformation("No Nurse or Doctor roles found. No fixes needed.");
                    return;
                }

                // Add required permissions to the database if they don't exist
                var requiredPermissions = new[]
                {
                    new { Name = "Access Dashboard", Category = "Dashboard" },
                    new { Name = "Access Nurse Dashboard", Category = "Dashboard" },
                    new { Name = "Access Doctor Dashboard", Category = "Dashboard" },
                    new { Name = "View Patients", Category = "Patient" },
                    new { Name = "Manage Patients", Category = "Patient" },
                    new { Name = "View Appointments", Category = "Appointment" },
                    new { Name = "Manage Appointments", Category = "Appointment" },
                    new { Name = "View Medical Records", Category = "Medical" },
                    new { Name = "View Vital Signs", Category = "Medical" },
                    new { Name = "Record Vital Signs", Category = "Medical" },
                    new { Name = "View Patient History", Category = "Patient" },
                    new { Name = "View Patient Details", Category = "Patient" }
                };

                foreach (var permission in requiredPermissions)
                {
                    var exists = await dbContext.Permissions
                        .AnyAsync(p => p.Name == permission.Name && p.Category == permission.Category, cancellationToken);

                    if (!exists)
                    {
                        dbContext.Permissions.Add(new Barangay.Models.Permission
                        {
                            Name = permission.Name,
                            Category = permission.Category,
                            Description = $"{permission.Name} permission"
                        });
                    }
                }

                await dbContext.SaveChangesAsync(cancellationToken);

                // Get permission IDs for nurse role
                var nursePermissionNames = new[]
                {
                    "Access Dashboard",
                    "Access Nurse Dashboard",
                    "View Patients",
                    "View Appointments",
                    "View Medical Records",
                    "View Vital Signs",
                    "Record Vital Signs",
                    "View Patient History",
                    "View Patient Details"
                };

                if (!string.IsNullOrEmpty(nurseRoleId))
                {
                    await AddRolePermissionsAsync(dbContext, nurseRoleId, nursePermissionNames, cancellationToken);
                }

                // Get permission IDs for doctor role
                var doctorPermissionNames = new[]
                {
                    "Access Dashboard",
                    "Access Doctor Dashboard",
                    "View Patients",
                    "Manage Patients",
                    "View Appointments",
                    "Manage Appointments",
                    "View Medical Records",
                    "View Patient History",
                    "View Patient Details"
                };

                if (!string.IsNullOrEmpty(doctorRoleId))
                {
                    await AddRolePermissionsAsync(dbContext, doctorRoleId, doctorPermissionNames, cancellationToken);
                }

                // Update user permissions from roles
                await SyncUserPermissionsFromRolesAsync(dbContext, cancellationToken);

                _logger.LogInformation("Role permission fix completed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fixing role permissions.");
                throw;
            }
        }

        private async Task AddRolePermissionsAsync(
            ApplicationDbContext dbContext, 
            string roleId, 
            string[] permissionNames, 
            CancellationToken cancellationToken)
        {
            // Get permission IDs for the given names
            var permissions = await dbContext.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync(cancellationToken);

            // Get existing role permissions
            var existingRolePermissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            // Add missing permissions
            foreach (var permission in permissions)
            {
                if (!existingRolePermissions.Contains(permission.Id))
                {
                    dbContext.RolePermissions.Add(new Barangay.Models.RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = permission.Id
                    });
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task SyncUserPermissionsFromRolesAsync(
            ApplicationDbContext dbContext,
            CancellationToken cancellationToken)
        {
            // Get all user role mappings
            var userRoles = await dbContext.UserRoles.ToListAsync(cancellationToken);

            // Process each user role
            foreach (var userRole in userRoles)
            {
                // Get role permissions for this role
                var rolePermissions = await dbContext.RolePermissions
                    .Where(rp => rp.RoleId == userRole.RoleId)
                    .ToListAsync(cancellationToken);

                // Get existing user permissions
                var existingUserPermissions = await dbContext.UserPermissions
                    .Where(up => up.UserId == userRole.UserId)
                    .Select(up => up.PermissionId)
                    .ToListAsync(cancellationToken);

                // Add missing permissions from role to user
                foreach (var rolePermission in rolePermissions)
                {
                    if (!existingUserPermissions.Contains(rolePermission.PermissionId))
                    {
                        dbContext.UserPermissions.Add(new Barangay.Models.UserPermission
                        {
                            UserId = userRole.UserId,
                            PermissionId = rolePermission.PermissionId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Database fix service stopping.");
            return Task.CompletedTask;
        }
    }
} 