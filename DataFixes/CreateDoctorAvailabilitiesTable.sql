-- Create DoctorAvailabilities table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DoctorAvailabilities]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[DoctorAvailabilities](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [DoctorId] [nvarchar](450) NOT NULL,
        [IsAvailable] [bit] NOT NULL DEFAULT(1),
        [Monday] [bit] NOT NULL DEFAULT(1),
        [Tuesday] [bit] NOT NULL DEFAULT(1),
        [Wednesday] [bit] NOT NULL DEFAULT(1),
        [Thursday] [bit] NOT NULL DEFAULT(1),
        [Friday] [bit] NOT NULL DEFAULT(1),
        [Saturday] [bit] NOT NULL DEFAULT(1),
        [Sunday] [bit] NOT NULL DEFAULT(1),
        [StartTime] [time] NULL DEFAULT('08:00'),
        [EndTime] [time] NULL DEFAULT('17:00'),
        [LastUpdated] [datetime2] NOT NULL DEFAULT(GETDATE()),
        CONSTRAINT [PK_DoctorAvailabilities] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_DoctorAvailabilities_AspNetUsers] FOREIGN KEY ([DoctorId]) 
        REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_DoctorAvailabilities_DoctorId] ON [dbo].[DoctorAvailabilities]
    (
        [DoctorId] ASC
    );
    
    PRINT 'DoctorAvailabilities table created successfully';
    
    -- Add default availability for existing doctors
    INSERT INTO [dbo].[DoctorAvailabilities] 
    (DoctorId, IsAvailable, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday, StartTime, EndTime, LastUpdated)
    SELECT u.Id, 1, 1, 1, 1, 1, 1, 0, 0, '09:00', '17:00', GETDATE()
    FROM [dbo].[AspNetUsers] u
    JOIN [dbo].[AspNetUserRoles] ur ON u.Id = ur.UserId
    JOIN [dbo].[AspNetRoles] r ON ur.RoleId = r.Id 
    WHERE r.Name = 'Doctor'
    AND NOT EXISTS (
        SELECT 1 FROM [dbo].[DoctorAvailabilities] da WHERE da.DoctorId = u.Id
    );
    
    PRINT 'Default availability added for existing doctors';
END
ELSE
BEGIN
    PRINT 'DoctorAvailabilities table already exists';
END 