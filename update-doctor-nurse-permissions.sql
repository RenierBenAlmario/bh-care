
USE [Barangay];
GO

-- Step 1: Get Role IDs for Doctor and Nurse
DECLARE @DoctorRoleId NVARCHAR(450), @NurseRoleId NVARCHAR(450);
SELECT @DoctorRoleId = Id FROM AspNetRoles WHERE Name = 'Doctor';
SELECT @NurseRoleId = Id FROM AspNetRoles WHERE Name = 'Nurse';

-- Step 2: Get all UserIDs for Doctors and Nurses
-- Temporary tables to hold user IDs
CREATE TABLE #DoctorUsers (UserId NVARCHAR(450));
CREATE TABLE #NurseUsers (UserId NVARCHAR(450));

INSERT INTO #DoctorUsers (UserId)
SELECT UserId FROM AspNetUserRoles WHERE RoleId = @DoctorRoleId;

INSERT INTO #NurseUsers (UserId)
SELECT UserId FROM AspNetUserRoles WHERE RoleId = @NurseRoleId;

-- Step 3: Remove existing permissions from UserPermissions and AspNetUserClaims

-- Remove from UserPermissions
DELETE FROM UserPermissions WHERE UserId IN (SELECT UserId FROM #DoctorUsers);
DELETE FROM UserPermissions WHERE UserId IN (SELECT UserId FROM #NurseUsers);

-- Remove from AspNetUserClaims
DELETE FROM AspNetUserClaims WHERE UserId IN (SELECT UserId FROM #DoctorUsers) AND ClaimType = 'Permission';
DELETE FROM AspNetUserClaims WHERE UserId IN (SELECT UserId FROM #NurseUsers) AND ClaimType = 'Permission';

-- Step 4: Get Permission IDs for the new permissions
DECLARE @ManageAppointmentsPermissionId INT, @AccessNurseDashboardPermissionId INT;
SELECT @ManageAppointmentsPermissionId = Id FROM Permissions WHERE Name = 'ManageAppointments';
SELECT @AccessNurseDashboardPermissionId = Id FROM Permissions WHERE Name = 'Access Nurse Dashboard';

-- Step 5: Grant new permissions

-- Grant 'ManageAppointments' to all doctors
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT UserId, @ManageAppointmentsPermissionId FROM #DoctorUsers;

INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT UserId, 'Permission', 'ManageAppointments' FROM #DoctorUsers;

-- Grant 'Access Nurse Dashboard' to all nurses
INSERT INTO UserPermissions (UserId, PermissionId)
SELECT UserId, @AccessNurseDashboardPermissionId FROM #NurseUsers;

INSERT INTO AspNetUserClaims (UserId, ClaimType, ClaimValue)
SELECT UserId, 'Permission', 'Access Nurse Dashboard' FROM #NurseUsers;

-- Clean up temporary tables
DROP TABLE #DoctorUsers;
DROP TABLE #NurseUsers;

PRINT 'Doctor and Nurse permissions have been updated.';
GO
