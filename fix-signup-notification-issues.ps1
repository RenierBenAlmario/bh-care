# PowerShell script to fix sign-up and notification issues
Write-Host "Barangay Health Center Sign-Up Fix Tool" -ForegroundColor Cyan
Write-Host "----------------------------------------" -ForegroundColor Cyan

# Configuration
$sqlInstance = "DESKTOP-NU53VS3"  # From appsettings.json
$database = "Barangay"            # From appsettings.json

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

# Function to execute SQL scripts
function Execute-SqlScript {
    param (
        [string]$scriptPath,
        [string]$description
    )
    
    try {
        Write-Host "Executing: $description..." -ForegroundColor Yellow
        
        if (-not (Test-Path $scriptPath)) {
            Write-Host "Error: Script file not found at path: $scriptPath" -ForegroundColor Red
            return $false
        }
        
        # Execute the SQL script using sqlcmd
        sqlcmd -S $sqlInstance -d $database -i $scriptPath -o "$($scriptPath)_output.txt"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Successfully executed: $description" -ForegroundColor Green
            return $true
        } else {
            Write-Host "✗ Failed to execute: $description (Exit code: $LASTEXITCODE)" -ForegroundColor Red
            if (Test-Path "$($scriptPath)_output.txt") {
                Write-Host "Error details:" -ForegroundColor Red
                Get-Content "$($scriptPath)_output.txt" | ForEach-Object {
                    if ($_ -match "error|exception|fail" -and $_ -notmatch "successfully") {
                        Write-Host "  $_" -ForegroundColor Red
                    }
                }
            }
            return $false
        }
    }
    catch {
        Write-Host "Exception occurred while executing SQL script: $_" -ForegroundColor Red
        return $false
    }
}

# Function to restart the application
function Restart-Application {
    Write-Host "Restarting the Barangay Health Center application..." -ForegroundColor Yellow
    
    try {
        # Check if the application is running as IIS site or process
        $iisAppCmd = "$env:SystemRoot\System32\inetsrv\appcmd.exe"
        if (Test-Path $iisAppCmd) {
            # Assume it's running in IIS
            Write-Host "Application appears to be running in IIS. Attempting to restart..." -ForegroundColor Yellow
            & $iisAppCmd stop apppool /apppool.name:"Barangay" 2>&1 | Out-Null
            Start-Sleep -Seconds 2
            & $iisAppCmd start apppool /apppool.name:"Barangay" 2>&1 | Out-Null
            Write-Host "IIS Application Pool restarted." -ForegroundColor Green
        }
        else {
            # Check for running dotnet process
            $processes = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowTitle -match "Barangay" -or $_.CommandLine -match "Barangay" }
            
            if ($processes -and $processes.Count -gt 0) {
                Write-Host "Found $($processes.Count) Barangay application processes running." -ForegroundColor Yellow
                foreach ($process in $processes) {
                    Write-Host "Stopping process: $($process.Id)" -ForegroundColor Yellow
                    $process.Kill()
                }
                Write-Host "Processes stopped. Please restart the application manually." -ForegroundColor Green
            }
            else {
                Write-Host "No running application detected. Please restart the application manually." -ForegroundColor Yellow
            }
        }
        
        return $true
    }
    catch {
        Write-Host "Failed to restart application: $_" -ForegroundColor Red
        Write-Host "Please restart the application manually." -ForegroundColor Yellow
        return $false
    }
}

# Main script execution

# 1. Check database connection
Write-Host "`n[1/5] Checking database connection..." -ForegroundColor Cyan
$checkScript = @"
PRINT 'Testing connection to ' + DB_NAME() + ' database on ' + @@SERVERNAME;
SELECT 'Connection successful' AS Result;
GO
"@

$checkScriptPath = ".\check-connection.sql"
Set-Content -Path $checkScriptPath -Value $checkScript -Encoding UTF8

if (-not (Execute-SqlScript -scriptPath $checkScriptPath -description "Database connection test")) {
    Write-Host "Cannot proceed without database connection. Please check SQL Server is running and connection string is correct." -ForegroundColor Red
    exit 1
}

# 2. Run the notification and user sync fix script
Write-Host "`n[2/5] Running notification and user sync fix script..." -ForegroundColor Cyan
if (-not (Execute-SqlScript -scriptPath ".\fix-notification-user-sync.sql" -description "Notification and user sync fix")) {
    $proceed = Read-Host "Database fix script encountered errors. Do you want to continue anyway? (y/n)"
    if ($proceed -ne "y") {
        Write-Host "Operation cancelled by user." -ForegroundColor Yellow
        exit 1
    }
}

# 3. Run verification SQL query to confirm results
Write-Host "`n[3/5] Verifying database state..." -ForegroundColor Cyan
$verifyScript = @"
-- Verification Script
PRINT 'Verifying database state after fixes...';

-- Users by status
SELECT 'Users by status:' AS Info;
SELECT Status, COUNT(*) AS Count 
FROM AspNetUsers 
GROUP BY Status;

-- Documents by status
SELECT 'Documents by status:' AS Info;
SELECT Status, COUNT(*) AS Count 
FROM UserDocuments 
GROUP BY Status;

-- Unread notifications
SELECT 'Notification status:' AS Info;
SELECT COUNT(*) AS TotalNotifications,
    SUM(CASE WHEN ReadAt IS NULL THEN 1 ELSE 0 END) AS UnreadNotifications
FROM Notifications;

-- Check for orphaned data
SELECT 'Orphaned data check:' AS Info;
SELECT 
    (SELECT COUNT(*) FROM UserDocuments WHERE UserId NOT IN (SELECT Id FROM AspNetUsers)) AS OrphanedDocuments,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Status = 'Pending' AND Id NOT IN (SELECT UserId FROM UserDocuments)) AS PendingUsersWithoutDocuments;
GO
"@

$verifyScriptPath = ".\verify-database-fix.sql"
Set-Content -Path $verifyScriptPath -Value $verifyScript -Encoding UTF8

Execute-SqlScript -scriptPath $verifyScriptPath -description "Database state verification"

# 4. Restart the application
Write-Host "`n[4/5] Restarting the application..." -ForegroundColor Cyan
Restart-Application

# 5. Provide post-fix instructions
Write-Host "`n[5/5] Post-fix instructions" -ForegroundColor Cyan
Write-Host @"

Database fixes have been applied to synchronize user registrations and notifications.

Next steps:
1. If the application didn't restart automatically, please restart it manually.
2. After starting the application, log in as admin and verify the User Management page.
3. The notification count should now accurately reflect the number of pending users.
4. Test the sign-up process with a new user to ensure it works properly.
5. Verify that the new user appears in both the database and the User Management page.

For ongoing monitoring, check the application logs for any errors during sign-up.
"@ -ForegroundColor White

Write-Host "`nFix process completed." -ForegroundColor Green 