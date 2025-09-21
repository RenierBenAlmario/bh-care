@echo off
echo ===== BHCARE Booking Form Database Update =====
echo.
echo This script will update your database with all necessary changes for the booking form.
echo.
echo Press any key to continue or CTRL+C to cancel...
pause > nul

powershell.exe -ExecutionPolicy Bypass -File "%~dp0Update-BookingForm-Database.ps1"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo An error occurred while updating the database. Please check the output above.
    echo.
    pause
    exit /b 1
) else (
    echo.
    echo Database update completed successfully!
    echo.
    pause
) 