-- Check what's actually in __EFMigrationsHistory
USE [bhcareDB]
GO

SELECT [MigrationId], [ProductVersion]
FROM [__EFMigrationsHistory]
ORDER BY [MigrationId];
