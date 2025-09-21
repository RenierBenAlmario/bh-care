USE [Barangay]
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Starting permission assignment for doctor...';

-- Begin transaction for safety
BEGIN TRANSACTION;

-- Find the doctor staff member
DECLARE @staffId INT;
SELECT @staffId = Id FROM StaffMembers WHERE Name = 'doctor';
PRINT 'Doctor staff ID: ' + CAST(ISNULL(@staffId, -1) AS NVARCHAR);

-- If staff member exists, proceed with permission assignment
IF @staffId IS NOT NULL
BEGIN
    -- Clear any existing permissions
    DELETE FROM StaffPermissions WHERE StaffMemberId = @staffId;
    PRINT 'Cleared existing permissions for doctor';

    -- Get all permission IDs
    DECLARE @permissionTable TABLE (Id INT);
    INSERT INTO @permissionTable
    SELECT Id FROM Permissions;

    -- Add all permissions
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
    SELECT @staffId, Id, GETUTCDATE() FROM @permissionTable;

    DECLARE @permissionCount INT;
    SET @permissionCount = @@ROWCOUNT;
    PRINT 'Added ' + CAST(@permissionCount AS NVARCHAR) + ' permissions for doctor';

    -- Force the permission cache to refresh by updating the staff member's user
    DECLARE @userId NVARCHAR(450);
    SELECT @userId = UserId FROM StaffMembers WHERE Id = @staffId;
    
    IF @userId IS NOT NULL
    BEGIN
        UPDATE AspNetUsers
        SET UpdatedAt = GETUTCDATE()
        WHERE Id = @userId;
        PRINT 'Updated doctor user timestamp to clear permission cache';
    END
    ELSE
    BEGIN
        PRINT 'Warning: Could not find user ID for staff member';
    END

    COMMIT TRANSACTION;
    PRINT 'Successfully assigned permissions to doctor';
END
ELSE
BEGIN
    ROLLBACK TRANSACTION;
    PRINT 'Error: Doctor staff member not found';
END

-- Show assigned permissions
PRINT 'Permissions assigned to doctor:';
SELECT p.Name
FROM StaffPermissions sp
JOIN Permissions p ON sp.PermissionId = p.Id
JOIN StaffMembers sm ON sp.StaffMemberId = sm.Id
WHERE sm.Name = 'doctor'
ORDER BY p.Name; 