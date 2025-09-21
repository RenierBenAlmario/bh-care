@echo off
echo BHCARE - Fix Guardian and Permission Issues

REM Set up variables
set SQLCMD="sqlcmd"
set SERVER=localhost
set DATABASE=Barangay
set TRUSTED_CONNECTION=-E

echo Running database fixes...

echo 1. Fixing GuardianInformation table...
%SQLCMD% %TRUSTED_CONNECTION% -S %SERVER% -d %DATABASE% -i "SQL/fix-guardian-information.sql" -o guardian_fix_results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error running fix-guardian-information.sql
    goto error
)

echo 2. Fixing Role Permissions...
%SQLCMD% %TRUSTED_CONNECTION% -S %SERVER% -d %DATABASE% -i "SQL/fix-role-permissions.sql" -o permission_fix_results.txt
if %ERRORLEVEL% NEQ 0 (
    echo Error running fix-role-permissions.sql
    goto error
)

echo All fixes applied successfully!
echo Check guardian_fix_results.txt and permission_fix_results.txt for details.
goto end

:error
echo An error occurred. Please check the error messages above.
exit /b 1

:end
echo Done!
pause 