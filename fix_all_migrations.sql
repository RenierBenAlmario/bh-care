-- Mark all already-applied migrations in history
USE [bhcareDB]
GO

-- 1. InitialCreate (Identity tables exist)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250715101311_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250715101311_InitialCreate', N'9.0.5');
    PRINT '✓ InitialCreate';
END

-- 2. AddAppointmentIdToMedicalRecords (column exists)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250730144649_AddAppointmentIdToMedicalRecords')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250730144649_AddAppointmentIdToMedicalRecords', N'9.0.5');
    PRINT '✓ AddAppointmentIdToMedicalRecords';
END

-- 3. AddNCDRiskAssessmentColumns (NCD table exists with many columns)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250809100607_AddNCDRiskAssessmentColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250809100607_AddNCDRiskAssessmentColumns', N'9.0.5');
    PRINT '✓ AddNCDRiskAssessmentColumns';
END

-- 4. AddMissingNCDRiskAssessmentColumns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250810045502_AddMissingNCDRiskAssessmentColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250810045502_AddMissingNCDRiskAssessmentColumns', N'9.0.5');
    PRINT '✓ AddMissingNCDRiskAssessmentColumns';
END

-- 5. AddUserBarangay
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250909160657_AddUserBarangay')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250909160657_AddUserBarangay', N'9.0.5');
    PRINT '✓ AddUserBarangay';
END

-- 6. AddNotificationSettings
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250912233707_AddNotificationSettings')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250912233707_AddNotificationSettings', N'9.0.5');
    PRINT '✓ AddNotificationSettings';
END

-- 7. FixMissingColumns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250913161343_FixMissingColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250913161343_FixMissingColumns', N'9.0.5');
    PRINT '✓ FixMissingColumns';
END

-- 8. UpdateVitalSignsToStringColumns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250915141549_UpdateVitalSignsToStringColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250915141549_UpdateVitalSignsToStringColumns', N'9.0.5');
    PRINT '✓ UpdateVitalSignsToStringColumns';
END

-- 9. AddEncryptedColumnsToVitalSigns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250915143035_AddEncryptedColumnsToVitalSigns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250915143035_AddEncryptedColumnsToVitalSigns', N'9.0.5');
    PRINT '✓ AddEncryptedColumnsToVitalSigns';
END

-- 10. AddAllMissingNCDRiskAssessmentColumns
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20250927202147_AddAllMissingNCDRiskAssessmentColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20250927202147_AddAllMissingNCDRiskAssessmentColumns', N'9.0.5');
    PRINT '✓ AddAllMissingNCDRiskAssessmentColumns';
END

-- 11. AddUserSuspensionSystem
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251001110232_AddUserSuspensionSystem')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251001110232_AddUserSuspensionSystem', N'9.0.5');
    PRINT '✓ AddUserSuspensionSystem';
END

-- 12. FixDatabaseSchema (the latest)
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20251001115429_FixDatabaseSchema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20251001115429_FixDatabaseSchema', N'9.0.5');
    PRINT '✓ FixDatabaseSchema';
END

PRINT 'All migrations marked as applied.';
GO

-- Verify
SELECT [MigrationId], [ProductVersion]
FROM [__EFMigrationsHistory]
ORDER BY [MigrationId];
