-- Create missing roles for BHCARE application
-- This script ensures all required roles exist in the database

SET QUOTED_IDENTIFIER ON
SET ANSI_NULLS ON

-- Check if roles exist and create them if they don't
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Patient')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Patient', 'PATIENT', NEWID())
    PRINT 'Created Patient role'
END
ELSE
    PRINT 'Patient role already exists'

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Doctor')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Doctor', 'DOCTOR', NEWID())
    PRINT 'Created Doctor role'
END
ELSE
    PRINT 'Doctor role already exists'

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Nurse')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Nurse', 'NURSE', NEWID())
    PRINT 'Created Nurse role'
END
ELSE
    PRINT 'Nurse role already exists'

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID())
    PRINT 'Created Admin role'
END
ELSE
    PRINT 'Admin role already exists'

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'System Administrator')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'System Administrator', 'SYSTEM ADMINISTRATOR', NEWID())
    PRINT 'Created System Administrator role'
END
ELSE
    PRINT 'System Administrator role already exists'

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin Staff')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin Staff', 'ADMIN STAFF', NEWID())
    PRINT 'Created Admin Staff role'
END
ELSE
    PRINT 'Admin Staff role already exists'

-- Show all roles
SELECT Name, NormalizedName FROM AspNetRoles ORDER BY Name
