-- Update the migrations table to prevent trying to add columns that already exist
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRY
    -- Check current migration state
    SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId;
    
    -- Delete any incomplete migrations
    DELETE FROM __EFMigrationsHistory 
    WHERE MigrationId IN ('20250507111754_FixApplicationUserProperties', '20250507111755_AddMiddleNameAndSuffixColumns');
    PRINT 'Removed incomplete migrations';
    
    -- Add the migrations as completed
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES 
        ('20250507111754_FixApplicationUserProperties', '8.0.4'),
        ('20250507111755_AddMiddleNameAndSuffixColumns', '8.0.4');
    PRINT 'Added migration records as completed';
    
    -- Show updated migration state
    SELECT MigrationId FROM __EFMigrationsHistory ORDER BY MigrationId;
END TRY
BEGIN CATCH
    PRINT 'Error updating migrations: ' + ERROR_MESSAGE();
END CATCH 