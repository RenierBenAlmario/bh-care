-- SQL Script to fix permission issues in the database
-- This script should be run to clean up any inconsistent permission data

-- 1. Delete orphaned UserPermissions (where the Permission no longer exists)
DELETE FROM UserPermissions 
WHERE PermissionId NOT IN (SELECT Id FROM Permissions);

-- 2. Delete orphaned UserPermissions (where the User no longer exists)
DELETE FROM UserPermissions 
WHERE UserId NOT IN (SELECT Id FROM AspNetUsers);

-- 3. Ensure all essential permissions exist
MERGE INTO Permissions AS target
USING (VALUES
    -- Dashboard permissions
    (1, 'Access Dashboard', 'Ability to access the main dashboard', 'Dashboard Access'),
    (2, 'Access Doctor Dashboard', 'Ability to access the doctor dashboard', 'Dashboard Access'),
    (3, 'Access Nurse Dashboard', 'Ability to access the nurse dashboard', 'Dashboard Access'),
    (4, 'Access Admin Dashboard', 'Ability to access the admin dashboard', 'Dashboard Access'),
    
    -- Appointment permissions
    (5, 'ManageAppointments', 'Ability to manage appointments', 'Appointments'),
    (6, 'View Appointments', 'Ability to view appointments', 'Appointments'),
    (7, 'Create Appointments', 'Ability to create appointments', 'Appointments'),
    
    -- Medical Records permissions
    (8, 'Manage Medical Records', 'Ability to manage medical records', 'Medical Records'),
    (9, 'View Patient History', 'Ability to view patient history', 'Medical Records'),
    (10, 'Create Medical Records', 'Ability to create medical records', 'Medical Records'),
    (11, 'Record Vital Signs', 'Ability to record vital signs', 'Medical Records'),
    (12, 'Create Prescriptions', 'Ability to create prescriptions', 'Medical Records'),
    (13, 'View Prescriptions', 'Ability to view prescriptions', 'Medical Records'),
    
    -- User Management permissions
    (14, 'Manage Users', 'Ability to manage users', 'User Management'),
    (15, 'Approve Users', 'Ability to approve new user registrations', 'User Management'),
    (16, 'Delete Users', 'Ability to delete users', 'User Management'),
    
    -- Administration permissions
    (17, 'Manage Permissions', 'Ability to manage user permissions', 'Administration'),
    
    -- Reporting permissions
    (18, 'View Reports', 'Ability to view reports', 'Reporting'),
    (19, 'Generate Reports', 'Ability to generate reports', 'Reporting')
) AS source (Id, Name, Description, Category)
ON target.Name = source.Name
WHEN NOT MATCHED BY target THEN
    INSERT (Name, Description, Category)
    VALUES (source.Name, source.Description, source.Category);

-- 4. Grant essential permissions to admin users
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Admin'
AND p.Name IN ('Access Dashboard', 'Access Admin Dashboard', 'Manage Users', 'Manage Permissions', 'View Reports')
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- 5. Grant essential permissions to doctor users
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Doctor'
AND p.Name IN ('Access Dashboard', 'Access Doctor Dashboard', 'ManageAppointments', 'Manage Medical Records', 'View Patient History', 'Create Prescriptions')
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- 6. Grant essential permissions to nurse users
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT u.Id, p.Id
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
CROSS JOIN Permissions p
WHERE r.Name = 'Nurse'
AND p.Name IN ('Access Dashboard', 'Access Nurse Dashboard', 'ManageAppointments', 'Record Vital Signs', 'View Patient History')
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = u.Id AND up.PermissionId = p.Id
);

-- 7. Update statistics
UPDATE STATISTICS UserPermissions;
UPDATE STATISTICS Permissions; 