-- Script to fix the foreign key constraint issue between NCDRiskAssessments and Appointments tables

-- Set proper options
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- Step 1: Make the AppointmentId nullable if it's not already
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'NCDRiskAssessments' 
    AND COLUMN_NAME = 'AppointmentId' 
    AND IS_NULLABLE = 'NO'
)
BEGIN
    -- Drop existing foreign key if it exists
    IF EXISTS (
        SELECT 1 FROM sys.foreign_keys 
        WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId'
    )
    BEGIN
        ALTER TABLE NCDRiskAssessments DROP CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId;
        PRINT 'Dropped existing foreign key constraint';
    END

    -- Drop default constraint if it exists
    DECLARE @DefaultConstraintName NVARCHAR(128);
    SELECT @DefaultConstraintName = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
    WHERE c.name = 'AppointmentId' AND OBJECT_NAME(dc.parent_object_id) = 'NCDRiskAssessments';
    
    IF @DefaultConstraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE NCDRiskAssessments DROP CONSTRAINT ' + @DefaultConstraintName);
        PRINT 'Dropped default constraint: ' + @DefaultConstraintName;
    END

    -- Make the column nullable
    ALTER TABLE NCDRiskAssessments ALTER COLUMN AppointmentId INT NULL;
    PRINT 'Made AppointmentId column nullable';
END
ELSE
BEGIN
    PRINT 'AppointmentId column is already nullable';
END

-- Step 2: Update any zero values to NULL (if zeros were used as default)
UPDATE NCDRiskAssessments SET AppointmentId = NULL WHERE AppointmentId = 0;
PRINT 'Updated zero AppointmentId values to NULL';

-- Step 3: Create a proper index on the column
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_NCDRiskAssessments_AppointmentId' 
    AND object_id = OBJECT_ID('NCDRiskAssessments')
)
BEGIN
    CREATE INDEX IX_NCDRiskAssessments_AppointmentId ON NCDRiskAssessments(AppointmentId);
    PRINT 'Created index IX_NCDRiskAssessments_AppointmentId';
END
ELSE
BEGIN
    PRINT 'Index IX_NCDRiskAssessments_AppointmentId already exists';
END

-- Step 4: Re-create the foreign key constraint with ON DELETE SET NULL
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId'
)
BEGIN
    ALTER TABLE NCDRiskAssessments 
    ADD CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId 
    FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) 
    ON DELETE SET NULL;
    
    PRINT 'Created foreign key constraint with ON DELETE SET NULL';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint already exists';
END

PRINT 'Fix completed successfully.';

-- Update the __EFMigrationsHistory table to mark the migration as completed
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250529035759_AddTimestampsToUserPermissions')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250529035759_AddTimestampsToUserPermissions', '8.0.2');
END

-- Apply the next migration manually
IF NOT EXISTS (SELECT * FROM __EFMigrationsHistory WHERE MigrationId = '20250529035948_AddCreatedAndUpdatedAtToUserPermissions')
BEGIN
    -- Add columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'CreatedAt')
    BEGIN
        ALTER TABLE UserPermissions ADD CreatedAt datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    END

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[UserPermissions]') AND name = 'UpdatedAt')
    BEGIN
        ALTER TABLE UserPermissions ADD UpdatedAt datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
    END

    -- Mark migration as completed
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20250529035948_AddCreatedAndUpdatedAtToUserPermissions', '8.0.2');
END 