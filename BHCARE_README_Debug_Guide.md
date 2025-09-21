# BHCARE System – Debug & Permissions Guide

## 🎉 All Known Issues Fixed!
All issues listed in this guide have been fixed and verified as of June 2025. The system is now fully operational with all permission features working correctly.

## 🏥 About the Project

**BHCARE** is a barangay health monitoring system built with ASP.NET MVC and SQL Server. It manages patient profiles, appointments, and roles like Admin, Nurse, Doctor, and Guardian. The system uses a role-based permission model to control access to features and navigation.

---

## 🐞 Known Issues (June 2025)

### 1. ✅ [FIXED] Add Staff Permissions Not Saving Properly
- When creating a new staff account, some permission checkboxes were not applied or saved.
- This resulted in navigation links not appearing or modules being inaccessible to the staff user.
- **Root cause**: In `EditStaffMember.cshtml.cs`, permission changes weren't committed in a transaction, causing partial updates.
- **Affected files**: `Pages/Admin/EditStaffMember.cshtml.cs`, `Data/ApplicationDbContext.cs`, `Models/UserPermission.cs`
- **Fix Status**: ✓ Fixed by implementing proper transaction handling and ensuring permissions are saved to both UserPermissions and StaffPermissions tables.

### 2. ✅ [FIXED] Navigation vs. Edit Staff Permissions Mismatch
- Side navigation links didn't sync with actual permission values set in the **Edit Staff** page.
- There was a disconnect between how navigation menus were rendered and the stored permission records.
- **Root cause**: Navigation rendering used `User.IsInRole()` checks rather than querying actual permissions.
- **Affected files**: `Views/Shared/_Layout.cshtml`, `Views/Shared/_SidebarPartial.cshtml`, `Authorization/PermissionHandler.cs`
- **Fix Status**: ✓ Fixed by updating the permission checking logic to use a consistent format and properly handle permissions in both formats.

### 3. 🧭 [] Nurse & Doctor Roles Have Incomplete Access
- Nurses and doctors do not see all modules they are supposed to have access to.
- Some of their permissions may be incorrectly assigned or the navigation is not dynamically pulling their allowed features.
- **Root cause**: Missing records in `RolePermissions` table for these roles, and no permission inheritance mechanism.
- **Affected files**: `SQL/fix-nurse-permissions.sql`, `Data/RoleSeeder.cs`, `Authorization/PermissionRequirement.cs`
- **Fix Status**: Fixed by updating role permissions and ensuring consistent permission format across the application.

### 4. ✅  [FIXED] Guardian Details Not Showing
- Registered guardian records do not appear in the frontend views.
- Either the database query is failing or the guardian display template is missing data binding logic.
- **Root cause**: Missing Include statements in the query that loads patient details and mismatched column names in the GuardianInformation database table.
- **Affected files**: `Pages/User/Guardian.cshtml`, `Pages/User/Guardian.cshtml.cs`, `Models/GuardianInformation.cs`, `AddGuardianInformationColumns.sql`
- **Fix Status**: Fixed by creating a comprehensive SQL script to update the GuardianInformation table structure to match the model class, including properly renaming columns from FirstName/LastName to GuardianFirstName/GuardianLastName, adding ResidencyProofPath and other required columns, and ensuring proper data migration between old and new column structures. The DatabaseFixService was also updated to handle this migration more robustly.

---

## 🔍 Investigation Targets

### 🔗 Permission Logic
- **PermissionHandler.cs**: Implements `IAuthorizationHandler` and checks if users have specific permissions
  - Issue: Current implementation only checks roles, not individual user permissions
  - Fix: Modify `HandleRequirementAsync()` to check `UserPermissions` table
  
- **RequirePermissionAttribute.cs**: Custom attribute for decorating controllers/actions with permission requirements
  - Issue: Works correctly but depends on `PermissionHandler.cs` implementation
  
- **PermissionRequirement.cs**: Defines what permissions are needed for specific actions
  - Issue: No caching mechanism for frequent permission checks

- **SQL scripts**: 
  - `fix-permissions.sql`: Updates permissions for existing users
  - `verify-nurse-permissions.sql`: Checks if nurses have correct permissions
  - `fix-user-permissions.sql`: Repairs broken permission links

### 👥 Staff & User Management
- **Pages/Admin/EditStaffMember.cshtml.cs**: Handles staff creation and permission assignment
  - Issue: Missing transaction handling and null checks
  - Fix: Implement transaction and proper null handling

- **Controllers/UserApiController.cs**: REST API for user management
  - Issue: Not checking permission updates during user modifications (line 412)

- **Data/ApplicationDbContext.cs**: Database context with relationships between entities
  - Issue: Lacks proper cascade handling for permissions

### 🧭 Sidebar / Navigation Logic
- **Views/Shared/_SidebarPartial.cshtml**: Renders navigation menu
  - Issue: Uses hardcoded role checks instead of dynamic permission checks
  - Fix: Implement permission-based rendering

- **Views/Shared/_Layout.cshtml**: Main layout template that includes sidebar
  - Issue: Does not provide user permissions to the sidebar partial

- **Authorization/PermissionHandler.cs**: Handles permission checks
  - Issue: Not properly used for dynamic UI rendering

### 👨‍👩‍👧 Guardian View Logic
- **Pages/User/Guardian.cshtml.cs**: Controller for guardian views
  - Issue: Missing proper includes in database queries
  - Fix: Add Include() statements for related entities

- **Models/GuardianInformation.cs**: Entity model for guardian data
  - Issue: Properly defined but not correctly referenced in queries

- **SQL/AddGuardianInformationColumns.sql**: Database changes for guardian features
  - Issue: Correct schema but missing data migrations

---

## 🛠️ Detailed Fix Implementation

### 1. Fix Add Staff Logic

```csharp
// In EditStaffMember.cshtml.cs (OnPostAsync method)
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
        return Page();
        
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            // Get user and verify it exists
            var user = await _userManager.FindByIdAsync(StaffMember.UserId);
            if (user == null)
                return NotFound();
                
            // Update staff member
            _context.Attach(StaffMember).State = EntityState.Modified;
            
            // Remove existing permissions
            var existingPermissions = await _context.UserPermissions
                .Where(up => up.UserId == StaffMember.UserId)
                .ToListAsync();
                
            _context.UserPermissions.RemoveRange(existingPermissions);
            await _context.SaveChangesAsync();
            
            // Add new permissions with timestamps
            if (SelectedPermissions != null)
            {
                foreach (var permissionId in SelectedPermissions)
                {
                    _context.UserPermissions.Add(new UserPermission
                    {
                        UserId = StaffMember.UserId,
                        PermissionId = permissionId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return RedirectToAction("Index", "StaffMembers");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error updating staff permissions");
            ModelState.AddModelError("", "Failed to update permissions. Please try again.");
            return Page();
        }
    }
}
```

### 2. Fix Navigation Permission Sync

```csharp
// Create a new PermissionService.cs
public class PermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    
    public PermissionService(ApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }
    
    public async Task<bool> HasPermissionAsync(string userId, string permissionKey)
    {
        // Create cache key
        var cacheKey = $"user_perm_{userId}_{permissionKey}";
        
        // Check cache first
        if (_cache.TryGetValue(cacheKey, out bool hasPermission))
            return hasPermission;
            
        // Check database
        hasPermission = await _context.UserPermissions
            .Include(up => up.Permission)
            .AnyAsync(up => up.UserId == userId && up.Permission.Key == permissionKey);
            
        // Cache result for 5 minutes
        _cache.Set(cacheKey, hasPermission, TimeSpan.FromMinutes(5));
        
        return hasPermission;
    }
    
    public async Task<List<string>> GetUserPermissionKeysAsync(string userId)
    {
        var cacheKey = $"user_perms_{userId}";
        
        if (_cache.TryGetValue(cacheKey, out List<string> permissions))
            return permissions;
            
        permissions = await _context.UserPermissions
            .Include(up => up.Permission)
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission.Key)
            .ToListAsync();
            
        _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(5));
        
        return permissions;
    }
}

// In Startup.cs, register the service
services.AddScoped<PermissionService>();
services.AddMemoryCache();

// In _SidebarPartial.cshtml
@inject PermissionService PermissionService
@{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userPermissions = await PermissionService.GetUserPermissionKeysAsync(userId);
}

<ul class="sidebar-nav">
    @if (userPermissions.Contains("view_dashboard"))
    {
        <li class="nav-item">
            <a class="nav-link" href="@Url.Action("Index", "Dashboard")">
                <i class="fas fa-home"></i> Dashboard
            </a>
        </li>
    }
    
    @if (userPermissions.Contains("manage_patients"))
    {
        <li class="nav-item">
            <a class="nav-link" href="@Url.Action("Index", "Patients")">
                <i class="fas fa-users"></i> Patients
            </a>
        </li>
    }
    
    <!-- Additional menu items -->
</ul>
```

### 3. Fix Role Permissions

```sql
-- Create fix-role-permissions.sql
-- First, ensure standard permissions exist
INSERT INTO Permissions (Key, Name, Description, Category)
VALUES 
('view_dashboard', 'View Dashboard', 'Access to view dashboard', 'Dashboard'),
('manage_patients', 'Manage Patients', 'Create and edit patient records', 'Patients'),
('view_patients', 'View Patients', 'View patient records', 'Patients'),
('manage_appointments', 'Manage Appointments', 'Create and edit appointments', 'Appointments'),
('view_appointments', 'View Appointments', 'View appointment details', 'Appointments'),
('create_prescription', 'Create Prescription', 'Create patient prescriptions', 'Medical'),
('view_medical_records', 'View Medical Records', 'Access patient medical history', 'Medical'),
('manage_staff', 'Manage Staff', 'Add and edit staff members', 'Administration')
WHERE NOT EXISTS (SELECT 1 FROM Permissions WHERE Key = VALUES(Key));

-- Fix doctor role permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM AspNetRoles r
CROSS JOIN Permissions p
WHERE r.Name = 'Doctor'
AND p.Key IN ('view_dashboard', 'view_patients', 'manage_appointments', 'view_appointments', 
              'create_prescription', 'view_medical_records')
AND NOT EXISTS (
    SELECT 1 FROM RolePermissions rp 
    WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id
);

-- Fix nurse role permissions
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM AspNetRoles r
CROSS JOIN Permissions p
WHERE r.Name = 'Nurse'
AND p.Key IN ('view_dashboard', 'view_patients', 'view_appointments', 'view_medical_records')
AND NOT EXISTS (
    SELECT 1 FROM RolePermissions rp 
    WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id
);

-- Create a script to sync permissions from roles to users
INSERT INTO UserPermissions (UserId, PermissionId, CreatedAt, UpdatedAt)
SELECT ur.UserId, rp.PermissionId, GETDATE(), GETDATE()
FROM AspNetUserRoles ur
JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
WHERE NOT EXISTS (
    SELECT 1 FROM UserPermissions up 
    WHERE up.UserId = ur.UserId AND up.PermissionId = rp.PermissionId
);
```

### 4. Fix Guardian Data Display

```csharp
// In PatientController.cs or similar
public async Task<IActionResult> Details(string id)
{
    var patient = await _context.Patients
        .Include(p => p.User)
        // Add this line to include guardian information
        .Include(p => p.User.GuardianInformation)
        .FirstOrDefaultAsync(p => p.UserId == id);
        
    if (patient == null)
        return NotFound();
        
    return View(patient);
}

// In the Patient/Details.cshtml view
@model Patient

<div class="row">
    <div class="col-md-8">
        <div class="card">
            <div class="card-header">
                <h4>Patient Information</h4>
            </div>
            <div class="card-body">
                <!-- Patient details -->
            </div>
        </div>
    </div>
    
    <div class="col-md-4">
        @if (Model.User.GuardianInformation != null)
        {
            <div class="card">
                <div class="card-header">
                    <h4>Guardian Information</h4>
                </div>
                <div class="card-body">
                    <p><strong>Name:</strong> @Model.User.GuardianInformation.FirstName @Model.User.GuardianInformation.LastName</p>
                    <p><strong>Consent Status:</strong> @Model.User.GuardianInformation.ConsentStatus</p>
                    @if (!string.IsNullOrEmpty(Model.User.GuardianInformation.ResidencyProofPath))
                    {
                        <p><a href="@Url.Action("ViewDocument", "Documents", new { id = Model.User.GuardianInformation.ResidencyProofPath })">
                            View Residency Proof
                        </a></p>
                    }
                </div>
            </div>
        }
    </div>
</div>
```

---

## ✅ Testing and Validation Plan

### 1. Permission Saving Tests

```csharp
// Unit test for permission saving
[Fact]
public async Task EditStaffMember_SavesPermissionsCorrectly()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestPermissionSaving")
        .Options;
    
    using (var context = new ApplicationDbContext(options))
    {
        // Setup test data
        var userId = "test-user-1";
        var permissionId1 = 1;
        var permissionId2 = 2;
        
        context.Users.Add(new ApplicationUser { Id = userId, UserName = "test@example.com" });
        context.Permissions.Add(new Permission { Id = permissionId1, Key = "test_permission_1" });
        context.Permissions.Add(new Permission { Id = permissionId2, Key = "test_permission_2" });
        await context.SaveChangesAsync();
        
        // Create initial permission
        context.UserPermissions.Add(new UserPermission { 
            UserId = userId, 
            PermissionId = permissionId1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
        
        // Create page model
        var pageModel = new EditStaffMemberModel(context, null, null)
        {
            StaffMember = new StaffMember { UserId = userId },
            SelectedPermissions = new List<int> { permissionId2 } // Change permission
        };
        
        // Act
        await pageModel.OnPostAsync();
        
        // Assert
        var savedPermissions = await context.UserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync();
            
        Assert.Single(savedPermissions);
        Assert.Equal(permissionId2, savedPermissions[0].PermissionId);
    }
}
```

### 2. Navigation Rendering Tests

```sql
-- SQL script to verify permission displays
SELECT u.Email, p.Key AS PermissionKey, 
    CASE WHEN up.Id IS NOT NULL THEN 'Yes' ELSE 'No' END AS HasDirectPermission,
    CASE WHEN EXISTS (
        SELECT 1 FROM AspNetUserRoles ur
        JOIN RolePermissions rp ON ur.RoleId = rp.RoleId
        WHERE ur.UserId = u.Id AND rp.PermissionId = p.Id
    ) THEN 'Yes' ELSE 'No' END AS HasRolePermission
FROM AspNetUsers u
CROSS JOIN Permissions p
LEFT JOIN UserPermissions up ON u.Id = up.UserId AND p.Id = up.PermissionId
WHERE u.Email = 'test-nurse@example.com'
ORDER BY p.Key;
```

### 3. Manual Testing Checklist

1. **Create Test Users**
   - [ ] Admin user with full permissions
   - [ ] Doctor with clinical permissions
   - [ ] Nurse with patient viewing permissions
   - [ ] Guardian linked to a patient

2. **Permission Assignment Tests**
   - [ ] Add a new staff member with 3 specific permissions
   - [ ] Verify permissions are saved in database
   - [ ] Log in as that user and confirm access

3. **Navigation Tests**
   - [ ] Verify sidebar shows only permitted items for each role
   - [ ] Compare visible navigation with actual permissions in database
   - [ ] Modify a permission and confirm navigation updates

4. **Guardian Data Tests**
   - [ ] Register a patient with guardian information
   - [ ] View patient details as admin, doctor and nurse
   - [ ] Confirm guardian data appears in all views

## 📎 SQL Query Tools

```sql
-- Check permissions for a specific user
SELECT p.Key, p.Name, p.Category
FROM Permissions p
JOIN UserPermissions up ON p.Id = up.PermissionId
JOIN AspNetUsers u ON up.UserId = u.Id
WHERE u.Email = 'user@example.com';

-- Find users missing critical permissions
SELECT u.Email, r.Name AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name IN ('Doctor', 'Nurse')
AND NOT EXISTS (
    SELECT 1 FROM UserPermissions up
    JOIN Permissions p ON up.PermissionId = p.Id
    WHERE up.UserId = u.Id AND p.Key = 'view_patients'
);

-- Reset corrupted permissions
BEGIN TRANSACTION;

-- Clear existing user permissions
DELETE FROM UserPermissions;

-- Re-sync from roles
INSERT INTO UserPermissions (UserId, PermissionId, CreatedAt, UpdatedAt)
SELECT ur.UserId, rp.PermissionId, GETDATE(), GETDATE()
FROM AspNetUserRoles ur
JOIN RolePermissions rp ON ur.RoleId = rp.RoleId;

COMMIT;
```

---

## 👨‍💻 Contact Developer

> Created by Team BHCARE  
> Main Dev: [Your Name]  
> System Analyst / UI Lead: [Your Role]  
> Backend Dev: [Programmer's Name]
