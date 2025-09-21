-- SQL Script to remove System Administrator and blank entries from patient list
USE [Barangay]
GO

PRINT 'Starting cleanup of patient list...';

-- 1. First, identify the users to remove
DECLARE @UsersToRemove TABLE (UserId NVARCHAR(450));

-- Add System Administrator to the removal list
INSERT INTO @UsersToRemove (UserId)
SELECT Id FROM AspNetUsers
WHERE Name = 'System Administrator' OR FullName = 'System Administrator';

-- Add blank or missing name entries to the removal list 
INSERT INTO @UsersToRemove (UserId)
SELECT Id FROM AspNetUsers
WHERE Name IS NULL OR Name = '' OR FullName IS NULL OR FullName = '';

-- Add staff members (doctors, nurses, etc.) to the removal list
INSERT INTO @UsersToRemove (UserId)
SELECT u.Id 
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name IN ('Admin', 'Doctor', 'Nurse', 'System Administrator')
AND u.Id NOT IN (SELECT PatientId FROM MedicalRecords); -- But keep staff who are also patients

-- 2. Show users that will be removed
PRINT 'The following users will be removed from patient list:';
SELECT u.Id, u.Name, u.FullName, u.Email, u.Status
FROM AspNetUsers u
WHERE u.Id IN (SELECT UserId FROM @UsersToRemove);

-- 3. Remove records from the Patients table for these users
DELETE FROM Patients
WHERE UserId IN (SELECT UserId FROM @UsersToRemove);

PRINT 'Removed patient records for non-patient users.';

-- 4. If needed, we can also keep the users but remove their patient status
-- This is an alternative approach if deleting the users would cause issues
UPDATE AspNetUsers
SET UserType = 'Staff' 
WHERE Id IN (SELECT UserId FROM @UsersToRemove)
AND UserType = 'Patient';

PRINT 'Updated user types for non-patient users.';

-- 5. Verify the remaining patients
PRINT 'Remaining patients after cleanup:';
SELECT u.Id, u.Name, u.FullName, u.Email, u.Status
FROM AspNetUsers u
JOIN Patients p ON u.Id = p.UserId
ORDER BY u.FullName; 