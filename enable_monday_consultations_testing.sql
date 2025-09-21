-- Enable Monday Consultations for Testing
-- This script ensures that Monday consultations are available for testing purposes

-- First, check current doctor availability status
PRINT '=== Current Doctor Availability Status ===';
SELECT 
    da.Id,
    da.DoctorId,
    u.UserName as DoctorEmail,
    u.FirstName + ' ' + u.LastName as DoctorName,
    da.IsAvailable,
    da.Monday,
    da.Tuesday,
    da.Wednesday,
    da.Thursday,
    da.Friday,
    da.Saturday,
    da.Sunday,
    da.StartTime,
    da.EndTime,
    da.LastUpdated
FROM DoctorAvailabilities da
LEFT JOIN AspNetUsers u ON da.DoctorId = u.Id
ORDER BY da.Id;

-- Check if we have any doctors with Doctor role
PRINT '=== Doctors with Doctor Role ===';
SELECT 
    u.Id,
    u.UserName,
    u.FirstName + ' ' + u.LastName as FullName,
    r.Name as Role
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor';

-- Enable Monday consultations for all doctors
PRINT '=== Enabling Monday Consultations for All Doctors ===';

-- Update existing doctor availability records to ensure Monday is enabled
UPDATE DoctorAvailabilities 
SET 
    Monday = 1,
    IsAvailable = 1,
    StartTime = '08:00:00',
    EndTime = '17:00:00',
    LastUpdated = GETDATE()
WHERE DoctorId IN (
    SELECT u.Id 
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = 'Doctor'
);

-- Create availability records for any doctors that don't have them yet
INSERT INTO DoctorAvailabilities (
    DoctorId,
    IsAvailable,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday,
    StartTime,
    EndTime,
    LastUpdated
)
SELECT 
    u.Id as DoctorId,
    1 as IsAvailable,
    1 as Monday,     -- Enable Monday for testing
    1 as Tuesday,
    1 as Wednesday,
    1 as Thursday,
    1 as Friday,
    1 as Saturday,   -- Enable weekends for testing
    1 as Sunday,     -- Enable weekends for testing
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

-- Verify the changes
PRINT '=== Updated Doctor Availability Status ===';
SELECT 
    da.Id,
    da.DoctorId,
    u.UserName as DoctorEmail,
    u.FirstName + ' ' + u.LastName as DoctorName,
    da.IsAvailable,
    da.Monday,
    da.Tuesday,
    da.Wednesday,
    da.Thursday,
    da.Friday,
    da.Saturday,
    da.Sunday,
    da.StartTime,
    da.EndTime,
    da.LastUpdated
FROM DoctorAvailabilities da
LEFT JOIN AspNetUsers u ON da.DoctorId = u.Id
ORDER BY da.Id;

-- Check consultation time slots for Monday
PRINT '=== Current Consultation Time Slots ===';
SELECT 
    Id,
    ConsultationType,
    StartTime,
    IsBooked,
    BookedById,
    BookedAt,
    CreatedAt
FROM ConsultationTimeSlots
WHERE DATEPART(weekday, StartTime) = 2  -- Monday = 2 in SQL Server
ORDER BY StartTime;

-- Summary
PRINT '=== Summary ===';
PRINT 'Monday consultations have been enabled for testing.';
PRINT 'All doctors are now available on Monday from 8:00 AM to 5:00 PM.';
PRINT 'Weekends are also enabled for additional testing flexibility.';
PRINT 'You can now book appointments for Monday consultations.';
