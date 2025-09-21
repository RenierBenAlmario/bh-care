-- Grant permissions to access the User Dashboard for all verified users
-- First, find the permission ID for 'Access User Dashboard'
DECLARE @PermissionId INT;
DECLARE @RoleId NVARCHAR(450);

-- Get or create the 'Access User Dashboard' permission
IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'Access User Dashboard')
BEGIN
    -- Create the permission if it doesn't exist
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access User Dashboard', 'Allows access to the user dashboard', 'Dashboard Access');
    
    SELECT @PermissionId = SCOPE_IDENTITY();
    PRINT 'Created new permission: Access User Dashboard';
END
ELSE
BEGIN
    -- Get the existing permission ID
    SELECT @PermissionId = Id FROM Permissions WHERE Name = 'Access User Dashboard';
    PRINT 'Found existing permission: Access User Dashboard';
END

-- Get User role ID
SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = 'USER';

IF @RoleId IS NULL
BEGIN
    PRINT 'User role not found. Creating User role...';
    -- Generate a new GUID for the role ID
    SET @RoleId = NEWID();
    
    -- Insert the User role
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, 'User', 'USER', NEWID());
    
    PRINT 'Created User role';
END

-- Add all verified users to the User role if they're not already in it
INSERT INTO AspNetUserRoles (UserId, RoleId)
SELECT u.Id, @RoleId
FROM AspNetUsers u
WHERE u.Status = 'Verified' 
AND NOT EXISTS (
    SELECT 1 FROM AspNetUserRoles ur 
    WHERE ur.UserId = u.Id AND ur.RoleId = @RoleId
);

PRINT 'Added verified users to User role';

-- Grant the permission to all verified users
INSERT INTO UserPermissions (UserId, PermissionId, CreatedAt, UpdatedAt)
SELECT u.Id, @PermissionId, GETDATE(), GETDATE()
FROM AspNetUsers u
WHERE u.Status = 'Verified'
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = u.Id AND up.PermissionId = @PermissionId
);

PRINT 'Granted Access User Dashboard permission to all verified users';

-- Add User Dashboard access policy to the User role in the authorization system
IF NOT EXISTS (SELECT * FROM AspNetRoleClaims WHERE RoleId = @RoleId AND ClaimType = 'Permission' AND ClaimValue = 'Access User Dashboard')
BEGIN
    INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
    VALUES (@RoleId, 'Permission', 'Access User Dashboard');
    
    PRINT 'Added Permission claim to User role';
END 