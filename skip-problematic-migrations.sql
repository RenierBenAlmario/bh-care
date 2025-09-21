-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- List of migrations to mark as applied
DECLARE @MigrationsToApply TABLE (MigrationId NVARCHAR(150));

-- Insert migration IDs to mark as applied
INSERT INTO @MigrationsToApply (MigrationId) VALUES
('20250529035948_AddCreatedAndUpdatedAtToUserPermissions'),
('20250529051329_AddHighSaltIntakeField'),
('20250529091817_UpdateSchema'),
('20250529103232_AddGuardianInformationColumns'),
('20250529114300_MakeDoctorIdNullable'),
('20250530021800_MakeNCDRiskAssessmentFieldsNullable'),
('20250614133027_FixNotificationLinkNullable'),
('20250614135131_AddUserNumberColumn'),
('20250616140349_AddGuardianProofTypeAndConsentStatus'),
('20250618061634_FixNCDRiskAssessmentAppointmentForeignKey'),
('BookingFormUpdates');

-- Mark migrations as complete
DECLARE @ProductVersion NVARCHAR(32) = '8.0.2';
DECLARE @MigrationId NVARCHAR(150);

-- Create a cursor to loop through migrations
DECLARE migration_cursor CURSOR FOR 
SELECT MigrationId FROM @MigrationsToApply;

OPEN migration_cursor;
FETCH NEXT FROM migration_cursor INTO @MigrationId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if migration is already applied
    IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = @MigrationId)
    BEGIN
        -- Insert migration record
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (@MigrationId, @ProductVersion);
        
        PRINT 'Migration ' + @MigrationId + ' marked as completed';
    END
    ELSE
    BEGIN
        PRINT 'Migration ' + @MigrationId + ' was already marked as completed';
    END
    
    FETCH NEXT FROM migration_cursor INTO @MigrationId;
END

CLOSE migration_cursor;
DEALLOCATE migration_cursor;

-- Add necessary columns for proper booking functionality
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'AttachmentPath'
)
BEGIN
    ALTER TABLE Appointments ADD AttachmentPath NVARCHAR(500) NULL;
    PRINT 'Added AttachmentPath column to Appointments table';
END

PRINT 'Migration skip completed successfully.'; 