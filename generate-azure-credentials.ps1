# Generate Azure Credentials for GitHub Actions
# This script creates a Service Principal and outputs the credentials needed for GitHub Actions

Write-Host "Generating Azure Credentials for GitHub Actions..." -ForegroundColor Green
Write-Host ""

# Set variables
$subscriptionId = ""
$resourceGroupName = "BHcare"
$appName = "bhcare-github-actions"
$roleName = "Contributor"

# Get current subscription
Write-Host "Getting current Azure subscription..." -ForegroundColor Yellow
$subscription = az account show --query "id" -o tsv
if ($LASTEXITCODE -ne 0) {
    Write-Host "Please login to Azure first: az login" -ForegroundColor Red
    exit 1
}

$subscriptionId = $subscription
Write-Host "Using subscription: $subscriptionId" -ForegroundColor Cyan

# Create Service Principal
Write-Host "Creating Service Principal..." -ForegroundColor Yellow
$spOutput = az ad sp create-for-rbac --name $appName --role $roleName --scopes "/subscriptions/$subscriptionId/resourceGroups/$resourceGroupName" --json-auth
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create Service Principal" -ForegroundColor Red
    exit 1
}

# Parse the JSON output
$credentials = $spOutput | ConvertFrom-Json

Write-Host ""
Write-Host "✅ Azure Credentials Generated Successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Add these secrets to your GitHub repository:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Go to: https://github.com/RenierBenAlmario/bh-care/settings/secrets/actions" -ForegroundColor White
Write-Host "2. Click 'New repository secret' and add these:" -ForegroundColor White
Write-Host ""

Write-Host "🔑 AZURE_CREDENTIALS:" -ForegroundColor Yellow
Write-Host $spOutput -ForegroundColor White
Write-Host ""

Write-Host "🔑 AZURE_WEBAPP_NAME:" -ForegroundColor Yellow
Write-Host "bhcare-webapp" -ForegroundColor White
Write-Host ""

Write-Host "🔑 AZURE_RESOURCE_GROUP:" -ForegroundColor Yellow
Write-Host "BHcare" -ForegroundColor White
Write-Host ""

Write-Host "🔑 AZURE_SQL_CONNECTION_STRING:" -ForegroundColor Yellow
Write-Host "Server=tcp:bhcare.database.windows.net,1433;Initial Catalog=bhcareDB;Persist Security Info=False;User ID=bhcare;Password=Thebenzzz10;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" -ForegroundColor White
Write-Host ""

Write-Host "📝 Service Principal Details:" -ForegroundColor Cyan
Write-Host "App ID: $($credentials.appId)" -ForegroundColor White
Write-Host "Password: $($credentials.password)" -ForegroundColor White
Write-Host "Tenant: $($credentials.tenant)" -ForegroundColor White
Write-Host ""

Write-Host "🚀 Next Steps:" -ForegroundColor Green
Write-Host "1. Add the secrets to GitHub" -ForegroundColor White
Write-Host "2. Push your code to GitHub: git add . && git commit -m 'Add Azure deployment' && git push origin main" -ForegroundColor White
Write-Host "3. GitHub Actions will automatically deploy your app!" -ForegroundColor White
Write-Host ""

Write-Host "⚠️  Important Security Notes:" -ForegroundColor Red
Write-Host "- Keep these credentials secure" -ForegroundColor White
Write-Host "- The password will only be shown once" -ForegroundColor White
Write-Host "- Store them only in GitHub Secrets" -ForegroundColor White
Write-Host ""

pause
