# Apply Migration Fix Script
# This script will apply the FixCascadePathsMigration to solve the cascade path issues

# Stop on first error
$ErrorActionPreference = "Stop"

Write-Host "Starting migration fix application process..." -ForegroundColor Cyan

# 1. First run our SQL fix to ensure the Messages table doesn't have problematic constraints
Write-Host "Running fix-cascade-paths.sql to clean up existing constraints..." -ForegroundColor Yellow
$serverName = "DESKTOP-NU53VS3"
$databaseName = "Barangay"
sqlcmd -S $serverName -E -d $databaseName -i fix-cascade-paths.sql

# 2. Add a new migration for the fix
Write-Host "Creating a new migration to fix cascade paths..." -ForegroundColor Yellow
dotnet ef migrations add FixCascadePathsMigration --force

# 3. Apply just this migration
Write-Host "Applying the migration..." -ForegroundColor Yellow
dotnet ef database update FixCascadePathsMigration

# 4. Resume normal migration application
Write-Host "Continuing with normal migrations..." -ForegroundColor Yellow
dotnet ef database update

# 5. Verify migration was applied correctly
Write-Host "Verifying foreign key constraints on Messages table..." -ForegroundColor Yellow
$verifyScript = @"
SELECT 
    fk.name AS 'Foreign Key', 
    OBJECT_NAME(fk.parent_object_id) AS 'Table',
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS 'Column',
    OBJECT_NAME(fk.referenced_object_id) AS 'Referenced Table',
    fk.delete_referential_action_desc AS 'Delete Action'
FROM 
    sys.foreign_keys fk
INNER JOIN 
    sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE 
    OBJECT_NAME(fk.parent_object_id) = 'Messages'
ORDER BY 
    fk.name;
"@

try {
    sqlcmd -S $serverName -E -d $databaseName -Q $verifyScript
    Write-Host "Migration fix verification complete." -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not verify migration. Please check manually." -ForegroundColor Yellow
}

Write-Host "Database migration fix completed successfully!" -ForegroundColor Cyan 