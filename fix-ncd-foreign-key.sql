-- Drop the foreign key constraint if it exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId')
    ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId];

-- Drop the index if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NCDRiskAssessments_AppointmentId')
    DROP INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments];

-- Make AppointmentId nullable
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'AppointmentId')
BEGIN
    DECLARE @DefaultConstraintName nvarchar(200)
    
    -- Find default constraint name
    SELECT @DefaultConstraintName = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.name = 'AppointmentId' AND OBJECT_NAME(dc.parent_object_id) = 'NCDRiskAssessments'
    
    -- Drop default constraint if it exists
    IF @DefaultConstraintName IS NOT NULL
        EXEC('ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @DefaultConstraintName + ']')
    
    -- Alter column to make it nullable
    ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentId] int NULL;
    
    -- Create index
    CREATE INDEX [IX_NCDRiskAssessments_AppointmentId] ON [NCDRiskAssessments] ([AppointmentId]);
    
    -- Add foreign key with ON DELETE SET NULL
    ALTER TABLE [NCDRiskAssessments] ADD CONSTRAINT [FK_NCDRiskAssessments_Appointments_AppointmentId] 
    FOREIGN KEY ([AppointmentId]) REFERENCES [Appointments] ([Id]) ON DELETE SET NULL;
    
    PRINT 'Successfully updated NCDRiskAssessments.AppointmentId to be nullable with proper foreign key';
END
ELSE
BEGIN
    PRINT 'Table NCDRiskAssessments or column AppointmentId does not exist';
END

-- Update migration history to mark the migration as applied if it's not already
IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250618061634_FixNCDRiskAssessmentAppointmentForeignKey')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250618061634_FixNCDRiskAssessmentAppointmentForeignKey', '8.0.2');
    
    PRINT 'Added migration 20250618061634_FixNCDRiskAssessmentAppointmentForeignKey to history';
END
ELSE
BEGIN
    PRINT 'Migration 20250618061634_FixNCDRiskAssessmentAppointmentForeignKey already in history';
END 