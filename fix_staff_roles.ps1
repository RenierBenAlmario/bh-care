# Fix staff roles using the API endpoint
try {
    $uri = "https://localhost:5003/api/Admin/FixStaffRoles"
    $response = Invoke-RestMethod -Uri $uri -Method POST -UseBasicParsing
    Write-Host "API Response: $($response | ConvertTo-Json)"
} catch {
    Write-Host "Error calling API: $($_.Exception.Message)"
    Write-Host "This is normal if the application is not running or SSL certificate issues"
}
