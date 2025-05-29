-- Add IsRead column to Notifications table
ALTER TABLE [dbo].[Notifications]
ADD [IsRead] BIT NOT NULL DEFAULT 0; 