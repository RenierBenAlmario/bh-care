-- Fix user permissions for approved users
USE [Barangay]
GO

-- First, ensure the basic permissions exist
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Dashboard', 'Basic access to the system dashboard', 'Dashboard Access');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Dashboard', 'View the system dashboard', 'Dashboard Access');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access User Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access User Dashboard', 'Access to the user dashboard', 'Dashboard Access');
END

-- Grant basic permissions to all approved users
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
CROSS JOIN Permissions p
WHERE u.Status = 'Approved'
AND p.Name IN ('Access Dashboard', 'View Dashboard', 'Access User Dashboard')
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

-- Grant role-specific permissions
-- For PATIENT role
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE u.Status = 'Approved'
AND r.Name = 'PATIENT'
AND p.Name IN (
    'Access Dashboard',
    'View Dashboard',
    'Access User Dashboard',
    'View Medical Records',
    'Book Appointments',
    'View Appointments'
)
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

PRINT 'User permissions have been updated successfully.'; 