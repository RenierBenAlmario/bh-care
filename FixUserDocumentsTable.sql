-- Script to fix the UserDocuments table structure to match the model class
-- First check if the table exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    PRINT 'Dropping and recreating UserDocuments table to match model...';
    
    -- Drop the table
    DROP TABLE [UserDocuments];
    
    -- Create the table with the correct schema matching the model
    CREATE TABLE [UserDocuments] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] NVARCHAR(450) NOT NULL,
        [FileName] NVARCHAR(256) NOT NULL,
        [FilePath] NVARCHAR(256) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT ('Pending'),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(256) NULL,
        [FileSize] BIGINT NOT NULL,
        [ContentType] NVARCHAR(100) NULL,
        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
    );
    
    -- Create index on UserId for better performance
    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments]([UserId]);
    
    PRINT 'UserDocuments table recreated with correct schema.';
END
ELSE
BEGIN
    PRINT 'UserDocuments table does not exist. Creating it...';
    
    -- Create the table with the correct schema matching the model
    CREATE TABLE [UserDocuments] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] NVARCHAR(450) NOT NULL,
        [FileName] NVARCHAR(256) NOT NULL,
        [FilePath] NVARCHAR(256) NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT ('Pending'),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(256) NULL,
        [FileSize] BIGINT NOT NULL,
        [ContentType] NVARCHAR(100) NULL,
        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
    );
    
    -- Create index on UserId for better performance
    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments]([UserId]);
    
    PRINT 'UserDocuments table created with correct schema.';
END

-- Create a test document to verify
DECLARE @testUserId NVARCHAR(450);
SELECT TOP 1 @testUserId = Id FROM AspNetUsers WHERE Status = 'Pending';

IF @testUserId IS NOT NULL
BEGIN
    INSERT INTO [UserDocuments] (
        [UserId],
        [FileName],
        [FilePath],
        [Status],
        [FileSize],
        [ContentType]
    )
    VALUES (
        @testUserId,
        'test-document.pdf',
        '/uploads/residency_proofs/test-document.pdf',
        'Pending',
        1024,
        'application/pdf'
    );
    
    PRINT 'Test document created for user ID: ' + @testUserId;
END
ELSE
BEGIN
    PRINT 'No pending users found to create test document.';
END 