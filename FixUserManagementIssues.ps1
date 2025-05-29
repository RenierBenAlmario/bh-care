# FixUserManagementIssues.ps1
# Script to automate running the DiagnoseAndFixUserManagement.sql script

# Configuration - Edit these values as needed
$serverName = "(localdb)\MSSQLLocalDB" # Default SQL Server instance
$databaseName = "Barangay"            # Default database name
$sqlFile = "DiagnoseAndFixUserManagement.sql"
$diagOutputFile = "UserManagementDiagnostics.txt"
$applyFixes = $false  # Set to $true to automatically apply fixes

# Banner
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "     Barangay Health Center Database Diagnostics      " -ForegroundColor Cyan
Write-Host "               User Management Issues                 " -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host ""

# Check if sqlcmd is available
try {
    $null = Get-Command sqlcmd -ErrorAction Stop
    Write-Host "✓ sqlcmd is available" -ForegroundColor Green
}
catch {
    Write-Host "❌ sqlcmd is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install SQL Server Command Line Utilities and try again." -ForegroundColor Yellow
    Write-Host "You can download it from: https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility" -ForegroundColor Yellow
    exit 1
}

# Check if diagnostic SQL file exists
if (-not (Test-Path $sqlFile)) {
    Write-Host "❌ SQL diagnostic file not found: $sqlFile" -ForegroundColor Red
    Write-Host "Please make sure the DiagnoseAndFixUserManagement.sql file is in the same directory as this script." -ForegroundColor Yellow
    exit 1
}

# Get server and database information if not provided
$promptForServer = Read-Host "Enter SQL Server name [$serverName]"
if ($promptForServer) {
    $serverName = $promptForServer
}

$promptForDatabase = Read-Host "Enter database name [$databaseName]"
if ($promptForDatabase) {
    $databaseName = $promptForDatabase
}

Write-Host ""
Write-Host "Checking connection to database server..." -ForegroundColor Cyan

# Test connection to SQL Server
try {
    $testConnection = sqlcmd -S $serverName -Q "SELECT @@VERSION" -b -h -1
    Write-Host "✓ Connected to SQL Server: $serverName" -ForegroundColor Green
    Write-Host $testConnection[0] -ForegroundColor Gray
}
catch {
    Write-Host "❌ Failed to connect to SQL Server: $serverName" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    exit 1
}

# Test connection to database
Write-Host ""
Write-Host "Checking connection to database..." -ForegroundColor Cyan
try {
    $testDbConnection = sqlcmd -S $serverName -d $databaseName -Q "SELECT DB_NAME() AS [Current Database]" -b -h -1
    Write-Host "✓ Connected to database: $databaseName" -ForegroundColor Green
}
catch {
    Write-Host "❌ Failed to connect to database: $databaseName" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    
    # List available databases
    Write-Host ""
    Write-Host "Available databases on server $serverName:" -ForegroundColor Cyan
    $databases = sqlcmd -S $serverName -Q "SELECT name FROM sys.databases ORDER BY name" -h -1
    foreach ($db in $databases) {
        if ($db -and $db.Trim() -ne "") {
            Write-Host "- $db" -ForegroundColor White
        }
    }
    
    $promptToCreate = Read-Host "Would you like to try a different database? (y/n)"
    if ($promptToCreate -eq "y") {
        $databaseName = Read-Host "Enter database name"
        # Test connection to the new database
        try {
            $testDbConnection = sqlcmd -S $serverName -d $databaseName -Q "SELECT DB_NAME() AS [Current Database]" -b -h -1
            Write-Host "✓ Connected to database: $databaseName" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Failed to connect to database: $databaseName" -ForegroundColor Red
            Write-Host "Please check database name and try again." -ForegroundColor Yellow
            exit 1
        }
    }
    else {
        exit 1
    }
}

# Ask if user wants to apply fixes
Write-Host ""
$promptToFix = Read-Host "Would you like to run diagnostics only (D) or diagnose and apply fixes (F)? [D/F]"
if ($promptToFix -eq "F" -or $promptToFix -eq "f") {
    $applyFixes = $true
    Write-Host "Will apply fixes automatically" -ForegroundColor Yellow
}
else {
    Write-Host "Running in diagnostic mode only (no changes will be made)" -ForegroundColor Yellow
    # Modify the SQL file to comment out the FIXES section
    $sqlContent = Get-Content $sqlFile -Raw
    $sqlContent = $sqlContent -replace "-- PART 2: AUTOMATIC FIXES", "/* -- PART 2: AUTOMATIC FIXES"
    $sqlContent = $sqlContent -replace "-- Final summary", "*/ -- Final summary"
    $tempSqlFile = ".\DiagnoseOnly.sql"
    $sqlContent | Out-File $tempSqlFile -Encoding utf8
    $sqlFile = $tempSqlFile
}

# Run the SQL script
Write-Host ""
Write-Host "Running diagnostics on database $databaseName..." -ForegroundColor Cyan
Write-Host "This may take a few moments..." -ForegroundColor Yellow
Write-Host ""

try {
    # Run SQL and capture output
    $output = sqlcmd -S $serverName -d $databaseName -i $sqlFile -o $diagOutputFile
    
    # Display results
    Write-Host "Diagnostic results:" -ForegroundColor Green
    Get-Content $diagOutputFile | ForEach-Object {
        if ($_ -match "❌") {
            Write-Host $_ -ForegroundColor Red
        }
        elseif ($_ -match "✓") {
            Write-Host $_ -ForegroundColor Green
        }
        elseif ($_ -match "====== SUMMARY ======") {
            Write-Host ""
            Write-Host $_ -ForegroundColor Cyan
        }
        elseif ($_ -match "====== ") {
            Write-Host ""
            Write-Host $_ -ForegroundColor Cyan
        }
        else {
            Write-Host $_
        }
    }
    
    # Write summary message
    Write-Host ""
    Write-Host "======================================================" -ForegroundColor Cyan
    Write-Host "                 Diagnostic Summary                    " -ForegroundColor Cyan
    Write-Host "======================================================" -ForegroundColor Cyan
    
    if ($applyFixes) {
        Write-Host "Diagnostics and fixes have been applied." -ForegroundColor Green
        Write-Host "Please refresh your browser and test the User Management page again." -ForegroundColor Yellow
    }
    else {
        Write-Host "Diagnostics completed successfully." -ForegroundColor Green
        Write-Host "No changes were made to the database." -ForegroundColor Yellow
        Write-Host "To apply fixes, run this script again and choose option F." -ForegroundColor Yellow
    }
    
    Write-Host ""
    Write-Host "Full diagnostic results have been saved to:" -ForegroundColor Cyan
    Write-Host $diagOutputFile
    
    # Clean up temp file if created
    if ($sqlFile -eq ".\DiagnoseOnly.sql" -and (Test-Path $sqlFile)) {
        Remove-Item $sqlFile
    }
}
catch {
    Write-Host "❌ Error running SQL script" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Yellow
    
    # Clean up temp file if created
    if ($sqlFile -eq ".\DiagnoseOnly.sql" -and (Test-Path $sqlFile)) {
        Remove-Item $sqlFile
    }
    
    exit 1
}

# Recommendations based on results
Write-Host ""
Write-Host "======================================================" -ForegroundColor Cyan
Write-Host "                  Recommendations                      " -ForegroundColor Cyan
Write-Host "======================================================" -ForegroundColor Cyan

$results = Get-Content $diagOutputFile -Raw

if ($results -match "AspNetUsers table does not exist") {
    Write-Host "❌ The AspNetUsers table doesn't exist in this database." -ForegroundColor Red
    Write-Host "   This indicates you may be connected to the wrong database." -ForegroundColor Yellow
    Write-Host "   Please check your connection string in appsettings.json" -ForegroundColor Yellow
}

if ($results -match "Total users: 0") {
    Write-Host "❌ No users found in the database." -ForegroundColor Red
    Write-Host "   This indicates either:" -ForegroundColor Yellow
    Write-Host "   1. The application is connected to a different database" -ForegroundColor Yellow
    Write-Host "   2. User registration is failing silently" -ForegroundColor Yellow
    Write-Host "   Check the sign-up process and connection strings." -ForegroundColor Yellow
}

if ($results -match "Status column is MISSING") {
    Write-Host "❌ The Status column is missing in AspNetUsers." -ForegroundColor Red
    Write-Host "   This indicates a migration issue or database schema mismatch." -ForegroundColor Yellow
    Write-Host "   Run database migrations to add this column." -ForegroundColor Yellow
}

if ($results -match "Total documents: 0" -and $results -match "Total users: [1-9]") {
    Write-Host "❌ Users exist but no documents are uploaded." -ForegroundColor Red
    Write-Host "   This could indicate issues with the file upload process" -ForegroundColor Yellow
    Write-Host "   or storage permissions." -ForegroundColor Yellow
}

if ($results -match "OrphanedDocuments:\s+[1-9]") {
    Write-Host "❌ Orphaned documents found (documents without associated users)." -ForegroundColor Red
    Write-Host "   This could indicate issues with the user registration process" -ForegroundColor Yellow
    Write-Host "   or database constraints." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Check your appsettings.json to ensure the connection string is correct" -ForegroundColor White
Write-Host "2. Clear browser cache and restart the application" -ForegroundColor White
Write-Host "3. Test the User Management page again" -ForegroundColor White
Write-Host "4. If problems persist, check application logs for errors" -ForegroundColor White
Write-Host "" 