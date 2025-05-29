-- Check for HasAgreedToTerms and AgreedAt columns in AspNetUsers
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE 
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'AspNetUsers' 
    AND COLUMN_NAME IN ('HasAgreedToTerms', 'AgreedAt');

-- Check all users
SELECT TOP 20
    Id, 
    UserName, 
    Email, 
    Status, 
    IsActive, 
    CreatedAt 
FROM 
    AspNetUsers 
ORDER BY 
    CreatedAt DESC;

-- Check pending users specifically
SELECT 
    Id, 
    UserName, 
    Email, 
    Status, 
    IsActive, 
    CreatedAt 
FROM 
    AspNetUsers 
WHERE 
    Status = 'Pending' 
ORDER BY 
    CreatedAt DESC;

-- Check user documents
SELECT 
    d.Id,
    d.UserId,
    d.FileName,
    d.ContentType,
    d.Status,
    u.UserName,
    u.Email
FROM 
    UserDocuments d
LEFT JOIN
    AspNetUsers u ON d.UserId = u.Id
ORDER BY 
    d.Id DESC;

-- Check notifications
SELECT TOP 20 * FROM Notifications
ORDER BY CreatedAt DESC;

-- Check for missing columns in AspNetUsers
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'HasAgreedToTerms' AND Object_ID = Object_ID('dbo.AspNetUsers'))
BEGIN
    PRINT 'HasAgreedToTerms column is MISSING'
END
ELSE
BEGIN
    PRINT 'HasAgreedToTerms column exists'
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = 'AgreedAt' AND Object_ID = Object_ID('dbo.AspNetUsers'))
BEGIN
    PRINT 'AgreedAt column is MISSING'
END
ELSE
BEGIN
    PRINT 'AgreedAt column exists'
END

-- Check database tables
SELECT 
    TABLE_SCHEMA,
    TABLE_NAME 
FROM 
    INFORMATION_SCHEMA.TABLES 
WHERE 
    TABLE_TYPE = 'BASE TABLE'
ORDER BY 
    TABLE_NAME; 