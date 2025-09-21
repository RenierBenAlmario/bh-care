# PowerShell script to trigger VitalSigns encryption
Write-Host "=== VitalSigns Encryption Trigger ===" -ForegroundColor Green
Write-Host ""

# Check if the application is running
Write-Host "Checking if application is running on port 5003..." -ForegroundColor Yellow
$process = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like "*Barangay*" }

if ($process) {
    Write-Host "✅ Application is running. Process ID: $($process.Id)" -ForegroundColor Green
} else {
    Write-Host "❌ Application is not running. Please start it first." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🔐 To encrypt your VitalSigns data, please follow these steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Open your web browser" -ForegroundColor White
Write-Host "2. Navigate to: https://localhost:5003/DataEncryption" -ForegroundColor White
Write-Host "3. Login as Admin:" -ForegroundColor White
Write-Host "   - Email: admin@example.com" -ForegroundColor Gray
Write-Host "   - Password: Admin@123" -ForegroundColor Gray
Write-Host "4. Check the confirmation checkbox" -ForegroundColor White
Write-Host "5. Click 'Encrypt All Existing Data'" -ForegroundColor White
Write-Host ""
Write-Host "📊 After encryption, verify in SQL Server Management Studio:" -ForegroundColor Yellow
Write-Host "   SELECT TOP 5 Id, Temperature, BloodPressure, HeartRate FROM [Barangay].[dbo].[VitalSigns];" -ForegroundColor Gray
Write-Host ""
Write-Host "   The data should now show as encrypted strings instead of plain text." -ForegroundColor Gray
Write-Host ""

# Try to open the browser automatically
try {
    Write-Host "🌐 Opening browser to DataEncryption page..." -ForegroundColor Cyan
    Start-Process "https://localhost:5003/DataEncryption"
    Write-Host "✅ Browser opened successfully!" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Could not open browser automatically. Please navigate manually." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
