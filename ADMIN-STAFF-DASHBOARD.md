# Admin Staff Dashboard Implementation Guide

## Overview

This document provides instructions for implementing the Admin Staff Dashboard in the Barangay Health Care System. The Admin Staff Dashboard is designed for users with the `admin_staff` role, providing them access to features for managing appointments, patients, and viewing reports.

## Implementation Date
May 21, 2025, 08:54 PM PST

## Features Implemented

1. **Admin Staff Dashboard**
   - Dashboard with statistics (appointments, patients, medical records)
   - Quick access to key functions

2. **Authorization Policy**
   - Created `AccessAdminDashboard` policy for `Admin Staff` role
   - Added route protection for `/AdminStaff` folder 

3. **Patient Management**
   - Patient listing with filtering and search
   - View patient details and medical records

4. **Appointment Management**
   - View and manage appointments
   - Create, edit, confirm, and cancel appointments

5. **Navigation Menu**
   - Custom navigation for Admin Staff users

## Files Created/Modified

### Core Dashboard Files
- `Pages/AdminStaff/Dashboard.cshtml`
- `Pages/AdminStaff/Dashboard.cshtml.cs`
- `Pages/AdminStaff/ManageAppointments.cshtml`
- `Pages/AdminStaff/ManageAppointments.cshtml.cs`
- `Pages/AdminStaff/PatientsList.cshtml`
- `Pages/AdminStaff/PatientsList.cshtml.cs`
- `Pages/Shared/_AdminStaffNav.cshtml`

### Configuration Files
- `Program.cs` (Modified to add admin staff policies)

## Implementation Steps

### 1. Create Database Role

If not already present in the database, ensure the Admin Staff role exists by running the SQL script:

```sql
-- Check if role exists
IF NOT EXISTS (SELECT * FROM AspNetRoles WHERE NormalizedName = 'ADMIN STAFF')
BEGIN
    -- Create a new role ID
    DECLARE @RoleId NVARCHAR(450) = NEWID();
    
    -- Insert the Admin Staff role
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, 'Admin Staff', 'ADMIN STAFF', NEWID());
    
    PRINT 'Admin Staff role created with ID: ' + @RoleId;
END
ELSE
BEGIN
    PRINT 'Admin Staff role already exists';
END
```

### 2. Configure Authorization Policies

In `Program.cs`, we've added the policy for the Admin Staff role:

```csharp
// Admin Staff Dashboard policies
options.AddPolicy("AccessAdminDashboard", policy => 
    policy.RequireRole("Admin", "Admin Staff"));
```

And configured the folder authorization:

```csharp
options.Conventions.AuthorizeFolder("/AdminStaff", "AccessAdminDashboard");
```

### 3. Create Navigation

We've created a navigation partial view for Admin Staff users:
- `Pages/Shared/_AdminStaffNav.cshtml` - Navigation menu for Admin Staff

Include this in the appropriate layout file.

### 4. Add Permission Assignments

When creating an Admin Staff user, ensure they have these permissions assigned:
- ManageAppointments
- AddPatients
- EditPatients
- ViewPatients
- ViewMedicalRecords
- ViewReports
- AccessAdminDashboard

## Testing

### Test User Setup

1. Create a test user with the Admin Staff role:
   - Navigate to Admin Dashboard > Staff Management
   - Add a new staff member with Role = "Admin Staff"
   - Assign appropriate permissions

### Test Scenarios

1. **Dashboard Access**
   - Log in as Admin Staff user
   - Verify you can access `/AdminStaff/Dashboard`
   - Verify dashboard statistics load correctly

2. **Patient Management**
   - Access the Patients List page
   - Test search and filtering functions
   - Try viewing patient details

3. **Appointment Management**
   - Access the Manage Appointments page
   - Create a new appointment
   - Update an existing appointment
   - Test filtering and search

4. **Authorization**
   - Verify Admin Staff users cannot access Admin-only pages
   - Verify regular users cannot access Admin Staff pages

## Troubleshooting

### Common Issues

1. **Permission Denied**
   - Ensure the user has the `Admin Staff` role assigned
   - Check if the `AccessAdminDashboard` policy is configured correctly 
   - Verify the user has all required permissions

2. **Navigation Not Showing**
   - Ensure the `_AdminStaffNav.cshtml` partial view is included in the layout
   - Check the layout used by Admin Staff pages

3. **Database Errors**
   - Make sure the database is up to date with the latest migrations
   - Verify the `AspNetRoles` table contains the `Admin Staff` role

## Additional Notes

- The Admin Staff Dashboard path is `/admin-staff-dashboard`
- All admin staff pages are in the `/AdminStaff` folder
- The Dashboard uses the `_AdminLayout` layout

## Contributors
- System Administrator
- Application Developer 