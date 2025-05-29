-- Check the structure of the MedicalRecords table
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE 
FROM 
    INFORMATION_SCHEMA.COLUMNS 
WHERE 
    TABLE_NAME = 'MedicalRecords'
ORDER BY 
    ORDINAL_POSITION; 