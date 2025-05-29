@echo off
echo Barangay Health Center System - Database Fix Utility
echo ===================================================
echo This script will fix the notification table structure by adding missing columns.
echo.

REM Get SQL Server instance name
set /p instance="Enter SQL Server instance name (default: localhost): " || set instance=localhost
if "%instance%"=="" set instance=localhost

REM Get database name
set /p database="Enter database name (default: Barangay): " || set database=Barangay
if "%database%"=="" set database=Barangay

REM Get authentication type
echo.
echo Authentication type:
echo 1. Windows Authentication
echo 2. SQL Server Authentication
set /p auth_type="Select authentication type (1/2): " || set auth_type=1
if "%auth_type%"=="" set auth_type=1

set sqlcmd_params=-S %instance% -d %database%

if "%auth_type%"=="2" (
    set /p username="Enter SQL Server username: "
    set /p password="Enter SQL Server password: "
    set sqlcmd_params=%sqlcmd_params% -U %username% -P %password%
) else (
    set sqlcmd_params=%sqlcmd_params% -E
)

echo.
echo Running database fix script...
echo.

REM Run the SQL script
sqlcmd %sqlcmd_params% -i fix-notification-columns.sql -o fix-notification-results.txt

echo.
echo Script execution completed. Check fix-notification-results.txt for details.
echo.

pause 