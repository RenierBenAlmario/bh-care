-- Create the trigger for UserNumber assignment
CREATE TRIGGER TR_AspNetUsers_AssignUserNumber
ON AspNetUsers
AFTER INSERT
AS
BEGIN
    DECLARE @MaxUserNumber int;

    -- Get the current maximum UserNumber
    SELECT @MaxUserNumber = ISNULL(MAX(UserNumber), 0) FROM AspNetUsers;

    -- Update the inserted records with incremented UserNumber
    UPDATE AspNetUsers
    SET UserNumber = @MaxUserNumber + 
        (SELECT ROW_NUMBER() OVER (ORDER BY i.CreatedAt) FROM inserted i WHERE i.Id = AspNetUsers.Id)
    FROM AspNetUsers
    INNER JOIN inserted ON AspNetUsers.Id = inserted.Id
    WHERE AspNetUsers.UserNumber = 0;
END;
GO

PRINT 'Created trigger TR_AspNetUsers_AssignUserNumber';

-- Show sample data
PRINT 'Sample data:';
SELECT TOP 5 Id, UserName, Email, UserNumber
FROM AspNetUsers
ORDER BY UserNumber; 