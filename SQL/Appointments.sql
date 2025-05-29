CREATE TABLE [dbo].[Appointments] (
    [Id]                    INT                IDENTITY (1, 1) NOT NULL,
    [PatientId]            NVARCHAR (450)     NOT NULL,
    [DoctorId]             NVARCHAR (450)     NOT NULL,
    [PatientName]          NVARCHAR (100)     NOT NULL,
    [AppointmentDate]      DATE               NOT NULL,
    [AppointmentTime]      TIME               NOT NULL,
    [Description]          NVARCHAR (500)     NULL,
    [Status]               INT                NOT NULL DEFAULT 0,
    [AgeValue]            INT                NULL,
    [ContactNumber]        NVARCHAR (20)      NULL,
    [RelationshipToDependent] NVARCHAR (50)   NULL,
    [CreatedAt]           DATETIME2          NOT NULL,
    [UpdatedAt]           DATETIME2          NOT NULL,
    CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Appointments_AspNetUsers_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Appointments_AspNetUsers_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

GO

CREATE NONCLUSTERED INDEX [IX_Appointments_PatientId] ON [dbo].[Appointments] ([PatientId] ASC);
CREATE NONCLUSTERED INDEX [IX_Appointments_DoctorId] ON [dbo].[Appointments] ([DoctorId] ASC);
CREATE NONCLUSTERED INDEX [IX_Appointments_AppointmentDate_AppointmentTime] ON [dbo].[Appointments] ([AppointmentDate] ASC, [AppointmentTime] ASC); 