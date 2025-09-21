-- Quick Verification Script for Consultation Time Selection
-- This script verifies that all components are working correctly

PRINT '=== CONSULTATION TIME VERIFICATION ===';

-- Check if we have doctors
SELECT COUNT(*) as 'Total Doctors' FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor';

-- Check doctor availability records
SELECT COUNT(*) as 'Doctor Availability Records' FROM DoctorAvailabilities;

-- Show doctor details
SELECT 
    u.Email,
    u.FullName,
    da.IsAvailable,
    da.StartTime,
    da.EndTime,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
WHERE r.Name = 'Doctor';

-- Test a specific date (September 18, 2025 - Wednesday)
DECLARE @TestDate DATE = '2025-09-18';
PRINT 'Test Date: ' + CAST(@TestDate AS NVARCHAR(10));
PRINT 'Day of Week: ' + DATENAME(WEEKDAY, @TestDate);

-- Check if this date should work for General Consult
SELECT 
    CASE 
        WHEN DATENAME(WEEKDAY, @TestDate) IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday') 
        THEN 'Valid for General Consult - Time slots should be available'
        ELSE 'Invalid for General Consult - No time slots expected'
    END as ExpectedResult;

PRINT '=== VERIFICATION COMPLETED ===';

