-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Make AppointmentType column nullable
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[NCDRiskAssessments]') 
           AND name = 'AppointmentType' 
           AND is_nullable = 0)
BEGIN
    -- Find default constraint name
    DECLARE @DefaultConstraintName nvarchar(200)
    
    SELECT @DefaultConstraintName = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.name = 'AppointmentType' AND OBJECT_NAME(dc.parent_object_id) = 'NCDRiskAssessments'
    
    -- Drop default constraint if it exists
    IF @DefaultConstraintName IS NOT NULL
        EXEC('ALTER TABLE [NCDRiskAssessments] DROP CONSTRAINT [' + @DefaultConstraintName + ']')
    
    -- Make column nullable
    ALTER TABLE [NCDRiskAssessments] ALTER COLUMN [AppointmentType] nvarchar(100) NULL;
    
    PRINT 'AppointmentType column is now nullable';
END
ELSE
BEGIN
    PRINT 'AppointmentType column is already nullable or does not exist';
END 