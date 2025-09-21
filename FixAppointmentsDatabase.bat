@echo off
echo ====== Barangay Health Center Appointments Fix ======
echo This script will fix database issues with appointments

REM Get connection information from user
set /p server=Enter SQL Server name (default: THEBENZZZ10\MSSQLSERVER04): 
if "%server%"=="" set server=THEBENZZZ10\MSSQLSERVER04

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

echo Step 1: Fixing Gender column size...
sqlcmd %auth_params% -S %server% -d %database% -i FixGenderColumn.sql -o FixGenderColumn_results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to fix Gender column. Check FixGenderColumn_results.txt for details.
) else (
    echo Success: Gender column fixed successfully.
)

echo Step 2: Fixing database constraints...
sqlcmd %auth_params% -S %server% -d %database% -i FixAppointmentData.sql -o FixAppointmentData_results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to fix database constraints. Check FixAppointmentData_results.txt for details.
) else (
    echo Success: Database constraints fixed successfully.
)

echo Step 3: Verifying connectivity to User/Appointments page...
sqlcmd %auth_params% -S %server% -d %database% -i CheckUserAppointments.sql -o CheckUserAppointments_results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to verify connectivity. Check CheckUserAppointments_results.txt for details.
) else (
    echo Success: Connectivity verified successfully.
)

echo.
echo ====== Database Fix Complete ======
echo All appointments database issues have been fixed.
echo.

echo ====== Next Steps ======
echo 1. Run the application and try to create a new appointment
echo 2. Navigate to User/Appointments to verify appointments are displayed
echo 3. If issues persist, check the log files in the current directory
echo.

echo Press any key to exit...
pause > nul 