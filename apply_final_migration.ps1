# Script to apply the final migration to fix foreign key constraints
Write-Host "Applying final migration to fix remaining foreign key constraints..." -ForegroundColor Cyan

try {
    # Step 1: Check database connection first
    Write-Host "Testing database connection..." -ForegroundColor Yellow
    try {
        dotnet ef database list --json
        Write-Host "Database connection successful." -ForegroundColor Green
    }
    catch {
        Write-Host "Cannot connect to the database. Please check your connection string." -ForegroundColor Red
        exit 1
    }

    # Step 2: Try dropping the database and recreating from scratch if specified
    $rebuildDatabase = $false  # Set to true if you want to start with a fresh database
    if ($rebuildDatabase) {
        Write-Host "Dropping database to start fresh..." -ForegroundColor Yellow
        dotnet ef database drop --force
        Write-Host "Database dropped." -ForegroundColor Green
        
        Write-Host "Creating new database..." -ForegroundColor Yellow
        dotnet ef database update
        Write-Host "Database created and migrations applied." -ForegroundColor Green
        exit 0
    }

    # Step 3: Try applying the most recent migration
    Write-Host "Checking if we can apply migrations normally..." -ForegroundColor Yellow
    try {
        dotnet ef database update --verbose
        Write-Host "Migration completed successfully!" -ForegroundColor Green
        exit 0
    }
    catch {
        Write-Host "Error during normal migration: $_" -ForegroundColor Red
        Write-Host "Switching to manual SQL fixes..." -ForegroundColor Yellow
    }

    # Step 4: Apply SQL directly using the sqlcmd utility
    $connectionString = $env:DefaultConnection
    if (-not $connectionString) {
        # Try to read from appsettings.json
        try {
            $appsettings = Get-Content -Path "appsettings.json" -Raw | ConvertFrom-Json
            $connectionString = $appsettings.ConnectionStrings.DefaultConnection
        }
        catch {
            Write-Host "Could not get connection string from appsettings.json" -ForegroundColor Red
        }
    }

    if (-not $connectionString) {
        Write-Host "Could not determine the database connection string. Please set the DefaultConnection environment variable." -ForegroundColor Red
        exit 1
    }

    # Parse connection string to get server, database, user, and password
    $parts = $connectionString.Split(';')
    $serverName = ""
    $databaseName = ""
    $userId = ""
    $password = ""

    foreach ($part in $parts) {
        if ($part -match "Server=(.+)") { $serverName = $matches[1] }
        if ($part -match "Database=(.+)") { $databaseName = $matches[1] }
        if ($part -match "User ID=(.+)") { $userId = $matches[1] }
        if ($part -match "Password=(.+)") { $password = $matches[1] }
    }

    # Save the SQL script to a temporary file
    $scriptPath = "$PSScriptRoot\fix-relationships.sql"
    
    # Execute the SQL script
    if (Test-Path $scriptPath) {
        Write-Host "Executing SQL fix script directly: $scriptPath" -ForegroundColor Yellow
        
        if ($userId -and $password) {
            # Use SQL authentication
            Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -Username $userId -Password $password -InputFile $scriptPath -QueryTimeout 120
        }
        else {
            # Use Windows authentication
            Invoke-Sqlcmd -ServerInstance $serverName -Database $databaseName -InputFile $scriptPath -QueryTimeout 120
        }
        
        Write-Host "SQL script executed successfully." -ForegroundColor Green
    }
    else {
        Write-Host "SQL fix script not found at: $scriptPath" -ForegroundColor Red
    }
    
    # Try to apply migrations after SQL fixes
    Write-Host "Applying migrations after SQL fixes..." -ForegroundColor Yellow
    try {
        dotnet ef database update --verbose
        Write-Host "Migration completed successfully after SQL fixes!" -ForegroundColor Green
    }
    catch {
        Write-Host "Error applying migrations after SQL fixes: $_" -ForegroundColor Red
    }
} 
catch {
    Write-Host "Error during migration process: $_" -ForegroundColor Red
    Write-Host "If there are issues with foreign key constraints, you may need to manually fix them in SQL Server." -ForegroundColor Red
    Write-Host "See the fix-relationships.sql script for the SQL commands to use as a reference." -ForegroundColor Yellow
} 