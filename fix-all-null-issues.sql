-- Comprehensive fix for NULL values in the database
USE [Barangay];

PRINT '===== BEGINNING DATABASE NULL VALUE FIXES =====';

-- =========================
-- Fix UserDocuments table
-- =========================
PRINT 'Checking UserDocuments table...';

-- 1. Check if table exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    PRINT 'UserDocuments table does not exist. Creating it...';
    
    CREATE TABLE [UserDocuments] (
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
        CONSTRAINT [FK_UserDocuments_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers]([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_UserDocuments_UserId] ON [UserDocuments]([UserId]);
    
    PRINT 'UserDocuments table created successfully.';
END
ELSE
BEGIN
    PRINT 'UserDocuments table exists. Checking for NULL values...';
    
    -- 2. Check and add UploadDate column if missing
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                   WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'UploadDate')
    BEGIN
        PRINT 'Adding UploadDate column to UserDocuments table...';
        ALTER TABLE [UserDocuments] ADD [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE();
    END
    
    -- 3. Fix NULL values in all string columns
    PRINT 'Fixing NULL values in UserDocuments...';
    
    UPDATE [UserDocuments] SET [FileName] = '' WHERE [FileName] IS NULL;
    UPDATE [UserDocuments] SET [FilePath] = '' WHERE [FilePath] IS NULL;
    UPDATE [UserDocuments] SET [Status] = 'Pending' WHERE [Status] IS NULL;
    UPDATE [UserDocuments] SET [ApprovedBy] = '' WHERE [ApprovedBy] IS NULL;
    UPDATE [UserDocuments] SET [ContentType] = 'application/octet-stream' WHERE [ContentType] IS NULL;
    UPDATE [UserDocuments] SET [FileSize] = 0 WHERE [FileSize] IS NULL;
    UPDATE [UserDocuments] SET [UploadDate] = GETDATE() WHERE [UploadDate] IS NULL;
    
    -- 4. Alter columns to prevent future NULL values
    PRINT 'Altering UserDocuments columns to prevent future NULL values...';
    
    -- Check and alter columns one by one to prevent future NULL values
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FileName' AND IS_NULLABLE = 'YES')
    BEGIN
        ALTER TABLE [UserDocuments] ALTER COLUMN [FileName] NVARCHAR(256) NOT NULL;
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FilePath' AND IS_NULLABLE = 'YES')
    BEGIN
        ALTER TABLE [UserDocuments] ALTER COLUMN [FilePath] NVARCHAR(256) NOT NULL;
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'Status' AND IS_NULLABLE = 'YES')
    BEGIN
        ALTER TABLE [UserDocuments] ALTER COLUMN [Status] NVARCHAR(50) NOT NULL;
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FileSize' AND IS_NULLABLE = 'YES')
    BEGIN
        ALTER TABLE [UserDocuments] ALTER COLUMN [FileSize] BIGINT NOT NULL;
    END
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'ContentType' AND IS_NULLABLE = 'YES')
    BEGIN
        ALTER TABLE [UserDocuments] ALTER COLUMN [ContentType] NVARCHAR(100) NOT NULL;
    END
    
    -- 5. Add default constraints
    PRINT 'Adding default constraints to UserDocuments...';
    
    -- Add default constraints to important columns
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_FileName')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_FileName] DEFAULT ('') FOR [FileName];
    END
    
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_FilePath')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_FilePath] DEFAULT ('') FOR [FilePath];
    END
    
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_Status')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_Status] DEFAULT ('Pending') FOR [Status];
    END
    
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_ContentType')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_ContentType] DEFAULT ('application/octet-stream') FOR [ContentType];
    END
    
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_FileSize')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_FileSize] DEFAULT (0) FOR [FileSize];
    END
    
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_UserDocuments_ApprovedBy')
    BEGIN
        ALTER TABLE [UserDocuments] ADD CONSTRAINT [DF_UserDocuments_ApprovedBy] DEFAULT ('') FOR [ApprovedBy];
    END
    
    -- 6. Verify if there are still any NULL values
    DECLARE @nullCount INT;
    SELECT @nullCount = COUNT(*) FROM [UserDocuments] 
    WHERE [FileName] IS NULL OR [FilePath] IS NULL OR [Status] IS NULL OR 
          [FileSize] IS NULL OR [ContentType] IS NULL;
          
    IF @nullCount > 0
    BEGIN
        PRINT 'WARNING: Found ' + CAST(@nullCount AS NVARCHAR) + ' rows with NULL values after fixes!';
        
        -- Print problematic rows for investigation
        SELECT * FROM [UserDocuments] 
        WHERE [FileName] IS NULL OR [FilePath] IS NULL OR [Status] IS NULL OR 
              [FileSize] IS NULL OR [ContentType] IS NULL;
    END
    ELSE
    BEGIN
        PRINT 'No NULL values found in UserDocuments after fixes.';
    END
END

-- =========================
-- Fix AspNetUsers table NULL values
-- =========================
PRINT 'Fixing NULL values in AspNetUsers table...';

-- Make sure core fields are never NULL
UPDATE [AspNetUsers] 
SET 
    [UserName] = COALESCE([UserName], [Email], CONVERT(NVARCHAR(256), NEWID())),
    [NormalizedUserName] = COALESCE([NormalizedUserName], UPPER([Email]), UPPER(CONVERT(NVARCHAR(256), NEWID()))),
    [Email] = COALESCE([Email], [UserName], CONVERT(NVARCHAR(256), NEWID())),
    [NormalizedEmail] = COALESCE([NormalizedEmail], UPPER([Email]), UPPER([UserName]), UPPER(CONVERT(NVARCHAR(256), NEWID()))),
    [Status] = COALESCE([Status], 'Pending'),
    [FirstName] = COALESCE([FirstName], ''),
    [LastName] = COALESCE([LastName], ''),
    [MiddleName] = COALESCE([MiddleName], ''),
    [Suffix] = COALESCE([Suffix], ''),
    [ProfilePicture] = COALESCE([ProfilePicture], ''),
    [PhoneNumber] = COALESCE([PhoneNumber], ''),
    [Address] = COALESCE([Address], ''),
    [Gender] = COALESCE([Gender], ''),
    [PhilHealthId] = COALESCE([PhilHealthId], ''),
    [EncryptedFullName] = COALESCE([EncryptedFullName], ''),
    [EncryptedStatus] = COALESCE([EncryptedStatus], '')
WHERE
    [UserName] IS NULL OR [Email] IS NULL OR [NormalizedUserName] IS NULL OR [NormalizedEmail] IS NULL OR
    [Status] IS NULL OR [FirstName] IS NULL OR [LastName] IS NULL;

PRINT '===== DATABASE NULL VALUE FIXES COMPLETED ====='; 