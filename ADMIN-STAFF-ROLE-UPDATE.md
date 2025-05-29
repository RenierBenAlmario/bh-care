# Admin Staff Role Update

## Overview

This document provides instructions for implementing the Admin Staff role update in the Barangay Health Care System. The update replaces the previously separate "Staff" and "Admin" roles with a unified "Admin Staff" role, which has specific permissions tailored for administrative staff members.

## Date of Implementation
May 21, 2025, 09:02 PM PST

## Changes Made

1. **UI Changes**
   - Updated the Role dropdown in the Add Staff Member form to replace "Staff" and "Admin" options with "Admin Staff"
   - Updated the Position dropdown to replace "Admin" and "Administrator" with "Admin Staff"
   - Added JavaScript to synchronize the Role and Position dropdowns

2. **Backend Changes**
   - Updated the role handling logic in `AddStaffMember.cshtml.cs` to map the "Admin Staff" role to the `admin_staff` role slug
   - Added permission assignments specific to the Admin Staff role
   - Added a dependency on `RoleManager<IdentityRole>` for proper role creation and management

3. **Database Changes**
   - Created a SQL script (`add-admin-staff-role.sql`) to ensure the Admin Staff role exists in the database
   - Added required permissions for the Admin Staff role

## Implementation Steps

### 1. Deploy Code Changes

The following files have been modified:
- `Pages/Admin/AddStaffMember.cshtml`
- `Pages/Admin/AddStaffMember.cshtml.cs`

### 2. Run Database Script

Run the SQL script to ensure the Admin Staff role and its permissions exist in the database:

```sql
-- Run this script from SQL Server Management Studio or via the command line
USE [Barangay];
GO

-- Content of add-admin-staff-role.sql
```

### 3. Testing

1. **Verify Role Dropdown**
   - Navigate to `/Admin/AddStaffMember`
   - Confirm that the Role dropdown contains "Doctor", "Nurse", and "Admin Staff" options
   - Confirm that "Staff" and "Admin" options are no longer present

2. **Test Role-Position Synchronization**
   - Select "Admin Staff" in the Role dropdown
   - Verify that the Position automatically changes to "Admin Staff"
   - Try the reverse (select a position and verify the role updates)

3. **Test Staff Creation**
   - Create a new staff member with the "Admin Staff" role
   - Verify that the user is created with the correct role in the database
   - Verify that the appropriate permissions are assigned to the user

4. **Verify Access**
   - Log in as the new Admin Staff user
   - Verify they can access the Admin Staff Dashboard at `/admin-staff-dashboard`
   - Verify they have the appropriate permissions (ManageAppointments, AddPatients, etc.)
   - Verify they cannot access restricted areas that should only be available to full Admins

## Database Verification Queries

Run these queries to verify the database changes:

```sql
-- Verify Admin Staff role exists
SELECT * FROM AspNetRoles WHERE Name = 'Admin Staff';

-- Verify permissions for a specific Admin Staff user (replace with actual user ID)
SELECT u.Email, p.Name AS Permission
FROM AspNetUsers u
JOIN UserPermissions up ON u.Id = up.UserId
JOIN Permissions p ON up.PermissionId = p.Id
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Admin Staff';
```

## Troubleshooting

### Common Issues

1. **Role not showing in dropdown**
   - Make sure the changes to `AddStaffMember.cshtml` were deployed correctly
   - Clear browser cache or try in incognito mode

2. **Permission errors**
   - Run the database script (`add-admin-staff-role.sql`) to ensure all required permissions exist
   - Check if the user was assigned to the correct role using the verification query above

3. **Database role missing**
   - Verify the SQL script executed successfully
   - Check for any errors in the application logs

## Rollback Plan

If issues are encountered:

1. Restore the previous versions of:
   - `Pages/Admin/AddStaffMember.cshtml`
   - `Pages/Admin/AddStaffMember.cshtml.cs`

2. Run the following SQL to restore previous role setup:
```sql
-- Only execute if rollback is needed
-- This restores the separate "Staff" and "Admin" roles if needed
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'STAFF')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Staff', 'STAFF', NEWID());
END

IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END
```

## Contributors
- System Administrator
- Database Administrator
- Application Developer 