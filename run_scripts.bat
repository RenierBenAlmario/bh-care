@echo off
echo ======================================
echo Barangay Health Care System SQL Setup
echo ======================================
echo.

rem Set your SQL Server connection details here
set server=DESKTOP-NU53VS3
set database=Barangay
set username=
set password=

echo Setting up User Permissions System...
echo ------------------------------------
if "%username%"=="" (
    sqlcmd -S %server% -d %database% -E -i permissions_schema.sql
) else (
    sqlcmd -S %server% -d %database% -U %username% -P %password% -i permissions_schema.sql
)
echo.

echo Creating Permission Management Procedures...
echo -----------------------------------------
if "%username%"=="" (
    sqlcmd -S %server% -d %database% -E -i permissions_procedures.sql
) else (
    sqlcmd -S %server% -d %database% -U %username% -P %password% -i permissions_procedures.sql
)
echo.

echo Creating Prescription Management Procedures...
echo ------------------------------------------
if "%username%"=="" (
    sqlcmd -S %server% -d %database% -E -i prescription_procedures.sql
) else (
    sqlcmd -S %server% -d %database% -U %username% -P %password% -i prescription_procedures.sql
)
echo.

echo Setting up Medical Records Linking...
echo ---------------------------------
if "%username%"=="" (
    sqlcmd -S %server% -d %database% -E -i prescription_linking.sql
) else (
    sqlcmd -S %server% -d %database% -U %username% -P %password% -i prescription_linking.sql
)
echo.

echo Adding Sample Permission Data...
echo -----------------------------
if "%username%"=="" (
    sqlcmd -S %server% -d %database% -E -i permission_sample_data.sql
) else (
    sqlcmd -S %server% -d %database% -U %username% -P %password% -i permission_sample_data.sql
)
echo.

echo ======================================
echo SQL Setup Complete
echo ======================================

pause 