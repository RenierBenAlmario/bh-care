-- Create UserDocuments Table Script
-- This script creates the UserDocuments table that was defined in DbContext but missing in the database

PRINT 'Starting UserDocuments table creation...';

-- Check if the table already exists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserDocuments')
BEGIN
    -- Create the UserDocuments table
    CREATE TABLE [dbo].[UserDocuments] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [FileName] NVARCHAR(256) NOT NULL,
        [FilePath] NVARCHAR(512) NOT NULL,
        [ContentType] NVARCHAR(100) NOT NULL,
        [FileType] NVARCHAR(50) NOT NULL,
        [FileSize] BIGINT NOT NULL,
        [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        [UploadDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ApprovedBy] NVARCHAR(450) NULL,
        [ApprovedAt] DATETIME2 NULL,
        CONSTRAINT [PK_UserDocuments] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserDocuments_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserDocuments_AspNetUsers_ApprovedBy] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[AspNetUsers] ([Id])
    );
    
    -- Create indexes
    CREATE NONCLUSTERED INDEX [IX_UserDocuments_UserId] ON [dbo].[UserDocuments] ([UserId] ASC);
    CREATE NONCLUSTERED INDEX [IX_UserDocuments_ApprovedBy] ON [dbo].[UserDocuments] ([ApprovedBy] ASC);
    CREATE NONCLUSTERED INDEX [IX_UserDocuments_Status] ON [dbo].[UserDocuments] ([Status] ASC);
    CREATE NONCLUSTERED INDEX [IX_UserDocuments_UploadDate] ON [dbo].[UserDocuments] ([UploadDate] DESC);
    
    PRINT 'UserDocuments table created successfully.';
END
ELSE
BEGIN
    PRINT 'UserDocuments table already exists.';
END

-- Add the migration entry to EF Migrations History if not exists
DECLARE @MigrationId nvarchar(150) = '20250630000001_AddUserDocumentsTable';
DECLARE @ProductVersion nvarchar(32) = '8.0.2';

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = @MigrationId)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (@MigrationId, @ProductVersion);
    
    PRINT 'Migration record added to __EFMigrationsHistory table.';
END
ELSE
BEGIN
    PRINT 'Migration record already exists in __EFMigrationsHistory table.';
END

PRINT 'UserDocuments table creation process completed.';

-- Verify the table was created correctly
SELECT 
    t.name AS TableName,
    c.name AS ColumnName,
    TYPE_NAME(c.user_type_id) AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM 
    sys.tables t
INNER JOIN 
    sys.columns c ON t.object_id = c.object_id
WHERE 
    t.name = 'UserDocuments'
ORDER BY 
    c.column_id; 