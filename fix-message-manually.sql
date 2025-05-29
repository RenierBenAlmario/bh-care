-- SQL script to fix Message-related foreign key constraints
-- This fixes the cyclic cascade path issue in one comprehensive script

USE [Barangay];

-- Step 1: Drop existing foreign keys if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_SenderId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_SenderId];
    
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Messages_AspNetUsers_ReceiverId')
    ALTER TABLE [Messages] DROP CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId];

-- Step 2: Re-add the foreign keys with NO ACTION to avoid cascade path issues
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_SenderId] 
    FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;
    
ALTER TABLE [Messages] ADD CONSTRAINT [FK_Messages_AspNetUsers_ReceiverId] 
    FOREIGN KEY ([ReceiverId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION;

-- Step 3: Add to the migration history to prevent reapplication
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = 'FixMessageUserRelationships')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('FixMessageUserRelationships', '7.0.5');

-- Step 4: Make sure all messages reference valid users
UPDATE Messages SET SenderId = NULL WHERE SenderId NOT IN (SELECT Id FROM AspNetUsers);
UPDATE Messages SET ReceiverId = NULL WHERE ReceiverId NOT IN (SELECT Id FROM AspNetUsers);

-- Log the completion
PRINT 'Message foreign key constraints have been fixed successfully.';
GO 