-- Test SQL script to verify user registration
PRINT 'Checking user registration data...'

-- Check AspNetUsers table for recent registrations
PRINT 'Recently registered users (last 7 days):'
SELECT 
    Id,
    UserName,
    Email,
    FirstName,
    LastName,
    Status,
    IsActive,
    CreatedAt
FROM 
    AspNetUsers
WHERE 
    CreatedAt > DATEADD(day, -7, GETDATE())
ORDER BY 
    CreatedAt DESC;
    
-- Check counts by status
PRINT 'User counts by status:'
SELECT 
    Status,
    COUNT(*) AS Count
FROM 
    AspNetUsers
GROUP BY 
    Status
ORDER BY 
    Count DESC;
    
-- Check UserDocuments for recent documents
PRINT 'Recent document uploads (last 7 days):'
SELECT 
    d.Id,
    d.UserId,
    d.FileName,
    d.Status,
    d.ContentType,
    u.UserName,
    u.Email
FROM 
    UserDocuments d
JOIN 
    AspNetUsers u ON d.UserId = u.Id
WHERE 
    u.CreatedAt > DATEADD(day, -7, GETDATE())
ORDER BY 
    u.CreatedAt DESC;
    
-- Check for mismatches between users and documents
PRINT 'Checking for mismatches between users and documents:'
SELECT 
    'Users without documents' AS Issue,
    COUNT(*) AS Count
FROM 
    AspNetUsers u
LEFT JOIN 
    UserDocuments d ON u.Id = d.UserId
WHERE 
    u.Status = 'Pending' 
    AND d.Id IS NULL
    AND u.CreatedAt > DATEADD(day, -7, GETDATE())
UNION ALL
SELECT 
    'Documents without valid users' AS Issue,
    COUNT(*) AS Count
FROM 
    UserDocuments d
LEFT JOIN 
    AspNetUsers u ON d.UserId = u.Id
WHERE 
    u.Id IS NULL;
    
-- Check for recent notifications
PRINT 'Recent notifications:'
SELECT TOP 5
    Id,
    Title,
    Message,
    CreatedAt,
    ReadAt,
    IsRead
FROM 
    Notifications
ORDER BY 
    CreatedAt DESC;
    
PRINT 'Registration verification complete!' 