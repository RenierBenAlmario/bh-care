-- Check if doctor user exists and has the correct role
SELECT 
    u.Email,
    u.UserName,
    u.FullName,
    r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'doctor@example.com' OR u.UserName = 'doctor@example.com';

-- If doctor exists but doesn't have Doctor role, add it
-- First, get the doctor user ID and Doctor role ID
DECLARE @DoctorUserId NVARCHAR(450);
DECLARE @DoctorRoleId NVARCHAR(450);

SELECT @DoctorUserId = Id FROM AspNetUsers WHERE Email = 'doctor@example.com' OR UserName = 'doctor@example.com';
SELECT @DoctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';

-- Check if the user-role relationship exists
IF @DoctorUserId IS NOT NULL AND @DoctorRoleId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @DoctorUserId AND RoleId = @DoctorRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@DoctorUserId, @DoctorRoleId);
        PRINT 'Doctor role assigned to doctor@example.com';
    END
    ELSE
    BEGIN
        PRINT 'Doctor role already assigned to doctor@example.com';
    END
END
ELSE
BEGIN
    PRINT 'Doctor user or Doctor role not found';
END

-- Verify the fix
SELECT 
    u.Email,
    u.UserName,
    u.FullName,
    r.Name as RoleName
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'doctor@example.com' OR u.UserName = 'doctor@example.com';
