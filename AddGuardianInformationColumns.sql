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

    -- Check if GuardianFirstName column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianFirstName')
    BEGIN
        -- If FirstName exists, add GuardianFirstName and copy values
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'FirstName')
        BEGIN
            -- Add new column
            ALTER TABLE [dbo].[GuardianInformation]
            ADD [GuardianFirstName] NVARCHAR(100) NULL
            PRINT 'Added GuardianFirstName column to GuardianInformation table'
            
            -- Copy values from FirstName
            UPDATE [dbo].[GuardianInformation]
            SET GuardianFirstName = FirstName
            PRINT 'Copied values from FirstName to GuardianFirstName'
        END
        ELSE
        BEGIN
            -- Just add the column
            ALTER TABLE [dbo].[GuardianInformation]
            ADD [GuardianFirstName] NVARCHAR(100) NULL
            PRINT 'Added GuardianFirstName column to GuardianInformation table'
        END
    END

    -- Check if GuardianLastName column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'GuardianLastName')
    BEGIN
        -- If LastName exists, add GuardianLastName and copy values
        IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'LastName')
        BEGIN
            -- Add new column
            ALTER TABLE [dbo].[GuardianInformation]
            ADD [GuardianLastName] NVARCHAR(100) NULL
            PRINT 'Added GuardianLastName column to GuardianInformation table'
            
            -- Copy values from LastName
            UPDATE [dbo].[GuardianInformation]
            SET GuardianLastName = LastName
            PRINT 'Copied values from LastName to GuardianLastName'
        END
        ELSE
        BEGIN
            -- Just add the column
            ALTER TABLE [dbo].[GuardianInformation]
            ADD [GuardianLastName] NVARCHAR(100) NULL
            PRINT 'Added GuardianLastName column to GuardianInformation table'
        END
    END

    -- Check if ResidencyProof column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProof')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProof] VARBINARY(MAX) NULL
        PRINT 'Added ResidencyProof column to GuardianInformation table'
    END

    -- Check if ResidencyProofPath column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ResidencyProofPath')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [ResidencyProofPath] NVARCHAR(MAX) NULL
        PRINT 'Added ResidencyProofPath column to GuardianInformation table'
    END

    -- Check if ConsentStatus column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ConsentStatus')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [ConsentStatus] NVARCHAR(50) NULL DEFAULT ('Pending')
        PRINT 'Added ConsentStatus column to GuardianInformation table'
    END

    -- Check if ProofType column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'ProofType')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation] 
        ADD [ProofType] NVARCHAR(50) NULL DEFAULT ('GuardianResidencyProof')
        PRINT 'Added ProofType column to GuardianInformation table'
    END

    -- Check if CreatedAt column doesn't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[GuardianInformation]') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE [dbo].[GuardianInformation]
        ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
        PRINT 'Added CreatedAt column to GuardianInformation table'
    END
END
ELSE
BEGIN
    PRINT 'GuardianInformation table does not exist'
END 