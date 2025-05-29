USE [Barangay];
GO

-- Function to check permissions for a specific user by email
DECLARE @userEmail NVARCHAR(256) = 'admin@example.com'; -- Change this to the email you want to check

-- Find the user and their permissions
SELECT 
    u.UserName,
    u.Email,
    r.Name AS Role,
    p.Name AS Permission,
    p.Description,
    p.Category
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN UserPermissions up ON u.Id = up.UserId
LEFT JOIN Permissions p ON up.PermissionId = p.Id
WHERE u.Email = @userEmail
ORDER BY p.Category, p.Name;

-- Show how many permissions this user has
SELECT 
    u.UserName,
    u.Email,
    COUNT(DISTINCT p.Id) AS PermissionCount
FROM AspNetUsers u
LEFT JOIN UserPermissions up ON u.Id = up.UserId
LEFT JOIN Permissions p ON up.PermissionId = p.Id
WHERE u.Email = @userEmail
GROUP BY u.UserName, u.Email;

-- Check permissions in the database

-- Check if Permissions table exists and has data
PRINT 'Checking Permissions table...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    PRINT 'Permissions table exists';
    
    -- Count permissions
    DECLARE @permissionCount INT;
    SELECT @permissionCount = COUNT(*) FROM Permissions;
    PRINT 'Number of permissions: ' + CAST(@permissionCount AS NVARCHAR(10));
    
    -- List permissions by category
    PRINT 'Permissions by category:';
    SELECT Category, COUNT(*) AS PermissionCount 
    FROM Permissions 
    GROUP BY Category 
    ORDER BY Category;
    
    -- List all permissions
    PRINT 'All permissions:';
    SELECT Id, Name, Category, Description 
    FROM Permissions 
    ORDER BY Category, Name;
END
ELSE
BEGIN
    PRINT 'ERROR: Permissions table does not exist';
END

-- Check if UserPermissions table exists and has data
PRINT 'Checking UserPermissions table...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
BEGIN
    PRINT 'UserPermissions table exists';
    
    -- Count user permissions
    DECLARE @userPermissionCount INT;
    SELECT @userPermissionCount = COUNT(*) FROM UserPermissions;
    PRINT 'Number of user permissions: ' + CAST(@userPermissionCount AS NVARCHAR(10));
    
    -- Count permissions per user
    PRINT 'Permissions per user:';
    SELECT u.UserName, u.Email, r.Name AS Role, COUNT(up.Id) AS PermissionCount
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    LEFT JOIN UserPermissions up ON u.Id = up.UserId
    GROUP BY u.UserName, u.Email, r.Name
    ORDER BY r.Name, u.UserName;
    
    -- List permissions for a specific role (e.g., Nurse)
    PRINT 'Permissions for Nurses:';
    SELECT u.UserName, u.Email, p.Name AS Permission, p.Category
    FROM AspNetUsers u
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    JOIN AspNetRoles r ON ur.RoleId = r.Id
    JOIN UserPermissions up ON u.Id = up.UserId
    JOIN Permissions p ON up.PermissionId = p.Id
    WHERE r.Name = 'Nurse'
    ORDER BY u.UserName, p.Category, p.Name;
END
ELSE
BEGIN
    PRINT 'ERROR: UserPermissions table does not exist';
END

-- Check for missing essential permissions
PRINT 'Checking for missing essential permissions for nurses:';
SELECT u.UserName, u.Email
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Nurse'
AND NOT EXISTS (
    SELECT 1
    FROM UserPermissions up
    JOIN Permissions p ON up.PermissionId = p.Id
    WHERE up.UserId = u.Id
    AND p.Name = 'ManageAppointments'
);

-- Check for permission inconsistencies
PRINT 'Checking for permission inconsistencies:';
SELECT u.UserName, u.Email, r.Name AS Role, p.Name AS Permission
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
JOIN UserPermissions up ON u.Id = up.UserId
JOIN Permissions p ON up.PermissionId = p.Id
WHERE 
    (r.Name = 'Nurse' AND p.Name LIKE '%Admin%') OR
    (r.Name = 'Doctor' AND p.Name LIKE '%Admin%')
ORDER BY u.UserName, p.Name; 