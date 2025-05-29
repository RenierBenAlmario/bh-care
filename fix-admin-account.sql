-- Fix admin account and roles
USE [Barangay]
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO

PRINT 'Starting admin account fix...';

-- 1. Ensure User role exists
DECLARE @UserRoleId NVARCHAR(450);
SELECT @UserRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'USER';

IF @UserRoleId IS NULL
BEGIN
    SET @UserRoleId = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@UserRoleId, 'User', 'USER', NEWID());
    PRINT 'User role created';
END
ELSE
BEGIN
    PRINT 'User role already exists';
END

-- 2. Ensure Admin role exists
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

-- 3. Check if admin account exists
DECLARE @AdminUserId NVARCHAR(450);
SELECT @AdminUserId = Id FROM AspNetUsers WHERE NormalizedEmail = 'ADMIN@EXAMPLE.COM';

IF @AdminUserId IS NULL
BEGIN
    -- Create admin account if it doesn't exist
    SET @AdminUserId = NEWID();
    
    INSERT INTO AspNetUsers (
        Id, 
        UserName,
        NormalizedUserName,
        Email,
        NormalizedEmail,
        EmailConfirmed,
        PasswordHash,
        SecurityStamp,
        ConcurrencyStamp,
        PhoneNumberConfirmed,
        TwoFactorEnabled,
        LockoutEnabled,
        AccessFailedCount,
        FirstName,
        LastName,
        Status
    )
    VALUES (
        @AdminUserId,
        'admin@example.com',
        'ADMIN@EXAMPLE.COM',
        'admin@example.com',
        'ADMIN@EXAMPLE.COM',
        1, -- EmailConfirmed
        'AQAAAAIAAYagAAAAELPFXBHOgxS4GwWrxZGPEjwxEXvpxPh5vDxvxEFxHlBH5f5LgHUPWXuQXpQPxHXQtw==', -- Password: Admin@123
        NEWID(), -- SecurityStamp
        NEWID(), -- ConcurrencyStamp
        0, -- PhoneNumberConfirmed
        0, -- TwoFactorEnabled
        1, -- LockoutEnabled
        0, -- AccessFailedCount
        'System',
        'Administrator',
        'Active'
    );
    PRINT 'Admin account created';
END
ELSE
BEGIN
    PRINT 'Admin account already exists';
    
    -- Update password if needed
    UPDATE AspNetUsers
    SET PasswordHash = 'AQAAAAIAAYagAAAAELPFXBHOgxS4GwWrxZGPEjwxEXvpxPh5vDxvxEFxHlBH5f5LgHUPWXuQXpQPxHXQtw=='
    WHERE Id = @AdminUserId;
    PRINT 'Admin password reset to: Admin@123';
END

-- 4. Ensure admin has User role
IF NOT EXISTS (
    SELECT 1 
    FROM AspNetUserRoles 
    WHERE UserId = @AdminUserId 
    AND RoleId = @UserRoleId
)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@AdminUserId, @UserRoleId);
    PRINT 'User role assigned to admin';
END
ELSE
BEGIN
    PRINT 'Admin already has User role';
END

-- 5. Ensure admin has Admin role
IF NOT EXISTS (
    SELECT 1 
    FROM AspNetUserRoles 
    WHERE UserId = @AdminUserId 
    AND RoleId = @AdminRoleId
)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@AdminUserId, @AdminRoleId);
    PRINT 'Admin role assigned to admin';
END
ELSE
BEGIN
    PRINT 'Admin already has Admin role';
END

-- 6. Ensure AccessAdminDashboard permission exists
IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'AccessAdminDashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('AccessAdminDashboard', 'Ability to access the admin dashboard', 'Dashboard Access');
    PRINT 'AccessAdminDashboard permission created';
END

-- 7. Grant admin permissions
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
    PRINT 'AccessAdminDashboard permission granted to admin';
END

-- 8. Grant other essential admin permissions
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

PRINT 'Admin account fix completed. You can now log in with:';
PRINT 'Email: admin@example.com';
PRINT 'Password: Admin@123'; 