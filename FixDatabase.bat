@echo off
echo ====== Barangay Health Center Database Fix ======
echo This script will fix database issues with appointments

REM Get connection information from user
set /p server=Enter SQL Server name (default: localhost): 
if "%server%"=="" set server=localhost

set /p database=Enter database name (default: Barangay): 
if "%database%"=="" set database=Barangay

echo Choose authentication method:
echo 1. Windows Authentication
echo 2. SQL Server Authentication
set /p auth_choice=Enter choice (1 or 2): 

if "%auth_choice%"=="2" (
    set /p username=Enter SQL Server username: 
    set /p password=Enter SQL Server password: 
    set auth_params=-U %username% -P %password%
) else (
    set auth_params=-E
)

echo.
echo ====== Starting Database Fix ======
echo.

echo Fixing database schema...
sqlcmd -S %server% -d %database% %auth_params% -i FixAppointmentData.sql -o FixAppointmentData_results.txt

echo Creating test data...
sqlcmd -S %server% -d %database% %auth_params% -i CreateTestData.sql -o CreateTestData_results.txt

echo.
echo ====== Database Fix Complete ======
echo Results saved to *_results.txt files

echo.
echo Press any key to exit...
pause > nul 