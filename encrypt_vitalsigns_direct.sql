-- Direct VitalSigns Encryption Script
-- WARNING: This is a demonstration script. In practice, you should use the application's encryption service
-- to ensure proper AES-256 encryption with correct IV handling.

-- First, let's see the current data
SELECT 
    Id,
    PatientId,
    Temperature,
    BloodPressure,
    HeartRate,
    RespiratoryRate,
    SpO2,
    Weight,
    Height,
    Notes,
    RecordedAt
FROM [Barangay].[dbo].[VitalSigns];

-- Count current records
SELECT COUNT(*) as TotalVitalSignsRecords FROM [Barangay].[dbo].[VitalSigns];

-- NOTE: The following updates would need to be done through the application's encryption service
-- to ensure proper AES-256 encryption with correct IV handling.
-- This is just a demonstration of what the encrypted data would look like.

-- Example of what encrypted data might look like (DO NOT RUN THESE UPDATES DIRECTLY):
/*
UPDATE [Barangay].[dbo].[VitalSigns] 
SET 
    Temperature = 'ENCRYPTED_VALUE_HERE',
    BloodPressure = 'ENCRYPTED_VALUE_HERE',
    HeartRate = 'ENCRYPTED_VALUE_HERE',
    RespiratoryRate = 'ENCRYPTED_VALUE_HERE',
    SpO2 = 'ENCRYPTED_VALUE_HERE',
    Weight = 'ENCRYPTED_VALUE_HERE',
    Height = 'ENCRYPTED_VALUE_HERE',
    Notes = 'ENCRYPTED_VALUE_HERE'
WHERE Id IN (1, 2, 3, 4, 5);
*/

-- To properly encrypt the data, you need to:
-- 1. Use the application's DataEncryptionService
-- 2. Navigate to https://localhost:5003/DataEncryption
-- 3. Login as Admin (admin@example.com / Admin@123)
-- 4. Click "Encrypt All Existing Data"
