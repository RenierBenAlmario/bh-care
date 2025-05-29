-- Add sample permission data
-- This script adds sample permissions and assigns them to users

SET NOCOUNT ON;
PRINT 'Adding sample permission data...';

-- Add sample permissions if they don't exist
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'View Patients')
BEGIN
    INSERT INTO Permissions (Name, Description, Category)
    VALUES
    -- Patient Management
    ('View Patients', 'Can view patient records', 'Patient Management'),
    ('Add Patients', 'Can add new patients', 'Patient Management'),
    ('Edit Patients', 'Can edit patient information', 'Patient Management'),
    ('Delete Patients', 'Can delete patient records', 'Patient Management'),
    
    -- Prescription Management
    ('View Prescriptions', 'Can view prescriptions', 'Prescription Management'),
    ('Create Prescriptions', 'Can create new prescriptions', 'Prescription Management'),
    ('Edit Prescriptions', 'Can edit existing prescriptions', 'Prescription Management'),
    ('Cancel Prescriptions', 'Can cancel prescriptions', 'Prescription Management'),
    
    -- Medical Records
    ('View Medical Records', 'Can view patient medical records', 'Medical Records'),
    ('Add Medical Records', 'Can add new medical records', 'Medical Records'),
    ('Edit Medical Records', 'Can modify existing medical records', 'Medical Records'),
    ('Delete Medical Records', 'Can delete medical records', 'Medical Records'),
    
    -- Admin Functions
    ('Manage Users', 'Can manage user accounts', 'Administration'),
    ('View Reports', 'Can view system reports', 'Administration'),
    ('Configure System', 'Can change system configuration', 'Administration'),
    ('Audit Logs', 'Can view audit logs', 'Administration');
    
    PRINT 'Sample permissions added successfully.';
END
ELSE
BEGIN
    PRINT 'Sample permissions already exist.';
END

-- Sample user permissions assignment (assuming user IDs exist)
DECLARE @AdminUserId NVARCHAR(450) = (SELECT TOP 1 Id FROM Users WHERE Email LIKE '%admin%');
DECLARE @DoctorUserId NVARCHAR(450) = (SELECT TOP 1 Id FROM Users WHERE Email LIKE '%doctor%');
DECLARE @NurseUserId NVARCHAR(450) = (SELECT TOP 1 Id FROM Users WHERE Email LIKE '%nurse%');

-- If we found users, assign permissions
IF @AdminUserId IS NOT NULL
BEGIN
    -- Get all permission IDs
    DECLARE @AllPermissionIds NVARCHAR(MAX) = '';
    
    SELECT @AllPermissionIds = @AllPermissionIds + CAST(Id AS NVARCHAR(10)) + ','
    FROM Permissions;
    
    -- Remove trailing comma
    IF LEN(@AllPermissionIds) > 0
        SET @AllPermissionIds = LEFT(@AllPermissionIds, LEN(@AllPermissionIds) - 1);
    
    -- Assign all permissions to admin
    EXEC sp_AssignUserPermissions @AdminUserId, @AllPermissionIds;
    PRINT 'All permissions assigned to admin user.';
END

IF @DoctorUserId IS NOT NULL
BEGIN
    -- Get doctor-specific permission IDs
    DECLARE @DoctorPermissionIds NVARCHAR(MAX) = '';
    
    SELECT @DoctorPermissionIds = @DoctorPermissionIds + CAST(Id AS NVARCHAR(10)) + ','
    FROM Permissions
    WHERE Name IN (
        'View Patients', 'View Prescriptions', 'Create Prescriptions', 
        'Edit Prescriptions', 'View Medical Records', 'Add Medical Records', 
        'Edit Medical Records'
    );
    
    -- Remove trailing comma
    IF LEN(@DoctorPermissionIds) > 0
        SET @DoctorPermissionIds = LEFT(@DoctorPermissionIds, LEN(@DoctorPermissionIds) - 1);
    
    -- Assign permissions to doctor
    EXEC sp_AssignUserPermissions @DoctorUserId, @DoctorPermissionIds;
    PRINT 'Doctor permissions assigned.';
END

IF @NurseUserId IS NOT NULL
BEGIN
    -- Get nurse-specific permission IDs
    DECLARE @NursePermissionIds NVARCHAR(MAX) = '';
    
    SELECT @NursePermissionIds = @NursePermissionIds + CAST(Id AS NVARCHAR(10)) + ','
    FROM Permissions
    WHERE Name IN (
        'View Patients', 'View Prescriptions', 'View Medical Records'
    );
    
    -- Remove trailing comma
    IF LEN(@NursePermissionIds) > 0
        SET @NursePermissionIds = LEFT(@NursePermissionIds, LEN(@NursePermissionIds) - 1);
    
    -- Assign permissions to nurse
    EXEC sp_AssignUserPermissions @NurseUserId, @NursePermissionIds;
    PRINT 'Nurse permissions assigned.';
END

PRINT 'Sample data setup complete.';
GO 