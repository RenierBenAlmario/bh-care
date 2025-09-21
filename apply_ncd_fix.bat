@echo off
echo Running NCDRiskAssessment database fix...
sqlcmd -S .\SQLEXPRESS -d Barangay -i fix_ncd_risk_assessment.sql -o ncd_fix_results.txt
echo Fix completed. Check ncd_fix_results.txt for details.
pause