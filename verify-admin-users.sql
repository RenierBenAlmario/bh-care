-- Verify Admin Users SQL Script
-- This script ensures all admin users are set to "Verified" status and are active

-- Fix QUOTED_IDENTIFIER issue
SET QUOTED_IDENTIFIER ON;

PRINT 'Starting admin user verification process...'

-- First, get all users in the Admin role
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
    
    -- Print updated admin users
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