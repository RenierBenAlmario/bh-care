-- Create sample doctor availability records for testing
-- This will enable appointment booking by providing clinic schedules

-- First, let's check if we have any doctor availability records
SELECT COUNT(*) as 'Current DoctorAvailability Records' FROM DoctorAvailabilities;

-- Insert a sample doctor availability record for weekdays (Monday-Friday)
-- This assumes we have at least one doctor in the system
INSERT INTO DoctorAvailabilities (
    DoctorId, 
    Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
    StartTime, EndTime, 
    IsAvailable, 
    LastUpdated
)
SELECT TOP 1
    u.Id as DoctorId,
    1 as Monday,     -- Available on Monday
    1 as Tuesday,    -- Available on Tuesday  
    1 as Wednesday,  -- Available on Wednesday
    1 as Thursday,   -- Available on Thursday
    1 as Friday,     -- Available on Friday
    0 as Saturday,   -- Not available on Saturday
    0 as Sunday,     -- Not available on Sunday
    '08:00:00' as StartTime,  -- 8:00 AM
    '17:00:00' as EndTime,    -- 5:00 PM
    1 as IsAvailable,
    GETDATE() as LastUpdated
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor'
AND NOT EXISTS (
    SELECT 1 FROM DoctorAvailabilities da WHERE da.DoctorId = u.Id
);

-- If no doctors exist, create a generic availability record
-- (This is a fallback - ideally you should have doctor users)
IF NOT EXISTS (SELECT 1 FROM DoctorAvailabilities)
BEGIN
    INSERT INTO DoctorAvailabilities (
        DoctorId, 
        Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
        StartTime, EndTime, 
        IsAvailable, 
        LastUpdated
    )
    VALUES (
        NULL,  -- Generic availability (not tied to specific doctor)
        1, 1, 1, 1, 1, 0, 0,  -- Monday-Friday available
        '08:00:00',  -- 8:00 AM
        '17:00:00',  -- 5:00 PM
        1,
        GETDATE()
    );
END

-- Verify the records were created
SELECT 
    da.Id,
    da.DoctorId,
    CASE WHEN u.FirstName IS NOT NULL THEN u.FirstName + ' ' + u.LastName ELSE 'Generic Clinic' END as DoctorName,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday,
    da.StartTime, da.EndTime, da.IsAvailable
FROM DoctorAvailabilities da
LEFT JOIN AspNetUsers u ON da.DoctorId = u.Id;
