-- SQL Script to verify nurse permissions were properly updated
USE [Barangay]
GO

PRINT 'Verifying nurse permissions...';

-- Find the staff ID for the nurse from the screenshot
DECLARE @NurseStaffId INT = 3;
DECLARE @NurseUserId NVARCHAR(450);
SELECT @NurseUserId = UserId FROM StaffMembers WHERE Id = @NurseStaffId;

PRINT 'Nurse UserId: ' + @NurseUserId;

-- Check UserPermissions
PRINT '-----User Permissions-----';
SELECT p.Category, p.Name, p.Description
FROM UserPermissions up
JOIN Permissions p ON up.PermissionId = p.Id
WHERE up.UserId = @NurseUserId
ORDER BY p.Category, p.Name;

-- Check StaffPermissions
PRINT '-----Staff Permissions-----';
SELECT p.Category, p.Name, p.Description
FROM StaffPermissions sp
JOIN Permissions p ON sp.PermissionId = p.Id
WHERE sp.StaffMemberId = @NurseStaffId
ORDER BY p.Category, p.Name;

-- Check Claims
PRINT '-----User Claims-----';
SELECT ClaimType, ClaimValue
FROM AspNetUserClaims
WHERE UserId = @NurseUserId
ORDER BY ClaimType, ClaimValue;

-- Check permission counts to match the badges in the UI
PRINT '-----Permission Counts by Category-----';
SELECT 
    p.Category,
    COUNT(DISTINCT p.Id) AS Total,
    SUM(CASE WHEN up.UserId IS NOT NULL THEN 1 ELSE 0 END) AS Granted,
    CAST(SUM(CASE WHEN up.UserId IS NOT NULL THEN 1 ELSE 0 END) AS VARCHAR) + '/' + 
    CAST(COUNT(DISTINCT p.Id) AS VARCHAR) AS Ratio
FROM Permissions p
LEFT JOIN UserPermissions up ON p.Id = up.PermissionId AND up.UserId = @NurseUserId
WHERE p.Category IS NOT NULL
GROUP BY p.Category
ORDER BY p.Category; 