-- SQL Script to fix permission assignments for Nurse and Doctor roles
USE [Barangay]
GO

PRINT 'Fixing permissions for Nurse and Doctor roles';

-- First, ensure all required permissions exist
INSERT INTO Permissions (Name, Category, Description)
VALUES 
('Access Dashboard', 'Dashboard', 'Access to the main dashboard'),
('Access Nurse Dashboard', 'Dashboard', 'Access to the nurse dashboard'),
('Access Doctor Dashboard', 'Dashboard', 'Access to the doctor dashboard'),
('View Patients', 'Patient', 'View patient records'),
('Manage Patients', 'Patient', 'Create and manage patient records'),
('View Appointments', 'Appointment', 'View appointment details'),
('Manage Appointments', 'Appointment', 'Create and manage appointments'),
('View Medical Records', 'Medical', 'View patient medical records'),
('Create Medical Records', 'Medical', 'Create patient medical records'),
('View Vital Signs', 'Medical', 'View patient vital signs'),
('Record Vital Signs', 'Medical', 'Record patient vital signs'),
('View Prescriptions', 'Prescription', 'View patient prescriptions'),
('Create Prescriptions', 'Prescription', 'Create patient prescriptions'),
('View Reports', 'Report', 'View system reports'),
('Generate Reports', 'Report', 'Generate system reports'),
('View Patient History', 'Patient', 'View patient medical history'),
('Access Vital Signs', 'Medical', 'Access vital signs module'),
('View Vital Signs Data', 'Medical', 'View vital signs data'),
('Record Vital Signs Data', 'Medical', 'Record new vital signs data'),
('View Patient Details', 'Patient', 'View detailed patient information')
WHERE NOT EXISTS (
    SELECT 1 FROM Permissions p 
    WHERE p.Name = VALUES(Name) AND p.Category = VALUES(Category)
);

-- Get role IDs
DECLARE @NurseRoleId NVARCHAR(450);
DECLARE @DoctorRoleId NVARCHAR(450);

SELECT @NurseRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'NURSE';
SELECT @DoctorRoleId = Id FROM AspNetRoles WHERE NormalizedName = 'DOCTOR';

IF @NurseRoleId IS NULL
BEGIN
    PRINT 'Nurse role not found. Creating it.';
    SET @NurseRoleId = NEWID();
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
    VALUES (@NurseRoleId, 'Nurse', 'NURSE');
END

IF @DoctorRoleId IS NULL
BEGIN
    PRINT 'Doctor role not found. Creating it.';
    SET @DoctorRoleId = NEWID();
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
    VALUES (@DoctorRoleId, 'Doctor', 'DOCTOR');
END

PRINT 'Nurse Role ID: ' + @NurseRoleId;
PRINT 'Doctor Role ID: ' + @DoctorRoleId;

-- Clear existing role permissions to avoid duplicates
DELETE FROM RolePermissions WHERE RoleId IN (@NurseRoleId, @DoctorRoleId);

-- Assign Nurse permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT @NurseRoleId, p.Id
FROM Permissions p
WHERE p.Name IN (
    'Access Dashboard',
    'Access Nurse Dashboard',
    'View Patients',
    'View Appointments', 
    'View Medical Records',
    'View Vital Signs',
    'Record Vital Signs',
    'View Prescriptions',
    'View Reports',
    'View Patient History',
    'Access Vital Signs',
    'View Vital Signs Data',
    'Record Vital Signs Data',
    'View Patient Details'
);

-- Assign Doctor permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT @DoctorRoleId, p.Id
FROM Permissions p
WHERE p.Name IN (
    'Access Dashboard',
    'Access Doctor Dashboard',
    'View Patients',
    'Manage Patients',
    'View Appointments',
    'Manage Appointments',
    'View Medical Records',
    'Create Medical Records',
    'View Vital Signs',
    'View Prescriptions',
    'Create Prescriptions',
    'View Reports',
    'Generate Reports',
    'View Patient History',
    'Access Vital Signs',
    'View Vital Signs Data',
    'View Patient Details'
);

-- Propagate permissions from roles to users
PRINT 'Propagating role permissions to users';

-- First remove any existing user permissions for these specified permissions to avoid duplicates
DELETE FROM UserPermissions 
WHERE UserId IN (SELECT UserId FROM AspNetUserRoles WHERE RoleId IN (@NurseRoleId, @DoctorRoleId))
AND PermissionId IN (
    SELECT p.Id FROM Permissions p 
    WHERE p.Name IN (
        'Access Dashboard', 'Access Nurse Dashboard', 'Access Doctor Dashboard',
        'View Patients', 'Manage Patients', 'View Appointments', 'Manage Appointments',
        'View Medical Records', 'Create Medical Records', 'View Vital Signs', 'Record Vital Signs',
        'View Prescriptions', 'Create Prescriptions', 'View Reports', 'Generate Reports',
        'View Patient History', 'Access Vital Signs', 'View Vital Signs Data', 
        'Record Vital Signs Data', 'View Patient Details'
    )
);

-- Now add the permissions from roles
INSERT INTO UserPermissions (UserId, PermissionId, CreatedAt, UpdatedAt)
SELECT ur.UserId, rp.PermissionId, GETDATE(), GETDATE()
FROM AspNetUserRoles ur
JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
WHERE ur.RoleId IN (@NurseRoleId, @DoctorRoleId)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = ur.UserId AND up.PermissionId = rp.PermissionId
);

-- Check if there are any StaffMembers that need permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT sm.Id, rp.PermissionId, GETDATE()
FROM StaffMembers sm
JOIN AspNetUsers u ON sm.UserId = u.Id
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
WHERE ur.RoleId IN (@NurseRoleId, @DoctorRoleId)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = sm.Id AND sp.PermissionId = rp.PermissionId
);

-- Summary
DECLARE @NurseCount INT, @DoctorCount INT;
SELECT @NurseCount = COUNT(*) FROM AspNetUserRoles WHERE RoleId = @NurseRoleId;
SELECT @DoctorCount = COUNT(*) FROM AspNetUserRoles WHERE RoleId = @DoctorRoleId;

PRINT 'Fixed permissions for ' + CAST(@NurseCount AS NVARCHAR) + ' nurses and ' + CAST(@DoctorCount AS NVARCHAR) + ' doctors.';
PRINT 'Permission fix completed.'; 