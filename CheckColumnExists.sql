-- Check if MiddleName and Suffix columns exist in AspNetUsers table
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'AspNetUsers' 
    AND COLUMN_NAME IN ('MiddleName', 'Suffix'); 