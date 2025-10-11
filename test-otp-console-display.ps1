# OTP Console Display Test Script
# This script helps test the OTP console display functionality

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "BHCARE OTP Console Display Test" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "This script will help you test the OTP console display feature." -ForegroundColor Yellow
Write-Host ""

# Check if the application is running
Write-Host "1. Starting the BHCARE application..." -ForegroundColor Green
Write-Host "   Run: dotnet run" -ForegroundColor White
Write-Host ""

Write-Host "2. Test Accounts that require OTP:" -ForegroundColor Green
Write-Host "   - Any Gmail account (e.g., test@gmail.com)" -ForegroundColor White
Write-Host "   - doctor@example.com (if OTP is enabled)" -ForegroundColor White
Write-Host "   - nurse@example.com (if OTP is enabled)" -ForegroundColor White
Write-Host ""

Write-Host "3. Test Steps:" -ForegroundColor Green
Write-Host "   a) Open your browser and go to the login page" -ForegroundColor White
Write-Host "   b) Enter a Gmail email address" -ForegroundColor White
Write-Host "   c) Enter any password" -ForegroundColor White
Write-Host "   d) Click Login" -ForegroundColor White
Write-Host "   e) Check the console output for the OTP display box" -ForegroundColor White
Write-Host ""

Write-Host "4. Expected Console Output:" -ForegroundColor Green
Write-Host "   You should see a formatted box like this:" -ForegroundColor White
Write-Host ""
Write-Host "   ╔══════════════════════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "   ║                              OTP VERIFICATION CODE                          ║" -ForegroundColor Cyan
Write-Host "   ╠══════════════════════════════════════════════════════════════════════════════╣" -ForegroundColor Cyan
Write-Host "   ║  Email: your-email@gmail.com                                                ║" -ForegroundColor Cyan
Write-Host "   ║  OTP Code: 123456                                                           ║" -ForegroundColor Cyan
Write-Host "   ║  Generated: 2024-01-15 14:30:25 UTC                                         ║" -ForegroundColor Cyan
Write-Host "   ║  Expires: 2024-01-15 14:35:25 UTC                                           ║" -ForegroundColor Cyan
Write-Host "   ╚══════════════════════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

Write-Host "5. Configuration Check:" -ForegroundColor Green
Write-Host "   Make sure OTPSettings.EnableConsoleDisplay is set to 'true' in:" -ForegroundColor White
Write-Host "   - appsettings.json" -ForegroundColor White
Write-Host "   - appsettings.Development.json" -ForegroundColor White
Write-Host "   - appsettings.Production.json" -ForegroundColor White
Write-Host ""

Write-Host "6. Troubleshooting:" -ForegroundColor Green
Write-Host "   - If OTP doesn't appear on console, check configuration files" -ForegroundColor White
Write-Host "   - If build fails, check for syntax errors in OTPService.cs" -ForegroundColor White
Write-Host "   - If OTP validation fails, ensure you're using the exact code from console" -ForegroundColor White
Write-Host ""

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "Ready to test! Start the application with 'dotnet run'" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
