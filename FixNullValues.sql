-- Fix NULL values in AspNetUsers table
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- Update any NULL values in MiddleName column to empty string
BEGIN TRY
    UPDATE AspNetUsers 
    SET MiddleName = '' 
    WHERE MiddleName IS NULL;
    PRINT 'Fixed NULL values in MiddleName column';
END TRY
BEGIN CATCH
    PRINT 'Error fixing MiddleName NULL values: ' + ERROR_MESSAGE();
END CATCH

-- Update any NULL values in Suffix column to empty string
BEGIN TRY
    UPDATE AspNetUsers 
    SET Suffix = '' 
    WHERE Suffix IS NULL;
    PRINT 'Fixed NULL values in Suffix column';
END TRY
BEGIN CATCH
    PRINT 'Error fixing Suffix NULL values: ' + ERROR_MESSAGE();
END CATCH

-- Fix any NULL values in other important columns
BEGIN TRY
    UPDATE AspNetUsers 
    SET 
        FirstName = COALESCE(FirstName, ''),
        LastName = COALESCE(LastName, ''),
        FullName = COALESCE(FullName, ''),
        Email = COALESCE(Email, UserName),
        EncryptedStatus = COALESCE(EncryptedStatus, ''),
        EncryptedFullName = COALESCE(EncryptedFullName, ''),
        Gender = COALESCE(Gender, ''),
        Address = COALESCE(Address, ''),
        PhilHealthId = COALESCE(PhilHealthId, ''),
        ProfilePicture = COALESCE(ProfilePicture, ''),
        WorkingHours = COALESCE(WorkingHours, ''),
        Specialization = COALESCE(Specialization, '');
    PRINT 'Fixed NULL values in other columns';
END TRY
BEGIN CATCH
    PRINT 'Error fixing other NULL values: ' + ERROR_MESSAGE();
END CATCH 