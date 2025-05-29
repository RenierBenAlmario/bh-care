-- Update Admin Account Status
-- This script updates the status of admin accounts to "Approved"

-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Starting admin account status update...'

-- 1. Update admin account status
UPDATE AspNetUsers
SET Status = 'Approved'
WHERE Email = 'admin@example.com';
PRINT 'Updated status for admin@example.com to Approved'

-- 2. Update all accounts with NULL status
UPDATE AspNetUsers
SET Status = 'Approved'
WHERE Status IS NULL;
PRINT 'Updated all accounts with NULL status to Approved'

-- 3. Add default constraint for Status column
IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_AspNetUsers_Status')
BEGIN
    ALTER TABLE AspNetUsers
    ADD CONSTRAINT DF_AspNetUsers_Status
    DEFAULT 'Pending' FOR Status;
    PRINT 'Added default constraint: Status will default to Pending for new accounts'
END

-- 4. Verify the updates
SELECT 
    Email, 
    Status,
    UserName
FROM 
    AspNetUsers
WHERE 
    Email = 'admin@example.com' OR Status IS NULL;

PRINT 'Admin account status update completed!' 