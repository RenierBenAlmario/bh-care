-- Drop existing constraints and indexes
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_NCDRiskAssessments_Appointments_AppointmentId')
    ALTER TABLE NCDRiskAssessments DROP CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId;

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_NCDRiskAssessments_AppointmentId')
    DROP INDEX IX_NCDRiskAssessments_AppointmentId ON NCDRiskAssessments;

-- Drop default constraint if it exists
DECLARE @ConstraintName nvarchar(200);
SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('NCDRiskAssessments')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = 'AppointmentId'
AND object_id = OBJECT_ID('NCDRiskAssessments'));

IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE NCDRiskAssessments DROP CONSTRAINT ' + @ConstraintName);

-- Make AppointmentId nullable
ALTER TABLE NCDRiskAssessments ALTER COLUMN AppointmentId INT NULL;

-- Recreate index
CREATE INDEX IX_NCDRiskAssessments_AppointmentId ON NCDRiskAssessments(AppointmentId);

-- Add foreign key with NO ACTION
ALTER TABLE NCDRiskAssessments ADD CONSTRAINT FK_NCDRiskAssessments_Appointments_AppointmentId 
FOREIGN KEY (AppointmentId) REFERENCES Appointments(Id) ON DELETE NO ACTION; 