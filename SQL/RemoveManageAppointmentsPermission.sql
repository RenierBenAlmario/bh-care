-- SQL script to remove the ManageAppointments permission
-- First, get the ID of the ManageAppointments permission
DECLARE @ManageAppointmentsPermissionId INT;

SELECT @ManageAppointmentsPermissionId = Id
FROM Permissions
WHERE Name = 'ManageAppointments';

-- If the permission exists, remove it from UserPermissions first (foreign key constraint)
IF @ManageAppointmentsPermissionId IS NOT NULL
BEGIN
    -- Delete from UserPermissions
    DELETE FROM UserPermissions 
    WHERE PermissionId = @ManageAppointmentsPermissionId;
    
    -- Delete from RolePermissions if it exists
    IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RolePermissions')
    BEGIN
        DELETE FROM RolePermissions 
        WHERE PermissionId = @ManageAppointmentsPermissionId;
    END;
    
    -- Now delete the permission itself
    DELETE FROM Permissions 
    WHERE Id = @ManageAppointmentsPermissionId;
    
    PRINT 'ManageAppointments permission has been removed';
END
ELSE
BEGIN
    PRINT 'ManageAppointments permission not found';
END 