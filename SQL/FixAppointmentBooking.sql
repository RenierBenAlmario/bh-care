-- Fix appointment booking issues by ensuring correct patient records exist
-- This script addresses the "FK_Appointments_Patients_PatientId" constraint error

-- Step 1: Create patient records for any users who don't have them
PRINT 'Creating patient records for users who dont have them...';
    
INSERT INTO Patients (UserId, BirthDate, Gender, BloodType, Allergies, MedicalHistory, CreatedAt, UpdatedAt, FullName, Address, ContactNumber, EmergencyContact, EmergencyContactNumber, Email)
SELECT 
    u.Id,
    COALESCE(u.BirthDate, DATEADD(YEAR, -30, GETDATE())), -- Default to 30 years ago if birthdate is null
    COALESCE(u.Gender, 'Other'),
    'Unknown', -- Default blood type
    '', -- Empty allergies
    '', -- Empty medical history
    GETDATE(), -- Current timestamp for CreatedAt
    GETDATE(), -- Current timestamp for UpdatedAt
    COALESCE(u.FullName, 
        CASE 
            WHEN u.FirstName IS NOT NULL AND u.LastName IS NOT NULL THEN u.FirstName + ' ' + u.LastName
            ELSE COALESCE(u.UserName, 'Unknown User')
        END
    ), -- Use FullName if available, otherwise combine FirstName and LastName, fallback to UserName or 'Unknown User'
    COALESCE(u.Address, 'Not specified'), -- Use user's address or default
    COALESCE(u.PhoneNumber, 'Not specified'), -- Use user's phone or default
    'Emergency Contact', -- Default emergency contact
    'Not specified', -- Default emergency contact number
    COALESCE(u.Email, 'not@specified.com') -- Use user's email or default
FROM AspNetUsers u
WHERE NOT EXISTS (
    SELECT 1 FROM Patients p WHERE p.UserId = u.Id
);
    
DECLARE @PatientRecordsCreated INT = @@ROWCOUNT;
PRINT 'Created ' + CAST(@PatientRecordsCreated AS NVARCHAR) + ' patient records.';

-- Check for any appointments with missing patient records
DECLARE @InvalidAppointments INT;
SELECT @InvalidAppointments = COUNT(*)
FROM Appointments a
LEFT JOIN Patients p ON a.PatientId = p.UserId
WHERE p.UserId IS NULL;

IF @InvalidAppointments > 0
BEGIN
    PRINT 'Found ' + CAST(@InvalidAppointments AS NVARCHAR) + ' appointments with missing patient records.';
    PRINT 'Please ensure all appointments have valid patient records.';
END
ELSE
BEGIN
    PRINT 'All appointments have valid patient records.';
END

PRINT 'Appointment booking fix script completed.'; 