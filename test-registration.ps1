# PowerShell script to test registration process
Write-Host "Barangay Health Center Registration Test" -ForegroundColor Cyan
Write-Host "--------------------------------------" -ForegroundColor Cyan

# Configuration
$sqlInstance = "DESKTOP-NU53VS3"  # From appsettings.json
$database = "Barangay"            # From appsettings.json
$testScriptPath = ".\test-registration.sql"

# Check if sqlcmd is available
try {
    $sqlcmd = Get-Command sqlcmd -ErrorAction Stop
    Write-Host "SQL Command utility found: $($sqlcmd.Path)" -ForegroundColor Green
}
catch {
    Write-Host "SQL Command utility (sqlcmd) not found. Please install SQL Server Command Line Tools." -ForegroundColor Red
    Write-Host "You can download it from: https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility" -ForegroundColor Yellow
    exit 1
}

# Run the test script
try {
    Write-Host "Running registration test script..." -ForegroundColor Yellow
    
    # Execute the SQL script using sqlcmd
    sqlcmd -S $sqlInstance -d $database -i $testScriptPath -o "test-registration-output.txt"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Test script executed successfully" -ForegroundColor Green
        
        # Display the output content
        if (Test-Path "test-registration-output.txt") {
            Write-Host "`nTest results:" -ForegroundColor Cyan
            Get-Content "test-registration-output.txt" | ForEach-Object {
                if ($_ -match "Checking|Recently|User counts|Recent document|Checking for mismatches|Recent notifications|Registration verification") {
                    Write-Host "`n$_" -ForegroundColor Yellow
                } else {
                    Write-Host "  $_" -ForegroundColor White
                }
            }
        }
    } else {
        Write-Host "✗ Failed to execute test script (Exit code: $LASTEXITCODE)" -ForegroundColor Red
    }
}
catch {
    Write-Host "Error executing test script: $_" -ForegroundColor Red
}

Write-Host "`nScript completed." -ForegroundColor Cyan 