-- Fix migration history: Mark InitialCreate as applied
USE [bhcareDB]
GO

-- Mark InitialCreate as applied so EF skips recreating Identity tables
IF NOT EXISTS (
    SELECT 1
    FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250715101311_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250715101311_InitialCreate', N'9.0.5');
    PRINT 'InitialCreate migration marked as applied.';
END
ELSE
BEGIN
    PRINT 'InitialCreate migration already in history.';
END
GO
