# BHCARE Database Cleanup PowerShell Script
# This script removes all data except the system admin account

Write-Host "========================================" -ForegroundColor Yellow
Write-Host "BHCARE Database Cleanup Script" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Yellow
Write-Host ""
Write-Host "WARNING: This will delete ALL data except the system admin account!" -ForegroundColor Red
Write-Host "Admin account (admin@example.com) will be preserved." -ForegroundColor Green
Write-Host ""
Write-Host "This action cannot be undone!" -ForegroundColor Red
Write-Host ""

$confirm = Read-Host "Are you sure you want to continue? (yes/no)"

if ($confirm -ne "yes") {
    Write-Host "Operation cancelled." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Starting database cleanup..." -ForegroundColor Green
Write-Host ""

# Database connection parameters
$server = "tcp:bhcare.database.windows.net,1433"
$database = "bhcareDB"
$username = "bhcare"
$password = "Thebenzzz10"
$scriptPath = "SQL\force-cleanup.sql"
$logFile = "cleanup-log.txt"

try {
    # Check if SQL script exists
    if (-not (Test-Path $scriptPath)) {
        Write-Host "ERROR: SQL script not found at: $scriptPath" -ForegroundColor Red
        exit 1
    }

    # Run the SQL script using sqlcmd
    $sqlcmdArgs = @(
        "-S", $server,
        "-d", $database,
        "-U", $username,
        "-P", $password,
        "-i", $scriptPath,
        "-o", $logFile
    )

    Write-Host "Executing SQL script..." -ForegroundColor Cyan
    $result = & sqlcmd @sqlcmdArgs

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "Database cleanup completed successfully!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "Check $logFile for detailed output." -ForegroundColor Cyan
        Write-Host ""
        Write-Host "The database has been reset with only the admin account preserved." -ForegroundColor Green
        Write-Host "Admin login: admin@example.com" -ForegroundColor Yellow
        Write-Host "Admin password: Admin@123" -ForegroundColor Yellow
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Red
        Write-Host "Database cleanup failed!" -ForegroundColor Red
        Write-Host "========================================" -ForegroundColor Red
        Write-Host ""
        Write-Host "Check $logFile for error details." -ForegroundColor Red
        Write-Host "Exit code: $LASTEXITCODE" -ForegroundColor Red
        Write-Host ""
    }
}
catch {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Error occurred during cleanup!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
}

Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
