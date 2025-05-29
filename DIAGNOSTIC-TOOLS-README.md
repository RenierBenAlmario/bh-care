# Barangay Health Center Diagnostic Tools

This package contains diagnostic tools to help troubleshoot issues with the User Management page and notification system.

## Quick Start

1. **Database Diagnostics**:
   ```powershell
   .\FixUserManagementIssues.ps1
   ```
   This interactive PowerShell script will connect to your database, run diagnostics, and offer to apply fixes.

2. **UI Diagnostics**:
   Add this line to your `_AdminLayout.cshtml` file before the closing `</body>` tag:
   ```html
   <script src="~/js/troubleshoot-ui.js"></script>
   ```
   A diagnostic panel will appear in the bottom-right corner of your admin pages.

## Included Tools

### 1. Database Diagnostic Scripts

- **DiagnoseAndFixUserManagement.sql**
  - Performs comprehensive database checks
  - Identifies issues with user data and notifications
  - Automatically fixes common problems (status case sensitivity, NULL values)
  - Generates detailed reports

- **InvestigateNotifications.sql**
  - Specifically checks notification-related tables and data
  - Looks for database triggers or stored procedures
  - Checks for multiple databases

### 2. Automated PowerShell Script

- **FixUserManagementIssues.ps1**
  - User-friendly interface for running diagnostics
  - Checks database connectivity
  - Offers to apply fixes automatically
  - Provides clear recommendations based on findings

### 3. UI Diagnostic Tool

- **troubleshoot-ui.js**
  - Real-time JavaScript error detection
  - Tests notification API endpoints
  - Checks UI elements on the User Management page
  - Captures console logs for debugging

## Common Issues and Solutions

1. **No Users Appear in User Management**
   - Database connection to wrong database
   - Missing or NULL Status values
   - Case sensitivity issues ("pending" vs "Pending")
   - Silent failures in the registration process

2. **Notification Bell Shows Count But No Dropdown**
   - JavaScript errors preventing dropdown display
   - DOM element ID mismatches
   - CSS issues with the dropdown visibility

3. **Duplicate Notification Bells**
   - Multiple notification components conflicting
   - Both page-specific and global notification systems active

## How to Use

1. Start with database diagnostics using `FixUserManagementIssues.ps1`
2. Add the UI diagnostic tool temporarily to identify client-side issues
3. Check the browser console for JavaScript errors
4. Test the API endpoints directly using the debug endpoint

## Next Steps After Diagnostics

If the diagnostics identify issues but don't automatically fix them:

1. Check your connection string in `appsettings.json`
2. Examine your registration process for silent failures
3. Look for mismatches between UI filter values and database values
4. Check CSS styles for notification dropdowns

## Removal After Use

1. Remove `troubleshoot-ui.js` from your layout when done troubleshooting
2. Keep diagnostic scripts for future use 