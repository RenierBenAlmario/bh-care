-- Drop the existing trigger
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_AspNetUsers_AssignUserNumber')
BEGIN
    DROP TRIGGER TR_AspNetUsers_AssignUserNumber;
    PRINT 'Dropped existing trigger';
END
GO 