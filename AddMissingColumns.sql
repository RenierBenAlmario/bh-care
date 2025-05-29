-- Add missing columns to AspNetUsers table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'AgreedAt')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [AgreedAt] datetime2 NULL;
END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[AspNetUsers]') AND name = 'HasAgreedToTerms')
BEGIN
    ALTER TABLE [AspNetUsers] ADD [HasAgreedToTerms] bit NOT NULL DEFAULT CAST(0 AS bit);
END

-- Update migration history
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250511120000_AddAgreedToTermsColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250511120000_AddAgreedToTermsColumns', N'8.0.2');
END 