# Azure Credentials Setup Guide for GitHub Actions

## 🔐 Step-by-Step Setup

### Step 1: Create Azure Service Principal

**Option A: Using PowerShell Script (Recommended)**
```powershell
# Run the provided script
.\generate-azure-credentials.ps1
```

**Option B: Manual Azure CLI Commands**
```bash
# Login to Azure
az login

# Create Service Principal
az ad sp create-for-rbac --name "bhcare-github-actions" --role "Contributor" --scopes "/subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/BHcare" --json-auth
```

### Step 2: Get Your Subscription ID
```bash
az account show --query "id" -o tsv
```

### Step 3: Create Service Principal with Proper Scope
```bash
az ad sp create-for-rbac --name "bhcare-github-actions" --role "Contributor" --scopes "/subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/BHcare" --json-auth
```

This will output JSON like:
```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}
```

### Step 4: Add Secrets to GitHub

1. **Go to your GitHub repository**: https://github.com/RenierBenAlmario/bh-care
2. **Click Settings** → **Secrets and variables** → **Actions**
3. **Click "New repository secret"** and add these secrets:

#### 🔑 Required Secrets:

**AZURE_CREDENTIALS**
- Copy the entire JSON output from Step 3
- This contains all the authentication information

**AZURE_WEBAPP_NAME**
- Value: `bhcare-webapp` (or your App Service name)

**AZURE_RESOURCE_GROUP**
- Value: `BHcare`

**AZURE_SQL_CONNECTION_STRING**
- Value: `Server=tcp:bhcare.database.windows.net,1433;Initial Catalog=bhcareDB;Persist Security Info=False;User ID=bhcare;Password=Thebenzzz10;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`

### Step 5: Create App Service (if not exists)

If you haven't created the App Service yet, run:
```bash
# Create App Service Plan
az appservice plan create --name "bhcare-plan" --resource-group "BHcare" --location "East US" --sku B1 --is-linux

# Create Web App
az webapp create --name "bhcare-webapp" --resource-group "BHcare" --plan "bhcare-plan" --runtime "DOTNET|8.0"
```

### Step 6: Push Code to GitHub

```bash
# Add all files
git add .

# Commit changes
git commit -m "Add Azure deployment with credentials"

# Push to GitHub
git push origin main
```

### Step 7: Monitor Deployment

1. **Go to GitHub Actions**: https://github.com/RenierBenAlmario/bh-care/actions
2. **Watch the deployment progress**
3. **Check for any errors** in the logs

## 🔧 Troubleshooting

### Common Issues:

**1. Permission Denied**
- Ensure Service Principal has Contributor role on the Resource Group
- Check if the subscription ID is correct

**2. App Service Not Found**
- Verify the App Service name matches `AZURE_WEBAPP_NAME` secret
- Ensure the Resource Group name is correct

**3. Connection String Issues**
- Verify SQL Server firewall allows Azure services
- Check if the connection string format is correct

**4. Build Failures**
- Check if all dependencies are included in the project
- Verify the .NET version matches (8.0)

## 🚀 Benefits of Using Azure Credentials

✅ **More Secure**: No publish profiles to manage
✅ **Better Permissions**: Granular control over what the service can do
✅ **Automated Configuration**: App settings and connection strings are set automatically
✅ **Audit Trail**: All actions are logged in Azure Activity Log
✅ **Easy Rotation**: Can regenerate credentials without redeploying

## 📋 Verification Checklist

- [ ] Service Principal created successfully
- [ ] All 4 secrets added to GitHub
- [ ] App Service exists in Azure
- [ ] Code pushed to GitHub main branch
- [ ] GitHub Actions workflow runs successfully
- [ ] Application accessible at https://bhcare-webapp.azurewebsites.net

## 🔒 Security Best Practices

1. **Least Privilege**: Service Principal only has access to the specific Resource Group
2. **Secret Rotation**: Regenerate credentials periodically
3. **Monitor Usage**: Check Azure Activity Log for any suspicious activity
4. **Secure Storage**: Never commit credentials to code
5. **Access Review**: Regularly review who has access to the GitHub repository
