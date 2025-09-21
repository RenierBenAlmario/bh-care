-- SQL Script to consolidate duplicate dashboard permissions
-- This script will:
-- 1. Create a single "Access Dashboard" permission if it doesn't exist
-- 2. Map all existing dashboard permissions to this single permission
-- 3. Update user permissions to use the consolidated permission

-- First, check if the consolidated permission exists, if not create it
DECLARE @ConsolidatedPermissionId INT;
DECLARE @ConsolidatedPermissionName NVARCHAR(50) = 'Access Dashboard';
DECLARE @ConsolidatedPermissionDesc NVARCHAR(255) = 'Ability to access all dashboards in the system';
DECLARE @ConsolidatedPermissionCategory NVARCHAR(50) = 'Dashboard Access';

-- Check if the consolidated permission already exists
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = @ConsolidatedPermissionName)
BEGIN
    -- Create the consolidated permission
    INSERT INTO [Permissions] ([Name], [Description], [Category])
    VALUES (@ConsolidatedPermissionName, @ConsolidatedPermissionDesc, @ConsolidatedPermissionCategory);
    
    SET @ConsolidatedPermissionId = SCOPE_IDENTITY();
    PRINT 'Created new consolidated dashboard permission with ID: ' + CAST(@ConsolidatedPermissionId AS NVARCHAR(10));
END
ELSE
BEGIN
    -- Get the existing permission ID
    SELECT @ConsolidatedPermissionId = [Id] FROM [Permissions] WHERE [Name] = @ConsolidatedPermissionName;
    PRINT 'Using existing consolidated dashboard permission with ID: ' + CAST(@ConsolidatedPermissionId AS NVARCHAR(10));
END

-- Create a temporary table to store the IDs of all dashboard permissions
CREATE TABLE #DashboardPermissions (
    [Id] INT,
    [Name] NVARCHAR(50)
);

-- Insert all dashboard permissions into the temporary table
INSERT INTO #DashboardPermissions ([Id], [Name])
SELECT [Id], [Name] FROM [Permissions] 
WHERE [Name] IN (
    'Access Admin Dashboard', 
    'Access Doctor Dashboard', 
    'Access Nurse Dashboard',
    'AccessAdminDashboard',
    'AccessDoctorDashboard',
    'AccessNurseDashboard'
) OR [Category] LIKE '%Dashboard%';

-- Print the permissions that will be consolidated
PRINT 'The following dashboard permissions will be consolidated:';
SELECT * FROM #DashboardPermissions;

-- Update all UserPermissions that reference any dashboard permission to use the consolidated permission
-- First, identify users who have any dashboard permission
INSERT INTO [UserPermissions] ([UserId], [PermissionId])
SELECT DISTINCT up.[UserId], @ConsolidatedPermissionId
FROM [UserPermissions] up
INNER JOIN #DashboardPermissions dp ON up.[PermissionId] = dp.[Id]
WHERE NOT EXISTS (
    SELECT 1 FROM [UserPermissions] 
    WHERE [UserId] = up.[UserId] AND [PermissionId] = @ConsolidatedPermissionId
);

PRINT 'Updated user permissions to use the consolidated dashboard permission';

-- Now we can safely delete the old dashboard permissions from UserPermissions
DELETE up
FROM [UserPermissions] up
INNER JOIN #DashboardPermissions dp ON up.[PermissionId] = dp.[Id]
WHERE dp.[Id] <> @ConsolidatedPermissionId;

PRINT 'Removed old dashboard permissions from UserPermissions';

-- Keep the permissions in the Permissions table for reference, but you could delete them if desired
-- DELETE FROM [Permissions] WHERE [Id] IN (SELECT [Id] FROM #DashboardPermissions WHERE [Id] <> @ConsolidatedPermissionId);

-- Clean up
DROP TABLE #DashboardPermissions;

PRINT 'Dashboard permissions consolidation completed successfully'; 