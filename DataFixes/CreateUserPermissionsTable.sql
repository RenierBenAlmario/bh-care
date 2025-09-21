-- Create UserPermissions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserPermissions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [PermissionId] [int] NOT NULL,
        CONSTRAINT [PK_UserPermissions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY([UserId])
            REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPermissions_Permissions_PermissionId] FOREIGN KEY([PermissionId])
            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_UserPermissions_UserId] ON [dbo].[UserPermissions]
    (
        [UserId] ASC
    );
    
    CREATE NONCLUSTERED INDEX [IX_UserPermissions_PermissionId] ON [dbo].[UserPermissions]
    (
        [PermissionId] ASC
    );
    
    PRINT 'UserPermissions table created successfully';
END
ELSE
BEGIN
    PRINT 'UserPermissions table already exists';
END

-- Add default access permissions for Staff Members to their Users
INSERT INTO [dbo].[UserPermissions] (UserId, PermissionId)
SELECT s.UserId, p.Id
FROM [dbo].[StaffMembers] s
CROSS JOIN [dbo].[Permissions] p
WHERE p.Name IN ('Access Admin Dashboard', 'Access Doctor Dashboard', 'Access Nurse Dashboard')
AND s.UserId IS NOT NULL 
AND s.Role IN ('Admin', 'Doctor', 'Nurse')
AND NOT EXISTS (
    SELECT 1 
    FROM [dbo].[UserPermissions] up 
    WHERE up.UserId = s.UserId AND up.PermissionId = p.Id
);

-- Grant specific permissions based on role
INSERT INTO [dbo].[UserPermissions] (UserId, PermissionId)
SELECT s.UserId, p.Id
FROM [dbo].[StaffMembers] s
INNER JOIN [dbo].[Permissions] p ON 
    (s.Role = 'Doctor' AND p.Name = 'Access Doctor Dashboard') OR
    (s.Role = 'Nurse' AND p.Name = 'Access Nurse Dashboard') OR
    (s.Role = 'Admin' AND p.Name = 'Access Admin Dashboard')
WHERE s.UserId IS NOT NULL
AND NOT EXISTS (
    SELECT 1 
    FROM [dbo].[UserPermissions] up 
    WHERE up.UserId = s.UserId AND up.PermissionId = p.Id
);

PRINT 'Default access permissions added to users based on roles'; 