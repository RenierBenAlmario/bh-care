-- Comprehensive SQL script to fix all permissions in the Barangay Health Care System
USE [Barangay]
GO

PRINT 'Starting comprehensive permissions fix...';

-- 1. Ensure all essential permissions exist
PRINT 'Ensuring all required permissions exist...';

-- Dashboard Access permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Dashboard', 'Basic access to the system dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Dashboard', 'View the system dashboard', 'Dashboard Access');
    PRINT 'Added permission: View Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access User Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access User Dashboard', 'Access to the user dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access User Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Admin Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Admin Dashboard', 'Access to the admin dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Admin Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Doctor Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Doctor Dashboard', 'Access to the doctor dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Doctor Dashboard';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Access Nurse Dashboard')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Access Nurse Dashboard', 'Access to the nurse dashboard', 'Dashboard Access');
    PRINT 'Added permission: Access Nurse Dashboard';
END

-- Appointment permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'ManageAppointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('ManageAppointments', 'Ability to manage appointments', 'Appointments');
    PRINT 'Added permission: ManageAppointments';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Appointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Appointments', 'Ability to view appointments', 'Appointments');
    PRINT 'Added permission: View Appointments';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Create Appointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Create Appointments', 'Ability to create new appointments', 'Appointments');
    PRINT 'Added permission: Create Appointments';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Book Appointments')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Book Appointments', 'Ability to book appointments', 'Appointments');
    PRINT 'Added permission: Book Appointments';
END

-- Medical Records permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Medical Records', 'Ability to view medical records', 'Medical Records');
    PRINT 'Added permission: View Medical Records';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Medical Records', 'Ability to manage medical records', 'Medical Records');
    PRINT 'Added permission: Manage Medical Records';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Create Medical Records')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Create Medical Records', 'Ability to create medical records', 'Medical Records');
    PRINT 'Added permission: Create Medical Records';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Patient History')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Patient History', 'Ability to view patient history', 'Medical Records');
    PRINT 'Added permission: View Patient History';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Record Vital Signs')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Record Vital Signs', 'Ability to record vital signs', 'Medical Records');
    PRINT 'Added permission: Record Vital Signs';
END

-- Prescription permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Create Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Create Prescriptions', 'Ability to create prescriptions', 'Prescriptions');
    PRINT 'Added permission: Create Prescriptions';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Prescriptions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Prescriptions', 'Ability to view prescriptions', 'Prescriptions');
    PRINT 'Added permission: View Prescriptions';
END

-- User Management permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Users')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Users', 'Ability to manage users', 'User Management');
    PRINT 'Added permission: Manage Users';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Approve Users')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Approve Users', 'Ability to approve new user registrations', 'User Management');
    PRINT 'Added permission: Approve Users';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Delete Users')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Delete Users', 'Ability to delete users', 'User Management');
    PRINT 'Added permission: Delete Users';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Manage Permissions')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Manage Permissions', 'Ability to manage permissions', 'User Management');
    PRINT 'Added permission: Manage Permissions';
END

-- Reporting permissions
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Reports')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('View Reports', 'Ability to view reports', 'Reporting');
    PRINT 'Added permission: View Reports';
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'Generate Reports')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES ('Generate Reports', 'Ability to generate reports', 'Reporting');
    PRINT 'Added permission: Generate Reports';
END

-- 2. Remove orphaned permissions
PRINT 'Cleaning up orphaned permissions...';

-- Delete UserPermissions where the Permission no longer exists
DELETE FROM UserPermissions 
WHERE PermissionId NOT IN (SELECT Id FROM Permissions);
PRINT 'Removed orphaned UserPermissions (missing Permission)';

-- Delete UserPermissions where the User no longer exists
DELETE FROM UserPermissions 
WHERE UserId NOT IN (SELECT Id FROM AspNetUsers);
PRINT 'Removed orphaned UserPermissions (missing User)';

-- Delete StaffPermissions where the Permission no longer exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffPermissions')
BEGIN
    DELETE FROM StaffPermissions 
    WHERE PermissionId NOT IN (SELECT Id FROM Permissions);
    PRINT 'Removed orphaned StaffPermissions (missing Permission)';

    -- Delete StaffPermissions where the StaffMember no longer exists
    DELETE FROM StaffPermissions 
    WHERE StaffMemberId NOT IN (SELECT Id FROM StaffMembers);
    PRINT 'Removed orphaned StaffPermissions (missing StaffMember)';
END

-- 3. Grant permissions based on roles
PRINT 'Granting permissions based on roles...';

-- Admin role permissions
PRINT 'Granting permissions to Admin users...';
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Admin' OR r.Name = 'System Administrator'
AND p.Name IN (
    'Access Dashboard', 
    'View Dashboard', 
    'Access Admin Dashboard', 
    'Manage Users', 
    'Approve Users', 
    'Delete Users', 
    'Manage Permissions', 
    'View Reports', 
    'Generate Reports',
    'ManageAppointments', 
    'View Appointments', 
    'Create Appointments',
    'Manage Medical Records', 
    'View Medical Records',
    'View Patient History',
    'Create Prescriptions',
    'View Prescriptions'
)
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

-- Doctor role permissions
PRINT 'Granting permissions to Doctor users...';
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Doctor'
AND p.Name IN (
    'Access Dashboard', 
    'View Dashboard', 
    'Access Doctor Dashboard', 
    'ManageAppointments', 
    'View Appointments', 
    'Create Appointments',
    'Manage Medical Records', 
    'View Medical Records',
    'Create Medical Records',
    'View Patient History',
    'Record Vital Signs',
    'Create Prescriptions',
    'View Prescriptions',
    'View Reports'
)
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

-- Nurse role permissions
PRINT 'Granting permissions to Nurse users...';
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Nurse'
AND p.Name IN (
    'Access Dashboard', 
    'View Dashboard', 
    'Access Nurse Dashboard', 
    'ManageAppointments', 
    'View Appointments', 
    'Create Appointments',
    'View Medical Records',
    'View Patient History',
    'Record Vital Signs',
    'View Prescriptions'
)
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

-- Patient role permissions
PRINT 'Granting permissions to Patient users...';
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'PATIENT'
AND p.Name IN (
    'Access Dashboard',
    'View Dashboard',
    'Access User Dashboard',
    'View Medical Records',
    'Book Appointments',
    'View Appointments',
    'View Prescriptions'
)
AND NOT EXISTS (
    SELECT 1 
    FROM UserPermissions up 
    WHERE up.UserId = u.Id 
    AND up.PermissionId = p.Id
);

-- 4. Update the claims to match permissions
PRINT 'Updating user claims to match permissions...';

-- First, remove all permission claims
DELETE FROM AspNetUserClaims 
WHERE ClaimType = 'Permission';
PRINT 'Removed all existing permission claims';

-- Add permission claims based on UserPermissions
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT DISTINCT up.UserId, 'Permission', p.Name
FROM UserPermissions up
JOIN Permissions p ON up.PermissionId = p.Id;
PRINT 'Added permission claims based on UserPermissions table';

-- 5. Update staff permissions if the table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'StaffPermissions')
BEGIN
    PRINT 'Updating StaffPermissions table...';
    
    -- Doctor staff permissions
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
    SELECT s.Id, p.Id, GETDATE()
    FROM StaffMembers s
    CROSS JOIN Permissions p
    WHERE s.Role = 'Doctor'
    AND p.Name IN (
        'Access Dashboard', 
        'View Dashboard', 
        'Access Doctor Dashboard', 
        'ManageAppointments', 
        'View Appointments', 
        'Create Appointments',
        'Manage Medical Records', 
        'View Medical Records',
        'Create Medical Records',
        'View Patient History',
        'Record Vital Signs',
        'Create Prescriptions',
        'View Prescriptions',
        'View Reports'
    )
    AND NOT EXISTS (
        SELECT 1 
        FROM StaffPermissions sp 
        WHERE sp.StaffMemberId = s.Id 
        AND sp.PermissionId = p.Id
    );
    
    -- Nurse staff permissions
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
    SELECT s.Id, p.Id, GETDATE()
    FROM StaffMembers s
    CROSS JOIN Permissions p
    WHERE s.Role = 'Nurse'
    AND p.Name IN (
        'Access Dashboard', 
        'View Dashboard', 
        'Access Nurse Dashboard', 
        'ManageAppointments', 
        'View Appointments', 
        'Create Appointments',
        'View Medical Records',
        'View Patient History',
        'Record Vital Signs',
        'View Prescriptions'
    )
    AND NOT EXISTS (
        SELECT 1 
        FROM StaffPermissions sp 
        WHERE sp.StaffMemberId = s.Id 
        AND sp.PermissionId = p.Id
    );
    
    -- Admin staff permissions
    INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
    SELECT s.Id, p.Id, GETDATE()
    FROM StaffMembers s
    CROSS JOIN Permissions p
    WHERE s.Role = 'Admin'
    AND NOT EXISTS (
        SELECT 1 
        FROM StaffPermissions sp 
        WHERE sp.StaffMemberId = s.Id 
        AND sp.PermissionId = p.Id
    );
END

-- 6. Ensure role claims exist
PRINT 'Ensuring role claims exist...';
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT DISTINCT u.Id, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', r.Name
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE NOT EXISTS (
    SELECT 1 
    FROM AspNetUserClaims c 
    WHERE c.UserId = u.Id 
    AND c.ClaimType = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' 
    AND c.ClaimValue = r.Name
);

PRINT 'Permission fix completed successfully.'; 