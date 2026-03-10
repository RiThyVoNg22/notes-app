-- Notes Application - PostgreSQL Schema
-- Run this script after creating the database, e.g.:
--   createdb notesdb
--   psql -d notesdb -f 002_PostgreSQL_CreateTables.sql

-- Optional: Users table (for future JWT auth)
CREATE TABLE IF NOT EXISTS "Users" (
    "Id"          SERIAL PRIMARY KEY,
    "Email"       VARCHAR(256) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(256) NOT NULL,
    "CreatedAt"   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Notes table (quoted identifiers to match C# PascalCase)
-- UserId NULL = no auth; add FK to "Users" when using auth
CREATE TABLE IF NOT EXISTS "Notes" (
    "Id"          SERIAL PRIMARY KEY,
    "UserId"      INTEGER NULL,
    "Title"       VARCHAR(200) NOT NULL,
    "Content"     TEXT NULL,
    "CreatedAt"   TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "UpdatedAt"   TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE INDEX IF NOT EXISTS "IX_Notes_UserId" ON "Notes"("UserId");
CREATE INDEX IF NOT EXISTS "IX_Notes_CreatedAt" ON "Notes"("CreatedAt" DESC);
CREATE INDEX IF NOT EXISTS "IX_Notes_UpdatedAt" ON "Notes"("UpdatedAt" DESC);

-- Optional: add FK when using auth (run after "Users" exists)
-- ALTER TABLE "Notes" ADD CONSTRAINT "FK_Notes_Users" FOREIGN KEY ("UserId") REFERENCES "Users"("Id");
