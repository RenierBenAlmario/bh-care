-- Create StaffPermissions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StaffPermissions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[StaffPermissions](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [StaffMemberId] [int] NOT NULL,
        [PermissionId] [int] NOT NULL,
        [GrantedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_StaffPermissions] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_StaffPermissions_StaffMembers_StaffMemberId] FOREIGN KEY([StaffMemberId])
            REFERENCES [dbo].[StaffMembers] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_StaffPermissions_Permissions_PermissionId] FOREIGN KEY([PermissionId])
            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_StaffPermissions_StaffMemberId] ON [dbo].[StaffPermissions]
    (
        [StaffMemberId] ASC
    );
    
    CREATE NONCLUSTERED INDEX [IX_StaffPermissions_PermissionId] ON [dbo].[StaffPermissions]
    (
        [PermissionId] ASC
    );
    
    PRINT 'StaffPermissions table created successfully';
END
ELSE
BEGIN
    PRINT 'StaffPermissions table already exists';
END

-- Add some sample permissions for existing staff members
INSERT INTO [dbo].[StaffPermissions] (StaffMemberId, PermissionId, GrantedAt)
SELECT s.Id, p.Id, GETDATE()
FROM [dbo].[StaffMembers] s
CROSS JOIN [dbo].[Permissions] p
WHERE p.Name IN ('Access Admin Dashboard', 'Access Doctor Dashboard', 'Access Nurse Dashboard')
AND NOT EXISTS (
    SELECT 1 
    FROM [dbo].[StaffPermissions] sp 
    WHERE sp.StaffMemberId = s.Id AND sp.PermissionId = p.Id
);

PRINT 'Sample permissions assigned to staff members'; 