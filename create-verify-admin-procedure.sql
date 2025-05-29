-- Fix QUOTED_IDENTIFIER issue
SET QUOTED_IDENTIFIER ON;
GO

-- Drop the procedure if it already exists
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'VerifyAdminUsers')
BEGIN
    DROP PROCEDURE VerifyAdminUsers
    PRINT 'Dropped existing VerifyAdminUsers procedure'
END
GO

-- Create Verify Admin Users Stored Procedure
-- This script creates a stored procedure that ensures all admin users are set to "Verified" status
CREATE PROCEDURE VerifyAdminUsers
AS
BEGIN
    SET NOCOUNT ON;

    PRINT 'Starting admin user verification process...'

    -- Get the Admin role ID
    DECLARE @AdminRoleId NVARCHAR(450)
    SELECT @AdminRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN'

    IF @AdminRoleId IS NOT NULL
    BEGIN
        -- Update all admin users to have Verified status and IsActive=1
        UPDATE AspNetUsers
        SET Status = 'Verified', IsActive = 1
        FROM AspNetUsers u
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        WHERE ur.RoleId = @AdminRoleId

        PRINT 'Admin users updated to have Verified status and IsActive=1'
        
        -- Return updated admin users
        SELECT u.Id, u.UserName, u.Email, u.Status, u.IsActive
        FROM AspNetUsers u
        INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
        WHERE ur.RoleId = @AdminRoleId
    END
    ELSE
    BEGIN
        PRINT 'Admin role not found'
    END

    PRINT 'Admin user verification process completed'
END
GO

PRINT 'Created VerifyAdminUsers stored procedure'
GO

-- Execute the procedure to verify current admin users
EXEC VerifyAdminUsers
GO 