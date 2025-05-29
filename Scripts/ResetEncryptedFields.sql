SET QUOTED_IDENTIFIER ON;
GO

-- Reset all encrypted fields to empty strings (not NULL to satisfy non-null constraints)
UPDATE [dbo].[AspNetUsers] SET 
    [EncryptedFullName] = N'',
    [EncryptedAddress] = N'',
    [EncryptedGender] = N'',
    [EncryptedSpecialization] = N'',
    [EncryptedWorkingDays] = N'',
    [EncryptedWorkingHours] = N'',
    [EncryptedProfileImage] = N'',
    [EncryptedStatus] = N''; 