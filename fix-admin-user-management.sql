-- SQL Fix Script for "Invalid object name 'Users'" error
USE [Barangay];

-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Check if AspNetUsers exists
PRINT 'Checking if AspNetUsers table exists...';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
BEGIN
    PRINT 'AspNetUsers table exists.';
END
ELSE
BEGIN
    PRINT 'ERROR: AspNetUsers table does not exist!';
    RETURN;
END

-- Create Users view as a compatibility layer to avoid the error
PRINT 'Creating or updating Users view...';
IF EXISTS (SELECT * FROM sys.views WHERE name = 'Users')
BEGIN
    PRINT 'Users view already exists, dropping it to recreate...';
    DROP VIEW [dbo].[Users];
END
GO

-- Create the Users view that maps to AspNetUsers
CREATE VIEW [dbo].[Users] AS
SELECT 
    [Id],
    [UserName],
    [NormalizedUserName],
    [Email],
    [NormalizedEmail],
    [EmailConfirmed],
    [PasswordHash],
    [SecurityStamp],
    [ConcurrencyStamp],
    [PhoneNumber],
    [PhoneNumberConfirmed],
    [TwoFactorEnabled],
    [LockoutEnd],
    [LockoutEnabled],
    [AccessFailedCount],
    [FirstName],
    [LastName],
    [MiddleName],
    [Suffix],
    [FullName],
    [Status],
    [CreatedAt],
    [ProfilePicture],
    [ProfileImage]
FROM [dbo].[AspNetUsers];
GO

PRINT 'Users view created successfully.';

-- Create or update any other views needed for compatibility
PRINT 'Checking for other related tables...';

-- Check for UserDocuments permissions
PRINT 'Ensuring UserDocuments table has proper permissions...';
GRANT SELECT, INSERT, UPDATE, DELETE ON [dbo].[UserDocuments] TO PUBLIC;
GO

-- Fix any null values in AspNetUsers table
PRINT 'Fixing NULL values in AspNetUsers table...';

-- Set proper options again for safety
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Fix UserName
UPDATE [dbo].[AspNetUsers] 
SET [UserName] = '' 
WHERE [UserName] IS NULL;

-- Fix Email
UPDATE [dbo].[AspNetUsers] 
SET [Email] = '' 
WHERE [Email] IS NULL;

-- Fix NormalizedUserName
UPDATE [dbo].[AspNetUsers] 
SET [NormalizedUserName] = '' 
WHERE [NormalizedUserName] IS NULL;

-- Fix NormalizedEmail
UPDATE [dbo].[AspNetUsers] 
SET [NormalizedEmail] = '' 
WHERE [NormalizedEmail] IS NULL;

-- Fix FirstName
UPDATE [dbo].[AspNetUsers] 
SET [FirstName] = '' 
WHERE [FirstName] IS NULL;

-- Fix LastName
UPDATE [dbo].[AspNetUsers] 
SET [LastName] = '' 
WHERE [LastName] IS NULL;

-- Fix MiddleName
UPDATE [dbo].[AspNetUsers] 
SET [MiddleName] = '' 
WHERE [MiddleName] IS NULL;

-- Fix Suffix
UPDATE [dbo].[AspNetUsers] 
SET [Suffix] = '' 
WHERE [Suffix] IS NULL;

-- Fix FullName
UPDATE [dbo].[AspNetUsers] 
SET [FullName] = '' 
WHERE [FullName] IS NULL;

-- Fix Status
UPDATE [dbo].[AspNetUsers] 
SET [Status] = 'Unknown' 
WHERE [Status] IS NULL;
GO

PRINT 'SQL Fix Script completed successfully.';
GO 