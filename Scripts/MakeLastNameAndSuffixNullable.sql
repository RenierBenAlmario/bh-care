-- Script to make LastName and Suffix columns nullable in AspNetUsers table

-- Modify LastName column to be nullable
ALTER TABLE AspNetUsers
ALTER COLUMN LastName nvarchar(max) NULL;

-- Modify Suffix column to be nullable
ALTER TABLE AspNetUsers
ALTER COLUMN Suffix nvarchar(max) NULL; 