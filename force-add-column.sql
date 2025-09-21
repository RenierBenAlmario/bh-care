-- Force add the UserNumber column
BEGIN TRY
    PRINT 'Adding UserNumber column to AspNetUsers';
    ALTER TABLE AspNetUsers ADD UserNumber INT NOT NULL DEFAULT 0;
    PRINT 'Column added successfully';
    
    -- Number existing users
    PRINT 'Numbering existing users...';
    WITH NumberedUsers AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt) AS RowNum
        FROM AspNetUsers
    )
    UPDATE AspNetUsers
    SET UserNumber = NumberedUsers.RowNum
    FROM NumberedUsers
    WHERE AspNetUsers.Id = NumberedUsers.Id;
    PRINT 'Users numbered successfully';
END TRY
BEGIN CATCH
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 