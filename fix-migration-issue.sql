-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Create a cascade relationship instead of SET NULL since the column is not nullable
IF EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId'
)
BEGIN
    ALTER TABLE NCDRiskAssessments DROP CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId;
    PRINT 'Dropped existing foreign key constraint';
END

-- Add the constraint with ON DELETE CASCADE
ALTER TABLE NCDRiskAssessments 
ADD CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId 
FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) 
ON DELETE CASCADE;
    
PRINT 'Created foreign key constraint with ON DELETE CASCADE';

-- Mark the migration as completed
-- Check if the migration has already been applied
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory] 
    WHERE [MigrationId] = '20250529035759_AddTimestampsToUserPermissions'
)
BEGIN
    -- Add entry to __EFMigrationsHistory to mark the migration as completed
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529035759_AddTimestampsToUserPermissions', '8.0.2');
    
    PRINT 'Migration marked as completed';
END
ELSE
BEGIN
    PRINT 'Migration was already marked as completed';
END

-- Apply the next migration entry if it doesn't exist
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory] 
    WHERE [MigrationId] = '20250529051202_AddHealthFacilityAndFamilyNoFields'
)
BEGIN
    -- Add entry to __EFMigrationsHistory 
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250529051202_AddHealthFacilityAndFamilyNoFields', '8.0.2');
    
    PRINT 'Migration 20250529051202_AddHealthFacilityAndFamilyNoFields marked as completed';
END
ELSE
BEGIN
    PRINT 'Migration 20250529051202_AddHealthFacilityAndFamilyNoFields was already marked as completed';
END

PRINT 'Fix completed successfully.'; 