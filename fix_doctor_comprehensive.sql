-- Comprehensive Doctor Fix Script
-- This script ensures the doctor user is properly configured for consultation time selection

PRINT '=== COMPREHENSIVE DOCTOR FIX ===';

-- Step 1: Check current state
PRINT 'Step 1: Checking current state...';

-- Check if doctor user exists
SELECT COUNT(*) as 'Doctor User Count' FROM AspNetUsers WHERE Email = 'doctor@example.com';

-- Check if Doctor role exists
SELECT COUNT(*) as 'Doctor Role Count' FROM AspNetRoles WHERE Name = 'Doctor';

-- Check doctor role assignments
SELECT COUNT(*) as 'Doctor Role Assignments' FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id 
    WHERE u.Email = 'doctor@example.com' AND r.Name = 'Doctor';

-- Check doctor availability records
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

-- Step 3: Get doctor user ID
PRINT 'Step 3: Getting doctor user ID...';
DECLARE @DoctorUserId NVARCHAR(450);
DECLARE @DoctorRoleId NVARCHAR(450);

SELECT @DoctorUserId = Id FROM AspNetUsers WHERE Email = 'doctor@example.com';
SELECT @DoctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';

PRINT 'Doctor User ID: ' + ISNULL(@DoctorUserId, 'NOT FOUND');
PRINT 'Doctor Role ID: ' + ISNULL(@DoctorRoleId, 'NOT FOUND');

-- Step 4: Assign Doctor role to doctor user
PRINT 'Step 4: Assigning Doctor role...';
IF @DoctorUserId IS NOT NULL AND @DoctorRoleId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @DoctorUserId AND RoleId = @DoctorRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId)
        VALUES (@DoctorUserId, @DoctorRoleId);
        PRINT 'Doctor role assigned to doctor@example.com';
    END
    ELSE
    BEGIN
        PRINT 'Doctor role already assigned to doctor@example.com';
    END
END
ELSE
BEGIN
    PRINT 'ERROR: Cannot assign role - missing user or role ID';
END

-- Step 5: Create/update doctor availability record
PRINT 'Step 5: Creating doctor availability record...';
IF @DoctorUserId IS NOT NULL
BEGIN
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
END
ELSE
BEGIN
    PRINT 'ERROR: Cannot create availability record - missing doctor user ID';
END

-- Step 6: Final verification
PRINT 'Step 6: Final verification...';
SELECT 
    u.Id as UserId,
    u.Email,
    u.FullName,
    r.Name as RoleName,
    da.IsAvailable,
    da.StartTime,
    da.EndTime,
    da.Monday, da.Tuesday, da.Wednesday, da.Thursday, da.Friday, da.Saturday, da.Sunday
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
LEFT JOIN DoctorAvailabilities da ON u.Id = da.DoctorId
WHERE u.Email = 'doctor@example.com';

PRINT '=== DOCTOR FIX COMPLETED ===';

