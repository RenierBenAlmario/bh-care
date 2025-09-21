USE [Barangay]
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Get table schema information first
PRINT 'Getting schema information for PrescriptionMedications table...';
SELECT 
    COLUMN_NAME, 
    DATA_TYPE,
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'PrescriptionMedications';

-- Begin transaction for safety
BEGIN TRANSACTION;

-- Create a backup table if it doesn't exist
PRINT 'Creating backup of PrescriptionMedications table...';
IF OBJECT_ID('PrescriptionMedications_Backup', 'U') IS NOT NULL
    DROP TABLE PrescriptionMedications_Backup;

SELECT * INTO PrescriptionMedications_Backup FROM PrescriptionMedications;
PRINT 'Backup created successfully.';

-- First, identify the data type of PrescriptionId column
DECLARE @DataType NVARCHAR(128);
SELECT @DataType = DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PrescriptionMedications' AND COLUMN_NAME = 'PrescriptionId';

PRINT 'PrescriptionId data type: ' + @DataType;

-- Fix the problematic records based on data type
IF @DataType = 'int'
BEGIN
    PRINT 'PrescriptionId is an integer column. Fixing numeric conversion issues...';
    
    -- Create a temporary table with the structure matching the actual table
    -- Get column names and types dynamically
    DECLARE @createTempTableSQL NVARCHAR(MAX) = 'CREATE TABLE #TempPrescriptionMedications (';
    
    SELECT @createTempTableSQL = @createTempTableSQL + 
        COLUMN_NAME + ' ' + 
        DATA_TYPE + 
        CASE 
            WHEN CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN '(' + CAST(CHARACTER_MAXIMUM_LENGTH AS NVARCHAR) + ')' 
            WHEN NUMERIC_PRECISION IS NOT NULL THEN '(' + CAST(NUMERIC_PRECISION AS NVARCHAR) + 
                CASE WHEN NUMERIC_SCALE IS NOT NULL THEN ',' + CAST(NUMERIC_SCALE AS NVARCHAR) ELSE '' END + ')'
            ELSE '' 
        END +
        CASE WHEN IS_NULLABLE = 'NO' THEN ' NOT NULL' ELSE ' NULL' END +
        ', '
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'PrescriptionMedications'
    ORDER BY ORDINAL_POSITION;
    
    -- Remove trailing comma and space
    SET @createTempTableSQL = LEFT(@createTempTableSQL, LEN(@createTempTableSQL) - 1) + ')';
    
    PRINT 'Creating temp table with structure: ' + @createTempTableSQL;
    EXEC sp_executesql @createTempTableSQL;
    
    -- Use a simpler approach - clean data directly
    UPDATE PrescriptionMedications
    SET PrescriptionId = NULL
    WHERE ISNUMERIC(CAST(PrescriptionId AS NVARCHAR(50))) = 0;
    
    PRINT 'Fixed records with non-numeric PrescriptionId values.';
END
ELSE
BEGIN
    -- Handle nvarchar PrescriptionId
    PRINT 'PrescriptionId is a string column. Fixing string format issues...';
    
    -- Replace commas with empty string
    UPDATE PrescriptionMedications
    SET PrescriptionId = REPLACE(PrescriptionId, ',', '')
    WHERE PrescriptionId LIKE '%,%';
    
    PRINT 'Fixed records with commas in PrescriptionId values.';
END

-- Verify the fix
PRINT 'Verifying fix...';
IF @DataType = 'int'
BEGIN
    -- For integer type, check if any non-numeric values remain
    DECLARE @invalidCount INT;
    SELECT @invalidCount = COUNT(*)
    FROM PrescriptionMedications
    WHERE ISNUMERIC(CAST(PrescriptionId AS NVARCHAR(50))) = 0 AND PrescriptionId IS NOT NULL;
    
    PRINT 'Remaining invalid records: ' + CAST(@invalidCount AS NVARCHAR);
    
    -- If all looks good, commit the transaction
    IF @invalidCount = 0
    BEGIN
        COMMIT TRANSACTION;
        PRINT 'Fix applied successfully. Transaction committed.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT 'Fix failed. Transaction rolled back.';
    END
END
ELSE
BEGIN
    -- For string type, check if any commas remain
    DECLARE @invalidCount INT;
    SELECT @invalidCount = COUNT(*)
    FROM PrescriptionMedications
    WHERE PrescriptionId LIKE '%,%';
    
    PRINT 'Remaining records with commas: ' + CAST(@invalidCount AS NVARCHAR);
    
    -- If all looks good, commit the transaction
    IF @invalidCount = 0
    BEGIN
        COMMIT TRANSACTION;
        PRINT 'Fix applied successfully. Transaction committed.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT 'Fix failed. Transaction rolled back.';
    END
END

-- Check if a synonym needs to be created for compatibility
IF NOT EXISTS (SELECT * FROM sys.synonyms WHERE name = 'PrescriptionMedicines')
BEGIN
    PRINT 'Creating synonym PrescriptionMedicines for PrescriptionMedications...';
    CREATE SYNONYM PrescriptionMedicines FOR PrescriptionMedications;
END

PRINT 'Process completed.'; 