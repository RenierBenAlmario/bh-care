@echo off
echo Starting database migration update process...
echo.

:: 1. Check if dotnet ef tools are installed
echo Checking if Entity Framework Core tools are installed...
dotnet tool list --global | findstr dotnet-ef > nul
if %ERRORLEVEL% NEQ 0 (
    echo Installing Entity Framework Core tools...
    dotnet tool install --global dotnet-ef
    if %ERRORLEVEL% NEQ 0 (
        echo Failed to install Entity Framework Core tools. Please install manually with: dotnet tool install --global dotnet-ef
        exit /b 1
    )
)

:: 2. Create timestamp for migration name
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "timestamp=%dt:~0,8%_%dt:~8,6%"
set "migrationName=UpdateDatabase_%timestamp%"

:: 3. Add a new migration
echo Creating a new migration: %migrationName%...
dotnet ef migrations add %migrationName%
if %ERRORLEVEL% NEQ 0 (
    echo Failed to create migration. Check for errors above.
    exit /b 1
)
echo Migration created successfully: %migrationName%

:: 4. Update the database
echo Updating the database...
dotnet ef database update
if %ERRORLEVEL% NEQ 0 (
    echo Failed to update database. Check for errors above.
    exit /b 1
)
echo Database updated successfully!

echo.
echo Database migration process completed successfully!
pause 