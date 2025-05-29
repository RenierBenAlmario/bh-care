@echo off
echo Database Schema Fix for Barangay Health Center System
echo ====================================================
echo.

echo Checking if sqlcmd is available...
where sqlcmd >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: sqlcmd is not available. Please install SQL Server Command Line Utilities.
    echo You can run the SQL script directly in SQL Server Management Studio instead.
    echo See README-DATABASE-FIX.md for instructions.
    pause
    exit /b 1
)

echo sqlcmd is available. Proceeding with database fix.
echo.

set /p server="Enter SQL Server instance name (default=(localdb)\MSSQLLocalDB): "
if "%server%"=="" set server=(localdb)\MSSQLLocalDB

set /p database="Enter database name (default=BarangayHealthCenter): "
if "%database%"=="" set database=BarangayHealthCenter

echo.
echo Applying migration to %database% on %server%...
echo.

sqlcmd -S %server% -d %database% -i ApplyAgreedToTermsColumnsMigration.sql

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Failed to apply migration. Please check your server name and database name.
    pause
    exit /b 1
)

echo.
echo Verifying database columns...
echo.

sqlcmd -S %server% -d %database% -i VerifyDatabaseColumns.sql

echo.
echo Database fix completed. Please check the output above to verify the columns were added successfully.
echo.
echo Restart the application to ensure the changes take effect.
echo.

pause 