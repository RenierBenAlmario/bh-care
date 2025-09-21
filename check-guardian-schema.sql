-- Check the schema of GuardianInformation table
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS 
WHERE 
    TABLE_NAME = 'GuardianInformation'
ORDER BY 
    ORDINAL_POSITION; 