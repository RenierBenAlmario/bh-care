-- SQL Script to make Department column nullable in StaffMembers table
ALTER TABLE StaffMembers
ALTER COLUMN Department NVARCHAR(4000) NULL;

-- Add a default value for existing records with NULL Department
UPDATE StaffMembers
SET Department = 'General'
WHERE Department IS NULL;

PRINT 'Department column in StaffMembers table has been modified to allow NULL values'; 