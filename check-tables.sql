-- Check all tables in the database
SELECT 
    table_name
FROM 
    information_schema.tables
WHERE 
    table_type = 'BASE TABLE'
ORDER BY 
    table_name; 