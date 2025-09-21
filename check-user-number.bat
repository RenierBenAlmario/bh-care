@echo off
echo Checking UserNumber column...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF EXISTS(SELECT * FROM sys.columns WHERE Name = N'UserNumber' AND Object_ID = Object_ID(N'AspNetUsers')) PRINT 'UserNumber column exists' ELSE PRINT 'UserNumber column does not exist'"

echo Checking sample data...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "SELECT TOP 5 Id, UserName, Email, UserNumber FROM AspNetUsers ORDER BY UserNumber"

pause 