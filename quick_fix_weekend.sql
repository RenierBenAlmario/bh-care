-- Quick fix for weekend appointments
-- This will enable Saturday and Sunday for all doctors

-- Update existing DoctorAvailability records
UPDATE DoctorAvailabilities 
SET Saturday = 1, 
    Sunday = 1, 
    StartTime = '08:00:00', 
    EndTime = '17:00:00',
    IsAvailable = 1
WHERE DoctorId IN (
    SELECT u.Id 
    FROM AspNetUsers u 
    JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor'
);

-- Create DoctorAvailability records for doctors who don't have them
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
SELECT 'Updated DoctorAvailability records:' as Info;
SELECT da.DoctorId, da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday, da.StartTime, da.EndTime, da.IsAvailable
FROM DoctorAvailabilities da;
