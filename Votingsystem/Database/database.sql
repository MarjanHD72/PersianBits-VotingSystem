-- ============================================================
--  PersianBits Voting System — Database Schema
--  Engine: SQLite
--
--  Used to create the SQLite schema for the PersianBits Voting System.
--  Run this file in DB Browser for SQLite or any SQLite-compatible tool
--  to manually recreate the database structure.
-- ============================================================

-- Required: SQLite disables foreign key enforcement by default
PRAGMA foreign_keys = ON;

-- ── Users ────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Users (
    Id                 INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName           TEXT    NOT NULL DEFAULT '',
    Email              TEXT    NOT NULL DEFAULT '' UNIQUE,
    PasswordHash       TEXT    NOT NULL DEFAULT '',
    DateOfBirth        TEXT    NOT NULL,
    Role               TEXT    NOT NULL DEFAULT 'User',   -- 'User' | 'Admin' | 'Developer'
    Avatar             TEXT    NOT NULL DEFAULT '',
    IsActive           INTEGER NOT NULL DEFAULT 1,        -- 0 = disabled, 1 = active
    RequestedAdmin     INTEGER NOT NULL DEFAULT 0,
    AdminRequestReason TEXT    NOT NULL DEFAULT '',
    CreatedAt          TEXT    NOT NULL DEFAULT (datetime('now'))
);

-- ── Elections ─────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Elections (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Title       TEXT    NOT NULL DEFAULT '',
    Description TEXT    NOT NULL DEFAULT '',
    SessionId   TEXT    NOT NULL DEFAULT '' UNIQUE,
    Type        TEXT    NOT NULL DEFAULT 'election',  -- 'election' | 'rating' | 'text' | 'multichoice' | 'ranking'
    Status      TEXT    NOT NULL DEFAULT 'draft',     -- 'draft' | 'running' | 'closed'
    CreatorId   INTEGER NOT NULL,
    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now')),

    FOREIGN KEY (CreatorId) REFERENCES Users(Id)
);

-- ── Candidates (for election-type polls) ──────────────────────
CREATE TABLE IF NOT EXISTS Candidates (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    ElectionId  INTEGER NOT NULL,
    Name        TEXT    NOT NULL DEFAULT '',
    Description TEXT    NOT NULL DEFAULT '',
    ImageUrl    TEXT    NOT NULL DEFAULT '',

    FOREIGN KEY (ElectionId) REFERENCES Elections(Id)
);

-- ── Options (for multichoice / ranking polls) ─────────────────
CREATE TABLE IF NOT EXISTS Options (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    ElectionId INTEGER NOT NULL,
    Text       TEXT    NOT NULL DEFAULT '',

    FOREIGN KEY (ElectionId) REFERENCES Elections(Id)
);

-- ── Votes ─────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS Votes (
    Id              INTEGER PRIMARY KEY AUTOINCREMENT,
    ElectionId      INTEGER NOT NULL,
    UserId          INTEGER NOT NULL,
    CandidateId     INTEGER,                          -- election type only
    RatingValue     INTEGER,                          -- rating type only (1–5)
    TextResponse    TEXT,                             -- text type only
    SelectedOptions TEXT,                             -- multichoice: comma-separated option IDs
    RankingOrder    TEXT,                             -- ranking: comma-separated option IDs in ranked order
    SubmittedAt     TEXT    NOT NULL DEFAULT (datetime('now')),

    FOREIGN KEY (ElectionId)  REFERENCES Elections(Id),
    FOREIGN KEY (UserId)      REFERENCES Users(Id),
    FOREIGN KEY (CandidateId) REFERENCES Candidates(Id)
);

-- Enforces one vote per user per election at the database level
CREATE UNIQUE INDEX IF NOT EXISTS IX_Votes_ElectionId_UserId
    ON Votes (ElectionId, UserId);
