-- Set required options
SET QUOTED_IDENTIFIER ON;
GO

-- Test to insert a basic user
DECLARE @userId NVARCHAR(450) = CONVERT(NVARCHAR(450), NEWID());

-- Insert a test user with ALL required fields
INSERT INTO AspNetUsers (
    -- Required fields (NOT NULL)
    Id, FullName, Specialization, WorkingDays, WorkingHours, ProfileImage, 
    MaxDailyPatients, IsActive, CreatedAt, EmailConfirmed, PhoneNumberConfirmed, 
    TwoFactorEnabled, LockoutEnabled, AccessFailedCount, MiddleName, Status, HasAgreedToTerms,

    -- Optional fields that we want to set
    UserName, NormalizedUserName, Email, NormalizedEmail, PasswordHash, 
    SecurityStamp, ConcurrencyStamp, FirstName, LastName, EncryptedStatus, EncryptedFullName
)
VALUES (
    -- Required fields (NOT NULL)
    @userId, 'Test User', '', '', '', '', 
    20, 0, GETDATE(), 1, 0, 
    0, 1, 0, '', 'Pending', 1,

    -- Optional fields that we want to set
    'testuser2', 'TESTUSER2', 'test2@example.com', 'TEST2@EXAMPLE.COM', 
    'AQAAAAIAAYagAAAAELcQq7GJD63G8d6w3oSMYZVHAvKfmMTJjkHpICQUCYJKlE/J6EbP55FQb78Q8NrEiw==',
    CONVERT(NVARCHAR(450), NEWID()), CONVERT(NVARCHAR(450), NEWID()), 'Test', NULL, '', ''
);

-- Print the ID
SELECT @userId AS UserId, 'testuser2' AS UserName; 