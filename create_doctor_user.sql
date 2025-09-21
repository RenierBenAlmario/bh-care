-- Create a doctor user for testing
-- This script creates a doctor user and assigns them the Doctor role

-- First, check if the Doctor role exists
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Doctor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Doctor', 'DOCTOR', NEWID())
    PRINT 'Created Doctor role'
END
ELSE
    PRINT 'Doctor role already exists'

-- Create a doctor user
DECLARE @DoctorId NVARCHAR(450) = NEWID()
DECLARE @DoctorRoleId NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Doctor')

-- Check if doctor user already exists
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'doctor@example.com')
BEGIN
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, 
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnd, 
        LockoutEnabled, AccessFailedCount, FirstName, LastName, FullName,
        IsActive, CreatedAt, UpdatedAt, Status
    )
    VALUES (
        @DoctorId,
        'doctor@example.com',
        'DOCTOR@EXAMPLE.COM',
        'doctor@example.com',
        'DOCTOR@EXAMPLE.COM',
        1, -- EmailConfirmed
        'AQAAAAEAACcQAAAAEExamplePasswordHash', -- This is a placeholder - you'll need to set a real password
        NEWID(),
        NEWID(),
        NULL, -- PhoneNumber
        0, -- PhoneNumberConfirmed
        0, -- TwoFactorEnabled
        NULL, -- LockoutEnd
        1, -- LockoutEnabled
        0, -- AccessFailedCount
        'Dr. John',
        'Smith',
        'Dr. John Smith',
        1, -- IsActive
        GETUTCDATE(), -- CreatedAt
        GETUTCDATE(), -- UpdatedAt
        'Verified' -- Status
    )
    
    -- Assign Doctor role
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@DoctorId, @DoctorRoleId)
    
    PRINT 'Created doctor user: doctor@example.com'
END
ELSE
    PRINT 'Doctor user already exists'

-- Verify the doctor user was created
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.IsActive,
    r.Name AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor'
