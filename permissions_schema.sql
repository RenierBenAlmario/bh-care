-- Users table (if not already exists)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id NVARCHAR(450) PRIMARY KEY,
        Email NVARCHAR(256) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        JoinDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END

-- Permissions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE Permissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(255) NULL,
        Category NVARCHAR(100) NOT NULL,
        CONSTRAINT UQ_Permission_Name UNIQUE (Name)
    );
END

-- UserPermissions pivot table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserPermissions')
BEGIN
    CREATE TABLE UserPermissions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UserId NVARCHAR(450) NOT NULL,
        PermissionId INT NOT NULL,
        CONSTRAINT FK_UserPermissions_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
        CONSTRAINT FK_UserPermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_UserPermissions_UserPermission UNIQUE (UserId, PermissionId)
    );
END

PRINT 'Permissions schema created successfully.' 