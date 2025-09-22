-- Create UrlTokens table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UrlTokens' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[UrlTokens](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Token] [nvarchar](500) NOT NULL,
        [ResourceType] [nvarchar](50) NOT NULL,
        [ResourceId] [nvarchar](450) NOT NULL,
        [OriginalUrl] [nvarchar](500) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [ExpiresAt] [datetime2](7) NOT NULL,
        [IsUsed] [bit] NOT NULL,
        [UsedAt] [datetime2](7) NULL,
        [UserAgent] [nvarchar](256) NULL,
        [ClientIp] [nvarchar](45) NULL,
        CONSTRAINT [PK_UrlTokens] PRIMARY KEY CLUSTERED ([Id] ASC)
    )

    -- Create indexes
    CREATE UNIQUE NONCLUSTERED INDEX [IX_UrlTokens_Token] ON [dbo].[UrlTokens] ([Token] ASC)
    CREATE NONCLUSTERED INDEX [IX_UrlTokens_ExpiresAt] ON [dbo].[UrlTokens] ([ExpiresAt] ASC)
    CREATE NONCLUSTERED INDEX [IX_UrlTokens_IsUsed] ON [dbo].[UrlTokens] ([IsUsed] ASC)
    CREATE NONCLUSTERED INDEX [IX_UrlTokens_ResourceId] ON [dbo].[UrlTokens] ([ResourceId] ASC)
    CREATE NONCLUSTERED INDEX [IX_UrlTokens_ResourceType] ON [dbo].[UrlTokens] ([ResourceType] ASC)

    -- Create foreign key constraint
    ALTER TABLE [dbo].[UrlTokens] 
    ADD CONSTRAINT [FK_UrlTokens_AspNetUsers_ResourceId] 
    FOREIGN KEY([ResourceId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION

    PRINT 'UrlTokens table created successfully'
END
ELSE
BEGIN
    PRINT 'UrlTokens table already exists'
END
