-- Fix UserDocuments table
-- This script ensures the UserDocuments table exists and has the correct schema
-- It also fixes any missing or incorrect data

-- Check if table exists, create if not
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments')
BEGIN
    -- First check the name of the users table
    DECLARE @UsersTable NVARCHAR(128)
    
    IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
        SET @UsersTable = 'AspNetUsers'
    ELSE IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
        SET @UsersTable = 'Users'
    ELSE
        SET @UsersTable = NULL
        
    IF @UsersTable IS NULL
    BEGIN
        PRINT 'Error: Users table not found. Cannot create UserDocuments table.'
        RETURN
    END
    
    -- Create the table with the correct foreign key reference
    DECLARE @SQL NVARCHAR(MAX)
    SET @SQL = '
    CREATE TABLE [UserDocuments](
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [UserId] NVARCHAR(450) NOT NULL,
        [FilePath] NVARCHAR(512) NOT NULL DEFAULT (''''),
        [FileName] NVARCHAR(256) NOT NULL DEFAULT (''''),
        [FileSize] BIGINT NOT NULL DEFAULT (0),
        [ContentType] NVARCHAR(100) NOT NULL DEFAULT (''application/octet-stream''),
        [Status] NVARCHAR(50) NOT NULL DEFAULT (''Pending''),
        [ApprovedAt] DATETIME2 NULL,
        [ApprovedBy] NVARCHAR(450) NULL,
        [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [FK_UserDocuments_Users] FOREIGN KEY ([UserId]) REFERENCES [' + @UsersTable + ']([Id]) ON DELETE CASCADE
    )'
    
    EXEC sp_executesql @SQL
    PRINT 'Created UserDocuments table with foreign key to ' + @UsersTable
END
ELSE
BEGIN
    PRINT 'UserDocuments table already exists';
    
    -- Check and add missing columns
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FilePath')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [FilePath] NVARCHAR(512) NOT NULL DEFAULT ('');
        PRINT 'Added FilePath column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FileName')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [FileName] NVARCHAR(256) NOT NULL DEFAULT ('');
        PRINT 'Added FileName column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'FileSize')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [FileSize] BIGINT NOT NULL DEFAULT (0);
        PRINT 'Added FileSize column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'ContentType')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [ContentType] NVARCHAR(100) NOT NULL DEFAULT ('application/octet-stream');
        PRINT 'Added ContentType column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'Status')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [Status] NVARCHAR(50) NOT NULL DEFAULT ('Pending');
        PRINT 'Added Status column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'ApprovedAt')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [ApprovedAt] DATETIME2 NULL;
        PRINT 'Added ApprovedAt column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'ApprovedBy')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [ApprovedBy] NVARCHAR(450) NULL;
        PRINT 'Added ApprovedBy column';
    END
    
    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserDocuments' AND COLUMN_NAME = 'UploadDate')
    BEGIN
        ALTER TABLE [UserDocuments] ADD [UploadDate] DATETIME2 NOT NULL DEFAULT GETDATE();
        PRINT 'Added UploadDate column';
    END
END

-- Fix NULL values in existing records
UPDATE [UserDocuments]
SET 
    [FilePath] = ISNULL([FilePath], ''),
    [FileName] = ISNULL([FileName], ''),
    [ContentType] = ISNULL([ContentType], 'application/octet-stream'),
    [Status] = ISNULL([Status], 'Pending'),
    [UploadDate] = ISNULL([UploadDate], GETDATE())
WHERE 
    [FilePath] IS NULL OR 
    [FileName] IS NULL OR 
    [ContentType] IS NULL OR 
    [Status] IS NULL OR 
    [UploadDate] IS NULL;

PRINT 'Fixed NULL values in existing records';

-- Determine the users table name for the migration
DECLARE @UsersTableForMigration NVARCHAR(128)
    
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUsers')
    SET @UsersTableForMigration = 'AspNetUsers'
ELSE IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
    SET @UsersTableForMigration = 'Users'
ELSE
    SET @UsersTableForMigration = NULL

-- Migrate legacy documents from ProfilePicture
IF @UsersTableForMigration IS NOT NULL AND EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @UsersTableForMigration AND COLUMN_NAME = 'ProfilePicture')
BEGIN
    -- Find users with residency proofs in ProfilePicture that don't have entries in UserDocuments
    DECLARE @MigrationSQL NVARCHAR(MAX)
    SET @MigrationSQL = '
    INSERT INTO [UserDocuments] ([UserId], [FilePath], [FileName], [ContentType], [Status], [UploadDate])
    SELECT 
        u.[Id], 
        u.[ProfilePicture], 
        SUBSTRING(u.[ProfilePicture], CHARINDEX(''/'', u.[ProfilePicture], CHARINDEX(''/'', u.[ProfilePicture], 1) + 1) + 1, 255) AS [FileName],
        CASE 
            WHEN u.[ProfilePicture] LIKE ''%.pdf'' THEN ''application/pdf''
            WHEN u.[ProfilePicture] LIKE ''%.png'' THEN ''image/png''
            WHEN u.[ProfilePicture] LIKE ''%.jpg'' OR u.[ProfilePicture] LIKE ''%.jpeg'' THEN ''image/jpeg''
            ELSE ''application/octet-stream''
        END AS [ContentType],
        ''Pending'' AS [Status],
        GETDATE() AS [UploadDate]
    FROM [' + @UsersTableForMigration + '] u
    LEFT JOIN [UserDocuments] ud ON u.[Id] = ud.[UserId]
    WHERE 
        u.[ProfilePicture] IS NOT NULL 
        AND u.[ProfilePicture] LIKE ''%/uploads/residency_proofs/%''
        AND ud.[Id] IS NULL'
        
    EXEC sp_executesql @MigrationSQL
    PRINT 'Migrated legacy documents from ProfilePicture'
    
    -- Clear ProfilePicture for migrated users
    SET @MigrationSQL = '
    UPDATE [' + @UsersTableForMigration + ']
    SET [ProfilePicture] = NULL
    FROM [' + @UsersTableForMigration + '] u
    INNER JOIN [UserDocuments] ud ON u.[Id] = ud.[UserId]
    WHERE u.[ProfilePicture] LIKE ''%/uploads/residency_proofs/%'''
    
    EXEC sp_executesql @MigrationSQL
    PRINT 'Cleared ProfilePicture for migrated users'
END

PRINT 'UserDocuments table fix completed successfully'; 