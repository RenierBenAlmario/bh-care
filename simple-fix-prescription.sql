USE [Barangay]
GO

SET QUOTED_IDENTIFIER ON;
GO

PRINT 'Starting simple fix for PrescriptionMedications table...';

-- Begin transaction for safety
BEGIN TRANSACTION;

-- Check data type of PrescriptionId column
DECLARE @DataType NVARCHAR(128);
SELECT @DataType = DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PrescriptionMedications' AND COLUMN_NAME = 'PrescriptionId';

PRINT 'PrescriptionId data type: ' + @DataType;

-- Fix the problematic records based on data type
IF @DataType = 'int'
BEGIN
    PRINT 'PrescriptionId is an integer column. Setting invalid values to NULL...';
    
    -- Update records with non-numeric values to NULL
    UPDATE PrescriptionMedications
    SET PrescriptionId = NULL
    WHERE ISNUMERIC(CAST(PrescriptionId AS NVARCHAR(50))) = 0 AND PrescriptionId IS NOT NULL;
    
    PRINT 'Fixed records with non-numeric PrescriptionId values.';
END
ELSE
BEGIN
    PRINT 'PrescriptionId is a string column. Removing commas...';
    
    -- Replace commas with empty string
    UPDATE PrescriptionMedications
    SET PrescriptionId = REPLACE(PrescriptionId, ',', '')
    WHERE PrescriptionId LIKE '%,%';
    
    PRINT 'Fixed records with commas in PrescriptionId values.';
END

-- Create a synonym if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = 'PrescriptionMedicines')
BEGIN
    PRINT 'Creating synonym PrescriptionMedicines for PrescriptionMedications...';
    CREATE SYNONYM PrescriptionMedicines FOR PrescriptionMedications;
END

COMMIT TRANSACTION;
PRINT 'Fix applied successfully. Transaction committed.'; 