-- SQL Script to check and fix role assignment for 123@example.com
-- Created: May 22, 2025, 01:26 AM PST

-- First, check the user exists
DECLARE @UserId nvarchar(450);
SELECT @UserId = Id FROM AspNetUsers WHERE Email = '123@example.com';

-- Print user ID for verification
IF @UserId IS NULL
    PRINT 'User with email 123@example.com not found in database.'
ELSE
    PRINT 'Found user with ID: ' + @UserId;

-- Check if Admin Staff role exists
DECLARE @AdminStaffRoleId nvarchar(450);
SELECT @AdminStaffRoleId = Id FROM AspNetRoles WHERE Name = 'Admin Staff';

-- Print role ID for verification
IF @AdminStaffRoleId IS NULL
BEGIN
    PRINT 'Admin Staff role not found. Creating the role...';
    
    -- Create the Admin Staff role if it doesn't exist
    DECLARE @RoleId nvarchar(450) = NEWID();
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, 'Admin Staff', 'ADMIN STAFF', NEWID());
    
    SET @AdminStaffRoleId = @RoleId;
    PRINT 'Created Admin Staff role with ID: ' + @AdminStaffRoleId;
END
ELSE
    PRINT 'Found Admin Staff role with ID: ' + @AdminStaffRoleId;

-- Check if the user has the Admin Staff role
IF @UserId IS NOT NULL AND @AdminStaffRoleId IS NOT NULL
BEGIN
    DECLARE @HasRole bit = 0;
    
    SELECT @HasRole = 1 FROM AspNetUserRoles 
    WHERE UserId = @UserId AND RoleId = @AdminStaffRoleId;
    
    IF @HasRole = 1
        PRINT 'User already has Admin Staff role assigned.';
    ELSE
    BEGIN
        -- Assign the Admin Staff role to the user
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@UserId, @AdminStaffRoleId);
        
        PRINT 'Admin Staff role assigned to user successfully.';
    END
    
    -- Make sure user account is active and verified
    UPDATE AspNetUsers
    SET Status = 'Verified',
        EncryptedStatus = 'Active',
        IsActive = 1,
        EmailConfirmed = 1
    WHERE Id = @UserId;
    
    PRINT 'User account confirmed and activated.';
END 