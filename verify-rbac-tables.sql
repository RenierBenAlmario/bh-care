USE [Barangay];
GO

-- Check if tables exist
SELECT 
    'Permissions' AS TableName, 
    CASE WHEN OBJECT_ID('Permissions') IS NOT NULL THEN 'Exists' ELSE 'Does not exist' END AS Status
UNION ALL
SELECT 
    'UserPermissions' AS TableName, 
    CASE WHEN OBJECT_ID('UserPermissions') IS NOT NULL THEN 'Exists' ELSE 'Does not exist' END AS Status
UNION ALL
SELECT 
    'StaffPositions' AS TableName, 
    CASE WHEN OBJECT_ID('StaffPositions') IS NOT NULL THEN 'Exists' ELSE 'Does not exist' END AS Status;

-- Check permissions data
SELECT * FROM Permissions;

-- Check staff positions data
SELECT * FROM StaffPositions;

-- Check user permissions
SELECT 
    u.UserName, 
    p.Name AS PermissionName, 
    p.Category
FROM UserPermissions up
JOIN AspNetUsers u ON up.UserId = u.Id
JOIN Permissions p ON up.PermissionId = p.Id
ORDER BY u.UserName, p.Category, p.Name; 