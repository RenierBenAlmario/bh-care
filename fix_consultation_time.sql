-- Fix Consultation Time Selection Issue
-- This script ensures proper doctor availability configuration

-- Check current state
PRINT '=== CURRENT STATE CHECK ===';
SELECT COUNT(*) as 'Total Users' FROM AspNetUsers;
SELECT COUNT(*) as 'Total Doctors' FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor';
SELECT COUNT(*) as 'Doctor Availability Records' FROM DoctorAvailabilities;

-- Ensure we have at least one doctor with proper role assignment
PRINT '=== ENSURING DOCTOR ROLE ASSIGNMENT ===';

-- Check if doctor@example.com exists and has Doctor role
IF NOT EXISTS (
    SELECT 1 FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE u.Email = 'doctor@example.com' AND r.Name = 'Doctor'
)
BEGIN
    -- Assign Doctor role to doctor@example.com if it exists
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT u.Id, r.Id 
    FROM AspNetUsers u, AspNetRoles r 
    WHERE u.Email = 'doctor@example.com' AND r.Name = 'Doctor'
    AND NOT EXISTS (
        SELECT 1 FROM AspNetUserRoles ur 
        WHERE ur.UserId = u.Id AND ur.RoleId = r.Id
    );
    
    PRINT 'Doctor role assigned to doctor@example.com';
END
ELSE
BEGIN
    PRINT 'Doctor role already assigned to doctor@example.com';
END

-- Create doctor availability records for all doctors
PRINT '=== CREATING DOCTOR AVAILABILITY RECORDS ===';

INSERT INTO DoctorAvailabilities (
    DoctorId, 
    IsAvailable, 
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
    StartTime, EndTime, 
    LastUpdated
)
SELECT 
    u.Id as DoctorId,
    1 as IsAvailable,
    1 as Monday,     -- Available Monday-Friday
    1 as Tuesday,
    1 as Wednesday,
    1 as Thursday,
    1 as Friday,
    1 as Saturday,   -- Enable weekends
    1 as Sunday,     -- Enable weekends
    '08:00:00' as StartTime,  -- 8:00 AM
    '17:00:00' as EndTime,    -- 5:00 PM
    GETDATE() as LastUpdated
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor'
AND NOT EXISTS (
    SELECT 1 FROM DoctorAvailabilities da WHERE da.DoctorId = u.Id
);

PRINT 'Doctor availability records created/updated';

-- Verify the fix
PRINT '=== VERIFICATION ===';
SELECT 
    da.Id,
    da.DoctorId,
    u.Email as DoctorEmail,
    u.FirstName + ' ' + u.LastName as DoctorName,
    da.IsAvailable,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday,
    da.StartTime,
    da.EndTime
FROM DoctorAvailabilities da
INNER JOIN AspNetUsers u ON da.DoctorId = u.Id
ORDER BY da.Id;

PRINT '=== CONSULTATION TIME FIX COMPLETED ===';

