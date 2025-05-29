-- Fix Migration History table to mark problematic migrations as already applied
USE [Barangay];

-- First check if __EFMigrationsHistory table exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    PRINT 'Checking migration history...';
    
    -- Check if the problematic migration exists
    IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250508150438_MakeLastNameAndSuffixNullable')
    BEGIN
        -- Insert the migration as if it was already applied
        PRINT 'Marking migration 20250508150438_MakeLastNameAndSuffixNullable as applied';
        INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
        VALUES ('20250508150438_MakeLastNameAndSuffixNullable', '7.0.5');
    END
    ELSE
    BEGIN
        PRINT 'Migration 20250508150438_MakeLastNameAndSuffixNullable is already marked as applied';
    END
END
ELSE
BEGIN
    PRINT 'Migration history table does not exist. No fixes needed.';
END

PRINT 'Migration history check complete'; 