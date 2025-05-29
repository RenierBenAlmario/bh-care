-- Fix user access and permissions
USE [Barangay]
GO

-- 1. Ensure all necessary roles exist
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'PATIENT')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'PATIENT', 'PATIENT', NEWID());
END

-- 2. Ensure all necessary permissions exist
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access User Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access User Dashboard', 'Access to user dashboard', 'Dashboard Access');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Medical Records', 'Ability to view medical records', 'Medical Records');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Book Appointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Book Appointments', 'Ability to book appointments', 'Appointments');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Appointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Appointments', 'Ability to view appointments', 'Appointments');
END

-- 3. Grant PATIENT role to approved users who don't have it
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, r.Id
FROM AspNetUsers u
CROSS JOIN AspNetRoles r
WHERE u.Status = 'Approved'
AND r.Name = 'PATIENT'
AND NOT EXISTS (
    SELECT 1 
    FROM AspNetUserRoles ur 
    WHERE ur.UserId = u.Id 
    AND ur.RoleId = r.Id
);

-- 4. Grant essential permissions to approved users
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
CROSS JOIN Permissions p
WHERE u.Status = 'Approved'
AND p.Name IN (
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

-- 5. Add claims for permissions
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT DISTINCT u.Id, 'Permission', p.Name
FROM AspNetUsers u
JOIN UserPermissions up ON u.Id = up.UserId
JOIN Permissions p ON up.PermissionId = p.Id
WHERE u.Status = 'Approved'
AND NOT EXISTS (
    SELECT 1 
    FROM AspNetUserClaims c 
    WHERE c.UserId = u.Id 
    AND c.ClaimType = 'Permission' 
    AND c.ClaimValue = p.Name
);

-- 6. Add role claims
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT DISTINCT u.Id, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', r.Name
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Status = 'Approved'
AND NOT EXISTS (
    SELECT 1 
    FROM AspNetUserClaims c 
    WHERE c.UserId = u.Id 
    AND c.ClaimType = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' 
    AND c.ClaimValue = r.Name
);

PRINT 'User access and permissions have been fixed.'; 