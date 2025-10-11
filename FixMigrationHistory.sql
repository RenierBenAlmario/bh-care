-- Fix Migration History table to mark all existing migrations as already applied
USE [Barangay];

-- First check if __EFMigrationsHistory table exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
BEGIN
    PRINT 'Checking migration history...';
    
    -- List of migrations that should be marked as applied since the database structure exists
    DECLARE @migrations TABLE (MigrationId NVARCHAR(150), ProductVersion NVARCHAR(32))
    
    INSERT INTO @migrations VALUES 
        ('20250115000000_AddUrlTokensTable', '8.0.0'),
        ('20250715101311_InitialCreate', '8.0.0'),
        ('20250730144649_AddAppointmentIdToMedicalRecords', '8.0.0'),
        ('20250803133403_AddUnitPropertyToPrescriptionMedications', '8.0.0'),
        ('20250809100607_AddNCDRiskAssessmentColumns', '8.0.0'),
        ('20250810045502_AddMissingNCDRiskAssessmentColumns', '8.0.0'),
        ('20250909160657_AddUserBarangay', '8.0.0'),
        ('20250912233707_AddNotificationSettings', '8.0.0'),
        ('20250913161343_FixMissingColumns', '8.0.0'),
        ('20250915141549_UpdateVitalSignsToStringColumns', '8.0.0'),
        ('20250915143035_AddEncryptedColumnsToVitalSigns', '8.0.0'),
        ('20250927202147_AddAllMissingNCDRiskAssessmentColumns', '8.0.0'),
        ('20251001110232_AddUserSuspensionSystem', '8.0.0'),
        ('20251001115429_FixDatabaseSchema', '8.0.0'),
        ('20251003101955_ConfigureHEEADSSSColumnTypes_Clean', '8.0.0'),
        ('20251004030323_AddRemainingNCDRiskAssessmentColumns', '8.0.0'),
        ('20251004112318_AddHasStrokeSymptomsColumn', '8.0.0'),
        ('20251004125509_AddMissingHEEADSSSColumnsSafely', '8.0.0'),
        ('20251004130115_AddReferredByColumn', '8.0.0'),
        ('20251004132422_AddEatingHabitsColumns', '8.0.0'),
        ('20251005040757_AddCOPDColumns', '8.0.0');
        
    -- Insert missing migrations into history
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    SELECT m.MigrationId, m.ProductVersion 
    FROM @migrations m
    WHERE NOT EXISTS (
        SELECT 1 FROM __EFMigrationsHistory h 
        WHERE h.MigrationId = m.MigrationId
    );
    
    PRINT 'All missing migrations have been marked as applied';
END
ELSE
BEGIN
    PRINT 'Migration history table does not exist. No fixes needed.';
END

PRINT 'Migration history fix complete'; 