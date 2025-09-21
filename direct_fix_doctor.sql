-- Direct fix for doctor role assignment
-- Run this in SQL Server Management Studio

-- Step 1: Check current doctor user and role status
SELECT 
    u.Id as UserId,
    u.UserName,
    u.Email,
    u.FullName,
    r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'doctor@example.com';

-- Step 2: Get the Doctor role ID
SELECT Id as DoctorRoleId, Name 
FROM AspNetRoles 
WHERE Name = 'Doctor';

-- Step 3: Get the doctor user ID
SELECT Id as DoctorUserId, UserName, FullName 
FROM AspNetUsers 
WHERE UserName = 'doctor@example.com';

-- Step 4: Assign Doctor role to doctor user
-- Replace the IDs below with the actual IDs from steps 2 and 3
DECLARE @DoctorUserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE UserName = 'doctor@example.com');
DECLARE @DoctorRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Doctor');

-- Check if assignment already exists
IF EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @DoctorUserId AND RoleId = @DoctorRoleId)
BEGIN
    PRINT 'Doctor role already assigned to doctor@example.com';
END
ELSE
BEGIN
    -- Insert the role assignment
    INSERT INTO AspNetUserRoles (UserId, RoleId) 
    VALUES (@DoctorUserId, @DoctorRoleId);
    PRINT 'Doctor role successfully assigned to doctor@example.com';
END

-- Step 5: Verify the fix
SELECT 
    u.UserName,
    u.FullName,
    r.Name as RoleName,
    'Doctor role assigned successfully!' as Status
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'doctor@example.com' AND r.Name = 'Doctor';
