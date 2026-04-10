# PersianBits Voting System

A full-stack voting and polling platform that supports multiple poll types
(election, rating, ranking, multi-choice, and text), built with ASP.NET Core
Razor Pages, Entity Framework Core, and SQLite.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 9 — Razor Pages |
| Database | SQLite via Entity Framework Core 9 |
| Auth | Cookie-based authentication with role claims |
| Styling | Tailwind CSS (CDN) + custom design system |
| Font | DM Sans (Google Fonts) |

---

## Project Structure

```
PersianBits-VotingSystem/
├── VideoDemonstration.md                    # Link to video demonstration
├── Daily Standup Meetings                   # Sprint meeting notes + All meeting minutes
├── Database/
│   └── database.sql                         # SQL statements to create and seed the database
├── PersianBits_Report.pdf        # Full project report
├── README.md
├── VotingSystem.Domain/          # Models, DbContext, helpers, VoteTally
│   ├── IntHashMap.cs             # Custom open-addressing hash map (core data structure)
│   ├── VoteTally.cs              # Vote aggregation engine — uses IntHashMap
│   ├── AppUser.cs
│   ├── Election.cs
│   ├── Vote.cs
│   ├── Candidate.cs
│   ├── Option.cs
│   ├── Notification.cs
│   ├── AppDbContext.cs
│   ├── BCryptHelper.cs
│   ├── EmailService.cs
│   └── SessionIdGenerator.cs
├── VotingSystem.Application/     # Application layer
├── Votingsystem.frontend/        # Razor Pages application
│   ├── Pages/
│   │   ├── Admin/                # Dashboard, results, users, profile
│   │   ├── Developer/            # User & role management
│   │   ├── User/                 # Voter dashboard & profile
│   │   ├── Poll/                 # Vote form & review
│   │   ├── Join/                 # Session ID entry to join an election
│   │   └── Auth/                 # Login & register
│   ├── DemoSeeder.cs
│   └── Program.cs
└── VotingSystem.Tests/           # MSTest unit tests
    ├── IntHashMapTests.cs        # Tests for the custom hash map
    ├── VoteTallyTests.cs         # Tests for all five poll type aggregations
    └── BCryptHelperTests.cs      # Tests for password hashing
```

---

## Roles

| Role | Access |
|---|---|
| User | Vote in running elections, review submitted votes, manage profile |
| Admin | Create and manage elections, view live results, see participants |
| Developer | Approve admin requests, manage all users and roles, system-wide stats |

---

## Poll Types

| Type | Description |
|---|---|
| Election | Single candidate selection — produces a ranked leaderboard with winner |
| Rating | 1–5 star scale — shows average score and distribution |
| Text | Free-text response — displays all submissions |
| Multi-choice | Select multiple options — shows % of voters per option |
| Ranking | Drag-to-rank — scored using Borda count |

---

## Custom Data Structure — VoteTally

`VoteTally` is a custom data structure designed to efficiently count and process votes in a single pass.

**Internal structure:**
- Parallel arrays `_ids[]`, `_labels[]`, `_scores[]` — one slot per candidate/option
- `_slotIndex` — `IntHashMap` (custom open-addressing hash map) mapping entity ID → array index for O(1) average lookup per vote

**Factory methods:**

```csharp
VoteTally.ForCandidates(candidates)   // election type
VoteTally.ForOptions(options)         // multichoice / ranking
VoteTally.ForRatingScale()            // rating type (slots 1–5)
```

**Recording votes:**

```csharp
tally.RecordSingleVote(candidateId)   // O(1)
tally.RecordRating(ratingValue)       // O(1)
tally.RecordMultiChoice(csv)          // O(selections) — parses CSV once
tally.RecordBordaRanking(csv)         // O(options) — awards Borda points in one pass
```

**Reading results:**

```csharp
tally.GetRankedResults()              // O(k log k) — sorted TallyEntry[]
tally.WeightedAverage()              // rating average from same tally
```

**Algorithmic improvement over the previous LINQ approach:**

| Poll type | Before | After |
|---|---|---|
| Election | O(votes × candidates) | O(votes) |
| Rating | O(votes × 5) + separate average | O(votes) |
| Multi-choice | O(votes × options) + CSV split per option | O(votes × avg selections) |
| Ranking | O(votes × options²) nested loop | O(votes × options) |

---

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download)

### Run locally

```bash
git clone https://github.com/MarjanHD72/PersianBits-VotingSystem.git
cd PersianBits-VotingSystem/Votingsystem.frontend
dotnet run
```

Open `https://localhost:5001` in your browser. The database is created and seeded automatically on first run.

### Run unit tests

```bash
cd PersianBits-VotingSystem/VotingSystem.Tests
dotnet test
```

### Change the database file path

Edit `Votingsystem.frontend/appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Data Source=/path/to/your/database.db"
}
```

### Demo accounts

| Role | Email | Password |
|---|---|---|
| Admin | admin@persianbits.com | admin123 |
| Developer | dev@persianbits.com | dev123 |
| Users (×100) | (random) | demo123 |

---

## Key Features

- **Session ID voting** — admins share a short code (e.g. `PRES-2025`); users enter it to join
- **Vote review** — users can revisit any poll they voted in and see their answer read-only
- **Live avatar** — changing your profile avatar reflects in the header instantly without re-login
- **Candidate photos** — election polls support candidate photos in the vote form and results page
- **Real-time results** — results page updates on every load with winner highlighted and photo
- **Participant-scoped users** — admins only see users who voted in their own elections
- **Dark / light theme** — toggle in the header
- **AI assistant** — in-app chat powered by a local Ollama model (requires Ollama on port 11434)

---

## Seeded Demo Data

On first run the seeder:
1. Fetches 100 user profiles from randomuser.me (falls back to built-in names if offline)
2. Downloads 7 candidate portrait photos into `wwwroot/uploads/`
3. Creates 7 elections (4 running, 2 closed, 1 draft) all owned by the default admin
4. Records votes for all 100 users with weighted distributions so elections have a clear winner

---

## Email Configuration

For admin-request notification emails, configure `appsettings.json`:

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "SmtpUser": "",
  "SmtpPass": "",
  "FromEmail": "",
  "AdminEmail": ""
}
```

Leave empty to disable — the app works fully without it.

---

## Contributors

| Role | Name |
|---|---|
| Team Leader | Marjan Haghighatdoost |
| Secretary | Ronika Mohammadi Bazargan |
| Developer | Danial Nazari |
| Developer | Daniel Asadi |
| Tester · Debugger · UI Designer | Mohammad Darvishi |
