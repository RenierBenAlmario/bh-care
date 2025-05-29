-- Script to automatically approve existing admin staff users
-- Created: May 22, 2025
-- Author: System Administrator

-- First, identify the Admin Staff role ID
DECLARE @AdminStaffRoleId NVARCHAR(450);

SELECT @AdminStaffRoleId = Id
FROM AspNetRoles
WHERE Name = 'Admin Staff';

-- Get list of users with Admin Staff role
WITH AdminStaffUsers AS (
    SELECT UserId
    FROM AspNetUserRoles
    WHERE RoleId = @AdminStaffRoleId
)

-- Update status for all Admin Staff users
UPDATE AspNetUsers
SET Status = 'Verified',
    EncryptedStatus = 'Active',
    IsActive = 1
WHERE Id IN (SELECT UserId FROM AdminStaffUsers)
  AND (Status != 'Verified' OR IsActive = 0);

-- Update the specific user mentioned in the issue
UPDATE AspNetUsers
SET Status = 'Verified',
    EncryptedStatus = 'Active',
    IsActive = 1
WHERE Email = 'kc12345@email.com';

-- Print success message
PRINT 'Successfully updated admin staff users to Verified status.';
PRINT 'This includes the user with email: kc12345@email.com'; 