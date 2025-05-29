-- Drop existing procedures to avoid errors
IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_AssignUserPermissions')
    DROP PROCEDURE sp_AssignUserPermissions;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserPermissions')
    DROP PROCEDURE sp_GetUserPermissions;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_CheckUserPermission')
    DROP PROCEDURE sp_CheckUserPermission;
GO

-- Assign/unassign permissions to a user (replacing previous ones)
CREATE PROCEDURE sp_AssignUserPermissions
    @UserId NVARCHAR(450),
    @PermissionIds NVARCHAR(MAX) -- Comma-separated list of permission IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Delete existing permissions
        DELETE FROM UserPermissions WHERE UserId = @UserId;
        
        -- Insert new permissions
        IF LEN(@PermissionIds) > 0
        BEGIN
            -- Create a temporary table to hold the permission IDs
            CREATE TABLE #TempPermissions (PermissionId INT);
            
            -- Split the comma-separated string and insert into temp table
            INSERT INTO #TempPermissions (PermissionId)
            SELECT value FROM STRING_SPLIT(@PermissionIds, ',');
            
            -- Insert new permissions
            INSERT INTO UserPermissions (UserId, PermissionId)
            SELECT @UserId, PermissionId FROM #TempPermissions;
            
            -- Drop the temporary table
            DROP TABLE #TempPermissions;
        END
        
        COMMIT TRANSACTION;
        
        -- Return success
        SELECT 1 AS Success, 'Permissions assigned successfully' AS Message;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        -- Return error
        SELECT 0 AS Success, ERROR_MESSAGE() AS Message;
    END CATCH
END;
GO

-- Get assigned permissions for a user
CREATE PROCEDURE sp_GetUserPermissions
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.Id,
        p.Name,
        p.Description,
        p.Category
    FROM 
        Permissions p
    INNER JOIN 
        UserPermissions up ON p.Id = up.PermissionId
    WHERE 
        up.UserId = @UserId
    ORDER BY 
        p.Category, p.Name;
END;
GO

-- Check if a user has a specific permission
CREATE PROCEDURE sp_CheckUserPermission
    @UserId NVARCHAR(450),
    @PermissionName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HasPermission BIT = 0;
    
    IF EXISTS (
        SELECT 1
        FROM UserPermissions up
        INNER JOIN Permissions p ON up.PermissionId = p.Id
        WHERE up.UserId = @UserId AND p.Name = @PermissionName
    )
    BEGIN
        SET @HasPermission = 1;
    END
    
    SELECT @HasPermission AS HasPermission;
END;
GO

PRINT 'Permissions stored procedures created successfully.' 