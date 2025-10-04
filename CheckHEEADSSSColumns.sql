-- Script to check HEEADSSS Assessment table columns
-- Compare with the model to identify missing columns

-- Check if table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HEEADSSSAssessments')
BEGIN
    PRINT 'HEEADSSSAssessments table exists. Checking columns...';
    
    -- List all columns in the table
    SELECT 
        COLUMN_NAME as ColumnName,
        DATA_TYPE as DataType,
        IS_NULLABLE as IsNullable,
        CHARACTER_MAXIMUM_LENGTH as MaxLength,
        COLUMN_DEFAULT as DefaultValue
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'HEEADSSSAssessments'
    ORDER BY ORDINAL_POSITION;
    
    -- Check for specific missing columns based on the model
    PRINT 'Checking for missing columns...';
    
    -- Check for missing columns that should exist based on the model
    DECLARE @MissingColumns TABLE (ColumnName NVARCHAR(255));
    
    -- Insert missing columns
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'EducationBullyingExperience'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'ActivitiesInternetGadgetUse'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'DrugsStreetDrugs'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'SexualityGay'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'SexualityLesbian'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'SexualityBisexual'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'SafetyWeaponAccess'
    );
    
    INSERT INTO @MissingColumns (ColumnName)
    SELECT 'MissingColumn' WHERE NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = 'HEEADSSSAssessments' AND COLUMN_NAME = 'HomeRunawayThoughts'
    );
    
    -- Show missing columns
    SELECT COUNT(*) as MissingColumnCount FROM @MissingColumns;
    
    IF (SELECT COUNT(*) FROM @MissingColumns) > 0
    BEGIN
        PRINT 'Missing columns detected. Please check the model vs database schema.';
    END
    ELSE
    BEGIN
        PRINT 'All expected columns are present.';
    END
    
    -- Show table structure
    PRINT 'Current table structure:';
    SELECT 
        'ALTER TABLE [dbo].[HEEADSSSAssessments] ADD [' + COLUMN_NAME + '] ' + 
        CASE 
            WHEN DATA_TYPE = 'nvarchar' THEN 'NVARCHAR(MAX)'
            WHEN DATA_TYPE = 'varchar' THEN 'VARCHAR(MAX)'
            WHEN DATA_TYPE = 'bit' THEN 'BIT'
            WHEN DATA_TYPE = 'int' THEN 'INT'
            WHEN DATA_TYPE = 'datetime2' THEN 'DATETIME2'
            ELSE UPPER(DATA_TYPE)
        END +
        CASE WHEN IS_NULLABLE = 'YES' THEN ' NULL' ELSE ' NOT NULL' END +
        CASE WHEN COLUMN_DEFAULT IS NOT NULL THEN ' DEFAULT ' + COLUMN_DEFAULT ELSE '' END
        as AddColumnStatement
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'HEEADSSSAssessments'
    ORDER BY ORDINAL_POSITION;
    
END
ELSE
BEGIN
    PRINT 'HEEADSSSAssessments table does not exist!';
END
