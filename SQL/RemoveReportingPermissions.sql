-- Script to remove Reporting permissions from the database

-- Get the IDs of Reporting category permissions
DECLARE @ReportingPermissionIds TABLE (Id INT);

-- Insert permission IDs for Reporting category
INSERT INTO @ReportingPermissionIds
SELECT Id FROM Permissions 
WHERE Category = 'Reports' 
   OR Category = 'Reporting'
   OR Name LIKE '%Report%';

-- Delete from UserPermissions first (foreign key constraint)
DELETE FROM UserPermissions 
WHERE PermissionId IN (SELECT Id FROM @ReportingPermissionIds);

-- Delete from StaffPermissions
DELETE FROM StaffPermissions
WHERE PermissionId IN (SELECT Id FROM @ReportingPermissionIds);

-- Delete from RolePermissions if it exists
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RolePermissions')
BEGIN
    DELETE FROM RolePermissions 
    WHERE PermissionId IN (SELECT Id FROM @ReportingPermissionIds);
END;

-- Now delete the permissions themselves
DELETE FROM Permissions 
WHERE Id IN (SELECT Id FROM @ReportingPermissionIds);

PRINT 'Reporting permissions have been removed'; 