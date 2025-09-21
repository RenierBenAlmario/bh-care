-- Verify staff users and their roles
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.IsActive,
    r.Name AS Role
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.Email IN ('admin@example.com', 'doctor@example.com', 'nurse@example.com')
ORDER BY r.Name, u.UserName;

-- Check specifically for Doctor role users
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.IsActive,
    'Doctor' AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor';

-- Check specifically for Nurse role users
SELECT 
    u.Id,
    u.UserName,
    u.Email,
    u.FirstName,
    u.LastName,
    u.IsActive,
    'Nurse' AS Role
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Nurse';

-- Count users by role
SELECT 
    r.Name AS Role,
    COUNT(ur.UserId) AS UserCount
FROM AspNetRoles r
LEFT JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
GROUP BY r.Name, r.Id
ORDER BY UserCount DESC;
