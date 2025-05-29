@echo off
echo ======================================================
echo Barangay Management System - Database Update Launcher
echo ======================================================
echo.

echo Step 1: Running migration and database update script...
powershell -ExecutionPolicy Bypass -File .\update-database.ps1
if %ERRORLEVEL% NEQ 0 (
    echo PowerShell script failed with error level %ERRORLEVEL%
    pause
    exit /B %ERRORLEVEL%
)

echo.
echo Step 2: Verifying database structure...

:: Extract server and database information from appsettings.json using PowerShell
for /f "tokens=* USEBACKQ" %%a in (
    `powershell -Command "(Get-Content -Raw appsettings.json | ConvertFrom-Json).ConnectionStrings.DefaultConnection -match 'Server=(.*?);' | Out-Null; $Matches[1]"`
) do set SERVER=%%a

for /f "tokens=* USEBACKQ" %%a in (
    `powershell -Command "(Get-Content -Raw appsettings.json | ConvertFrom-Json).ConnectionStrings.DefaultConnection -match 'Database=(.*?);' | Out-Null; $Matches[1]"`
) do set DATABASE=%%a

echo Using SQL Server: %SERVER%
echo Database: %DATABASE%

:: Run SQL verification script
sqlcmd -S %SERVER% -d %DATABASE% -E -i verify-database-structure.sql -o database-verification-results.txt

if %ERRORLEVEL% NEQ 0 (
    echo SQL verification script failed with error level %ERRORLEVEL%
    pause
    exit /B %ERRORLEVEL%
) else (
    echo Database verification completed. Results saved to database-verification-results.txt
    echo Opening results file...
    start notepad database-verification-results.txt
)

echo.
echo Step 3: Applying additional fixes to database...

echo Step 3.1: Adding missing columns to AspNetUsers table...
sqlcmd -S %SERVER% -d %DATABASE% -E -i fix-missing-columns.sql

if %ERRORLEVEL% NEQ 0 (
    echo Adding missing columns script failed with error level %ERRORLEVEL%
    pause
    exit /B %ERRORLEVEL%
) else (
    echo Missing columns added successfully.
)

echo Step 3.2: Adding missing roles...
sqlcmd -S %SERVER% -d %DATABASE% -E -i fix-missing-roles.sql

if %ERRORLEVEL% NEQ 0 (
    echo Adding missing roles script failed with error level %ERRORLEVEL%
    pause
    exit /B %ERRORLEVEL%
) else (
    echo Missing roles added successfully.
)

echo.
echo Database update and verification completed successfully!
echo Please review the verification results to confirm everything is correct.
echo.

pause 