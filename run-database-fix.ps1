# PowerShell script to run the database fix script
Write-Host "Barangay Health Center Database Fix" -ForegroundColor Cyan
Write-Host "-----------------------------------" -ForegroundColor Cyan

# Configuration
$sqlInstance = "DESKTOP-NU53VS3"  # From appsettings.json
$database = "Barangay"            # From appsettings.json
$fixScriptPath = ".\fix-notification-user-sync.sql"

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

# Check if fix script exists
if (-not (Test-Path $fixScriptPath)) {
    Write-Host "Error: Fix script not found at: $fixScriptPath" -ForegroundColor Red
    exit 1
}

# Function to execute the fix script
function Execute-FixScript {
    try {
        Write-Host "Executing database fix script..." -ForegroundColor Yellow
        
        # Execute the SQL script using sqlcmd
        sqlcmd -S $sqlInstance -d $database -i $fixScriptPath -o "fix-script-output.txt"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Database fix script executed successfully" -ForegroundColor Green
            
            # Display the output content
            if (Test-Path "fix-script-output.txt") {
                Write-Host "`nScript output:" -ForegroundColor Cyan
                Get-Content "fix-script-output.txt" | ForEach-Object {
                    # Color coding for various log types
                    if ($_ -match "Error:") {
                        Write-Host "  $_" -ForegroundColor Red
                    }
                    elseif ($_ -match "Updated|Deleted|Fixed|Reconciled") {
                        Write-Host "  $_" -ForegroundColor Green
                    }
                    elseif ($_ -match "Starting|Checking|Fixing|Synchronizing") {
                        Write-Host "  $_" -ForegroundColor Yellow
                    }
                    elseif ($_ -match "SUMMARY REPORT|Transaction committed") {
                        Write-Host "  $_" -ForegroundColor Cyan
                    }
                    else {
                        Write-Host "  $_" -ForegroundColor White
                    }
                }
            }
            
            return $true
        } else {
            Write-Host "✗ Failed to execute database fix script (Exit code: $LASTEXITCODE)" -ForegroundColor Red
            if (Test-Path "fix-script-output.txt") {
                Write-Host "Error details:" -ForegroundColor Red
                Get-Content "fix-script-output.txt" | ForEach-Object {
                    if ($_ -match "error|exception|fail" -and $_ -notmatch "successfully") {
                        Write-Host "  $_" -ForegroundColor Red
                    }
                }
            }
            return $false
        }
    }
    catch {
        Write-Host "Exception occurred while executing fix script: $_" -ForegroundColor Red
        return $false
    }
}

# Main script execution
$choice = Read-Host "Do you want to run the database fix script? (y/n)"

if ($choice -eq "y") {
    $success = Execute-FixScript
    
    if ($success) {
        Write-Host "`nDatabase fix completed successfully." -ForegroundColor Green
        Write-Host "`nNext steps:" -ForegroundColor Cyan
        Write-Host "1. Restart the application to apply the changes" -ForegroundColor White
        Write-Host "2. Run the verify-registration-fixes.ps1 script to verify the fixes" -ForegroundColor White
    }
    else {
        Write-Host "`nDatabase fix encountered errors. Check the output for details." -ForegroundColor Red
    }
}
else {
    Write-Host "Operation cancelled." -ForegroundColor Yellow
}

Write-Host "`nScript completed." -ForegroundColor Cyan 