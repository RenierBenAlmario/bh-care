-- Fix Login Status Script
-- This script updates both Status and EncryptedStatus fields for all users

-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Starting login status fix process...'

-- 1. Update admin account status fields
UPDATE AspNetUsers
SET Status = 'Approved', EncryptedStatus = 'Active'
WHERE Email = 'admin@example.com';
PRINT 'Updated status for admin@example.com to Approved/Active'

-- 2. Update all accounts with NULL status
UPDATE AspNetUsers
SET Status = 'Approved', EncryptedStatus = 'Active'
WHERE Status IS NULL OR EncryptedStatus IS NULL OR EncryptedStatus = '';
PRINT 'Updated all accounts with NULL status to Approved/Active'

-- 3. Verify the updates
SELECT 
    Email, 
    Status,
    EncryptedStatus,
    UserName
FROM 
    AspNetUsers
WHERE 
    Email = 'admin@example.com' OR Status IS NULL OR EncryptedStatus IS NULL OR EncryptedStatus = '';

PRINT 'Login status fix completed!' 