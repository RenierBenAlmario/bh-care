-- Fix the mismatch between StaffMembers.UserId and AspNetUsers.Id
PRINT 'Starting to fix StaffMembers UserId mapping...';

-- First, create a temporary table to store the mapping
IF OBJECT_ID('tempdb..#UserMapping') IS NOT NULL
    DROP TABLE #UserMapping;

CREATE TABLE #UserMapping (
    StaffId INT,
    StaffName NVARCHAR(255),
    StaffEmail NVARCHAR(255),
    StaffRole NVARCHAR(50),
    UserId NVARCHAR(450)
);

-- Insert mappings based on email match
INSERT INTO #UserMapping (StaffId, StaffName, StaffEmail, StaffRole, UserId)
SELECT s.Id, s.Name, s.Email, s.Role, u.Id
FROM StaffMembers s
JOIN AspNetUsers u ON s.Email = u.Email
WHERE s.Email IS NOT NULL AND s.Email != '';

-- Print the mapping
SELECT * FROM #UserMapping;

-- Update StaffMembers with the correct UserId
UPDATE StaffMembers
SET UserId = m.UserId
FROM StaffMembers s
JOIN #UserMapping m ON s.Id = m.StaffId
WHERE m.UserId IS NOT NULL;

-- For staff without email matches, try to match based on role and name similarity
-- For admin
UPDATE StaffMembers
SET UserId = (
    SELECT TOP 1 u.Id 
    FROM AspNetUsers u
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = 'Admin' 
    AND StaffMembers.Role = 'Admin'
    AND StaffMembers.UserId IS NULL
)
WHERE Role = 'Admin' AND UserId IS NULL;

-- For doctors
UPDATE StaffMembers
SET UserId = (
    SELECT TOP 1 u.Id 
    FROM AspNetUsers u
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = 'Doctor' 
    AND StaffMembers.Role = 'Doctor'
    AND StaffMembers.UserId IS NULL
)
WHERE Role = 'Doctor' AND UserId IS NULL;

-- For nurses
UPDATE StaffMembers
SET UserId = (
    SELECT TOP 1 u.Id 
    FROM AspNetUsers u
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = 'Nurse' 
    AND StaffMembers.Role = 'Nurse'
    AND StaffMembers.UserId IS NULL
)
WHERE Role = 'Nurse' AND UserId IS NULL;

-- Add the missing permissions after fixing the mapping
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT s.UserId, p.Id
FROM StaffMembers s
CROSS JOIN Permissions p
WHERE p.Name IN ('Access Admin Dashboard', 'Access Doctor Dashboard', 'Access Nurse Dashboard')
AND s.UserId IS NOT NULL 
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = s.UserId AND up.PermissionId = p.Id
);

-- Count and print the results
DECLARE @StaffWithUserId INT = (SELECT COUNT(*) FROM StaffMembers WHERE UserId IS NOT NULL);
DECLARE @StaffCount INT = (SELECT COUNT(*) FROM StaffMembers);
DECLARE @PermCount INT = (SELECT COUNT(*) FROM UserPermissions);

PRINT 'Fixed UserId mapping for ' + CAST(@StaffWithUserId AS NVARCHAR) + ' out of ' + CAST(@StaffCount AS NVARCHAR) + ' staff members';
PRINT 'Total permissions in UserPermissions table: ' + CAST(@PermCount AS NVARCHAR);

-- Clean up
DROP TABLE #UserMapping; 