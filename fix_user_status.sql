-- Set proper SQL options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Get the user ID for test123@email.com
DECLARE @userId NVARCHAR(450);
SELECT @userId = Id FROM AspNetUsers WHERE Email = 'test123@email.com';

-- Update user status to Verified
UPDATE AspNetUsers
SET 
    Status = 'Verified',
    EmailConfirmed = 1,
    IsActive = 1
WHERE Id = @userId;

PRINT 'Updated user test123@email.com status to Verified';

-- Verify the user status
SELECT Email, UserName, Status, IsActive, EmailConfirmed
FROM AspNetUsers
WHERE Email = 'test123@email.com'; 