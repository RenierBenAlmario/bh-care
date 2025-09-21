@echo off
echo Running SQL script to insert test appointment for July 15, 2025...

REM Set your SQL Server connection details
set SERVER_NAME=localhost
set DATABASE_NAME=Barangay

echo Using database connection: %SERVER_NAME%/%DATABASE_NAME%
echo.

REM Check if sqlcmd is available
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo Error: sqlcmd is not available in your PATH. Please install SQL Server Command Line Tools.
    goto end
)

REM Try to use Windows Authentication first
echo Attempting to connect using Windows Authentication...
sqlcmd -S %SERVER_NAME% -d %DATABASE_NAME% -E -i InsertTestAppointment.sql -o sql_output.txt

REM Check if the command was successful
if %ERRORLEVEL% EQU 0 (
    echo SQL script execution completed successfully using Windows Authentication.
    goto output
)

REM If Windows Authentication fails, try SQL Authentication
echo Windows Authentication failed, attempting SQL Authentication...
set /p USERNAME=Enter SQL Server username: 
set /p PASSWORD=Enter SQL Server password: 

sqlcmd -S %SERVER_NAME% -d %DATABASE_NAME% -U %USERNAME% -P %PASSWORD% -i InsertTestAppointment.sql -o sql_output.txt

if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to execute SQL script.
    goto end
)

echo SQL script execution completed successfully using SQL Authentication.

:output
echo.
echo Output from SQL script:
echo ----------------------
type sql_output.txt
echo ----------------------
echo.
echo Please check the Doctor Dashboard now to see the test appointment.
echo If the appointment is still not visible, try the following:
echo 1. Verify the doctor ID in the script matches your current login
echo 2. Check if the AppointmentDate field format is correct
echo 3. Look for additional debug information in the console output

:end
pause 