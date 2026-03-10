-- Notes Application - SQL Server Schema
-- Run this script to create the database and tables.
-- For local dev: create a database first, e.g. "NotesDb", then run this.

-- Optional: Create database (skip if using existing DB)
-- CREATE DATABASE NotesDb;
-- GO
-- USE NotesDb;
-- GO

-- Optional: Users table (for future JWT auth - users only see their own notes)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Email       NVARCHAR(256) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(256) NOT NULL,
        CreatedAt   DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Notes table
-- UserId NULL = no auth (all notes shared); with auth, set UserId and add FK to Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
BEGIN
    CREATE TABLE dbo.Notes (
        Id          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId      INT NULL,  -- NULL when auth disabled; FK to Users when auth enabled
        Title       NVARCHAR(200) NOT NULL,
        Content     NVARCHAR(MAX) NULL,
        CreatedAt   DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt   DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
    );

    CREATE INDEX IX_Notes_UserId ON dbo.Notes(UserId);
    CREATE INDEX IX_Notes_CreatedAt ON dbo.Notes(CreatedAt DESC);
    CREATE INDEX IX_Notes_UpdatedAt ON dbo.Notes(UpdatedAt DESC);
END
GO

-- Add FK so users can only access their own notes (run once after tables exist):
-- ALTER TABLE dbo.Notes ADD CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id);
-- For the strict "correct design" with UserId NOT NULL, use script 003_SQLServer_UsersAndNotes_CorrectDesign.sql
