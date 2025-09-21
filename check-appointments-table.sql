-- Check if Appointments table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Appointments')
BEGIN
    PRINT 'Appointments table exists';
    
    -- Get column information
    SELECT 
        c.name AS ColumnName,
        t.name AS DataType,
        c.max_length AS MaxLength,
        c.is_nullable AS IsNullable
    FROM sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID('Appointments')
    ORDER BY c.column_id;
END
ELSE
BEGIN
    PRINT 'Appointments table does not exist';
END 