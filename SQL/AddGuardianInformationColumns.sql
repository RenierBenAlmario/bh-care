-- Add missing columns to GuardianInformation table
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    -- Check if table has a primary key
    DECLARE @HasPK BIT = 0;
    SELECT @HasPK = 1 FROM sys.indexes 
    WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') 
    AND is_primary_key = 1;

    -- If GuardianId column doesn't exist and no primary key
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianId') 
        AND @HasPK = 0
    BEGIN
        -- Add GuardianId as identity and primary key
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [GuardianId] INT IDENTITY(1,1) PRIMARY KEY
        PRINT 'Added GuardianId column to GuardianInformation table'
    END
    -- If GuardianId exists but is not primary key
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianId') 
        AND @HasPK = 0
    BEGIN
        -- Add primary key constraint on existing column
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD CONSTRAINT PK_GuardianInformation PRIMARY KEY (GuardianId)
        PRINT 'Added primary key constraint to GuardianId column'
    END
    -- If GuardianId doesn't exist but there is a primary key on Id
    ELSE IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianId') 
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'Id')
    BEGIN
        -- Rename Id column to GuardianId
        EXEC sp_rename 'GuardianInformation.Id', 'GuardianId', 'COLUMN';
        PRINT 'Renamed Id column to GuardianId in GuardianInformation table'
    END
END
ELSE
BEGIN
    PRINT 'GuardianInformation table does not exist'
END
GO -- Separate batch for column additions and updates

-- Add GuardianFirstName column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [GuardianFirstName] NVARCHAR(100) NULL
        PRINT 'Added GuardianFirstName column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Copy FirstName values to GuardianFirstName
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'FirstName')
       AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
    BEGIN
        UPDATE [dbo].[GuardianInformation]
        SET GuardianFirstName = FirstName
        WHERE GuardianFirstName IS NULL AND FirstName IS NOT NULL
        PRINT 'Copied values from FirstName to GuardianFirstName'
    END
END
GO -- Batch separator

-- Add GuardianLastName column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [GuardianLastName] NVARCHAR(100) NULL
        PRINT 'Added GuardianLastName column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Copy LastName values to GuardianLastName
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'LastName')
       AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName')
    BEGIN
        UPDATE [dbo].[GuardianInformation]
        SET GuardianLastName = LastName
        WHERE GuardianLastName IS NULL AND LastName IS NOT NULL
        PRINT 'Copied values from LastName to GuardianLastName'
    END
END
GO -- Batch separator

-- Add ResidencyProof column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProof')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProof] VARBINARY(MAX) NULL
        PRINT 'Added ResidencyProof column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Add ResidencyProofPath column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProofPath')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProofPath] NVARCHAR(MAX) NULL
        PRINT 'Added ResidencyProofPath column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Add ConsentStatus column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ConsentStatus')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [ConsentStatus] NVARCHAR(50) NULL DEFAULT ('Pending')
        PRINT 'Added ConsentStatus column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Add ProofType column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ProofType')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [ProofType] NVARCHAR(50) NULL DEFAULT ('GuardianResidencyProof')
        PRINT 'Added ProofType column to GuardianInformation table'
    END
END
GO -- Batch separator

-- Add CreatedAt column
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
        PRINT 'Added CreatedAt column to GuardianInformation table'
    END
END 