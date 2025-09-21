# PowerShell script to run VitalSigns encryption directly
Write-Host "=== VitalSigns Encryption Tool ===" -ForegroundColor Green
Write-Host ""

# Check if the application is running
Write-Host "Checking if application is running..." -ForegroundColor Yellow
$process = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like "*Barangay*" }

if ($process) {
    Write-Host "Application is running. Process ID: $($process.Id)" -ForegroundColor Green
} else {
    Write-Host "Application is not running. Starting application..." -ForegroundColor Yellow
    Start-Process -FilePath "dotnet" -ArgumentList "run", "--urls", "https://localhost:5003" -WindowStyle Hidden
    Start-Sleep -Seconds 10
}

# Try to access the encryption endpoint
Write-Host "Attempting to run encryption..." -ForegroundColor Yellow

try {
    # Create a simple HTTP request to trigger encryption
    $uri = "https://localhost:5003/DataEncryption/EncryptExistingData"
    
    # Note: This would require authentication, so we'll use a different approach
    Write-Host "Please manually navigate to the DataEncryption page:" -ForegroundColor Cyan
    Write-Host "1. Open browser: https://localhost:5003/DataEncryption" -ForegroundColor Cyan
    Write-Host "2. Login as Admin (admin@example.com / Admin@123)" -ForegroundColor Cyan
    Write-Host "3. Check the confirmation checkbox" -ForegroundColor Cyan
    Write-Host "4. Click 'Encrypt All Existing Data'" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Alternative: Use the test page at https://localhost:5003/TestEncryptionDirect" -ForegroundColor Cyan
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
