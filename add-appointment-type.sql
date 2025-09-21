-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Add AppointmentType column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[NCDRiskAssessments]') AND name = 'AppointmentType')
BEGIN
    ALTER TABLE [NCDRiskAssessments] ADD [AppointmentType] nvarchar(100) NULL;
    PRINT 'AppointmentType column added to NCDRiskAssessments table';
END
ELSE
BEGIN
    PRINT 'AppointmentType column already exists in NCDRiskAssessments table';
END 