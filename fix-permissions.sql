-- Fix permissions database issues

-- Ensure tables exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE [Permissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(255) NULL,
        [Category] NVARCHAR(50) NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
    );
    PRINT 'Created Permissions table';
END

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
BEGIN
    CREATE TABLE [UserPermissions] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [PermissionId] INT NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_UserPermissions_AspNetUsers] FOREIGN KEY ([UserId]) 
            REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions] FOREIGN KEY ([PermissionId]) 
            REFERENCES [Permissions] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Created UserPermissions table';
END

-- Create indexes if they don't exist
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserPermissions_UserId' AND object_id = OBJECT_ID('UserPermissions'))
BEGIN
    CREATE INDEX [IX_UserPermissions_UserId] ON [UserPermissions]([UserId]);
    PRINT 'Created index IX_UserPermissions_UserId';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UserPermissions_PermissionId' AND object_id = OBJECT_ID('UserPermissions'))
BEGIN
    CREATE INDEX [IX_UserPermissions_PermissionId] ON [UserPermissions]([PermissionId]);
    PRINT 'Created index IX_UserPermissions_PermissionId';
END

-- Add missing permissions for different roles
-- Nurse permissions
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'ManageAppointments')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('ManageAppointments', 'Ability to manage appointments', 'Appointment Management');
    PRINT 'Added permission: ManageAppointments';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Access Nurse Dashboard')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Access Nurse Dashboard', 'Access to the nurse dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Nurse Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Record Vital Signs')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Record Vital Signs', 'Ability to record patient vital signs', 'Medical Records');
    PRINT 'Added permission: Record Vital Signs';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'View Patient History')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('View Patient History', 'Ability to view patient medical history', 'Medical Records');
    PRINT 'Added permission: View Patient History';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Manage Medical Records')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Manage Medical Records', 'Ability to manage patient medical records', 'Medical Records');
    PRINT 'Added permission: Manage Medical Records';
END

-- Doctor permissions
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Access Doctor Dashboard')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Access Doctor Dashboard', 'Access to the doctor dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Doctor Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Create Prescriptions')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Create Prescriptions', 'Ability to create prescriptions for patients', 'Medical Records');
    PRINT 'Added permission: Create Prescriptions';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'View Reports')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('View Reports', 'Ability to view system reports', 'Reporting');
    PRINT 'Added permission: View Reports';
END

-- Admin permissions
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Manage Users')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Manage Users', 'Ability to manage system users', 'User Management');
    PRINT 'Added permission: Manage Users';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Access Admin Dashboard')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Access Admin Dashboard', 'Access to the admin dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Admin Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Approve Users')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Approve Users', 'Ability to approve new user registrations', 'User Management');
    PRINT 'Added permission: Approve Users';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Manage Permissions')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Manage Permissions', 'Ability to manage user permissions', 'User Management');
    PRINT 'Added permission: Manage Permissions';
END

-- Common permissions
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Access Dashboard')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('Access Dashboard', 'Basic access to the system dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'View Dashboard')
BEGIN
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES ('View Dashboard', 'View the system dashboard', 'Dashboard Access');
    PRINT 'Added permission: View Dashboard';
END

-- Grant essential permissions to existing staff members
PRINT 'Granting essential permissions to nurses...';
INSERT INTO [UserPermissions] ([UserId], [PermissionId])
SELECT u.Id, p.Id
FROM [AspNetUsers] u
JOIN [AspNetUserRoles] ur ON u.Id = ur.UserId
JOIN [AspNetRoles] r ON ur.RoleId = r.Id
CROSS JOIN [Permissions] p
WHERE r.Name = 'Nurse'
AND p.Name IN ('ManageAppointments', 'Access Nurse Dashboard', 'Record Vital Signs', 'View Patient History', 'Manage Medical Records')
AND NOT EXISTS (
    SELECT 1 
    FROM [UserPermissions] up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- Grant essential permissions to doctors
PRINT 'Granting essential permissions to doctors...';
INSERT INTO [UserPermissions] ([UserId], [PermissionId])
SELECT u.Id, p.Id
FROM [AspNetUsers] u
JOIN [AspNetUserRoles] ur ON u.Id = ur.UserId
JOIN [AspNetRoles] r ON ur.RoleId = r.Id
CROSS JOIN [Permissions] p
WHERE r.Name = 'Doctor'
AND p.Name IN ('Access Doctor Dashboard', 'Manage Appointments', 'Manage Medical Records', 'View Patient History', 'Create Prescriptions', 'View Reports')
AND NOT EXISTS (
    SELECT 1 
    FROM [UserPermissions] up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- Grant essential permissions to admins
PRINT 'Granting essential permissions to admins...';
INSERT INTO [UserPermissions] ([UserId], [PermissionId])
SELECT u.Id, p.Id
FROM [AspNetUsers] u
JOIN [AspNetUserRoles] ur ON u.Id = ur.UserId
JOIN [AspNetRoles] r ON ur.RoleId = r.Id
CROSS JOIN [Permissions] p
WHERE r.Name = 'Admin'
AND p.Name IN ('Manage Users', 'Access Admin Dashboard', 'View Reports', 'Approve Users', 'Manage Appointments', 'Manage Medical Records', 'Manage Permissions')
AND NOT EXISTS (
    SELECT 1 
    FROM [UserPermissions] up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- Record migration in migration history
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250525_FixPermissions')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250525_FixPermissions', '8.0.0');
    PRINT 'Migration recorded in __EFMigrationsHistory';
END

PRINT 'Permission fix script completed successfully.'; 