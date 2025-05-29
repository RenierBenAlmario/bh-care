# Fix Cascade Paths Issue Script
# This script will fix the cascade paths issue in the database

# Stop on first error
$ErrorActionPreference = "Stop"

Write-Host "Starting cascade paths fix process..." -ForegroundColor Cyan

# 1. Create a new migration that fixes the cascade paths issue
Write-Host "Creating a new migration to fix cascade paths..." -ForegroundColor Yellow
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$migrationName = "FixCascadePaths_$timestamp"
dotnet ef migrations add $migrationName
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create migration. Check for errors above." -ForegroundColor Red
    exit 1
}
Write-Host "Migration created successfully: $migrationName" -ForegroundColor Green

# 2. Modify the migration file to fix the cascade paths issue
Write-Host "Finding the latest migration file..." -ForegroundColor Yellow
$migrationFiles = Get-ChildItem -Path "Migrations" -Filter "$migrationName.cs"
if ($migrationFiles.Count -eq 0) {
    Write-Host "Could not find the migration file. Please check the Migrations folder." -ForegroundColor Red
    exit 1
}

$migrationFile = $migrationFiles[0].FullName
Write-Host "Found migration file: $migrationFile" -ForegroundColor Green

# 3. Edit the migration file to change cascade delete behavior
Write-Host "Editing the migration file to fix cascade paths..." -ForegroundColor Yellow
$content = Get-Content -Path $migrationFile -Raw

# Replace CASCADE with NO ACTION for Messages table
$content = $content -replace "onDelete: ReferentialAction.Cascade\)\s*\.Annotation\(""SqlServer:ValueGenerationStrategy", "onDelete: ReferentialAction.NoAction).Annotation(""SqlServer:ValueGenerationStrategy"

# Save the changes
Set-Content -Path $migrationFile -Value $content
Write-Host "Migration file updated successfully." -ForegroundColor Green

# 4. Update the database with the fixed migration
Write-Host "Updating the database with the fixed migration..." -ForegroundColor Yellow
dotnet ef database update
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to update database. Check for errors above." -ForegroundColor Red
    exit 1
}
Write-Host "Database updated successfully!" -ForegroundColor Green

Write-Host "Cascade paths fix process completed successfully!" -ForegroundColor Cyan 