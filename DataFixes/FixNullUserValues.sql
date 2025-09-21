-- Fix null values in AspNetUsers table
UPDATE AspNetUsers
SET FirstName = 'Unknown' 
WHERE FirstName IS NULL;

UPDATE AspNetUsers
SET LastName = 'User' 
WHERE LastName IS NULL;

UPDATE AspNetUsers
SET UserName = Email
WHERE UserName IS NULL AND Email IS NOT NULL;

UPDATE AspNetUsers
SET Email = 'unknown@example.com'
WHERE Email IS NULL;

UPDATE AspNetUsers
SET NormalizedEmail = UPPER(Email)
WHERE NormalizedEmail IS NULL AND Email IS NOT NULL;

UPDATE AspNetUsers
SET NormalizedUserName = UPPER(UserName)
WHERE NormalizedUserName IS NULL AND UserName IS NOT NULL;

-- Set default values for other potentially null columns
UPDATE AspNetUsers
SET 
    PhoneNumber = COALESCE(PhoneNumber, 'Not provided'),
    Address = COALESCE(Address, 'Not provided'),
    Status = COALESCE(Status, 'Active'),
    IsActive = COALESCE(IsActive, 1),
    LastActive = COALESCE(LastActive, GETDATE())
WHERE 
    PhoneNumber IS NULL OR 
    Address IS NULL OR 
    Status IS NULL OR 
    IsActive IS NULL OR
    LastActive IS NULL;

-- Fix null values in StaffMembers table
UPDATE StaffMembers
SET Name = 'Unknown Staff' 
WHERE Name IS NULL OR Name = '';

UPDATE StaffMembers
SET Email = 'unknown@example.com'
WHERE Email IS NULL OR Email = '';

UPDATE StaffMembers
SET Role = 'Staff'
WHERE Role IS NULL OR Role = '';

UPDATE StaffMembers
SET Department = 'General'
WHERE Department IS NULL OR Department = '';

UPDATE StaffMembers
SET Position = 'Staff Member'
WHERE Position IS NULL OR Position = '';

UPDATE StaffMembers
SET ContactNumber = 'Not provided'
WHERE ContactNumber IS NULL OR ContactNumber = '';

UPDATE StaffMembers
SET WorkingDays = 'Mon-Fri'
WHERE WorkingDays IS NULL OR WorkingDays = '';

UPDATE StaffMembers
SET WorkingHours = '9:00 AM - 5:00 PM'
WHERE WorkingHours IS NULL OR WorkingHours = '';

-- Log the fix
PRINT 'Fixed null values in AspNetUsers and StaffMembers tables'; 