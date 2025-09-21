-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Add remaining migrations to history table
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529091817_UpdateSchema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529091817_UpdateSchema', '8.0.2');
    
    PRINT 'Marked migration 20250529091817_UpdateSchema as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529103232_AddGuardianInformationColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529103232_AddGuardianInformationColumns', '8.0.2');
    
    PRINT 'Marked migration 20250529103232_AddGuardianInformationColumns as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250529114300_MakeDoctorIdNullable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529114300_MakeDoctorIdNullable', '8.0.2');
    
    PRINT 'Marked migration 20250529114300_MakeDoctorIdNullable as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250530021800_MakeNCDRiskAssessmentFieldsNullable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250530021800_MakeNCDRiskAssessmentFieldsNullable', '8.0.2');
    
    PRINT 'Marked migration 20250530021800_MakeNCDRiskAssessmentFieldsNullable as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250614133027_FixNotificationLinkNullable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250614133027_FixNotificationLinkNullable', '8.0.2');
    
    PRINT 'Marked migration 20250614133027_FixNotificationLinkNullable as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250614135131_AddUserNumberColumn')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250614135131_AddUserNumberColumn', '8.0.2');
    
    PRINT 'Marked migration 20250614135131_AddUserNumberColumn as applied';
END

IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250616140349_AddGuardianProofTypeAndConsentStatus')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250616140349_AddGuardianProofTypeAndConsentStatus', '8.0.2');
    
    PRINT 'Marked migration 20250616140349_AddGuardianProofTypeAndConsentStatus as applied';
END

PRINT 'Migration history update completed successfully!'; 