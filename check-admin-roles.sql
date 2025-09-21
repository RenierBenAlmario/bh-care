-- Check if the user exists and what roles they have
DECLARE @UserEmail NVARCHAR(256) = 'admin@bhcare.local';

-- Find the user ID for the given email
DECLARE @UserId NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = @UserEmail);

IF @UserId IS NOT NULL
BEGIN
    -- Check the roles assigned to the user
    SELECT r.Name
    FROM AspNetUserRoles ur
    JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE ur.UserId = @UserId;
END
ELSE
BEGIN
    -- User not found
    SELECT 'User with email ' + @UserEmail + ' not found.' AS Message;
END