-- SQL script to fix NULL string values in AspNetUsers table
-- This targets specific columns that are causing SqlNullValueException in GetString()

USE [Barangay];

-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

-- Identify all string columns in AspNetUsers table that could be NULL
DECLARE @sql NVARCHAR(MAX) = '';

SELECT @sql = @sql + 'UPDATE [AspNetUsers] SET [' + COLUMN_NAME + '] = '''' WHERE [' + COLUMN_NAME + '] IS NULL;' + CHAR(13)
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AspNetUsers'
  AND DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar', 'text', 'ntext');

-- Print generated statements for logging
PRINT 'Executing the following SQL:';
PRINT @sql;

-- Execute the dynamic SQL
EXEC sp_executesql @sql;

-- Additional manual fixes for specific known columns that might be causing issues
UPDATE [dbo].[AspNetUsers] SET [UserName] = '' WHERE [UserName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Email] = '' WHERE [Email] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [FullName] = '' WHERE [FullName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [FirstName] = '' WHERE [FirstName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [LastName] = '' WHERE [LastName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [MiddleName] = '' WHERE [MiddleName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Suffix] = '' WHERE [Suffix] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [PhoneNumber] = '' WHERE [PhoneNumber] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Status] = 'Pending' WHERE [Status] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [NormalizedEmail] = '' WHERE [NormalizedEmail] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [NormalizedUserName] = '' WHERE [NormalizedUserName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Specialization] = '' WHERE [Specialization] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [WorkingDays] = '' WHERE [WorkingDays] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [WorkingHours] = '' WHERE [WorkingHours] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [ProfilePicture] = '' WHERE [ProfilePicture] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [ProfileImage] = '' WHERE [ProfileImage] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Address] = '' WHERE [Address] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [PasswordHash] = '' WHERE [PasswordHash] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [ConcurrencyStamp] = NEWID() WHERE [ConcurrencyStamp] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [SecurityStamp] = NEWID() WHERE [SecurityStamp] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [EncryptedFullName] = '' WHERE [EncryptedFullName] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [EncryptedStatus] = '' WHERE [EncryptedStatus] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [PhilHealthId] = '' WHERE [PhilHealthId] IS NULL;
UPDATE [dbo].[AspNetUsers] SET [Gender] = '' WHERE [Gender] IS NULL;

-- Print completion message
PRINT 'NULL string values in AspNetUsers table have been fixed.';
GO 