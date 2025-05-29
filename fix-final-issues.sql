-- Fix final issues in Barangay database
USE [Barangay];

-- First drop existing foreign keys to fix the cyclic cascade path
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];
GO

-- Re-add them with proper delete behavior
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
GO

-- Second, fix the NULL values in required columns
-- This is more comprehensive than previous script
UPDATE [AspNetUsers]
SET 
    [UserName] = COALESCE([UserName], COALESCE([Email], CONCAT('user_', [Id]))),
    [NormalizedUserName] = UPPER(COALESCE([UserName], COALESCE([Email], CONCAT('user_', [Id])))),
    [Email] = COALESCE([Email], CONCAT('user_', [Id], '@example.com')),
    [NormalizedEmail] = UPPER(COALESCE([Email], CONCAT('user_', [Id], '@example.com'))),
    [SecurityStamp] = COALESCE([SecurityStamp], CONVERT(NVARCHAR(36), NEWID())),
    [ConcurrencyStamp] = COALESCE([ConcurrencyStamp], CONVERT(NVARCHAR(36), NEWID())),
    [PasswordHash] = COALESCE([PasswordHash], '$2a$11$aPNTKTAc2nYBVwhT.wnbJewz.YAXxlwT5CBDvkwpnPO4RVmdYg4w.'),
    [FirstName] = COALESCE([FirstName], ''),
    [LastName] = COALESCE([LastName], ''),
    [FullName] = CASE 
                    WHEN [FullName] IS NULL THEN CONCAT(COALESCE([FirstName], ''), ' ', COALESCE([LastName], ''))
                    ELSE [FullName]
                 END,
    [Gender] = COALESCE([Gender], ''),
    [Address] = COALESCE([Address], ''),
    [ProfilePicture] = COALESCE([ProfilePicture], ''),
    [PhilHealthId] = COALESCE([PhilHealthId], ''),
    [EncryptedFullName] = COALESCE([EncryptedFullName], ''),
    [EncryptedStatus] = COALESCE([EncryptedStatus], ''),
    [WorkingHours] = COALESCE([WorkingHours], ''),
    [Specialization] = COALESCE([Specialization], ''),
    [JoinDate] = CASE WHEN [JoinDate] = '0001-01-01 00:00:00.0000000' OR [JoinDate] IS NULL THEN GETDATE() ELSE [JoinDate] END,
    [LastActive] = CASE WHEN [LastActive] = '0001-01-01 00:00:00.0000000' OR [LastActive] IS NULL THEN GETDATE() ELSE [LastActive] END,
    [EmailConfirmed] = COALESCE([EmailConfirmed], 1)
WHERE 
    [Id] IN (
        SELECT [Id] FROM [AspNetUsers] 
        WHERE [UserName] IS NULL 
        OR [Email] IS NULL 
        OR [FullName] IS NULL
        OR [UserName] = ''
        OR [Email] = ''
        OR [NormalizedUserName] IS NULL
        OR [NormalizedEmail] IS NULL
    );
GO

-- Record that we applied this fix in migrations history
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250603000006_FixMessageUserRelationships')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250603000006_FixMessageUserRelationships', '8.0.0');
END
GO

PRINT 'Final fixes applied successfully';
GO 