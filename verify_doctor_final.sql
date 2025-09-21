-- Final Doctor Verification Script
-- This script verifies that the doctor is properly configured

PRINT '=== FINAL DOCTOR VERIFICATION ===';

-- Check doctor user
SELECT 
    'Doctor User' as CheckType,
    Id,
    Email,
    FullName,
    FirstName,
    LastName,
    IsActive
FROM AspNetUsers 
WHERE Email = 'doctor@example.com';

-- Check doctor role assignment
SELECT 
    'Doctor Role Assignment' as CheckType,
    u.Email,
    r.Name as RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'doctor@example.com' AND r.Name = 'Doctor';

-- Check doctor availability
SELECT 
    'Doctor Availability' as CheckType,
    da.DoctorId,
    da.IsAvailable,
    da.StartTime,
    da.EndTime,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday
FROM DoctorAvailabilities da
INNER JOIN AspNetUsers u ON da.DoctorId = u.Id
WHERE u.Email = 'doctor@example.com';

-- Complete doctor information
SELECT 
    'Complete Doctor Info' as CheckType,
    u.Id as UserId,
    u.Email,
    u.FullName,
    r.Name as RoleName,
    da.IsAvailable,
    da.StartTime,
    da.EndTime
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
WHERE u.Email = 'doctor@example.com';

PRINT '=== DOCTOR VERIFICATION COMPLETED ===';

