-- SQL Script to fix guardian information issues
USE [Barangay]
GO

PRINT 'Fixing GuardianInformation table issues';

-- Check if GuardianInformation table exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    PRINT 'GuardianInformation table does not exist, nothing to fix';
    RETURN;
END

-- Add missing primary key if needed
DECLARE @HasPK BIT = 0;
SELECT @HasPK = 1 FROM sys.indexes 
WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') 
AND is_primary_key = 1;

IF @HasPK = 0
BEGIN
    PRINT 'Adding primary key to GuardianInformation table';
    
    -- Check if GuardianId exists
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianId')
    BEGIN
        -- Add primary key to existing GuardianId
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD CONSTRAINT PK_GuardianInformation PRIMARY KEY (GuardianId);
        PRINT 'Added primary key constraint to existing GuardianId column';
    END
    ELSE
    BEGIN
        -- Add GuardianId column with primary key
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [GuardianId] INT IDENTITY(1,1) PRIMARY KEY;
        PRINT 'Added new GuardianId column as primary key';
    END
END

-- Check if we have both sets of columns (FirstName/LastName and GuardianFirstName/GuardianLastName)
DECLARE @HasFirstName BIT = 0;
DECLARE @HasLastName BIT = 0;
DECLARE @HasGuardianFirstName BIT = 0;
DECLARE @HasGuardianLastName BIT = 0;

SELECT @HasFirstName = 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'FirstName';
SELECT @HasLastName = 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'LastName';
SELECT @HasGuardianFirstName = 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName';
SELECT @HasGuardianLastName = 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName';

PRINT 'Column status: FirstName=' + CAST(@HasFirstName AS NVARCHAR) + 
      ', LastName=' + CAST(@HasLastName AS NVARCHAR) + 
      ', GuardianFirstName=' + CAST(@HasGuardianFirstName AS NVARCHAR) + 
      ', GuardianLastName=' + CAST(@HasGuardianLastName AS NVARCHAR);

-- Add missing columns if needed
IF @HasGuardianFirstName = 0
BEGIN
    PRINT 'Adding GuardianFirstName column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [GuardianFirstName] NVARCHAR(100) NULL;
END

IF @HasGuardianLastName = 0
BEGIN
    PRINT 'Adding GuardianLastName column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [GuardianLastName] NVARCHAR(100) NULL;
END

-- Copy data from old columns to new ones if needed
IF @HasFirstName = 1 AND @HasGuardianFirstName = 1
BEGIN
    PRINT 'Syncing data between FirstName and GuardianFirstName';
    
    -- Update GuardianFirstName from FirstName where GuardianFirstName is null
    UPDATE [dbo].[GuardianInformation]
    SET [GuardianFirstName] = [FirstName]
    WHERE [GuardianFirstName] IS NULL AND [FirstName] IS NOT NULL;
    
    -- Update FirstName from GuardianFirstName where FirstName is null
    UPDATE [dbo].[GuardianInformation]
    SET [FirstName] = [GuardianFirstName]
    WHERE [FirstName] IS NULL AND [GuardianFirstName] IS NOT NULL;
END

IF @HasLastName = 1 AND @HasGuardianLastName = 1
BEGIN
    PRINT 'Syncing data between LastName and GuardianLastName';
    
    -- Update GuardianLastName from LastName where GuardianLastName is null
    UPDATE [dbo].[GuardianInformation]
    SET [GuardianLastName] = [LastName]
    WHERE [GuardianLastName] IS NULL AND [LastName] IS NOT NULL;
    
    -- Update LastName from GuardianLastName where LastName is null
    UPDATE [dbo].[GuardianInformation]
    SET [LastName] = [GuardianLastName]
    WHERE [LastName] IS NULL AND [GuardianLastName] IS NOT NULL;
END

-- Ensure that the UserId column exists and has a foreign key constraint to ApplicationUsers
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'UserId')
BEGIN
    -- Check if there's a foreign key constraint
    DECLARE @HasFK BIT = 0;
    SELECT @HasFK = 1 FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') 
    AND referenced_object_id = OBJECT_ID(N'[dbo].[AspNetUsers]');
    
    IF @HasFK = 0
    BEGIN
        PRINT 'Adding foreign key constraint to UserId column';
        
        -- First make sure all UserId values exist in AspNetUsers
        DELETE FROM [dbo].[GuardianInformation]
        WHERE [UserId] NOT IN (SELECT [Id] FROM [dbo].[AspNetUsers]);
        
        -- Add foreign key constraint
        ALTER TABLE [dbo].[GuardianInformation]
        ADD CONSTRAINT FK_GuardianInformation_AspNetUsers FOREIGN KEY (UserId)
        REFERENCES [dbo].[AspNetUsers] (Id) ON DELETE CASCADE;
        
        PRINT 'Added foreign key constraint to UserId column';
    END
END

-- Add other required columns if missing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProofPath')
BEGIN
    PRINT 'Adding ResidencyProofPath column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [ResidencyProofPath] NVARCHAR(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProof')
BEGIN
    PRINT 'Adding ResidencyProof column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [ResidencyProof] VARBINARY(MAX) NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ProofType')
BEGIN
    PRINT 'Adding ProofType column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [ProofType] NVARCHAR(50) DEFAULT 'GuardianResidencyProof';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ConsentStatus')
BEGIN
    PRINT 'Adding ConsentStatus column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [ConsentStatus] NVARCHAR(50) DEFAULT 'Pending';
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'CreatedAt')
BEGIN
    PRINT 'Adding CreatedAt column';
    ALTER TABLE [dbo].[GuardianInformation]
    ADD [CreatedAt] DATETIME DEFAULT GETDATE();
END

-- Update any NULL values for required fields
UPDATE [dbo].[GuardianInformation]
SET 
    [GuardianFirstName] = COALESCE([GuardianFirstName], [FirstName], 'Unknown'),
    [GuardianLastName] = COALESCE([GuardianLastName], [LastName], 'Unknown'),
    [ConsentStatus] = COALESCE([ConsentStatus], 'Pending'),
    [ProofType] = COALESCE([ProofType], 'GuardianResidencyProof'),
    [CreatedAt] = COALESCE([CreatedAt], GETDATE())
WHERE 
    [GuardianFirstName] IS NULL OR
    [GuardianLastName] IS NULL OR
    [ConsentStatus] IS NULL OR
    [ProofType] IS NULL OR
    [CreatedAt] IS NULL;

PRINT 'GuardianInformation table fixes completed.'; 