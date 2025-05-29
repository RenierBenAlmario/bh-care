-- SQL script to fix NULL values in AspNetUsers table
-- This fixes the SqlNullValueException during user loading

USE [Barangay];

-- Set proper quoted identifier option
SET QUOTED_IDENTIFIER ON;
GO

-- Update NULL values in required string columns
UPDATE [AspNetUsers] SET [Email] = '' WHERE [Email] IS NULL;
UPDATE [AspNetUsers] SET [UserName] = '' WHERE [UserName] IS NULL;
UPDATE [AspNetUsers] SET [NormalizedEmail] = '' WHERE [NormalizedEmail] IS NULL;
UPDATE [AspNetUsers] SET [NormalizedUserName] = '' WHERE [NormalizedUserName] IS NULL;
UPDATE [AspNetUsers] SET [SecurityStamp] = NEWID() WHERE [SecurityStamp] IS NULL;
UPDATE [AspNetUsers] SET [ConcurrencyStamp] = NEWID() WHERE [ConcurrencyStamp] IS NULL;

-- Update other potentially problematic columns
UPDATE [AspNetUsers] SET [FullName] = '' WHERE [FullName] IS NULL;
UPDATE [AspNetUsers] SET [FirstName] = '' WHERE [FirstName] IS NULL;
UPDATE [AspNetUsers] SET [LastName] = '' WHERE [LastName] IS NULL;
UPDATE [AspNetUsers] SET [MiddleName] = '' WHERE [MiddleName] IS NULL;
UPDATE [AspNetUsers] SET [Suffix] = '' WHERE [Suffix] IS NULL;
UPDATE [AspNetUsers] SET [PhoneNumber] = '' WHERE [PhoneNumber] IS NULL;
UPDATE [AspNetUsers] SET [PasswordHash] = '' WHERE [PasswordHash] IS NULL;
UPDATE [AspNetUsers] SET [Status] = 'Pending' WHERE [Status] IS NULL;
UPDATE [AspNetUsers] SET [Address] = '' WHERE [Address] IS NULL;
UPDATE [AspNetUsers] SET [EncryptedFullName] = '' WHERE [EncryptedFullName] IS NULL;
UPDATE [AspNetUsers] SET [EncryptedStatus] = '' WHERE [EncryptedStatus] IS NULL;
UPDATE [AspNetUsers] SET [ProfilePicture] = '' WHERE [ProfilePicture] IS NULL;
UPDATE [AspNetUsers] SET [ProfileImage] = '' WHERE [ProfileImage] IS NULL;
UPDATE [AspNetUsers] SET [Gender] = '' WHERE [Gender] IS NULL;
UPDATE [AspNetUsers] SET [PhilHealthId] = '' WHERE [PhilHealthId] IS NULL;
UPDATE [AspNetUsers] SET [WorkingDays] = '' WHERE [WorkingDays] IS NULL;
UPDATE [AspNetUsers] SET [WorkingHours] = '' WHERE [WorkingHours] IS NULL;
UPDATE [AspNetUsers] SET [Specialization] = '' WHERE [Specialization] IS NULL;

-- Set default values for boolean fields
UPDATE [AspNetUsers] SET [EmailConfirmed] = 0 WHERE [EmailConfirmed] IS NULL;
UPDATE [AspNetUsers] SET [PhoneNumberConfirmed] = 0 WHERE [PhoneNumberConfirmed] IS NULL;
UPDATE [AspNetUsers] SET [TwoFactorEnabled] = 0 WHERE [TwoFactorEnabled] IS NULL;
UPDATE [AspNetUsers] SET [LockoutEnabled] = 0 WHERE [LockoutEnabled] IS NULL;
UPDATE [AspNetUsers] SET [AccessFailedCount] = 0 WHERE [AccessFailedCount] IS NULL;
UPDATE [AspNetUsers] SET [HasAgreedToTerms] = 0 WHERE [HasAgreedToTerms] IS NULL;
UPDATE [AspNetUsers] SET [IsActive] = 0 WHERE [IsActive] IS NULL;

-- Set default values for date fields
UPDATE [AspNetUsers] SET [CreatedAt] = GETDATE() WHERE [CreatedAt] IS NULL;
UPDATE [AspNetUsers] SET [LastActive] = GETDATE() WHERE [LastActive] IS NULL;
UPDATE [AspNetUsers] SET [AgreedAt] = GETDATE() WHERE [HasAgreedToTerms] = 1 AND [AgreedAt] IS NULL;
UPDATE [AspNetUsers] SET [JoinDate] = GETDATE() WHERE [JoinDate] IS NULL;

-- Log the completion
PRINT 'NULL values in AspNetUsers table have been fixed successfully.';
GO 