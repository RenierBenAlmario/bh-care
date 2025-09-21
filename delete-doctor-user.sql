-- SQL Script to delete the doctor@example.com user with ID 6e497297-54b5-4dc7-8d41-d4c176cbd142
USE [Barangay]
GO

-- Set proper options
SET QUOTED_IDENTIFIER ON;
GO

-- Begin Transaction for safety
BEGIN TRANSACTION;

DECLARE @userId NVARCHAR(450) = '6e497297-54b5-4dc7-8d41-d4c176cbd142';
DECLARE @email NVARCHAR(256) = 'doctor@example.com';

-- Print the user we are deleting for verification
SELECT Id, UserName, Email, NormalizedEmail 
FROM AspNetUsers 
WHERE Id = @userId OR Email = @email;

-- Delete related records first to avoid foreign key constraint errors
-- Delete from AspNetUserRoles
DELETE FROM AspNetUserRoles WHERE UserId = @userId;

-- Delete from AspNetUserClaims
DELETE FROM AspNetUserClaims WHERE UserId = @userId;

-- Delete from AspNetUserLogins
DELETE FROM AspNetUserLogins WHERE UserId = @userId;

-- Delete from AspNetUserTokens
DELETE FROM AspNetUserTokens WHERE UserId = @userId;

-- Delete from UserPermissions
DELETE FROM UserPermissions WHERE UserId = @userId;

-- Delete from Appointments where the user is a doctor
DELETE FROM Appointments WHERE DoctorId = @userId;

-- Delete from UserDocuments
DELETE FROM UserDocuments WHERE UserId = @userId;

-- Delete any Prescriptions created by this doctor
DELETE FROM PrescriptionMedications WHERE PrescriptionId IN (
    SELECT Id FROM Prescriptions WHERE DoctorId = @userId
);
DELETE FROM Prescriptions WHERE DoctorId = @userId;

-- Finally, delete the user itself
DELETE FROM AspNetUsers WHERE Id = @userId;

-- Check if deletion was successful
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = @userId OR Email = @email)
BEGIN
    PRINT 'User with ID ' + @userId + ' and email ' + @email + ' deleted successfully.';
    -- If all operations were successful, commit the transaction
    COMMIT TRANSACTION;
END
ELSE
BEGIN
    PRINT 'Failed to delete user with ID ' + @userId + ' and email ' + @email;
    ROLLBACK TRANSACTION;
    RAISERROR('User deletion failed', 16, 1);
END 