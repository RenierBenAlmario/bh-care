# BHCARE Azure Deployment Guide

## Overview
This guide will help you deploy the BHCARE (Barangay Health Center Application) to Microsoft Azure.

## Prerequisites
- Azure subscription with active billing
- Azure CLI installed locally
- Git repository (GitHub recommended)
- .NET 8.0 SDK installed locally

## Deployment Options

### Option 1: Quick Deployment (PowerShell Script)
1. Open PowerShell as Administrator
2. Navigate to your project directory
3. Run: `.\deploy-to-azure.ps1`
4. Follow the prompts and wait for deployment to complete

### Option 2: Manual Azure Portal Deployment
1. **Create Resource Group**
   - Go to Azure Portal
   - Create new Resource Group: `BHCARE-RG`
   - Location: `East US` (or your preferred region)

2. **Create App Service Plan**
   - Service: App Service Plans
   - Name: `bhcare-plan`
   - OS: Linux
   - Pricing Tier: B1 (Basic)

3. **Create Web App**
   - Service: Web Apps
   - Name: `bhcare-app-[random]`
   - Runtime: .NET 8.0
   - Region: Same as Resource Group

4. **Configure SQL Database**
   - Service: SQL databases
   - Server: `bhcare-sql-[random]`
   - Database: `bhcareDB`
   - Pricing Tier: Basic

### Option 3: GitHub Actions (Recommended)
1. **Set up GitHub Secrets**
   - Go to your GitHub repository
   - Settings → Secrets and variables → Actions
   - Add these secrets:
     - `AZURE_WEBAPP_NAME`: Your Azure Web App name
     - `AZURE_WEBAPP_PUBLISH_PROFILE`: Download from Azure Portal
     - `AZURE_RESOURCE_GROUP`: Your resource group name

2. **Deploy**
   - Push code to main branch
   - GitHub Actions will automatically build and deploy

## Configuration Steps

### 1. Update Connection Strings
Update your `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:YOUR_SERVER.database.windows.net,1433;Initial Catalog=bhcareDB;Persist Security Info=False;User ID=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

### 2. Configure App Settings
In Azure Portal → App Service → Configuration → Application settings:
- `ASPNETCORE_ENVIRONMENT`: `Production`
- `EncryptionKey`: `BHCARE_Production_Encryption_Key_2024_Secure_32Chars`
- `DataEncryption__Key`: `BHCARE_Production_DataEncryption_Key_2024_Secure_32Chars`

### 3. Configure Email Settings
Add these to Application settings:
- `EmailSettings__SmtpHost`: `smtp.gmail.com`
- `EmailSettings__SmtpPort`: `587`
- `EmailSettings__SmtpUsername`: `your-email@gmail.com`
- `EmailSettings__SmtpPassword`: `your-app-password`

## Database Migration

### Option 1: Azure CLI
```bash
az webapp config appsettings set --name YOUR_APP_NAME --resource-group YOUR_RG --settings "ASPNETCORE_ENVIRONMENT=Production"
az webapp restart --name YOUR_APP_NAME --resource-group YOUR_RG
```

### Option 2: Manual Migration
1. Connect to your Azure SQL Database
2. Run Entity Framework migrations:
```bash
dotnet ef database update --connection "YOUR_CONNECTION_STRING"
```

## Security Configuration

### 1. SQL Server Firewall
- Allow Azure services: Yes
- Add your IP address for development access

### 2. App Service Security
- Enable HTTPS only
- Configure custom domain (optional)
- Set up SSL certificates

### 3. Environment Variables
- Never commit passwords to source control
- Use Azure Key Vault for sensitive data (recommended)

## Monitoring and Maintenance

### 1. Application Insights
- Enable Application Insights in Azure Portal
- Monitor performance and errors
- Set up alerts for critical issues

### 2. Logging
- Configure Application Logging in App Service
- Set up Log Analytics workspace
- Monitor application logs

### 3. Backup
- Enable automated backups for SQL Database
- Configure retention policies
- Test restore procedures

## Troubleshooting

### Common Issues
1. **Connection String Issues**
   - Verify SQL Server firewall rules
   - Check connection string format
   - Ensure SQL Server is accessible

2. **Deployment Failures**
   - Check build logs in GitHub Actions
   - Verify all dependencies are included
   - Check Azure App Service logs

3. **Performance Issues**
   - Monitor CPU and memory usage
   - Check database performance
   - Optimize queries and caching

### Support Resources
- Azure Documentation: https://docs.microsoft.com/azure/
- .NET on Azure: https://docs.microsoft.com/azure/app-service/
- Azure SQL Database: https://docs.microsoft.com/azure/sql-database/

## Cost Optimization

### 1. App Service
- Use Basic tier for development
- Scale up to Standard for production
- Consider Premium for high-traffic applications

### 2. SQL Database
- Use Basic tier for development
- Scale up based on usage
- Consider reserved capacity for production

### 3. Monitoring
- Set up cost alerts
- Monitor usage patterns
- Optimize resource allocation

## Next Steps
1. Set up custom domain
2. Configure SSL certificates
3. Implement CI/CD pipeline
4. Set up monitoring and alerts
5. Configure backup and disaster recovery
