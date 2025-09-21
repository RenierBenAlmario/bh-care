USE [Barangay]
GO

-- Set proper options
SET QUOTED_IDENTIFIER ON;
GO

-- Find the doctor user by email
DECLARE @doctorUserId NVARCHAR(450);
SELECT @doctorUserId = Id FROM AspNetUsers WHERE Email = 'dicktor@example.com';
PRINT 'Doctor User ID: ' + ISNULL(@doctorUserId, 'NOT FOUND');

-- Find or create staff member for this user
DECLARE @doctorStaffId INT = NULL;
SELECT @doctorStaffId = Id FROM StaffMembers WHERE UserId = @doctorUserId;

IF @doctorStaffId IS NULL
BEGIN
    -- Create a staff member entry if none exists
    INSERT INTO StaffMembers (Name, Position, Department, UserId, IsActive, JoinDate, CreatedAt)
    VALUES ('doctor', 'Doctor', 'Medical', @doctorUserId, 1, GETDATE(), GETDATE());
    
    SET @doctorStaffId = SCOPE_IDENTITY();
    PRINT 'Created new staff member for doctor user with ID: ' + CAST(@doctorStaffId AS NVARCHAR);
END
ELSE
BEGIN
    PRINT 'Found existing staff member for doctor with ID: ' + CAST(@doctorStaffId AS NVARCHAR);
END

-- Clear existing permissions for this staff member
DELETE FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId;
PRINT 'Cleared existing staff permissions';

-- Current timestamp for GrantedAt
DECLARE @now DATETIME = GETUTCDATE();

-- Add ALL possible permissions for the doctor - this will ensure complete access
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @doctorStaffId, Id, @now FROM Permissions;

PRINT 'Added ALL permissions to doctor user';

-- Fix doctor's role if needed
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles ur JOIN AspNetRoles r ON ur.RoleId = r.Id WHERE ur.UserId = @doctorUserId AND r.Name = 'Doctor')
BEGIN
    -- Get the Doctor role ID
    DECLARE @doctorRoleId NVARCHAR(450);
    SELECT @doctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';
    
    IF @doctorRoleId IS NOT NULL
    BEGIN
        -- Add the doctor role
        INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@doctorUserId, @doctorRoleId);
        PRINT 'Added Doctor role to user';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: Doctor role not found in AspNetRoles';
    END
END
ELSE
BEGIN
    PRINT 'User already has Doctor role';
END

-- Make sure the doctor has some test appointments to see
DECLARE @patientId NVARCHAR(450);
SELECT TOP 1 @patientId = UserId FROM Patients ORDER BY CreatedAt DESC;

IF @patientId IS NOT NULL
BEGIN
    -- Check if doctor has any appointments
    DECLARE @appointmentCount INT;
    SELECT @appointmentCount = COUNT(*) FROM Appointments WHERE DoctorId = @doctorUserId;
    
    IF @appointmentCount = 0
    BEGIN
        -- Create some test appointments
        DECLARE @today DATE = CAST(GETDATE() AS DATE);
        DECLARE @tomorrow DATE = DATEADD(DAY, 1, @today);
        
        -- Today's appointment
        INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, AppointmentTime, Status, Type, CreatedAt, UpdatedAt)
        VALUES (@patientId, @doctorUserId, @today, '09:00', 1, 'General Checkup', @now, @now);
        
        -- Tomorrow's appointment
        INSERT INTO Appointments (PatientId, DoctorId, AppointmentDate, AppointmentTime, Status, Type, CreatedAt, UpdatedAt)
        VALUES (@patientId, @doctorUserId, @tomorrow, '10:30', 1, 'Follow-up', @now, @now);
        
        PRINT 'Added test appointments for doctor';
    END
    ELSE
    BEGIN
        PRINT 'Doctor already has ' + CAST(@appointmentCount AS NVARCHAR) + ' appointments';
    END
END
ELSE
BEGIN
    PRINT 'WARNING: No patients found to create test appointments';
END

-- Clear the permission cache by updating the user's UpdatedAt timestamp
UPDATE AspNetUsers SET UpdatedAt = GETUTCDATE() WHERE Id = @doctorUserId;
PRINT 'Updated user timestamp to clear permission cache';

-- Show assigned permissions
SELECT COUNT(*) AS 'Total Permissions Assigned'
FROM StaffPermissions 
WHERE StaffMemberId = @doctorStaffId;

-- Show appointments for this doctor
SELECT 
    a.Id,
    a.AppointmentDate,
    a.AppointmentTime,
    a.Status,
    a.Type,
    p.FullName AS PatientName
FROM 
    Appointments a
    INNER JOIN Patients p ON a.PatientId = p.UserId
WHERE 
    a.DoctorId = @doctorUserId
ORDER BY 
    a.AppointmentDate, a.AppointmentTime;

PRINT 'Doctor account fully configured with all permissions and test appointments';
GO 