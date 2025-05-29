@echo off
echo Running script to fix admin staff role assignment...
echo.

REM Set your SQL Server connection details here
set SERVER=localhost
set DATABASE=BarangayHealthCare
set TRUSTED_CONNECTION=yes

echo Executing fix-admin-staff-role.sql on %SERVER%, database %DATABASE%...
sqlcmd -S %SERVER% -d %DATABASE% -E -i scripts/fix-admin-staff-role.sql

echo.
echo Script execution completed.
echo Please check the output above for any errors or success messages.
pause 