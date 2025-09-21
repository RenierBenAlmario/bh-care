-- SQL Script to make nurse permissions match the first screenshot
USE [Barangay]
GO

PRINT 'Fixing nurse permissions to match first screenshot...';

-- Find the staff ID for the nurse
DECLARE @NurseStaffId INT;
SELECT @NurseStaffId = Id FROM StaffMembers WHERE Role = 'Nurse' AND Name LIKE '%nurse%';

IF @NurseStaffId IS NULL
BEGIN
    PRINT 'Nurse staff member not found';
    RETURN;
END

PRINT 'Found nurse with StaffId: ' + CAST(@NurseStaffId AS NVARCHAR);

-- Get the UserId for this staff member
DECLARE @NurseUserId NVARCHAR(450);
SELECT @NurseUserId = UserId FROM StaffMembers WHERE Id = @NurseStaffId;

IF @NurseUserId IS NULL
BEGIN
    PRINT 'Nurse user ID not found';
    RETURN;
END

PRINT 'Found nurse with UserId: ' + @NurseUserId;

-- First, identify all the permissions we need to add
DECLARE @AppointmentsPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @AppointmentsPermissions (Name) VALUES 
('Create Appointments'),
('View Appointments');

DECLARE @DashboardPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @DashboardPermissions (Name) VALUES 
('Access Dashboard'),
('View Dashboard'),
('Access Nurse Dashboard');

DECLARE @DashboardAccessPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @DashboardAccessPermissions (Name) VALUES 
('Dashboard Access');

DECLARE @MedicalRecordsPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @MedicalRecordsPermissions (Name) VALUES 
('View Medical Records'),
('View Patient History'),
('Record Vital Signs');

DECLARE @PatientManagementPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @PatientManagementPermissions (Name) VALUES 
('View Patient Details');

DECLARE @PrescriptionsPermissions TABLE (Name NVARCHAR(100));
INSERT INTO @PrescriptionsPermissions (Name) VALUES 
('View Prescriptions');

-- Update UserPermissions table
PRINT 'Updating UserPermissions table...';

-- Add Appointments permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @AppointmentsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Add Dashboard permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @DashboardPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Add Dashboard Access permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @DashboardAccessPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Add Medical Records permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @MedicalRecordsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Add Patient Management permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @PatientManagementPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Add Prescriptions permissions
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT @NurseUserId, p.Id
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @PrescriptionsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = @NurseUserId AND up.PermissionId = p.Id
);

-- Update StaffPermissions table
PRINT 'Updating StaffPermissions table...';

-- Add Appointments permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @AppointmentsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Add Dashboard permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @DashboardPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Add Dashboard Access permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @DashboardAccessPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Add Medical Records permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @MedicalRecordsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Add Patient Management permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @PatientManagementPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Add Prescriptions permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
SELECT @NurseStaffId, p.Id, GETDATE()
FROM Permissions p
WHERE p.Name IN (SELECT Name FROM @PrescriptionsPermissions)
AND NOT EXISTS (
    SELECT 1 FROM StaffPermissions sp 
    WHERE sp.StaffMemberId = @NurseStaffId AND sp.PermissionId = p.Id
);

-- Make sure we have permissions for nurse with staff ID 3 specifically (from URL in second screenshot)
IF @NurseStaffId <> 3
BEGIN
    PRINT 'Also updating nurse with staff ID 3...';
    
    -- Get the UserId for this staff member
    DECLARE @Nurse3UserId NVARCHAR(450);
    SELECT @Nurse3UserId = UserId FROM StaffMembers WHERE Id = 3;
    
    IF @Nurse3UserId IS NOT NULL
    BEGIN
        -- Update UserPermissions
        INSERT INTO UserPermissions (UserId, PermissionId)
        SELECT @Nurse3UserId, p.Id
        FROM Permissions p
        WHERE p.Name IN (
            SELECT Name FROM @AppointmentsPermissions
            UNION
            SELECT Name FROM @DashboardPermissions
            UNION
            SELECT Name FROM @DashboardAccessPermissions
            UNION
            SELECT Name FROM @MedicalRecordsPermissions
            UNION
            SELECT Name FROM @PatientManagementPermissions
            UNION
            SELECT Name FROM @PrescriptionsPermissions
        )
        AND NOT EXISTS (
            SELECT 1 FROM UserPermissions up 
            WHERE up.UserId = @Nurse3UserId AND up.PermissionId = p.Id
        );
        
        -- Update StaffPermissions
        INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt)
        SELECT 3, p.Id, GETDATE()
        FROM Permissions p
        WHERE p.Name IN (
            SELECT Name FROM @AppointmentsPermissions
            UNION
            SELECT Name FROM @DashboardPermissions
            UNION
            SELECT Name FROM @DashboardAccessPermissions
            UNION
            SELECT Name FROM @MedicalRecordsPermissions
            UNION
            SELECT Name FROM @PatientManagementPermissions
            UNION
            SELECT Name FROM @PrescriptionsPermissions
        )
        AND NOT EXISTS (
            SELECT 1 FROM StaffPermissions sp 
            WHERE sp.StaffMemberId = 3 AND sp.PermissionId = p.Id
        );
    END
END

-- Update claims for the nurse user
PRINT 'Updating claims for nurse user...';

-- First, remove existing permission claims
DELETE FROM AspNetUserClaims 
WHERE UserId = @NurseUserId AND ClaimType = 'Permission';

-- Add new permission claims
INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT @NurseUserId, 'Permission', p.Name
FROM Permissions p
WHERE p.Name IN (
    SELECT Name FROM @AppointmentsPermissions
    UNION
    SELECT Name FROM @DashboardPermissions
    UNION
    SELECT Name FROM @DashboardAccessPermissions
    UNION
    SELECT Name FROM @MedicalRecordsPermissions
    UNION
    SELECT Name FROM @PatientManagementPermissions
    UNION
    SELECT Name FROM @PrescriptionsPermissions
);

-- Also update claims for staff ID 3 if different
IF @NurseStaffId <> 3 AND @Nurse3UserId IS NOT NULL
BEGIN
    -- Remove existing permission claims
    DELETE FROM AspNetUserClaims 
    WHERE UserId = @Nurse3UserId AND ClaimType = 'Permission';
    
    -- Add new permission claims
    INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
    SELECT @Nurse3UserId, 'Permission', p.Name
    FROM Permissions p
    WHERE p.Name IN (
        SELECT Name FROM @AppointmentsPermissions
        UNION
        SELECT Name FROM @DashboardPermissions
        UNION
        SELECT Name FROM @DashboardAccessPermissions
        UNION
        SELECT Name FROM @MedicalRecordsPermissions
        UNION
        SELECT Name FROM @PatientManagementPermissions
        UNION
        SELECT Name FROM @PrescriptionsPermissions
    );
END

PRINT 'Nurse permissions updated successfully.'; 