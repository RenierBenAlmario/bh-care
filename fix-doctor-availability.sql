-- Fix Doctor Availability for Appointment Booking
-- This script creates doctor availability records for all doctors

-- First, check if DoctorAvailabilities table exists and get doctor IDs
DECLARE @DoctorId NVARCHAR(450);

-- Get the doctor user ID
SELECT @DoctorId = u.Id 
FROM AspNetUsers u 
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
WHERE u.UserName = 'doctor@example.com' AND r.Name = 'Doctor';

-- Check if availability record already exists
IF NOT EXISTS (SELECT 1 FROM DoctorAvailabilities WHERE DoctorId = @DoctorId)
BEGIN
    -- Create doctor availability record
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
    VALUES (
        @DoctorId,
        1, -- IsAvailable = true
        1, -- Monday = true
        1, -- Tuesday = true
        1, -- Wednesday = true
        1, -- Thursday = true
        1, -- Friday = true
        1, -- Saturday = true (enable weekends)
        1, -- Sunday = true (enable weekends)
        '08:00:00', -- StartTime = 8:00 AM
        '17:00:00', -- EndTime = 5:00 PM
        GETDATE() -- LastUpdated = now
    );
    
    PRINT 'Doctor availability record created successfully for doctor@example.com';
END
ELSE
BEGIN
    -- Update existing record to enable weekends
    UPDATE DoctorAvailabilities 
    SET 
        IsAvailable = 1,
        Saturday = 1,
        Sunday = 1,
        StartTime = '08:00:00',
        EndTime = '17:00:00',
        LastUpdated = GETDATE()
    WHERE DoctorId = @DoctorId;
    
    PRINT 'Doctor availability record updated successfully for doctor@example.com';
END

-- Verify the record was created/updated
SELECT 
    da.DoctorId,
    u.UserName,
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
INNER JOIN AspNetUsers u ON da.DoctorId = u.Id
WHERE u.UserName = 'doctor@example.com';
