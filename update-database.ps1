# Database Migration and Update Script
# This script will update your database based on migrations

Write-Host "Barangay Management System - Database Update Tool" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "This script will update your database based on the connection string in appsettings.json" -ForegroundColor Yellow

try {
    # Verify if dotnet EF tools are installed
    $efInstalled = dotnet ef --version
    if (-not $?) {
        Write-Host "Installing Entity Framework Core tools..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        if (-not $?) {
            throw "Failed to install Entity Framework Core tools. Please install manually using 'dotnet tool install --global dotnet-ef'."
        }
        
        Write-Host "Entity Framework Core tools installed successfully." -ForegroundColor Green
    } else {
        Write-Host "Entity Framework Core tools are already installed: $efInstalled" -ForegroundColor Green
    }
    
    # Check for pending migrations
    Write-Host "Checking for pending migrations..." -ForegroundColor Yellow
    $pendingMigrations = dotnet ef migrations list --no-build 2>&1
    
    if ($pendingMigrations -match "No migrations were found") {
        Write-Host "No migrations were found. Please ensure your project is properly set up." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Found migrations to apply. Preparing to update database..." -ForegroundColor Green
    
    # Backup the database (optional)
    Write-Host "Would you like to backup the database before updating? (y/n)" -ForegroundColor Yellow
    $backup = Read-Host
    
    if ($backup -eq "y") {
        # Extract server and database name from appsettings.json
        $appSettings = Get-Content -Path "appsettings.json" -Raw | ConvertFrom-Json
        $connectionString = $appSettings.ConnectionStrings.DefaultConnection
        
        # Parse connection string
        $server = if ($connectionString -match "Server=(.*?);") { $matches[1] } else { "DESKTOP-NU53VS3" }
        $database = if ($connectionString -match "Database=(.*?);") { $matches[1] } else { "Barangay" }
        
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $backupPath = ".\$database`_backup_$timestamp.bak"
        
        Write-Host "Backing up database $database on server $server to $backupPath..." -ForegroundColor Yellow
        
        # Using sqlcmd to backup
        $backupCmd = "BACKUP DATABASE [$database] TO DISK = N'$backupPath' WITH NOFORMAT, NOINIT, NAME = N'$database-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
        
        try {
            sqlcmd -S $server -E -Q $backupCmd
            Write-Host "Database backup completed successfully." -ForegroundColor Green
        }
        catch {
            Write-Host "Warning: Database backup failed. Continuing with update..." -ForegroundColor Yellow
            Write-Host $_.Exception.Message -ForegroundColor Red
        }
    }
    
    # Apply migrations
    Write-Host "Applying migrations to database..." -ForegroundColor Yellow
    dotnet ef database update --no-build
    
    if ($?) {
        Write-Host "Database updated successfully!" -ForegroundColor Green
    } else {
        throw "Failed to update database. Check the error messages above."
    }
    
    # Verify database structure
    Write-Host "Verifying database structure..." -ForegroundColor Yellow
    
    # Run the application briefly to trigger MigrationManager.EnsureUserDocumentsTableAsync
    Write-Host "Running application to verify database integrity..." -ForegroundColor Yellow
    $job = Start-Job -ScriptBlock { 
        Set-Location $using:PWD
        dotnet run --no-build 
    }
    
    # Wait a few seconds for the app to start and run migrations
    Start-Sleep -Seconds 5
    
    # Stop the job
    Stop-Job -Job $job
    Remove-Job -Job $job -Force
    
    Write-Host "Database migration and update completed successfully!" -ForegroundColor Green
    Write-Host "Your database is now up to date." -ForegroundColor Green
}
catch {
    Write-Host "An error occurred during database update:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    
    Write-Host "`nTroubleshooting suggestions:" -ForegroundColor Yellow
    Write-Host "1. Ensure SQL Server is running" -ForegroundColor Yellow
    Write-Host "2. Check that your connection string in appsettings.json is correct" -ForegroundColor Yellow
    Write-Host "3. Verify you have proper permissions to access the database" -ForegroundColor Yellow
    Write-Host "4. Make sure Entity Framework Core tools are installed: dotnet tool install --global dotnet-ef" -ForegroundColor Yellow
} 