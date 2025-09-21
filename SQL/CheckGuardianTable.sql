-- Check if GuardianInformation table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'GuardianInformation')
BEGIN
    PRINT 'GuardianInformation table exists'
    
    -- List all columns in the table
    SELECT 
        c.name AS ColumnName, 
        t.name AS DataType,
        c.max_length AS MaxLength,
        c.is_nullable AS IsNullable,
        c.is_identity AS IsIdentity,
        CASE WHEN pk.column_id IS NOT NULL THEN 'YES' ELSE 'NO' END AS IsPrimaryKey
    FROM 
        sys.columns c
    INNER JOIN 
        sys.types t ON c.user_type_id = t.user_type_id
    LEFT JOIN 
        (SELECT i.object_id, ic.column_id
         FROM sys.index_columns ic
         JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id
         WHERE i.is_primary_key = 1) pk
        ON pk.object_id = c.object_id AND pk.column_id = c.column_id
    WHERE 
        c.object_id = OBJECT_ID('GuardianInformation')
    ORDER BY 
        c.column_id;
    
    -- Get row count
    SELECT COUNT(*) AS RowCount FROM GuardianInformation;
    
    -- Sample data (first 5 rows)
    SELECT TOP 5 * FROM GuardianInformation;
END
ELSE
BEGIN
    PRINT 'GuardianInformation table does not exist'
END 