USE [Barangay]
GO

-- Set proper options
SET QUOTED_IDENTIFIER ON;
GO

-- Find the doctor user by email
DECLARE @doctorUserId NVARCHAR(450);
SELECT @doctorUserId = Id FROM AspNetUsers WHERE Email = 'dicktor@example.com';

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

-- Get permission IDs for all required permissions
DECLARE @AccessDashboardId INT = (SELECT Id FROM Permissions WHERE Name = 'Access Dashboard');
DECLARE @ManageConsultationsId INT = (SELECT Id FROM Permissions WHERE Name = 'Manage Consultations');
DECLARE @CreatePrescriptionsId INT = (SELECT Id FROM Permissions WHERE Name = 'Create Prescriptions');
DECLARE @ViewPrescriptionsId INT = (SELECT Id FROM Permissions WHERE Name = 'View Prescriptions');
DECLARE @EditPrescriptionsId INT = (SELECT Id FROM Permissions WHERE Name = 'Edit Prescriptions');
DECLARE @ViewPatientDetailsId INT = (SELECT Id FROM Permissions WHERE Name = 'View Patient Details');
DECLARE @ViewMedicalRecordsId INT = (SELECT Id FROM Permissions WHERE Name = 'View Medical Records');
DECLARE @CreateMedicalRecordsId INT = (SELECT Id FROM Permissions WHERE Name = 'Create Medical Records');
DECLARE @ViewAppointmentsId INT = (SELECT Id FROM Permissions WHERE Name = 'View Appointments');

-- Current timestamp for GrantedAt
DECLARE @now DATETIME = GETUTCDATE();

-- Add all necessary permissions
INSERT INTO StaffPermissions (StaffMemberId, PermissionId, GrantedAt) VALUES
(@doctorStaffId, @AccessDashboardId, @now),
(@doctorStaffId, @ManageConsultationsId, @now),
(@doctorStaffId, @CreatePrescriptionsId, @now),
(@doctorStaffId, @ViewPrescriptionsId, @now),
(@doctorStaffId, @EditPrescriptionsId, @now),
(@doctorStaffId, @ViewPatientDetailsId, @now),
(@doctorStaffId, @ViewMedicalRecordsId, @now),
(@doctorStaffId, @CreateMedicalRecordsId, @now),
(@doctorStaffId, @ViewAppointmentsId, @now);

PRINT 'Added all required permissions to doctor user';

-- Clear the permission cache by updating the user's UpdatedAt timestamp
UPDATE AspNetUsers SET UpdatedAt = GETUTCDATE() WHERE Id = @doctorUserId;
PRINT 'Updated user timestamp to clear permission cache';

-- Show assigned permissions
SELECT 
    p.Name AS 'Permission Name', 
    p.Category AS 'Category'
FROM 
    Permissions p
    INNER JOIN StaffPermissions sp ON p.Id = sp.PermissionId
WHERE 
    sp.StaffMemberId = @doctorStaffId
ORDER BY 
    p.Category, p.Name;

PRINT 'Doctor user permissions updated successfully';
GO 