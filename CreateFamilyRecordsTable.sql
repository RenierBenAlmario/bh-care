-- Script to create the FamilyRecords table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FamilyRecords')
BEGIN
    PRINT 'Creating FamilyRecords table...';
    
    CREATE TABLE [dbo].[FamilyRecords](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [FamilyNumber] [nvarchar](max) NOT NULL,
        [FirstName] [nvarchar](max) NOT NULL,
        [LastName] [nvarchar](max) NOT NULL,
        [DateOfBirth] [datetime2](7) NOT NULL,
        [Address] [nvarchar](max) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT (GETUTCDATE()),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT (GETUTCDATE()),
        CONSTRAINT [PK_FamilyRecords] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'FamilyRecords table created successfully.';
    
    -- Add a sample record to help with testing
    INSERT INTO [dbo].[FamilyRecords] (
        [FamilyNumber],
        [FirstName],
        [LastName],
        [DateOfBirth],
        [Address],
        [CreatedAt],
        [UpdatedAt]
    )
    VALUES (
        'A001',
        'Sample',
        'Patient',
        '1990-01-01',
        '123 Main Street',
        GETUTCDATE(),
        GETUTCDATE()
    );
    
    PRINT 'Added sample record to FamilyRecords table.';
END
ELSE
BEGIN
    PRINT 'FamilyRecords table already exists.';
END 