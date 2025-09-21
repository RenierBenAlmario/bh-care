-- Check for users with Doctor role
SELECT u.Id, u.UserName, u.Email, u.FirstName, u.LastName
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor';

-- Check for doctor availabilities
SELECT * FROM DoctorAvailabilities;

-- Check for doctor records in Doctors table
SELECT * FROM Doctors;

-- Count appointments
SELECT COUNT(*) FROM Appointments; 