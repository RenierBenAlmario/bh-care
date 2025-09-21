USE [Barangay]
GO

-- Add missing Unit column to PrescriptionMedications table
PRINT 'Adding Unit column to PrescriptionMedications table...'

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'PrescriptionMedications' AND COLUMN_NAME = 'Unit')
BEGIN
    ALTER TABLE PrescriptionMedications ADD Unit NVARCHAR(50) NOT NULL DEFAULT 'mg';
    PRINT 'Added Unit column to PrescriptionMedications table with default value "mg"';
END
ELSE
BEGIN
    PRINT 'Unit column already exists in PrescriptionMedications table';
END

-- Verify the column was added
SELECT 
    'PrescriptionMedications Columns' as TableInfo,
    COUNT(*) as ColumnCount
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PrescriptionMedications';

-- Show the updated table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PrescriptionMedications' 
ORDER BY ORDINAL_POSITION;

PRINT 'Unit column fix completed successfully!'; 