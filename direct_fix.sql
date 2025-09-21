-- Direct fix for weekend appointments
-- Run this to enable weekend appointments immediately

-- First, let's see what we have
SELECT 'BEFORE FIX - Current DoctorAvailability records:' as Info;
SELECT da.DoctorId, da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday, da.StartTime, da.EndTime, da.IsAvailable
FROM DoctorAvailabilities da;

-- Update ALL existing DoctorAvailability records to enable weekends
UPDATE DoctorAvailabilities 
SET Saturday = 1, 
    Sunday = 1, 
    StartTime = '08:00:00', 
    EndTime = '17:00:00',
    IsAvailable = 1;

-- Create DoctorAvailability records for doctors who don't have them yet
INSERT INTO DoctorAvailabilities (DoctorId, IsAvailable, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartTime, EndTime, LastUpdated)
SELECT u.Id, 1, 1, 1, 1, 1, 1, 1, 1, '08:00:00', '17:00:00', GETDATE()
FROM AspNetUsers u 
JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
JOIN AspNetRoles r ON ur.RoleId = r.Id 
WHERE r.Name = 'Doctor'
AND NOT EXISTS (
    SELECT 1 FROM DoctorAvailabilities da WHERE da.DoctorId = u.Id
);

-- Show the results
SELECT 'AFTER FIX - Updated DoctorAvailability records:' as Info;
SELECT da.DoctorId, da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday, da.StartTime, da.EndTime, da.IsAvailable
FROM DoctorAvailabilities da;

-- Show doctors with weekend availability
SELECT 'Doctors with weekend availability:' as Info;
SELECT u.UserName, u.Email, da.Saturday, da.Sunday, da.StartTime, da.EndTime, da.IsAvailable
FROM AspNetUsers u
JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
WHERE da.Saturday = 1 AND da.Sunday = 1;
