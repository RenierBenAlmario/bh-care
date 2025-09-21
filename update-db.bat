@echo off
echo Updating database with migrations...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -i complete-migrations.sql -E
if %ERRORLEVEL% NEQ 0 (
    echo Error running complete-migrations.sql
    pause
    exit /b 1
)

echo Running update-database-all.sql...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -i update-database-all.sql -E
if %ERRORLEVEL% NEQ 0 (
    echo Error running update-database-all.sql
    pause
    exit /b 1
)

echo Fixing UserNumber column...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -i add-user-number-simple.sql -E
if %ERRORLEVEL% NEQ 0 (
    echo Error adding UserNumber column
    pause
    exit /b 1
)

echo Updating UserNumber values...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -i drop-trigger.sql -E
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -i create-trigger.sql -E
if %ERRORLEVEL% NEQ 0 (
    echo Error updating UserNumber values
    pause
    exit /b 1
)

echo Applying pending migrations...
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'RelationshipToPatient') BEGIN ALTER TABLE [Appointments] ADD [RelationshipToPatient] nvarchar(100) NULL; PRINT 'Added RelationshipToPatient column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId') BEGIN ALTER TABLE [Appointments] ADD [BookedByUserId] nvarchar(450) NULL; PRINT 'Added BookedByUserId column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId') AND NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_BookedByUserId' AND object_id = OBJECT_ID('Appointments')) BEGIN CREATE INDEX [IX_Appointments_BookedByUserId] ON [Appointments] ([BookedByUserId]) WHERE [BookedByUserId] IS NOT NULL; PRINT 'Created index IX_Appointments_BookedByUserId'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookedByUserId') AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_AspNetUsers_BookedByUserId') BEGIN ALTER TABLE [Appointments] ADD CONSTRAINT [FK_Appointments_AspNetUsers_BookedByUserId] FOREIGN KEY ([BookedByUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE SET NULL; PRINT 'Added foreign key FK_Appointments_AspNetUsers_BookedByUserId'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookingNotes') BEGIN ALTER TABLE [Appointments] ADD [BookingNotes] nvarchar(max) NULL; PRINT 'Added BookingNotes column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerContactNumber') BEGIN ALTER TABLE [Appointments] ADD [BookerContactNumber] nvarchar(20) NULL; PRINT 'Added BookerContactNumber column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerName') BEGIN ALTER TABLE [Appointments] ADD [BookerName] nvarchar(200) NULL; PRINT 'Added BookerName column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[Appointments]') AND name = 'BookerEmail') BEGIN ALTER TABLE [Appointments] ADD [BookerEmail] nvarchar(200) NULL; PRINT 'Added BookerEmail column'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250626061924_BookingFormUpdates') BEGIN INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20250626061924_BookingFormUpdates', '8.0.2'); PRINT 'Marked migration 20250626061924_BookingFormUpdates as applied'; END"
sqlcmd -S DESKTOP-NU53VS3\SQLEXPRESS -d Barangay -E -Q "IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250629115500_AddBookingRelationshipColumns') BEGIN INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20250629115500_AddBookingRelationshipColumns', '8.0.2'); PRINT 'Marked migration 20250629115500_AddBookingRelationshipColumns as applied'; END"

echo Verifying migrations...
dotnet ef migrations list --context ApplicationDbContext

echo Database update completed successfully!
pause 