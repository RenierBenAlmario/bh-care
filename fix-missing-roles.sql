-- SQL Script to add missing roles to AspNetRoles table
-- This addresses the error: "Role USER does not exist"

-- Set proper options to handle QUOTED_IDENTIFIER issue
SET QUOTED_IDENTIFIER ON
GO

PRINT 'Checking and adding missing roles...'

-- Check for USER role and add if missing
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'USER')
BEGIN
    DECLARE @UserRoleId NVARCHAR(450) = NEWID() -- Generate a new GUID for the role ID
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@UserRoleId, 'User', 'USER', NEWID())
    
    PRINT 'Added missing role: User'
END
ELSE
BEGIN
    PRINT 'User role already exists'
END

-- Check for other important roles
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    DECLARE @AdminRoleId NVARCHAR(450) = NEWID()
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@AdminRoleId, 'Admin', 'ADMIN', NEWID())
    
    PRINT 'Added missing role: Admin'
END
ELSE
BEGIN
    PRINT 'Admin role already exists'
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'DOCTOR')
BEGIN
    DECLARE @DoctorRoleId NVARCHAR(450) = NEWID()
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@DoctorRoleId, 'Doctor', 'DOCTOR', NEWID())
    
    PRINT 'Added missing role: Doctor'
END
ELSE
BEGIN
    PRINT 'Doctor role already exists'
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'PATIENT')
BEGIN
    DECLARE @PatientRoleId NVARCHAR(450) = NEWID()
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@PatientRoleId, 'Patient', 'PATIENT', NEWID())
    
    PRINT 'Added missing role: Patient'
END
ELSE
BEGIN
    PRINT 'Patient role already exists'
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'NURSE')
BEGIN
    DECLARE @NurseRoleId NVARCHAR(450) = NEWID()
    
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@NurseRoleId, 'Nurse', 'NURSE', NEWID())
    
    PRINT 'Added missing role: Nurse'
END
ELSE
BEGIN
    PRINT 'Nurse role already exists'
END

PRINT 'Role verification completed.'
GO 