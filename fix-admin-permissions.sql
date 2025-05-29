-- Fix admin permissions script
USE [Barangay]
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

PRINT 'Starting admin permissions fix...';

-- 1. Ensure Admin role exists and get its ID
DECLARE @AdminRoleId NVARCHAR(450);
SELECT @AdminRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN';

IF @AdminRoleId IS NULL
BEGIN
    SET @AdminRoleId = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID());
    PRINT 'Admin role created';
END
ELSE
BEGIN
    PRINT 'Admin role already exists';
END

-- 2. Ensure AccessAdminDashboard permission exists
IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'AccessAdminDashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('AccessAdminDashboard', 'Ability to access the admin dashboard', 'Dashboard Access');
    PRINT 'AccessAdminDashboard permission created';
END

-- 3. Get the admin user
DECLARE @AdminUserId NVARCHAR(450);
SELECT @AdminUserId = Id FROM AspNetUsers WHERE Email LIKE '%admin%';

IF @AdminUserId IS NOT NULL
BEGIN
    -- 4. Ensure admin user has Admin role
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @AdminUserId AND RoleId = @AdminRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@AdminUserId, @AdminRoleId);
        PRINT 'Admin role assigned to admin user';
    END
    ELSE
    BEGIN
        PRINT 'Admin user already has Admin role';
    END

    -- 5. Ensure admin user has AccessAdminDashboard permission
    DECLARE @AccessAdminDashboardPermissionId INT;
    SELECT @AccessAdminDashboardPermissionId = Id FROM Permissions WHERE Name = 'AccessAdminDashboard';

    IF NOT EXISTS (
        SELECT 1 
        FROM UserPermissions 
        WHERE UserId = @AdminUserId 
        AND PermissionId = @AccessAdminDashboardPermissionId
    )
    BEGIN
        INSERT INTO UserPermissions (UserId, PermissionId)
        VALUES (@AdminUserId, @AccessAdminDashboardPermissionId);
        PRINT 'AccessAdminDashboard permission granted to admin user';
    END
    ELSE
    BEGIN
        PRINT 'Admin user already has AccessAdminDashboard permission';
    END

    -- 6. Grant other essential admin permissions
    INSERT INTO UserPermissions (UserId, PermissionId)
    SELECT @AdminUserId, p.Id
    FROM Permissions p
    WHERE p.Name IN (
        'ManageUsers',
        'ViewReports',
        'ApproveUsers',
        'ManageAppointments',
        'ManageMedicalRecords',
        'ManagePermissions'
    )
    AND NOT EXISTS (
        SELECT 1 
        FROM UserPermissions up 
        WHERE up.UserId = @AdminUserId 
        AND up.PermissionId = p.Id
    );
    PRINT 'Essential admin permissions granted';
END
ELSE
BEGIN
    PRINT 'Admin user not found';
END

PRINT 'Admin permissions fix completed'; 