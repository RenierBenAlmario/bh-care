@echo off
echo ==========================================
echo Barangay Database Safe Update Script
echo ==========================================
echo This script will safely update your database with the latest migrations
echo.

:: Create a timestamp for backup naming
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "YYYY=%dt:~0,4%"
set "MM=%dt:~4,2%"
set "DD=%dt:~6,2%"
set "HH=%dt:~8,2%"
set "Min=%dt:~10,2%"
set "Sec=%dt:~12,2%"
set "timestamp=%YYYY%%MM%%DD%_%HH%%Min%%Sec%"

:: 1. Create a backup first
echo Creating database backup...
sqlcmd -S DESKTOP-NU53VS3 -E -Q "BACKUP DATABASE [Barangay] TO DISK = N'Barangay_backup_%timestamp%.bak' WITH NOFORMAT, NOINIT, NAME = N'Barangay-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
if %ERRORLEVEL% NEQ 0 (
    echo Warning: Could not create database backup.
    echo Do you want to continue without a backup? (Y/N)
    set /p choice=
    if /i not "%choice%"=="Y" exit /b
)

:: 2. Run the fix-cascade-paths.sql script first to ensure any foreign key issues are fixed
echo Applying cascade paths fix...
sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -i fix-cascade-paths.sql -o fix-cascade-paths-results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to apply cascade paths fix.
    exit /b 1
)

:: 3. Apply migrations
echo Applying database migrations...
dotnet ef database update
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to apply migrations. Check the output for details.
    exit /b 1
)

:: 4. Verify the migrations were applied correctly
echo Verifying database constraints...
sqlcmd -S DESKTOP-NU53VS3 -E -d Barangay -Q "SELECT fk.name AS 'Foreign Key', OBJECT_NAME(fk.parent_object_id) AS 'Table', fk.delete_referential_action_desc AS 'Delete Action' FROM sys.foreign_keys fk WHERE OBJECT_NAME(fk.parent_object_id) = 'Messages' ORDER BY fk.name;"
if %ERRORLEVEL% NEQ 0 (
    echo Warning: Could not verify foreign key constraints.
)

echo.
echo Database update completed successfully!
echo Backup created: Barangay_backup_%timestamp%.bak
echo.
pause 