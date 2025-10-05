-- Fix missing COPD columns in NCDRiskAssessments table
-- This script adds the missing COPDMedication and COPDYear columns

-- Check if columns already exist before adding them
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'COPDMedication')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD COPDMedication NVARCHAR(100) NULL;
    PRINT 'Added COPDMedication column';
END
ELSE
BEGIN
    PRINT 'COPDMedication column already exists';
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'NCDRiskAssessments' AND COLUMN_NAME = 'COPDYear')
BEGIN
    ALTER TABLE NCDRiskAssessments ADD COPDYear NVARCHAR(10) NULL;
    PRINT 'Added COPDYear column';
END
ELSE
BEGIN
    PRINT 'COPDYear column already exists';
END

-- Verify the columns were added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'NCDRiskAssessments' 
AND COLUMN_NAME LIKE '%COPD%' 
ORDER BY COLUMN_NAME;
