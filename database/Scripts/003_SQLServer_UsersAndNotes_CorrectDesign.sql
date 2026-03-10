-- =============================================================================
-- Notes Application - SQL Server (CORRECT DESIGN for interview)
-- =============================================================================
-- This is the design interviewers look for:
--   Users table + Notes table with UserId FOREIGN KEY
--   So users can ONLY read, update, and delete their OWN notes.
-- =============================================================================
-- Run: create database NotesDb, then run this script in NotesDb.
-- =============================================================================

-- Users table (required for auth and note ownership)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE dbo.Users (
        Id           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Email        NVARCHAR(256) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(256) NOT NULL,
        CreatedAt    DATETIME2(7) NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- Notes table: UserId links each note to its owner (REQUIRED for "own notes only")
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notes')
BEGIN
    CREATE TABLE dbo.Notes (
        Id        INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        UserId    INT NOT NULL,  -- REQUIRED: every note belongs to a user
        Title     NVARCHAR(200) NOT NULL,
        Content   NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Notes_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
    );

    CREATE INDEX IX_Notes_UserId ON dbo.Notes(UserId);
    CREATE INDEX IX_Notes_CreatedAt ON dbo.Notes(CreatedAt DESC);
    CREATE INDEX IX_Notes_UpdatedAt ON dbo.Notes(UpdatedAt DESC);
END
GO
