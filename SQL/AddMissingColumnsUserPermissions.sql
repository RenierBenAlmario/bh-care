-- Add missing CreatedAt and UpdatedAt columns to UserPermissions table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND name = 'CreatedAt')
BEGIN
    ALTER TABLE [dbo].[UserPermissions] 
    ADD [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    PRINT 'Added CreatedAt column to UserPermissions table'
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND name = 'UpdatedAt')
BEGIN
    ALTER TABLE [dbo].[UserPermissions] 
    ADD [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETDATE()
    PRINT 'Added UpdatedAt column to UserPermissions table'
END 