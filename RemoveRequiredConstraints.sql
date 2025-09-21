-- Script to remove NOT NULL constraints from HEEADSSSAssessments table columns
PRINT 'Removing NOT NULL constraints from HEEADSSSAssessments columns...'

-- First check if the table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HEEADSSSAssessments')
BEGIN
    DECLARE @schema_name NVARCHAR(255) = 'dbo';
    DECLARE @table_name NVARCHAR(255) = 'HEEADSSSAssessments';
    DECLARE @column_name NVARCHAR(255);
    DECLARE @column_id INT;
    DECLARE @data_type NVARCHAR(255);
    DECLARE @max_length INT;
    DECLARE @is_nullable BIT;
    DECLARE @sql NVARCHAR(MAX);
    
    DECLARE column_cursor CURSOR FOR
    SELECT 
        c.name AS column_name, 
        c.column_id,
        t.name AS data_type,
        CASE 
            WHEN t.name IN ('nvarchar', 'varchar', 'char', 'nchar') THEN c.max_length / CASE WHEN t.name IN ('nvarchar', 'nchar') THEN 2 ELSE 1 END
            ELSE c.max_length 
        END AS max_length,
        c.is_nullable
    FROM 
        sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE 
        c.object_id = OBJECT_ID(@schema_name + '.' + @table_name)
        AND c.is_nullable = 0  -- Only get columns that are NOT NULL
        AND c.name NOT IN ('Id');  -- Skip primary key
    
    OPEN column_cursor;
    
    FETCH NEXT FROM column_cursor INTO @column_name, @column_id, @data_type, @max_length, @is_nullable;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @sql = 'ALTER TABLE ' + @schema_name + '.' + @table_name + 
                   ' ALTER COLUMN ' + @column_name + ' ' + @data_type + 
                   CASE 
                     WHEN @data_type IN ('varchar', 'nvarchar', 'char', 'nchar') THEN '(' + CASE WHEN @max_length = -1 THEN 'MAX' ELSE CAST(@max_length AS NVARCHAR) END + ')'
                     WHEN @data_type IN ('decimal', 'numeric') THEN '(18, 2)'  -- Default precision and scale
                     ELSE ''
                   END + ' NULL';
        
        PRINT 'Executing: ' + @sql;
        
        BEGIN TRY
            EXEC sp_executesql @sql;
            PRINT 'Successfully altered column ' + @column_name;
        END TRY
        BEGIN CATCH
            PRINT 'Error altering column ' + @column_name + ': ' + ERROR_MESSAGE();
        END CATCH
        
        FETCH NEXT FROM column_cursor INTO @column_name, @column_id, @data_type, @max_length, @is_nullable;
    END
    
    CLOSE column_cursor;
    DEALLOCATE column_cursor;
    
    PRINT 'Completed updating HEEADSSSAssessments table.';
END
ELSE
BEGIN
    PRINT 'Table HEEADSSSAssessments does not exist.';
END

-- Now do the same for NCDRiskAssessments table
PRINT 'Removing NOT NULL constraints from NCDRiskAssessments columns...'

-- Check if the table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'NCDRiskAssessments')
BEGIN
    DECLARE @ncd_schema_name NVARCHAR(255) = 'dbo';
    DECLARE @ncd_table_name NVARCHAR(255) = 'NCDRiskAssessments';
    DECLARE @ncd_column_name NVARCHAR(255);
    DECLARE @ncd_column_id INT;
    DECLARE @ncd_data_type NVARCHAR(255);
    DECLARE @ncd_max_length INT;
    DECLARE @ncd_is_nullable BIT;
    DECLARE @ncd_sql NVARCHAR(MAX);
    
    DECLARE ncd_column_cursor CURSOR FOR
    SELECT 
        c.name AS column_name, 
        c.column_id,
        t.name AS data_type,
        CASE 
            WHEN t.name IN ('nvarchar', 'varchar', 'char', 'nchar') THEN c.max_length / CASE WHEN t.name IN ('nvarchar', 'nchar') THEN 2 ELSE 1 END
            ELSE c.max_length 
        END AS max_length,
        c.is_nullable
    FROM 
        sys.columns c
        INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE 
        c.object_id = OBJECT_ID(@ncd_schema_name + '.' + @ncd_table_name)
        AND c.is_nullable = 0  -- Only get columns that are NOT NULL
        AND c.name NOT IN ('Id');  -- Skip primary key
    
    OPEN ncd_column_cursor;
    
    FETCH NEXT FROM ncd_column_cursor INTO @ncd_column_name, @ncd_column_id, @ncd_data_type, @ncd_max_length, @ncd_is_nullable;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @ncd_sql = 'ALTER TABLE ' + @ncd_schema_name + '.' + @ncd_table_name + 
                   ' ALTER COLUMN ' + @ncd_column_name + ' ' + @ncd_data_type + 
                   CASE 
                     WHEN @ncd_data_type IN ('varchar', 'nvarchar', 'char', 'nchar') THEN '(' + CASE WHEN @ncd_max_length = -1 THEN 'MAX' ELSE CAST(@ncd_max_length AS NVARCHAR) END + ')'
                     WHEN @ncd_data_type IN ('decimal', 'numeric') THEN '(18, 2)'  -- Default precision and scale
                     ELSE ''
                   END + ' NULL';
        
        PRINT 'Executing: ' + @ncd_sql;
        
        BEGIN TRY
            EXEC sp_executesql @ncd_sql;
            PRINT 'Successfully altered column ' + @ncd_column_name;
        END TRY
        BEGIN CATCH
            PRINT 'Error altering column ' + @ncd_column_name + ': ' + ERROR_MESSAGE();
        END CATCH
        
        FETCH NEXT FROM ncd_column_cursor INTO @ncd_column_name, @ncd_column_id, @ncd_data_type, @ncd_max_length, @ncd_is_nullable;
    END
    
    CLOSE ncd_column_cursor;
    DEALLOCATE ncd_column_cursor;
    
    PRINT 'Completed updating NCDRiskAssessments table.';
END
ELSE
BEGIN
    PRINT 'Table NCDRiskAssessments does not exist.';
END

PRINT 'Script completed.'; 