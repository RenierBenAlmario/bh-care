@echo off
echo === Barangay Health Center Fixes ===
echo.
echo This script will apply all fixes to resolve Entity Framework relationship issues
echo and fix SqlNullValueException during data seeding.
echo.
echo 1. Stopping any running Barangay processes...
taskkill /F /IM Barangay.exe 2>nul
echo.

echo 2. Applying database fixes...
sqlcmd -S "THEBENZZZ10\MSSQLSERVER04" -d Barangay -E -i final-database-fix.sql -o fix-results.txt
if %ERRORLEVEL% NEQ 0 (
  echo Database fix script failed! Check fix-results.txt for details.
  exit /b 1
) else (
  echo Database fixes applied successfully.
)
echo.

echo 3. Building application...
dotnet build
if %ERRORLEVEL% NEQ 0 (
  echo Build failed! Fix code errors first.
  exit /b 1
) else (
  echo Build completed successfully.
)
echo.

echo 4. Application is ready to start
echo.
echo To run the application, use:
echo   dotnet run
echo.
echo All fixes have been applied successfully!
echo For more information, see solution-summary.md 