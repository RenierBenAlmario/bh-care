-- SQL script to delete the specific medical record entry
-- Date: 9/16/2025, Doctor: dicktor, Diagnosis: dsa, Treatment: dasd

USE [Barangay]
GO

-- Begin Transaction for safety
BEGIN TRANSACTION;

-- First, let's see what records match our criteria
SELECT Id, Date, PatientId, DoctorId, Diagnosis, Treatment, CreatedAt
FROM MedicalRecords 
WHERE CAST(Date AS DATE) = '2025-09-16'
  AND Diagnosis LIKE '%dsa%'
  AND Treatment LIKE '%dasd%';

-- Delete the specific medical record
DELETE FROM MedicalRecords 
WHERE CAST(Date AS DATE) = '2025-09-16'
  AND Diagnosis LIKE '%dsa%'
  AND Treatment LIKE '%dasd%';

-- Check how many records were affected
DECLARE @RowsAffected INT = @@ROWCOUNT;
PRINT 'Deleted ' + CAST(@RowsAffected AS NVARCHAR) + ' medical record(s).';

-- Commit the transaction
COMMIT TRANSACTION;

PRINT 'Medical record deletion completed successfully.';
GO



