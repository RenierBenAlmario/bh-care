-- SQL Script to add the 'Manage Consultations' permission to the database
-- This script checks if the permission exists and adds it if it doesn't

-- Check if the permission already exists
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Consultations')
BEGIN
    -- Add the 'Manage Consultations' permission
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Consultations', 'Can create and manage patient consultations', 'Medical Records');
    
    PRINT 'Added "Manage Consultations" permission';
END
ELSE
BEGIN
    PRINT '"Manage Consultations" permission already exists';
END

-- Get the permission ID
DECLARE @permissionId INT;
SELECT @permissionId = Id FROM Permissions WHERE Name = 'Manage Consultations';

-- Add the permission to the Doctor role
DECLARE @doctorRoleId NVARCHAR(450);
SELECT @doctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';

IF @doctorRoleId IS NOT NULL
BEGIN
    -- Check if the role permission mapping already exists
    IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @doctorRoleId AND PermissionId = @permissionId)
    BEGIN
        -- Add the permission to the Doctor role
        INSERT INTO RolePermissions (RoleId, PermissionId)
        VALUES (@doctorRoleId, @permissionId);
        
        PRINT 'Added "Manage Consultations" permission to Doctor role';
    END
    ELSE
    BEGIN
        PRINT '"Manage Consultations" permission already assigned to Doctor role';
    END
END
ELSE
BEGIN
    PRINT 'Doctor role not found';
END

-- Clear any cached permissions for all users with the Doctor role
PRINT 'Note: You may need to clear the permission cache or restart the application for changes to take effect.'