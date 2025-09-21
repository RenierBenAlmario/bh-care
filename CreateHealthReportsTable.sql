-- Script to create HealthReports table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HealthReports')
BEGIN
    CREATE TABLE [dbo].[HealthReports](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [CheckupDate] [datetime2](7) NOT NULL,
        [BloodPressure] [nvarchar](20) NULL,
        [HeartRate] [int] NULL,
        [BloodSugar] [decimal](5,1) NULL,
        [Weight] [decimal](5,1) NULL,
        [Temperature] [decimal](4,1) NULL,
        [PhysicalActivity] [nvarchar](100) NULL,
        [Notes] [nvarchar](max) NULL,
        [DoctorId] [nvarchar](450) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_HealthReports] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_HealthReports_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
        CONSTRAINT [FK_HealthReports_AspNetUsers_DoctorId] FOREIGN KEY([DoctorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
    )
    PRINT 'HealthReports table created successfully.'
END
ELSE
BEGIN
    PRINT 'HealthReports table already exists.'
END 