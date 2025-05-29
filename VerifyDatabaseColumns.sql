-- Script to verify that the required columns exist in the AspNetUsers table
-- Run this after applying the migration to confirm success

-- Check if the HasAgreedToTerms and AgreedAt columns exist
SELECT 
    TABLE_NAME,
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'AspNetUsers' 
    AND COLUMN_NAME IN ('HasAgreedToTerms', 'AgreedAt');

-- Check if the migration is recorded in __EFMigrationsHistory
SELECT 
    MigrationId, 
    ProductVersion
FROM 
    __EFMigrationsHistory
WHERE 
    MigrationId = '20250511120000_AddAgreedToTermsColumns';

-- Check if users with Pending status are correctly stored
SELECT TOP 10
    UserName, 
    Email, 
    Status, 
    IsActive, 
    HasAgreedToTerms, 
    AgreedAt
FROM 
    AspNetUsers
WHERE 
    Status = 'Pending'
ORDER BY 
    CreatedAt DESC; 