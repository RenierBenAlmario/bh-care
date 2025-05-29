# User Management and Notification System Troubleshooting Guide

This guide has been updated to address the specific issues with the User Management page and notification system.

## Step 1: Automated Diagnostics and Fixes

The fastest way to diagnose and fix common issues is to use the provided automated tools:

1. Run the PowerShell script `FixUserManagementIssues.ps1`:
   ```
   .\FixUserManagementIssues.ps1
   ```
   This script will:
   - Connect to your database
   - Run comprehensive diagnostics
   - Offer to apply automatic fixes
   - Provide detailed recommendations

2. Alternative: Run the SQL script directly in SQL Server Management Studio:
   - Open `DiagnoseAndFixUserManagement.sql` in SSMS
   - Connect to your database
   - Execute the script
   - Review the diagnostic output

These tools will check for and fix:
- Missing notifications for pending users
- Case sensitivity issues in status values
- NULL status values
- Database connection issues

## Step 2: Investigate Notification Count Issues

1. Access the notification debug endpoint at `/api/Notification/debug` to get comprehensive diagnostics about:
   - Database connection
   - User counts by status
   - Notification counts
   - Recent users and notifications
   
2. Run the `InvestigateNotifications.sql` script in SSMS to check:
   - If there are hardcoded notifications
   - Multiple databases on the server
   - Database triggers or stored procedures

3. Check the browser console for any JavaScript errors in the notification system

## Step 3: Fix Missing User Data in Admin Panel

1. Run the database diagnostics using `/api/DatabaseTest` to check:
   - Database connection status
   - Column existence
   - User and document counts

2. If the database has the correct structure but no data, the issue might be:
   - Sign-up process not saving data
   - Two different databases being used

3. Verify the connection string in `appsettings.json` matches what the application is using

4. Check the sign-up process by registering a new user and monitoring the logs

## Step 4: Check Notification System Implementation

1. The notification bell has been removed from beside the User Management link
2. A single notification system has been implemented in the top navbar
3. The new implementation uses JavaScript to fetch notifications via AJAX
4. Check if the notification count matches the actual number of pending users

## Step 5: Check User Filter Settings

1. Make sure "Pending Approval" filter value matches "Pending" in the database
2. Inspect the page for any JavaScript errors that might prevent data loading
3. Check if the data is correctly fetched but not displayed properly

## Step 6: Use the UI Diagnostic Tool

We've created a UI diagnostic tool to help troubleshoot JavaScript issues in real-time:

1. Add the troubleshooting script to your admin layout:
   ```html
   <!-- Add this line at the end of _AdminLayout.cshtml, right before the </body> tag -->
   <script src="~/js/troubleshoot-ui.js"></script>
   ```

2. A diagnostic panel will appear in the bottom-right corner of the page with the following features:
   - Detect JavaScript errors in real-time
   - Test notification API endpoints directly
   - Check if UI elements are properly rendered
   - Capture console logs for debugging

3. Click the "Run Tests" button to perform a comprehensive check of:
   - JavaScript errors
   - Notification system
   - User Management UI components
   - API endpoint accessibility

4. The results will show you exactly what's working and what's not, with detailed error information and recommendations.

5. Remove the script when you're done troubleshooting.

## Specific Files Modified

1. **Layout Changes**:
   - Removed notification bell from User Management link in `_AdminLayout.cshtml`
   - Added a single notification dropdown in the navbar

2. **JavaScript Updates**:
   - Created `admin-notifications.js` to handle notification fetching and display
   - Added debugging to show detailed information about API responses

3. **API Enhancements**:
   - Added `/api/Notification/debug` endpoint for diagnostics
   - Enhanced NotificationController with detailed error handling

4. **SQL Diagnostics**:
   - Added `InvestigateNotifications.sql` script to check for database issues
   - Added query to check for multiple databases or connection issues
   - Created `DiagnoseAndFixUserManagement.sql` for comprehensive diagnostics and automatic fixes

5. **Automation Tools**:
   - Added `FixUserManagementIssues.ps1` PowerShell script for easy diagnostic execution

## Common Causes of Issues

1. **Database Connection Issues**:
   - Application connecting to a different database than expected
   - Missing tables or columns in the database schema

2. **Status Value Mismatches**:
   - Case sensitivity issues between "Pending" vs "pending"
   - UI showing "Pending Approval" while database stores "Pending"

3. **Silent Registration Failures**:
   - Users signing up but data not saving correctly
   - Missing required fields during registration

4. **Notification System Issues**:
   - JavaScript errors preventing notification display
   - Duplicate notification bell elements causing confusion

## How to Test Your Changes

1. Run the automated diagnostics with `FixUserManagementIssues.ps1`
2. Check your browser console for any JavaScript errors
3. Visit `/api/Notification/debug` to see detailed information about notifications
4. Register a new user and check if they appear in the User Management page
5. Click the notification bell to ensure the dropdown displays correctly

If users still don't appear in the User Management page after these fixes, the most likely issue is:
- The database connection string is pointing to a different database
- The sign-up process is failing silently
- The Status value is not set correctly during registration

These changes should resolve the notification display issues and provide clearer diagnostics for troubleshooting the missing user data. 