-- SQL script to fix critical fields in AspNetUsers table
-- This addresses the SqlNullValueException during sign-in
USE [Barangay];

-- Set proper quoted identifier option
SET QUOTED_IDENTIFIER ON;
GO

-- Update NULL values in critical fields required for sign-in
UPDATE [AspNetUsers] 
SET 
    [UserName] = [Email] 
WHERE 
    [UserName] IS NULL AND [Email] IS NOT NULL;
GO

UPDATE [AspNetUsers] 
SET 
    [NormalizedUserName] = UPPER([UserName]) 
WHERE 
    [NormalizedUserName] IS NULL AND [UserName] IS NOT NULL;
GO

UPDATE [AspNetUsers] 
SET 
    [NormalizedEmail] = UPPER([Email]) 
WHERE 
    [NormalizedEmail] IS NULL AND [Email] IS NOT NULL;
GO

-- For any users where Email and UserName are both null, set dummy values using their ID
UPDATE [AspNetUsers] 
SET 
    [Email] = CONCAT('user_', [Id], '@example.com'),
    [UserName] = CONCAT('user_', [Id])
WHERE 
    [Email] IS NULL OR [UserName] IS NULL;
GO

UPDATE [AspNetUsers] 
SET 
    [NormalizedEmail] = UPPER([Email]),
    [NormalizedUserName] = UPPER([UserName])
WHERE 
    [NormalizedEmail] IS NULL OR [NormalizedUserName] IS NULL;
GO

-- Ensure PasswordHash isn't NULL for any user
UPDATE [AspNetUsers]
SET
    [PasswordHash] = '$2a$11$aPNTKTAc2nYBVwhT.wnbJewz.YAXxlwT5CBDvkwpnPO4RVmdYg4w.' -- Hash for 'Password123!'
WHERE
    [PasswordHash] IS NULL;
GO

-- Ensure SecurityStamp and ConcurrencyStamp are not NULL
UPDATE [AspNetUsers]
SET
    [SecurityStamp] = NEWID()
WHERE
    [SecurityStamp] IS NULL;
GO

UPDATE [AspNetUsers]
SET
    [ConcurrencyStamp] = NEWID()
WHERE
    [ConcurrencyStamp] IS NULL;
GO

-- Set default values for EmailConfirmed and LockoutEnabled
UPDATE [AspNetUsers]
SET
    [EmailConfirmed] = 1
WHERE
    [EmailConfirmed] IS NULL;
GO

UPDATE [AspNetUsers]
SET
    [LockoutEnabled] = 1
WHERE
    [LockoutEnabled] IS NULL;
GO

PRINT 'AspNetUsers critical fields have been fixed.';
GO 