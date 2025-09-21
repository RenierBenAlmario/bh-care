@echo off
echo Applying fix to add missing NCD columns...
sqlcmd -S .\SQLEXPRESS -d Barangay -i add_missing_ncd_columns.sql -o add_missing_ncd_columns_results.txt
echo Fix completed. Check add_missing_ncd_columns_results.txt for details.
pause