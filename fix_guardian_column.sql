-- SQL Script to add the missing GuardianFirstName column
USE [Barangay]
GO

PRINT 'Adding missing GuardianFirstName column';

-- Check if GuardianFirstName column exists
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
BEGIN
    -- Add GuardianFirstName column
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [GuardianFirstName] NVARCHAR(100) NULL;
    
    PRINT 'GuardianFirstName column added';
    
    -- Copy data from FirstName to GuardianFirstName
    UPDATE [dbo].[GuardianInformation]
    SET [GuardianFirstName] = [FirstName]
    WHERE [FirstName] IS NOT NULL;
    
    PRINT 'Data copied from FirstName to GuardianFirstName';
END
ELSE
BEGIN
    PRINT 'GuardianFirstName column already exists';
END

PRINT 'Fix completed'; 