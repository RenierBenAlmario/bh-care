-- Fix Notification and User Synchronization Issues
-- This script analyzes and fixes issues with notification counts and user registration data

PRINT 'Starting notification and user sync fix script...'

-- Check for database connectivity
IF DB_ID() IS NULL
BEGIN
    PRINT 'Error: No database context.'
    RETURN
END

PRINT 'Connected to database: ' + DB_NAME()

-- 1. Check if notification and user tables exist
IF OBJECT_ID('Notifications', 'U') IS NULL
BEGIN
    PRINT 'Error: Notifications table does not exist.'
    RETURN
END

IF OBJECT_ID('AspNetUsers', 'U') IS NULL
BEGIN
    PRINT 'Error: AspNetUsers table does not exist.'
    RETURN
END

IF OBJECT_ID('UserDocuments', 'U') IS NULL
BEGIN
    PRINT 'Error: UserDocuments table does not exist.'
    RETURN
END

PRINT 'All required tables exist.'

-- 2. Check for orphaned notifications (no matching user)
PRINT 'Checking for orphaned notifications...'

SELECT COUNT(*) AS OrphanedNotificationsCount 
FROM Notifications n
LEFT JOIN AspNetUsers u ON n.Message LIKE '%' + u.UserName + '%'
WHERE n.Message LIKE '%has registered%'
AND u.Id IS NULL

-- 3. Check for users with status inconsistencies
PRINT 'Checking for user status inconsistencies...'

-- Users with missing Status
SELECT COUNT(*) AS MissingStatusCount
FROM AspNetUsers
WHERE Status IS NULL OR Status = ''

-- 4. Check for document status inconsistencies
PRINT 'Checking for document status inconsistencies...'

-- Count of UserDocuments without matching users
SELECT COUNT(*) AS OrphanedDocumentsCount
FROM UserDocuments d
LEFT JOIN AspNetUsers u ON d.UserId = u.Id
WHERE u.Id IS NULL

-- Users without documents
SELECT COUNT(*) AS UsersWithoutDocumentsCount
FROM AspNetUsers u
LEFT JOIN UserDocuments d ON u.Id = d.UserId
WHERE u.Status = 'Pending' AND d.Id IS NULL

-- 5. Fix orphaned notifications
PRINT 'Fixing orphaned notifications...'

BEGIN TRANSACTION

-- Mark orphaned registration notifications as read
UPDATE n
SET ReadAt = GETDATE(), IsRead = 1
FROM Notifications n
LEFT JOIN AspNetUsers u ON n.Message LIKE '%' + u.UserName + '%'
WHERE n.Message LIKE '%has registered%'
AND u.Id IS NULL
AND n.ReadAt IS NULL

-- Report how many were updated
DECLARE @UpdatedNotificationsCount INT = @@ROWCOUNT
PRINT 'Updated ' + CAST(@UpdatedNotificationsCount AS VARCHAR) + ' orphaned notifications'

-- 6. Fix users with missing Status
PRINT 'Fixing users with missing Status...'

UPDATE AspNetUsers
SET Status = 'Pending'
WHERE Status IS NULL OR Status = ''

-- Report how many were updated
DECLARE @UpdatedUsersCount INT = @@ROWCOUNT
PRINT 'Updated ' + CAST(@UpdatedUsersCount AS VARCHAR) + ' users with missing Status'

-- 7. Fix document status inconsistencies
PRINT 'Fixing document status inconsistencies...'

-- Delete orphaned documents
DELETE FROM UserDocuments
WHERE UserId NOT IN (SELECT Id FROM AspNetUsers)

-- Report how many were deleted
DECLARE @DeletedDocumentsCount INT = @@ROWCOUNT
PRINT 'Deleted ' + CAST(@DeletedDocumentsCount AS VARCHAR) + ' orphaned documents'

-- 8. Reconcile Status values between AspNetUsers and UserDocuments
PRINT 'Reconciling Status values...'

-- Update UserDocuments Status based on AspNetUsers Status
UPDATE d
SET d.Status = CASE 
    WHEN u.Status = 'Verified' THEN 'Approved'
    WHEN u.Status = 'Rejected' THEN 'Rejected'
    ELSE 'Pending'
END
FROM UserDocuments d
JOIN AspNetUsers u ON d.UserId = u.Id
WHERE d.Status <> CASE 
    WHEN u.Status = 'Verified' THEN 'Approved'
    WHEN u.Status = 'Rejected' THEN 'Rejected'
    ELSE 'Pending'
END

-- Report how many were updated
DECLARE @ReconciliationCount INT = @@ROWCOUNT
PRINT 'Reconciled ' + CAST(@ReconciliationCount AS VARCHAR) + ' document status values'

-- 9. Fix HasAgreedToTerms and AgreedAt values if needed
IF COL_LENGTH('AspNetUsers', 'HasAgreedToTerms') IS NOT NULL
BEGIN
    PRINT 'Fixing HasAgreedToTerms values...'
    
    UPDATE AspNetUsers
    SET HasAgreedToTerms = 1,
        AgreedAt = COALESCE(AgreedAt, CreatedAt)
    WHERE Status = 'Pending' OR Status = 'Verified'
    
    -- Report how many were updated
    DECLARE @TermsFixCount INT = @@ROWCOUNT
    PRINT 'Fixed ' + CAST(@TermsFixCount AS VARCHAR) + ' users with missing Terms agreement'
END

-- 10. Print summary counts
PRINT '------------------------------------------------------'
PRINT 'SUMMARY REPORT'
PRINT '------------------------------------------------------'

-- Current users by status
SELECT Status, COUNT(*) AS Count
FROM AspNetUsers
GROUP BY Status

-- Current documents by status
SELECT Status, COUNT(*) AS Count 
FROM UserDocuments
GROUP BY Status

-- Notification counts
SELECT COUNT(*) AS TotalNotifications,
    SUM(CASE WHEN ReadAt IS NULL THEN 1 ELSE 0 END) AS UnreadNotifications
FROM Notifications

PRINT '------------------------------------------------------'

-- Commit the transaction if all checks pass
IF @UpdatedNotificationsCount + @UpdatedUsersCount + @DeletedDocumentsCount + @ReconciliationCount > 0
BEGIN
    COMMIT TRANSACTION
    PRINT 'Transaction committed with ' + 
          CAST(@UpdatedNotificationsCount + @UpdatedUsersCount + @DeletedDocumentsCount + @ReconciliationCount AS VARCHAR) + 
          ' total changes made.'
END
ELSE
BEGIN
    ROLLBACK TRANSACTION
    PRINT 'No changes needed. Transaction rolled back.'
END

PRINT 'Notification and user sync fix script completed.' 