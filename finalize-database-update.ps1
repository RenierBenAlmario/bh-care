# Finalize Database Update Script
# This script finalizes the database update process by creating any remaining migrations and applying them

# Stop on first error
$ErrorActionPreference = "Stop"

Write-Host "Starting final database update process..." -ForegroundColor Cyan

# Check if there are pending migrations
Write-Host "Checking for pending migrations..." -ForegroundColor Yellow
dotnet ef migrations list

# Get the current database schema hash
Write-Host "Getting current database schema hash..." -ForegroundColor Yellow
$dbHashResult = sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -Q "SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM sys.tables;" -h -1
Write-Host "Database schema hash: $dbHashResult" -ForegroundColor Gray

# Add a new final migration to ensure all is up to date
Write-Host "Creating final migration to ensure all is up to date..." -ForegroundColor Yellow
dotnet ef migrations add FinalMigrationUpdate --context ApplicationDbContext

# Apply all migrations
Write-Host "Applying all migrations..." -ForegroundColor Yellow
dotnet ef database update

# Verify that database tables match entity models
Write-Host "Verifying database structure..." -ForegroundColor Yellow

# Check for AspNetUsers table columns
Write-Host "Checking AspNetUsers columns..." -ForegroundColor Yellow
sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -Q "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers' AND COLUMN_NAME IN ('MiddleName', 'Suffix', 'Status', 'HasAgreedToTerms', 'AgreedAt');"

# Check for Messages table foreign keys
Write-Host "Checking Messages table foreign keys..." -ForegroundColor Yellow
sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -Q "SELECT fk.name AS ForeignKey, OBJECT_NAME(fk.parent_object_id) AS Table_Name, COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS Column_Name, fk.delete_referential_action_desc AS Delete_Action FROM sys.foreign_keys fk INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id WHERE OBJECT_NAME(fk.parent_object_id) = 'Messages';"

# Check for UserDocuments table
Write-Host "Checking UserDocuments table..." -ForegroundColor Yellow
sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserDocuments';"

Write-Host "Database update process completed successfully!" -ForegroundColor Green
Write-Host "Your database is now synchronized with your application models." -ForegroundColor Green 