-- Create missing patient records for verified users
-- This script fixes the "FK_Appointments_Patients_PatientId" constraint error when booking appointments

-- Find all users who don't have corresponding patient records
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
WHERE u.Status = 'Verified' 
AND NOT EXISTS (
    SELECT 1 FROM Patients p WHERE p.UserId = u.Id
);

-- Report how many patient records were created
DECLARE @RecordsCreated INT;
SET @RecordsCreated = @@ROWCOUNT;
PRINT 'Created ' + CAST(@RecordsCreated AS NVARCHAR) + ' missing patient records';

-- Check for any remaining users without patient records
DECLARE @RemainingCount INT;
SELECT @RemainingCount = COUNT(*)
FROM AspNetUsers u
WHERE u.Status = 'Verified'
AND NOT EXISTS (
    SELECT 1 FROM Patients p WHERE p.UserId = u.Id
);

PRINT 'Remaining users without patient records: ' + CAST(@RemainingCount AS NVARCHAR);

-- Update existing patient records with correct information from user profiles if needed
UPDATE p
SET
    p.BirthDate = CASE
                    WHEN p.BirthDate IS NULL THEN COALESCE(u.BirthDate, DATEADD(YEAR, -30, GETDATE()))
                    ELSE p.BirthDate
                  END,
    p.Gender = CASE
                WHEN p.Gender IS NULL OR p.Gender = '' THEN COALESCE(u.Gender, 'Other')
                ELSE p.Gender
              END,
    p.FullName = CASE
                WHEN p.FullName IS NULL OR p.FullName = '' THEN 
                    COALESCE(u.FullName, 
                        CASE 
                            WHEN u.FirstName IS NOT NULL AND u.LastName IS NOT NULL THEN u.FirstName + ' ' + u.LastName
                            ELSE COALESCE(u.UserName, 'Unknown User')
                        END
                    )
                ELSE p.FullName
              END,
    p.Address = CASE
                WHEN p.Address IS NULL OR p.Address = '' THEN COALESCE(u.Address, 'Not specified')
                ELSE p.Address
              END,
    p.ContactNumber = CASE
                WHEN p.ContactNumber IS NULL OR p.ContactNumber = '' THEN COALESCE(u.PhoneNumber, 'Not specified')
                ELSE p.ContactNumber
              END,
    p.Email = CASE
                WHEN p.Email IS NULL OR p.Email = '' THEN COALESCE(u.Email, 'not@specified.com')
                ELSE p.Email
              END,
    p.UpdatedAt = GETDATE()
FROM Patients p
JOIN AspNetUsers u ON p.UserId = u.Id
WHERE p.BirthDate IS NULL OR p.Gender IS NULL OR p.Gender = '' OR p.FullName IS NULL OR p.FullName = '' 
   OR p.Address IS NULL OR p.Address = '' OR p.ContactNumber IS NULL OR p.ContactNumber = '' OR p.Email IS NULL OR p.Email = '';

PRINT 'Updated existing patient records with missing information';