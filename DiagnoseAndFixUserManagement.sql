-- DiagnoseAndFixUserManagement.sql
-- Script to diagnose and fix common issues with User Management and notifications

-- PART 1: DIAGNOSTICS
PRINT '====== RUNNING DIAGNOSTICS ======'

-- Check database connectivity
PRINT 'Checking database connectivity...'
SELECT DB_NAME() AS CurrentDatabase, SERVERPROPERTY('ServerName') AS ServerName

-- Check AspNetUsers table
PRINT 'Checking AspNetUsers table...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    -- Check if Status column exists
    IF EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'Status' AND Object_ID = Object_ID('dbo.AspNetUsers'))
    BEGIN
        PRINT '✓ Status column exists in AspNetUsers'
        
        -- Check status values distribution
        SELECT 
            ISNULL(Status, 'NULL') AS Status, 
            COUNT(*) AS Count 
        FROM 
            AspNetUsers 
        GROUP BY 
            Status
        ORDER BY 
            Count DESC
    END
    ELSE
    BEGIN
        PRINT '❌ Status column is MISSING in AspNetUsers'
    END

    -- Count users
    DECLARE @UserCount INT
    SELECT @UserCount = COUNT(*) FROM AspNetUsers
    PRINT 'Total users: ' + CAST(@UserCount AS VARCHAR)
END
ELSE
BEGIN
    PRINT '❌ AspNetUsers table does not exist'
END

-- Check UserDocuments table
PRINT 'Checking UserDocuments table...'
IF OBJECT_ID('UserDocuments', 'U') IS NOT NULL
BEGIN
    -- Count documents
    DECLARE @DocCount INT
    SELECT @DocCount = COUNT(*) FROM UserDocuments
    PRINT 'Total documents: ' + CAST(@DocCount AS VARCHAR)
    
    -- Check for orphaned documents (no associated user)
    SELECT 
        COUNT(*) AS OrphanedDocuments
    FROM 
        UserDocuments d
    LEFT JOIN
        AspNetUsers u ON d.UserId = u.Id
    WHERE
        u.Id IS NULL
END
ELSE
BEGIN
    PRINT '❌ UserDocuments table does not exist'
END

-- Check Notifications table
PRINT 'Checking Notifications table...'
IF OBJECT_ID('Notifications', 'U') IS NOT NULL
BEGIN
    -- Count notifications
    DECLARE @NotifCount INT
    SELECT @NotifCount = COUNT(*) FROM Notifications
    PRINT 'Total notifications: ' + CAST(@NotifCount AS VARCHAR)
    
    -- Check recent notifications
    SELECT TOP 5
        Id,
        Title,
        Message,
        Type,
        CreatedAt,
        ReadAt,
        RecipientId
    FROM
        Notifications
    ORDER BY
        CreatedAt DESC
END
ELSE
BEGIN
    PRINT '❌ Notifications table does not exist'
END

-- Check for users with Pending status but no notification
PRINT 'Checking for pending users with no notification...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL AND OBJECT_ID('Notifications', 'U') IS NOT NULL
BEGIN
    SELECT
        u.Id,
        u.UserName,
        u.Email,
        u.Status,
        u.CreatedAt,
        (SELECT COUNT(*) FROM Notifications n WHERE n.Message LIKE '%' + u.UserName + '%') AS NotificationCount
    FROM
        AspNetUsers u
    WHERE
        u.Status = 'Pending' AND
        NOT EXISTS (
            SELECT 1 FROM Notifications n 
            WHERE n.Message LIKE '%' + u.UserName + '%' OR n.Message LIKE '%' + u.Email + '%'
        )
    ORDER BY
        u.CreatedAt DESC
END

-- Check for case insensitivity issues
PRINT 'Checking for status case sensitivity issues...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    SELECT
        DISTINCT Status
    FROM
        AspNetUsers
    WHERE
        Status IS NOT NULL
END

-- PART 2: AUTOMATIC FIXES
PRINT '====== APPLYING FIXES ======'

-- Fix 1: Create notifications for pending users without notifications
PRINT 'Creating notifications for pending users without notifications...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL AND OBJECT_ID('Notifications', 'U') IS NOT NULL
BEGIN
    INSERT INTO Notifications (Title, Message, Type, Link, RecipientId, CreatedAt, ReadAt)
    SELECT
        'New User Registration',
        'User ' + u.UserName + ' has registered and is pending approval',
        'info',
        '/Admin/UserManagement',
        NULL, -- For admin notifications
        GETUTCDATE(),
        NULL
    FROM
        AspNetUsers u
    WHERE
        u.Status = 'Pending' AND
        NOT EXISTS (
            SELECT 1 FROM Notifications n 
            WHERE n.Message LIKE '%' + u.UserName + '%' OR n.Message LIKE '%' + u.Email + '%'
        )
        
    DECLARE @InsertedCount INT = @@ROWCOUNT
    PRINT 'Created ' + CAST(@InsertedCount AS VARCHAR) + ' new notifications'
END

-- Fix 2: Standardize status case
PRINT 'Standardizing status case...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    -- Convert 'pending' to 'Pending'
    UPDATE AspNetUsers
    SET Status = 'Pending'
    WHERE Status = 'pending'
    
    DECLARE @PendingFixed INT = @@ROWCOUNT
    
    -- Convert 'verified' to 'Verified'
    UPDATE AspNetUsers
    SET Status = 'Verified'
    WHERE Status = 'verified'
    
    DECLARE @VerifiedFixed INT = @@ROWCOUNT
    
    -- Convert 'rejected' to 'Rejected'
    UPDATE AspNetUsers
    SET Status = 'Rejected'
    WHERE Status = 'rejected'
    
    DECLARE @RejectedFixed INT = @@ROWCOUNT
    
    PRINT 'Fixed status for ' + CAST((@PendingFixed + @VerifiedFixed + @RejectedFixed) AS VARCHAR) + ' users'
END

-- Fix 3: Set default status for users without status
PRINT 'Setting default status for users with NULL status...'
IF OBJECT_ID('AspNetUsers', 'U') IS NOT NULL
BEGIN
    UPDATE AspNetUsers
    SET Status = 'Pending'
    WHERE Status IS NULL
    
    DECLARE @NullStatusFixed INT = @@ROWCOUNT
    PRINT 'Set default status for ' + CAST(@NullStatusFixed AS VARCHAR) + ' users'
END

-- Final summary
PRINT '====== SUMMARY ======'
SELECT
    DB_NAME() AS CurrentDatabase,
    (SELECT COUNT(*) FROM AspNetUsers) AS TotalUsers,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Pending') AS PendingUsers,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Verified') AS VerifiedUsers,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Rejected') AS RejectedUsers,
    (SELECT COUNT(*) FROM UserDocuments) AS TotalDocuments,
    (SELECT COUNT(*) FROM Notifications) AS TotalNotifications,
    (SELECT COUNT(*) FROM Notifications WHERE ReadAt IS NULL) AS UnreadNotifications 