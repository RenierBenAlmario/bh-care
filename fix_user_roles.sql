-- Set proper SQL options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Get the user ID for test123@email.com
DECLARE @userId NVARCHAR(450);
SELECT @userId = Id FROM AspNetUsers WHERE Email = 'test123@email.com';

-- Get the role ID for 'User' role
DECLARE @roleId NVARCHAR(450);
SELECT @roleId = Id FROM AspNetRoles WHERE Name = 'User';

-- Check if role exists, if not create it
IF @roleId IS NULL
BEGIN
    -- Create a new ID for the role
    SET @roleId = NEWID();
    
    -- Insert the role
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@roleId, 'User', 'USER', NEWID());
    
    PRINT 'Created User role with ID: ' + @roleId;
END

-- Check if user already has this role
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @userId AND RoleId = @roleId)
BEGIN
    -- Add the user to the role
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@userId, @roleId);
    
    PRINT 'Added user test123@email.com to User role';
END
ELSE
BEGIN
    PRINT 'User test123@email.com already has User role';
END

-- Verify the roles assigned to the user
SELECT u.Email, r.Name AS RoleName
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email = 'test123@email.com'; 