using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Barangay.Migrations
{
    public partial class FixPermissionsSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure the Permissions table exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
                BEGIN
                    CREATE TABLE [dbo].[Permissions] (
                        [Id] INT IDENTITY(1,1) NOT NULL,
                        [Name] NVARCHAR(50) NOT NULL,
                        [Description] NVARCHAR(255) NULL,
                        [Category] NVARCHAR(50) NULL,
                        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
                    )
                END
            ");

            // Ensure the UserPermissions table exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
                BEGIN
                    CREATE TABLE [dbo].[UserPermissions] (
                        [Id] INT IDENTITY(1,1) NOT NULL,
                        [UserId] NVARCHAR(450) NOT NULL,
                        [PermissionId] INT NOT NULL,
                        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
                    )
                END
            ");

            // Create index on UserPermissions if it doesn't exist
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserPermissions_UserId_PermissionId' AND object_id = OBJECT_ID('UserPermissions'))
                BEGIN
                    CREATE UNIQUE INDEX [IX_UserPermissions_UserId_PermissionId] ON [dbo].[UserPermissions] ([UserId], [PermissionId])
                END
            ");

            // Insert default permissions
            migrationBuilder.Sql(@"
                -- Ensure all essential permissions exist
                MERGE INTO Permissions AS target
                USING (VALUES
                    -- Dashboard permissions
                    (1, 'Access Dashboard', 'Ability to access the main dashboard', 'Dashboard Access'),
                    (2, 'Access Doctor Dashboard', 'Ability to access the doctor dashboard', 'Dashboard Access'),
                    (3, 'Access Nurse Dashboard', 'Ability to access the nurse dashboard', 'Dashboard Access'),
                    (4, 'Access Admin Dashboard', 'Ability to access the admin dashboard', 'Dashboard Access'),
                    
                    -- Appointment permissions
                    (5, 'ManageAppointments', 'Ability to manage appointments', 'Appointments'),
                    (6, 'View Appointments', 'Ability to view appointments', 'Appointments'),
                    (7, 'Create Appointments', 'Ability to create appointments', 'Appointments'),
                    
                    -- Medical Records permissions
                    (8, 'Manage Medical Records', 'Ability to manage medical records', 'Medical Records'),
                    (9, 'View Patient History', 'Ability to view patient history', 'Medical Records'),
                    (10, 'Create Medical Records', 'Ability to create medical records', 'Medical Records'),
                    (11, 'Record Vital Signs', 'Ability to record vital signs', 'Medical Records'),
                    (12, 'Create Prescriptions', 'Ability to create prescriptions', 'Medical Records'),
                    (13, 'View Prescriptions', 'Ability to view prescriptions', 'Medical Records'),
                    
                    -- User Management permissions
                    (14, 'Manage Users', 'Ability to manage users', 'User Management'),
                    (15, 'Approve Users', 'Ability to approve new user registrations', 'User Management'),
                    (16, 'Delete Users', 'Ability to delete users', 'User Management'),
                    
                    -- Administration permissions
                    (17, 'Manage Permissions', 'Ability to manage user permissions', 'Administration'),
                    
                    -- Reporting permissions
                    (18, 'View Reports', 'Ability to view reports', 'Reporting'),
                    (19, 'Generate Reports', 'Ability to generate reports', 'Reporting')
                ) AS source (Id, Name, Description, Category)
                ON target.Name = source.Name
                WHEN NOT MATCHED BY target THEN
                    INSERT (Name, Description, Category)
                    VALUES (source.Name, source.Description, source.Category);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // We don't want to drop these tables on rollback
            // Just log a warning
            migrationBuilder.Sql(@"
                PRINT 'Warning: The FixPermissionsSchema migration has been rolled back, but the Permissions and UserPermissions tables have not been dropped to preserve data.'
            ");
        }
    }
} 