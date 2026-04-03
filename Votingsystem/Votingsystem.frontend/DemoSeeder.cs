using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend;

public static class DemoSeeder
{
    private static readonly string[] Avatars = ["🌸","🦁","🌺","🐉","🌙","🔮","🌻","🦊","🦋","🌊","⭐","🎯"];
    private static readonly string[] AdminRequestReasons =
    [
        "I want to run polls for the Computer Science Society events.",
        "Student council representative — need to create election polls each semester."
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        // Only seed if no demo users exist yet (more than the 2 system accounts)
        if (await db.Users.CountAsync() > 2) return;

        // ── Fetch users from randomuser.me ────────────────────────────────
        List<RandomUser> fetched;
        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var json = await http.GetStringAsync("https://randomuser.me/api/?results=100&nat=gb,us,au&inc=name,email,dob");
            var doc  = JsonDocument.Parse(json);
            fetched  = doc.RootElement.GetProperty("results")
                .EnumerateArray()
                .Select(r => new RandomUser(
                    $"{r.GetProperty("name").GetProperty("first").GetString()} {r.GetProperty("name").GetProperty("last").GetString()}",
                    r.GetProperty("email").GetString()!,
                    DateTime.Parse(r.GetProperty("dob").GetProperty("date").GetString()!)
                ))
                .ToList();
        }
        catch
        {
            // API unavailable — fall back to built-in names
            fetched = FallbackUsers();
        }

        // ── Insert users ──────────────────────────────────────────────────
        var now = DateTime.UtcNow;
        var users = new List<AppUser>();

        for (int i = 0; i < fetched.Count; i++)
        {
            var f    = fetched[i];
            var role = i < 5 ? "Admin" : "User";
            var requestAdmin  = i >= fetched.Count - 5;
            users.Add(new AppUser
            {
                FullName           = f.Name,
                Email              = f.Email,
                PasswordHash       = BCryptHelper.Hash("demo123"),
                DateOfBirth        = f.Dob,
                Role               = role,
                Avatar             = Avatars[i % Avatars.Length],
                IsActive           = true,
                RequestedAdmin     = requestAdmin,
                AdminRequestReason = requestAdmin ? AdminRequestReasons[i % AdminRequestReasons.Length] : "",
                CreatedAt          = now.AddDays(-(fetched.Count - i))
            });
        }

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        // Capture IDs assigned by the DB
        var admins    = users.Where(u => u.Role == "Admin").ToList();
        var allVoters = users.Where(u => u.Role == "User").ToList();

        // ── Elections ─────────────────────────────────────────────────────
        var elections = new List<Election>
        {
            new() { Title = "Student Union President 2025",      Description = "Vote for your Student Union President. The elected candidate will represent all students in university governance meetings.", Type = "election",   Status = "running", SessionId = "PRES-2025", CreatorId = admins[0].Id, CreatedAt = now.AddDays(-30) },
            new() { Title = "Best Programming Language for Web Dev", Description = "Which programming language do you think is best suited for modern web development? Select all that apply.",                Type = "multichoice", Status = "running", SessionId = "PROG-LANG", CreatorId = admins[1].Id, CreatedAt = now.AddDays(-25) },
            new() { Title = "Rate Our Library Services",          Description = "How satisfied are you with the university library services this semester? Your feedback helps us improve.",                   Type = "rating",      Status = "running", SessionId = "LIB-RATE",  CreatorId = admins[1].Id, CreatedAt = now.AddDays(-20) },
            new() { Title = "Campus Improvement Suggestions",     Description = "What is the one thing you would most like to see improved on campus? Share your honest thoughts.",                           Type = "text",        Status = "running", SessionId = "CAMP-IMPR", CreatorId = admins[0].Id, CreatedAt = now.AddDays(-18) },
            new() { Title = "Rank Campus Facilities by Priority", Description = "Rank the following campus facilities in order of importance to you. Results will inform next year's budget allocation.",     Type = "ranking",     Status = "closed",  SessionId = "FAC-RANK",  CreatorId = admins[2].Id, CreatedAt = now.AddDays(-45) },
            new() { Title = "Best Student Project Award 2025",    Description = "Vote for the most outstanding student project presented at the Spring Showcase.",                                             Type = "election",    Status = "closed",  SessionId = "PROJ-2025", CreatorId = admins[2].Id, CreatedAt = now.AddDays(-40) },
            new() { Title = "End of Year Social Event Theme",     Description = "Help us pick the theme for this year's end-of-year social event.",                                                           Type = "multichoice", Status = "draft",   SessionId = "SOC-THEM",  CreatorId = admins[0].Id, CreatedAt = now.AddDays(-5)  },
        };

        db.Elections.AddRange(elections);
        await db.SaveChangesAsync();

        var elPres    = elections[0]; // election type, running
        var elProg    = elections[1]; // multichoice, running
        var elLib     = elections[2]; // rating, running
        var elCampus  = elections[3]; // text, running
        var elFac     = elections[4]; // ranking, closed
        var elProject = elections[5]; // election type, closed

        // ── Candidates ────────────────────────────────────────────────────
        var candidates = new List<Candidate>
        {
            new() { ElectionId = elPres.Id,    Name = "Alice Pemberton",         Description = "Third-year Law student. Focused on mental health support and affordable campus housing." },
            new() { ElectionId = elPres.Id,    Name = "Raj Kapoor",              Description = "Final-year Engineering student. Campaigning for better lab resources and study spaces." },
            new() { ElectionId = elPres.Id,    Name = "Chloe Beaumont",          Description = "Second-year Business student. Advocates for stronger industry partnerships and internships." },
            new() { ElectionId = elPres.Id,    Name = "Tobias Weiss",            Description = "Final-year Computer Science student. Pushing for improved digital infrastructure." },
            new() { ElectionId = elProject.Id, Name = "Team Alpha — SmartCampus", Description = "An IoT system that monitors room occupancy and reduces energy consumption across campus." },
            new() { ElectionId = elProject.Id, Name = "Team Beta — MindBridge",  Description = "A mental health check-in app that connects students anonymously with counsellors." },
            new() { ElectionId = elProject.Id, Name = "Team Gamma — VoteRight",  Description = "A transparent, open-source digital voting platform designed for student organisations." },
        };

        db.Candidates.AddRange(candidates);
        await db.SaveChangesAsync();

        // ── Options ───────────────────────────────────────────────────────
        var options = new List<Option>
        {
            new() { ElectionId = elProg.Id, Text = "JavaScript / TypeScript" },
            new() { ElectionId = elProg.Id, Text = "Python" },
            new() { ElectionId = elProg.Id, Text = "Go" },
            new() { ElectionId = elProg.Id, Text = "Rust" },
            new() { ElectionId = elProg.Id, Text = "C# / .NET" },
            new() { ElectionId = elFac.Id,  Text = "Library & Study Spaces" },
            new() { ElectionId = elFac.Id,  Text = "Sports & Fitness Centre" },
            new() { ElectionId = elFac.Id,  Text = "Student Union Building" },
            new() { ElectionId = elFac.Id,  Text = "Cafeteria & Food Options" },
            new() { ElectionId = elFac.Id,  Text = "Computer Labs & Tech Resources" },
        };

        db.Options.AddRange(options);
        await db.SaveChangesAsync();

        var progOptions = options.Take(5).ToList();
        var facOptions  = options.Skip(5).ToList();
        var presCands   = candidates.Take(4).ToList();
        var projCands   = candidates.Skip(4).ToList();

        // ── Votes ─────────────────────────────────────────────────────────
        var votes = new List<Vote>();
        var voters = allVoters.Take(60).ToList();

        var textResponses = new[]
        {
            "More 24-hour study spaces during exam season would make a huge difference.",
            "The Wi-Fi in the main lecture hall drops constantly. Better internet infrastructure is a must.",
            "Affordable healthy food options in the cafeteria. The current choices are either expensive or unhealthy.",
        };

        for (int i = 0; i < voters.Count; i++)
        {
            var v = voters[i];

            // Election 1 — Student Union President
            votes.Add(new Vote { ElectionId = elPres.Id,    UserId = v.Id, CandidateId = presCands[i % presCands.Count].Id, SubmittedAt = now.AddDays(-20 + i) });
            // Election 2 — Multichoice
            var selected = string.Join(",", progOptions.Where((_, idx) => (i + idx) % 3 == 0).Select(o => o.Id.ToString()));
            if (string.IsNullOrEmpty(selected)) selected = progOptions[0].Id.ToString();
            votes.Add(new Vote { ElectionId = elProg.Id,    UserId = v.Id, SelectedOptions = selected, SubmittedAt = now.AddDays(-18 + i) });
            // Election 3 — Rating
            votes.Add(new Vote { ElectionId = elLib.Id,     UserId = v.Id, RatingValue = (i % 5) + 1,  SubmittedAt = now.AddDays(-16 + i) });
            // Election 4 — Text (first 3 voters only)
            if (i < 3)
                votes.Add(new Vote { ElectionId = elCampus.Id, UserId = v.Id, TextResponse = textResponses[i], SubmittedAt = now.AddDays(-14 + i) });
            // Election 5 — Ranking (closed)
            var ranked = string.Join(",", facOptions.OrderBy(_ => (i * 7 + facOptions.IndexOf(_) * 3) % 11).Select(o => o.Id.ToString()));
            votes.Add(new Vote { ElectionId = elFac.Id,     UserId = v.Id, RankingOrder = ranked,       SubmittedAt = now.AddDays(-35 + i) });
            // Election 6 — Best Project (closed)
            votes.Add(new Vote { ElectionId = elProject.Id, UserId = v.Id, CandidateId = projCands[i % projCands.Count].Id, SubmittedAt = now.AddDays(-32 + i) });
        }

        db.Votes.AddRange(votes);
        await db.SaveChangesAsync();
    }

    private static List<RandomUser> FallbackUsers() =>
    [
        new("Sarah Mitchell",  "sarah.mitchell@demo.com",  new DateTime(1995, 3,  12)),
        new("James Okafor",    "james.okafor@demo.com",    new DateTime(1993, 7,  22)),
        new("Priya Sharma",    "priya.sharma@demo.com",    new DateTime(1997, 11,  5)),
        new("Liam Chen",       "liam.chen@demo.com",       new DateTime(2001, 4,  18)),
        new("Emma Williams",   "emma.williams@demo.com",   new DateTime(2000, 9,  30)),
        new("Noah Patel",      "noah.patel@demo.com",      new DateTime(1999, 6,  14)),
        new("Ava Rodriguez",   "ava.rodriguez@demo.com",   new DateTime(2002, 1,  25)),
        new("Ethan Nguyen",    "ethan.nguyen@demo.com",    new DateTime(2000, 8,   3)),
        new("Mia Thompson",    "mia.thompson@demo.com",    new DateTime(1998, 12,  9)),
        new("Oliver Hassan",   "oliver.hassan@demo.com",   new DateTime(2001, 5,  27)),
        new("Zara Ali",        "zara.ali@demo.com",        new DateTime(1999, 10, 15)),
        new("Marcus Johnson",  "marcus.johnson@demo.com",  new DateTime(2000, 2,   7)),
    ];

    private record RandomUser(string Name, string Email, DateTime Dob);
}
