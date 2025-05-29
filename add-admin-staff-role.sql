-- Script to add the Admin Staff role and associate permissions
-- Date: May 21, 2025

USE [Barangay];
GO

PRINT 'Starting Admin Staff role setup...';

-- Check if the role exists in AspNetRoles table
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ADMIN STAFF')
BEGIN
    -- Create a new role ID
    DECLARE @RoleId NVARCHAR(450) = NEWID();
    
    -- Insert the Admin Staff role
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, 'Admin Staff', 'ADMIN STAFF', NEWID());
    
    PRINT 'Admin Staff role created with ID: ' + @RoleId;
END
ELSE
BEGIN
    PRINT 'Admin Staff role already exists';
    -- Get the existing role ID for later use
    DECLARE @RoleId NVARCHAR(450);
    SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN STAFF';
END

-- Ensure admin_staff permissions exist in Permissions table
-- First, make sure the Permissions table exists
IF OBJECT_ID('dbo.Permissions', 'U') IS NOT NULL
BEGIN
    -- Check if required permissions exist
    DECLARE @MissingPermissions TABLE (Name NVARCHAR(100), Description NVARCHAR(255), Category NVARCHAR(100));
    
    INSERT INTO @MissingPermissions (Name, Description, Category)
    SELECT p.Name, p.Description, p.Category
    FROM (
        VALUES 
            ('ManageAppointments', 'Can manage all appointment bookings', 'Appointments'),
            ('AddPatients', 'Can add new patients', 'Patient Management'),
            ('EditPatients', 'Can edit patient information', 'Patient Management'),
            ('ViewPatients', 'Can view patient records', 'Patient Management'),
            ('ViewMedicalRecords', 'Can view medical records', 'Medical Records'),
            ('ViewReports', 'Can view system reports', 'Reporting'),
            ('AccessAdminDashboard', 'Can access the administration dashboard', 'System Access')
    ) AS p(Name, Description, Category)
    WHERE NOT EXISTS (
        SELECT 1 FROM Permissions WHERE Name = p.Name
    );
    
    -- Add any missing permissions
    IF EXISTS (SELECT 1 FROM @MissingPermissions)
    BEGIN
        INSERT INTO Permissions (Name, Description, Category)
        SELECT Name, Description, Category FROM @MissingPermissions;
        
        PRINT 'Added missing permissions for admin_staff role';
    END
    ELSE
    BEGIN
        PRINT 'All required permissions already exist';
    END
    
    -- Create default admin_staff role permissions mapping
    -- This could be used to automatically assign permissions to any users with this role
    -- if you have a role-permissions mapping table
END
ELSE
BEGIN
    PRINT 'WARNING: Permissions table does not exist. Please run the RBAC setup script first.';
END

PRINT 'Admin Staff role setup completed!';
GO 