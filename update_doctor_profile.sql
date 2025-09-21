-- Update Doctor Profile Information
-- This script ensures the doctor user has complete profile information

PRINT '=== UPDATING DOCTOR PROFILE ===';

-- Update doctor user profile
UPDATE AspNetUsers 
SET 
    FullName = 'Dr. John Smith',
    FirstName = 'John',
    LastName = 'Smith',
    IsActive = 1,
    UpdatedAt = GETDATE()
WHERE Email = 'doctor@example.com';

-- Check if update was successful
SELECT 
    Id,
    Email,
    FullName,
    FirstName,
    LastName,
    IsActive,
    UpdatedAt
FROM AspNetUsers 
WHERE Email = 'doctor@example.com';

PRINT '=== DOCTOR PROFILE UPDATE COMPLETED ===';

