-- Comprehensive Doctor Setup Script
-- This script ensures proper doctor configuration for consultation time selection

PRINT '=== COMPREHENSIVE DOCTOR SETUP ===';

-- Step 1: Check current state
PRINT 'Step 1: Checking current state...';
SELECT COUNT(*) as 'Total Users' FROM AspNetUsers;
SELECT COUNT(*) as 'Total Doctors' FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor';
SELECT COUNT(*) as 'Doctor Availability Records' FROM DoctorAvailabilities;

-- Step 2: Ensure Doctor role exists
PRINT 'Step 2: Ensuring Doctor role exists...';
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Doctor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Doctor', 'DOCTOR', NEWID());
    PRINT 'Doctor role created';
END
ELSE
BEGIN
    PRINT 'Doctor role already exists';
END

-- Step 3: Create or update doctor@example.com user
PRINT 'Step 3: Creating/updating doctor@example.com user...';
DECLARE @DoctorUserId NVARCHAR(450);
DECLARE @DoctorRoleId NVARCHAR(450);

-- Get or create doctor user
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'doctor@example.com')
BEGIN
    SET @DoctorUserId = NEWID();
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed,
        TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, FirstName, LastName
    )
    VALUES (
        @DoctorUserId, 'doctor@example.com', 'DOCTOR@EXAMPLE.COM', 'doctor@example.com', 'DOCTOR@EXAMPLE.COM', 1,
        'AQAAAAEAACcQAAAAEExamplePasswordHash', NEWID(), NEWID(), 0, 0, 1, 0,
        'Dr. John Smith', 'John', 'Smith'
    );
    PRINT 'Doctor user created';
END
ELSE
BEGIN
    SELECT @DoctorUserId = Id FROM AspNetUsers WHERE Email = 'doctor@example.com';
    PRINT 'Doctor user already exists';
END

-- Step 4: Assign Doctor role
PRINT 'Step 4: Assigning Doctor role...';
SELECT @DoctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';

IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @DoctorUserId AND RoleId = @DoctorRoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    VALUES (@DoctorUserId, @DoctorRoleId);
    PRINT 'Doctor role assigned';
END
ELSE
BEGIN
    PRINT 'Doctor role already assigned';
END

-- Step 5: Create doctor availability record
PRINT 'Step 5: Creating doctor availability record...';
IF NOT EXISTS (SELECT 1 FROM DoctorAvailabilities WHERE DoctorId = @DoctorUserId)
BEGIN
    INSERT INTO DoctorAvailabilities (
        DoctorId, IsAvailable, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday,
        StartTime, EndTime, LastUpdated
    )
    VALUES (
        @DoctorUserId, 1, 1, 1, 1, 1, 1, 1, 1,
        '08:00:00', '17:00:00', GETDATE()
    );
    PRINT 'Doctor availability record created';
END
ELSE
BEGIN
    -- Update existing record to ensure it's active
    UPDATE DoctorAvailabilities 
    SET IsAvailable = 1, 
        Monday = 1, Tuesday = 1, Wednesday = 1, Thursday = 1, Friday = 1, Saturday = 1, Sunday = 1,
        StartTime = '08:00:00', EndTime = '17:00:00', LastUpdated = GETDATE()
    WHERE DoctorId = @DoctorUserId;
    PRINT 'Doctor availability record updated';
END

-- Step 6: Verification
PRINT 'Step 6: Verification...';
SELECT 
    u.Id as UserId,
    u.Email,
    u.FullName,
    r.Name as RoleName,
    da.IsAvailable,
    da.StartTime,
    da.EndTime
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
WHERE r.Name = 'Doctor';

PRINT '=== DOCTOR SETUP COMPLETED ===';

