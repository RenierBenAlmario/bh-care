-- Fix migration state by updating the __EFMigrationsHistory table
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Check if the migration is already recorded
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20250507111754_FixApplicationUserProperties')
BEGIN
    -- Add the migration record to mark it as applied
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250507111754_FixApplicationUserProperties', '8.0.4');
    PRINT 'Added migration record for 20250507111754_FixApplicationUserProperties';
END
ELSE
BEGIN
    PRINT 'Migration 20250507111754_FixApplicationUserProperties is already recorded';
END

-- If there's a newer migration, ensure it's also marked as applied
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20250507111755_AddMiddleNameAndSuffixColumns')
AND EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'MiddleName')
BEGIN
    -- Add the migration record to mark it as applied
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250507111755_AddMiddleNameAndSuffixColumns', '8.0.4');
    PRINT 'Added migration record for 20250507111755_AddMiddleNameAndSuffixColumns';
END 