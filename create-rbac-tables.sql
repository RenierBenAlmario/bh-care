USE [Barangay];
GO

-- Create Permissions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Permissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(255) NOT NULL,
        [Category] NVARCHAR(50) NOT NULL
    );
    
    -- Insert default permissions
    INSERT INTO [dbo].[Permissions] ([Name], [Description], [Category])
    VALUES
        ('ApproveUsers', 'Can approve new user registrations', 'User Management'),
        ('DeleteUsers', 'Can delete existing users', 'User Management'),
        ('ViewReports', 'Can view system reports and analytics', 'Reporting'),
        ('ManageUsers', 'Can manage user accounts', 'User Management'),
        ('ManageAppointments', 'Can manage all appointment bookings', 'Appointments'),
        ('ManageMedicalRecords', 'Can manage patient medical records', 'Medical Records'),
        ('AccessAdminDashboard', 'Can access the administration dashboard', 'System Access'),
        ('AccessDoctorDashboard', 'Can access the doctor dashboard', 'System Access');
    
    PRINT 'Permissions table created and populated with default values';
END
ELSE
BEGIN
    PRINT 'Permissions table already exists';
END

-- Create UserPermissions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND type in (N'U'))
BEGIN
    -- Check that AspNetUsers table exists
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[UserPermissions] (
            [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [UserId] NVARCHAR(450) NOT NULL,
            [PermissionId] INT NOT NULL,
            CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
            CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
        );
        
        CREATE INDEX [IX_UserPermissions_UserId] ON [dbo].[UserPermissions] ([UserId]);
        CREATE INDEX [IX_UserPermissions_PermissionId] ON [dbo].[UserPermissions] ([PermissionId]);
        
        PRINT 'UserPermissions table created';
    END
    ELSE
    BEGIN
        PRINT 'AspNetUsers table does not exist, cannot create UserPermissions table';
    END
END
ELSE
BEGIN
    PRINT 'UserPermissions table already exists';
END

-- Create StaffPositions table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StaffPositions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StaffPositions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Name] NVARCHAR(50) NOT NULL,
        [Description] NVARCHAR(255) NOT NULL
    );
    
    -- Insert default staff positions
    INSERT INTO [dbo].[StaffPositions] ([Name], [Description])
    VALUES
        ('Admin', 'Administrator with full access'),
        ('Doctor', 'Medical doctor'),
        ('Nurse', 'Nursing staff'),
        ('IT', 'Information Technology staff');
    
    PRINT 'StaffPositions table created and populated with default values';
END
ELSE
BEGIN
    PRINT 'StaffPositions table already exists';
END

-- Add default permissions for admin user (only if both tables exist)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
   AND EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND type in (N'U'))
BEGIN
    DECLARE @adminUserId NVARCHAR(450);
    SELECT @adminUserId = u.Id FROM AspNetUsers u 
    INNER JOIN AspNetUserRoles r ON u.Id = r.UserId
    INNER JOIN AspNetRoles ar ON r.RoleId = ar.Id
    WHERE ar.Name = 'Admin';

    IF @adminUserId IS NOT NULL
    BEGIN
        -- Add all permissions to admin
        INSERT INTO [dbo].[UserPermissions] ([UserId], [PermissionId])
        SELECT @adminUserId, p.Id 
        FROM [dbo].[Permissions] p
        WHERE NOT EXISTS (
            SELECT 1 FROM [dbo].[UserPermissions] up 
            WHERE up.UserId = @adminUserId AND up.PermissionId = p.Id
        );
        
        PRINT 'Default permissions added for Admin user';
    END
    ELSE
    BEGIN
        PRINT 'No Admin user found';
    END
END

PRINT 'RBAC tables setup completed'; 