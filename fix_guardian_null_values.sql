-- SQL Script to fix null values in GuardianInformation table
USE [Barangay]
GO

PRINT 'Starting GuardianInformation null values fix';

-- Check if table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    PRINT 'GuardianInformation table found, updating null values';
    
    -- Update NULL values in GuardianFirstName column
    UPDATE [dbo].[GuardianInformation]
    SET [GuardianFirstName] = COALESCE([GuardianFirstName], [FirstName], 'Unknown')
    WHERE [GuardianFirstName] IS NULL;
    PRINT 'Updated NULL GuardianFirstName values';
    
    -- Update NULL values in GuardianLastName column
    UPDATE [dbo].[GuardianInformation]
    SET [GuardianLastName] = COALESCE([GuardianLastName], [LastName], 'Unknown')
    WHERE [GuardianLastName] IS NULL;
    PRINT 'Updated NULL GuardianLastName values';
    
    -- Update NULL values in ConsentStatus column
    UPDATE [dbo].[GuardianInformation]
    SET [ConsentStatus] = 'Pending'
    WHERE [ConsentStatus] IS NULL;
    PRINT 'Updated NULL ConsentStatus values';
    
    -- Update NULL values in ProofType column
    UPDATE [dbo].[GuardianInformation]
    SET [ProofType] = 'GuardianResidencyProof'
    WHERE [ProofType] IS NULL;
    PRINT 'Updated NULL ProofType values';
    
    -- Update NULL values in CreatedAt column
    UPDATE [dbo].[GuardianInformation]
    SET [CreatedAt] = GETDATE()
    WHERE [CreatedAt] IS NULL;
    PRINT 'Updated NULL CreatedAt values';
    
    -- Update NULL values in ResidencyProofPath column
    UPDATE [dbo].[GuardianInformation]
    SET [ResidencyProofPath] = '/uploads/default_proof.png'
    WHERE [ResidencyProofPath] IS NULL;
    PRINT 'Updated NULL ResidencyProofPath values';
    
    -- Make sure all records have proper UserId values
    IF EXISTS (SELECT 1 FROM [dbo].[GuardianInformation] WHERE [UserId] IS NULL)
    BEGIN
        PRINT 'WARNING: Found records with NULL UserId. These will be deleted as they are invalid.';
        DELETE FROM [dbo].[GuardianInformation] WHERE [UserId] IS NULL;
        PRINT 'Deleted records with NULL UserId';
    END
    ELSE
    BEGIN
        PRINT 'All records have valid UserId values';
    END
    
    PRINT 'GuardianInformation null values fix completed';
END
ELSE
BEGIN
    PRINT 'GuardianInformation table does not exist';
END 