USE [Barangay]
GO

-- Set proper options
SET QUOTED_IDENTIFIER ON;
GO

-- Show all staff members
PRINT 'All staff members:'
SELECT 
    sm.Id, 
    sm.Name, 
    sm.Position, 
    sm.UserId
FROM 
    StaffMembers sm
ORDER BY sm.Name;

-- Get the doctor staff member
DECLARE @doctorStaffId INT;
SELECT @doctorStaffId = Id FROM StaffMembers WHERE Name = 'doctor';
PRINT 'Doctor Staff ID: ' + CAST(@doctorStaffId AS NVARCHAR);

-- Show all permissions available in the system
PRINT 'All permissions:'
SELECT 
    p.Id,
    p.Name,
    p.Category
FROM 
    Permissions p
ORDER BY 
    p.Category, p.Name;

-- Show current doctor permissions
PRINT 'Current doctor permissions:'
SELECT 
    p.Id,
    p.Name,
    p.Category
FROM 
    Permissions p
    INNER JOIN StaffPermissions sp ON p.Id = sp.PermissionId
WHERE 
    sp.StaffMemberId = @doctorStaffId
ORDER BY 
    p.Category, p.Name;

-- Check key permissions that should be assigned for the doctor's menu
PRINT 'Checking key permissions for doctor''s menu:'
DECLARE @requiredPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @requiredPermissions (Name) VALUES 
('Access Dashboard'),
('Manage Consultations'),
('Create Prescriptions'),
('View Prescriptions'),
('Edit Prescriptions'),
('View Patient Details'),
('View Medical Records');

-- Show which required permissions are missing
SELECT 
    rp.Name AS 'Required Permission',
    CASE 
        WHEN p.Id IS NULL THEN 'MISSING'
        ELSE 'ASSIGNED'
    END AS Status
FROM 
    @requiredPermissions rp
    LEFT JOIN Permissions p ON rp.Name = p.Name
    LEFT JOIN StaffPermissions sp ON p.Id = sp.PermissionId AND sp.StaffMemberId = @doctorStaffId;

-- Fix any missing permissions - add them to the doctor
BEGIN TRANSACTION;

DECLARE @AccessDashboardId INT, @ManageConsultationsId INT, @CreatePrescriptionsId INT,
        @ViewPrescriptionsId INT, @EditPrescriptionsId INT, @ViewPatientDetailsId INT, 
        @ViewMedicalRecordsId INT;

SELECT @AccessDashboardId = Id FROM Permissions WHERE Name = 'Access Dashboard';
SELECT @ManageConsultationsId = Id FROM Permissions WHERE Name = 'Manage Consultations';
SELECT @CreatePrescriptionsId = Id FROM Permissions WHERE Name = 'Create Prescriptions';
SELECT @ViewPrescriptionsId = Id FROM Permissions WHERE Name = 'View Prescriptions';
SELECT @EditPrescriptionsId = Id FROM Permissions WHERE Name = 'Edit Prescriptions';
SELECT @ViewPatientDetailsId = Id FROM Permissions WHERE Name = 'View Patient Details';
SELECT @ViewMedicalRecordsId = Id FROM Permissions WHERE Name = 'View Medical Records';

-- Add Access Dashboard permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @AccessDashboardId)
    AND @AccessDashboardId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @AccessDashboardId);
    PRINT 'Added Access Dashboard permission';
END

-- Add Manage Consultations permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @ManageConsultationsId)
    AND @ManageConsultationsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @ManageConsultationsId);
    PRINT 'Added Manage Consultations permission';
END

-- Add Create Prescriptions permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @CreatePrescriptionsId)
    AND @CreatePrescriptionsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @CreatePrescriptionsId);
    PRINT 'Added Create Prescriptions permission';
END

-- Add View Prescriptions permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @ViewPrescriptionsId)
    AND @ViewPrescriptionsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @ViewPrescriptionsId);
    PRINT 'Added View Prescriptions permission';
END

-- Add Edit Prescriptions permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @EditPrescriptionsId)
    AND @EditPrescriptionsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @EditPrescriptionsId);
    PRINT 'Added Edit Prescriptions permission';
END

-- Add View Patient Details permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @ViewPatientDetailsId)
    AND @ViewPatientDetailsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @ViewPatientDetailsId);
    PRINT 'Added View Patient Details permission';
END

-- Add View Medical Records permission if missing
IF NOT EXISTS (SELECT 1 FROM StaffPermissions WHERE StaffMemberId = @doctorStaffId AND PermissionId = @ViewMedicalRecordsId)
    AND @ViewMedicalRecordsId IS NOT NULL
BEGIN
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId) VALUES (@doctorStaffId, @ViewMedicalRecordsId);
    PRINT 'Added View Medical Records permission';
END

-- Clear cached permissions by updating the UpdatedAt timestamp on the user record
DECLARE @doctorUserId NVARCHAR(450);
SELECT @doctorUserId = UserId FROM StaffMembers WHERE Id = @doctorStaffId;

IF @doctorUserId IS NOT NULL
BEGIN
    UPDATE AspNetUsers SET UpdatedAt = GETUTCDATE() WHERE Id = @doctorUserId;
    PRINT 'Updated timestamp to clear cached permissions';
END

COMMIT TRANSACTION;

-- Verify the permissions after the fix
PRINT 'Doctor permissions after fix:';
SELECT 
    p.Id,
    p.Name,
    p.Category
FROM 
    Permissions p
    INNER JOIN StaffPermissions sp ON p.Id = sp.PermissionId
WHERE 
    sp.StaffMemberId = @doctorStaffId
ORDER BY 
    p.Category, p.Name; 