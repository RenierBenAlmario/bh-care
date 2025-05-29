-- SQL Script to create the UserDocuments table
-- This table is missing according to the database verification

PRINT 'Creating UserDocuments table...'

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    CREATE TABLE [UserDocuments](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] NVARCHAR(450) NOT NULL,
        [FileName] NVARCHAR(256) NOT NULL DEFAULT (''),
        [FilePath] NVARCHAR(256) NOT NULL DEFAULT (''),
        [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending'),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(256) NULL DEFAULT (''),
        [FileSize] BIGINT NOT NULL DEFAULT (0),
        [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream'),
        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) 
            REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments] ([UserId]);
    
    PRINT 'UserDocuments table created successfully';
END
ELSE
BEGIN
    PRINT 'UserDocuments table already exists';
    
    -- Check if columns exist and add them if they don't
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'UploadDate')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE();
        PRINT 'UploadDate column added';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'ContentType')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream');
        PRINT 'ContentType column added';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                  WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FileSize')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [FileSize] BIGINT NOT NULL DEFAULT (0);
        PRINT 'FileSize column added';
    END
    
    -- Fix NULL values in all columns
    UPDATE [UserDocuments] SET [FileName] = '' WHERE [FileName] IS NULL;
    UPDATE [UserDocuments] SET [FilePath] = '' WHERE [FilePath] IS NULL;
    UPDATE [UserDocuments] SET [Status] = 'Pending' WHERE [Status] IS NULL;
    UPDATE [UserDocuments] SET [ApprovedBy] = '' WHERE [ApprovedBy] IS NULL;
    UPDATE [UserDocuments] SET [ContentType] = 'application/octet-stream' WHERE [ContentType] IS NULL;
    UPDATE [UserDocuments] SET [FileSize] = 0 WHERE [FileSize] IS NULL;
    PRINT 'NULL values fixed in UserDocuments table';
END 