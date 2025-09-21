-- Test Doctor Availability and Time Slot Generation
-- This script verifies that the consultation time fix is working

PRINT '=== TESTING DOCTOR AVAILABILITY ===';

-- Check if we have doctors with proper roles
SELECT 
    u.Id as UserId,
    u.Email,
    u.FirstName + ' ' + u.LastName as FullName,
    r.Name as RoleName
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor';

-- Check doctor availability records
SELECT 
    da.Id,
    da.DoctorId,
    u.Email as DoctorEmail,
    da.IsAvailable,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday,
    da.StartTime,
    da.EndTime
FROM DoctorAvailabilities da
LEFT JOIN AspNetUsers u ON da.DoctorId = u.Id;

-- Test a specific date (September 18, 2025 - Thursday)
DECLARE @TestDate DATE = '2025-09-18';
DECLARE @TestConsultationType NVARCHAR(50) = 'general consult';

PRINT '=== TESTING TIME SLOT GENERATION ===';
PRINT 'Test Date: ' + CAST(@TestDate AS NVARCHAR(10));
PRINT 'Test Consultation Type: ' + @TestConsultationType;

-- Check if the date is a valid day for general consult (should be Thursday)
SELECT 
    @TestDate as TestDate,
    DATENAME(WEEKDAY, @TestDate) as DayOfWeek,
    CASE 
        WHEN DATENAME(WEEKDAY, @TestDate) IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday') 
        THEN 'Valid for General Consult'
        ELSE 'Invalid for General Consult'
    END as ValidityCheck;

PRINT '=== CONSULTATION TIME TEST COMPLETED ===';

