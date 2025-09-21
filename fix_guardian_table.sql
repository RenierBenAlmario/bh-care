-- SQL Script to completely fix the GuardianInformation table
USE [Barangay]
GO

PRINT 'Starting complete GuardianInformation table fix';

-- Check if GuardianInformation table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    -- Add GuardianFirstName column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [GuardianFirstName] NVARCHAR(100) NULL;
        PRINT 'Added GuardianFirstName column';
    END

    -- Add GuardianLastName column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [GuardianLastName] NVARCHAR(100) NULL;
        PRINT 'Added GuardianLastName column';
    END

    -- Add ResidencyProofPath column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProofPath')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProofPath] NVARCHAR(MAX) NULL;
        PRINT 'Added ResidencyProofPath column';
    END

    -- Add ResidencyProof column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProof')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProof] VARBINARY(MAX) NULL;
        PRINT 'Added ResidencyProof column';
    END

    -- Add ProofType column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ProofType')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ProofType] NVARCHAR(50) DEFAULT 'GuardianResidencyProof';
        PRINT 'Added ProofType column';
    END

    -- Add ConsentStatus column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ConsentStatus')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ConsentStatus] NVARCHAR(50) DEFAULT 'Pending';
        PRINT 'Added ConsentStatus column';
    END

    -- Add CreatedAt column if it doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [CreatedAt] DATETIME DEFAULT GETDATE();
        PRINT 'Added CreatedAt column';
    END

    -- Copy data from FirstName to GuardianFirstName if both columns exist
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'FirstName')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
    BEGIN
        PRINT 'Copying data from FirstName to GuardianFirstName';
        UPDATE [dbo].[GuardianInformation]
        SET [GuardianFirstName] = [FirstName]
        WHERE [GuardianFirstName] IS NULL AND [FirstName] IS NOT NULL;
        PRINT 'Data copied from FirstName to GuardianFirstName';
    END

    -- Copy data from LastName to GuardianLastName if both columns exist
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'LastName')
    AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName')
    BEGIN
        PRINT 'Copying data from LastName to GuardianLastName';
        UPDATE [dbo].[GuardianInformation]
        SET [GuardianLastName] = [LastName]
        WHERE [GuardianLastName] IS NULL AND [LastName] IS NOT NULL;
        PRINT 'Data copied from LastName to GuardianLastName';
    END

    PRINT 'GuardianInformation table fix completed';
END
ELSE
BEGIN
    PRINT 'GuardianInformation table does not exist';
END 