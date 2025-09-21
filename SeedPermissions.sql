-- Check if Permissions table exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(255) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
END

-- Insert default permissions
-- First check if they already exist to avoid duplicates
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'ApproveUsers')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('ApproveUsers', 'Ability to approve user registrations', 'User Management');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'DeleteUsers')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('DeleteUsers', 'Ability to delete users from the system', 'User Management');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'ViewReports')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('ViewReports', 'Ability to view system reports', 'Reports');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'ManageUsers')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('ManageUsers', 'Ability to manage user accounts', 'User Management');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'ManageAppointments')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('ManageAppointments', 'Ability to manage appointments', 'Appointments');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'ManageMedicalRecords')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('ManageMedicalRecords', 'Ability to manage medical records', 'Medical Records');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'AccessAdminDashboard')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('AccessAdminDashboard', 'Ability to access the admin dashboard', 'Dashboard Access');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'AccessDoctorDashboard')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('AccessDoctorDashboard', 'Ability to access the doctor dashboard', 'Dashboard Access');

-- Permissions for Admin/Staff Navigation
IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'Admin/Staff Dashboard' AND [Category] = 'Dashboard Access')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('Admin/Staff Dashboard', 'Ability to access the admin and staff dashboard', 'Dashboard Access');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'View Patients' AND [Category] = 'Patient Management')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('View Patients', 'Ability to view the list of patients', 'Patient Management');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'Add Patient' AND [Category] = 'Patient Management')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('Add Patient', 'Ability to add new patients', 'Patient Management');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'View Records' AND [Category] = 'Medical Records')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('View Records', 'Ability to view patient medical records', 'Medical Records');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'Manage All Appointments' AND [Category] = 'Appointments')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('Manage All Appointments', 'Ability to manage all appointments in the system', 'Appointments');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Permissions] WHERE [Name] = 'View Reports' AND [Category] = 'Reporting')
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES ('View Reports', 'Ability to view system reports', 'Reporting');


-- Check if UserPermissions table exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
BEGIN
    CREATE TABLE [dbo].[UserPermissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [PermissionId] INT NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
    );
END

-- Grant admin user all permissions
DECLARE @AdminUserId NVARCHAR(450);
SELECT @AdminUserId = [Id] FROM [dbo].[AspNetUsers] WHERE [Email] = 'admin@example.com';

IF @AdminUserId IS NOT NULL
BEGIN
    -- Get all permission IDs
    DECLARE @PermissionIds TABLE (Id INT);
    INSERT INTO @PermissionIds SELECT [Id] FROM [dbo].[Permissions];

    -- Grant each permission to admin if not already granted
    DECLARE @PermissionId INT;
    DECLARE permission_cursor CURSOR FOR SELECT Id FROM @PermissionIds;
    OPEN permission_cursor;
    FETCH NEXT FROM permission_cursor INTO @PermissionId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [dbo].[UserPermissions] WHERE [UserId] = @AdminUserId AND [PermissionId] = @PermissionId)
        BEGIN
            INSERT INTO [dbo].[UserPermissions] ([UserId], [PermissionId])
            VALUES (@AdminUserId, @PermissionId);
        END
        FETCH NEXT FROM permission_cursor INTO @PermissionId;
    END

    CLOSE permission_cursor;
    DEALLOCATE permission_cursor;
END 