-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Add UserNumber column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'UserNumber')
BEGIN
    -- Add the column
    ALTER TABLE [AspNetUsers] ADD [UserNumber] int NULL;
    PRINT 'UserNumber column added to AspNetUsers table';
    
    -- Now update it with sequential values in a separate statement
    DECLARE @Sql nvarchar(max) = '
    ;WITH UsersCTE AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY Id) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = UsersCTE.RowNum
    FROM AspNetUsers INNER JOIN UsersCTE ON AspNetUsers.Id = UsersCTE.Id';
    
    EXEC sp_executesql @Sql;
    PRINT 'UserNumber column populated with sequential values';
END
ELSE
BEGIN
    PRINT 'UserNumber column already exists in AspNetUsers table';
END 