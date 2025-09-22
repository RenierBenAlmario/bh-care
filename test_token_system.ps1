# Test Token System Script

Write-Host "Testing Token Protection System..." -ForegroundColor Green

# Test 1: Check if application is running
Write-Host "`n1. Testing application connectivity..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/" -TimeoutSec 10
    Write-Host "✓ Application is running (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "✗ Application is not responding: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Test 2: Test token creation
Write-Host "`n2. Testing token creation..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/TokenTest/TestTokenCreation" -TimeoutSec 10
    $content = $response.Content | ConvertFrom-Json
    if ($content.success) {
        Write-Host "✓ Token creation successful" -ForegroundColor Green
        Write-Host "  Token: $($content.token.Substring(0, 50))..." -ForegroundColor Cyan
        Write-Host "  User ID: $($content.userId)" -ForegroundColor Cyan
        Write-Host "  Original URL: $($content.originalUrl)" -ForegroundColor Cyan
    } else {
        Write-Host "✗ Token creation failed: $($content.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Token creation test failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test protected URL generation
Write-Host "`n3. Testing protected URL generation..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/TokenTest/TestProtectedUrl" -TimeoutSec 10
    $content = $response.Content | ConvertFrom-Json
    if ($content.success) {
        Write-Host "✓ Protected URL generation successful" -ForegroundColor Green
        Write-Host "  Protected URL: $($content.protectedUrl)" -ForegroundColor Cyan
    } else {
        Write-Host "✗ Protected URL generation failed: $($content.message)" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Protected URL generation test failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 4: Test token system page
Write-Host "`n4. Testing token system page..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5003/TokenSystemTest" -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Token system test page is accessible" -ForegroundColor Green
    } else {
        Write-Host "✗ Token system test page returned status: $($response.StatusCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ Token system test page failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 Token Protection System Test Complete!" -ForegroundColor Green
Write-Host "Visit http://localhost:5003/TokenSystemTest to test the system interactively" -ForegroundColor Cyan
