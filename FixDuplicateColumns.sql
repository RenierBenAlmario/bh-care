-- Fix duplicate columns in AspNetUsers table
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

BEGIN TRY
    -- First check if there are duplicate column definitions
    DECLARE @columnCount INT;
    SELECT @columnCount = COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'MiddleName';
    
    IF @columnCount > 1
    BEGIN
        -- Drop the duplicate column - use a dynamic SQL approach to handle this safely
        -- First, get the constraint name if there is one
        DECLARE @constraintName NVARCHAR(128);
        SELECT @constraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
        JOIN sys.tables t ON c.object_id = t.object_id
        WHERE t.name = 'AspNetUsers' AND c.name = 'MiddleName';
        
        -- Drop the constraint if it exists
        IF @constraintName IS NOT NULL
        BEGIN
            DECLARE @sql NVARCHAR(500) = N'ALTER TABLE AspNetUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
            EXEC sp_executesql @sql;
            PRINT 'Dropped constraint on MiddleName column';
        END
        
        -- Drop one of the duplicate columns
        ALTER TABLE AspNetUsers DROP COLUMN MiddleName;
        PRINT 'Dropped duplicate MiddleName column';
        
        -- Add the column back properly
        ALTER TABLE AspNetUsers ADD MiddleName NVARCHAR(MAX) NOT NULL DEFAULT '';
        PRINT 'Re-added MiddleName column properly';
    END
    ELSE
    BEGIN
        PRINT 'No duplicate MiddleName column found';
    END
    
    -- Do the same for Suffix column
    SELECT @columnCount = COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME = 'Suffix';
    
    IF @columnCount > 1
    BEGIN
        -- Get the constraint name
        SELECT @constraintName = dc.name
        FROM sys.default_constraints dc
        JOIN sys.columns c ON dc.parent_column_id = c.column_id AND dc.parent_object_id = c.object_id
        JOIN sys.tables t ON c.object_id = t.object_id
        WHERE t.name = 'AspNetUsers' AND c.name = 'Suffix';
        
        -- Drop the constraint if it exists
        IF @constraintName IS NOT NULL
        BEGIN
            DECLARE @sql2 NVARCHAR(500) = N'ALTER TABLE AspNetUsers DROP CONSTRAINT ' + QUOTENAME(@constraintName);
            EXEC sp_executesql @sql2;
            PRINT 'Dropped constraint on Suffix column';
        END
        
        -- Drop one of the duplicate columns
        ALTER TABLE AspNetUsers DROP COLUMN Suffix;
        PRINT 'Dropped duplicate Suffix column';
        
        -- Add the column back properly
        ALTER TABLE AspNetUsers ADD Suffix NVARCHAR(MAX) NOT NULL DEFAULT '';
        PRINT 'Re-added Suffix column properly';
    END
    ELSE
    BEGIN
        PRINT 'No duplicate Suffix column found';
    END
END TRY
BEGIN CATCH
    PRINT 'Error fixing duplicate columns: ' + ERROR_MESSAGE();
END CATCH 