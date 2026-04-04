using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
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

    // Portrait photos from randomuser.me CDN — downloaded once and cached locally
    private static readonly (string Url, string FileName)[] CandidatePhotos =
    [
        ("https://randomuser.me/api/portraits/women/44.jpg", "seed-candidate-alice.jpg"),
        ("https://randomuser.me/api/portraits/men/32.jpg",   "seed-candidate-raj.jpg"),
        ("https://randomuser.me/api/portraits/women/17.jpg", "seed-candidate-chloe.jpg"),
        ("https://randomuser.me/api/portraits/men/55.jpg",   "seed-candidate-tobias.jpg"),
        ("https://randomuser.me/api/portraits/men/91.jpg",   "seed-candidate-team-alpha.jpg"),
        ("https://randomuser.me/api/portraits/women/8.jpg",  "seed-candidate-team-beta.jpg"),
        ("https://randomuser.me/api/portraits/men/63.jpg",   "seed-candidate-team-gamma.jpg"),
    ];

    public static async Task SeedAsync(AppDbContext db, IWebHostEnvironment env)
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

        // ── Look up system admin ──────────────────────────────────────────
        var systemAdmin = await db.Users.FirstOrDefaultAsync(u => u.Email == "admin@persianbits.com");
        if (systemAdmin == null) return;

        // ── Download candidate photos once, store in wwwroot/uploads ─────
        var photoUrls = await DownloadCandidatePhotosAsync(env);

        // ── Insert users ──────────────────────────────────────────────────
        var now = DateTime.UtcNow;
        var users = new List<AppUser>();

        for (int i = 0; i < fetched.Count; i++)
        {
            var f = fetched[i];
            var requestAdmin = i >= fetched.Count - 5;
            users.Add(new AppUser
            {
                FullName           = f.Name,
                Email              = f.Email,
                PasswordHash       = BCryptHelper.Hash("demo123"),
                DateOfBirth        = f.Dob,
                Role               = "User",
                Avatar             = Avatars[i % Avatars.Length],
                IsActive           = true,
                RequestedAdmin     = requestAdmin,
                AdminRequestReason = requestAdmin ? AdminRequestReasons[i % AdminRequestReasons.Length] : "",
                CreatedAt          = now.AddDays(-(fetched.Count - i))
            });
        }

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        // ── Elections — all owned by the default admin ────────────────────
        var elections = new List<Election>
        {
            new() { Title = "Student Union President 2025",         Description = "Vote for your Student Union President. The elected candidate will represent all students in university governance meetings.", Type = "election",   Status = "running", SessionId = "PRES-2025", CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-30) },
            new() { Title = "Best Programming Language for Web Dev", Description = "Which programming language do you think is best suited for modern web development? Select all that apply.",                 Type = "multichoice", Status = "running", SessionId = "PROG-LANG", CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-25) },
            new() { Title = "Rate Our Library Services",             Description = "How satisfied are you with the university library services this semester? Your feedback helps us improve.",                  Type = "rating",      Status = "running", SessionId = "LIB-RATE",  CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-20) },
            new() { Title = "Campus Improvement Suggestions",        Description = "What is the one thing you would most like to see improved on campus? Share your honest thoughts.",                          Type = "text",        Status = "running", SessionId = "CAMP-IMPR", CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-18) },
            new() { Title = "Rank Campus Facilities by Priority",    Description = "Rank the following campus facilities in order of importance to you. Results will inform next year's budget allocation.",    Type = "ranking",     Status = "closed",  SessionId = "FAC-RANK",  CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-45) },
            new() { Title = "Best Student Project Award 2025",       Description = "Vote for the most outstanding student project presented at the Spring Showcase.",                                            Type = "election",    Status = "closed",  SessionId = "PROJ-2025", CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-40) },
            new() { Title = "End of Year Social Event Theme",        Description = "Help us pick the theme for this year's end-of-year social event.",                                                          Type = "multichoice", Status = "draft",   SessionId = "SOC-THEM",  CreatorId = systemAdmin.Id, CreatedAt = now.AddDays(-5)  },
        };

        db.Elections.AddRange(elections);
        await db.SaveChangesAsync();

        var elPres    = elections[0];
        var elProg    = elections[1];
        var elLib     = elections[2];
        var elCampus  = elections[3];
        var elFac     = elections[4];
        var elProject = elections[5];

        // ── Candidates ────────────────────────────────────────────────────
        var candidates = new List<Candidate>
        {
            new() { ElectionId = elPres.Id,    Name = "Alice Pemberton",          Description = "Third-year Law student. Focused on mental health support and affordable campus housing.",          ImageUrl = photoUrls[0] },
            new() { ElectionId = elPres.Id,    Name = "Raj Kapoor",               Description = "Final-year Engineering student. Campaigning for better lab resources and study spaces.",           ImageUrl = photoUrls[1] },
            new() { ElectionId = elPres.Id,    Name = "Chloe Beaumont",           Description = "Second-year Business student. Advocates for stronger industry partnerships and internships.",      ImageUrl = photoUrls[2] },
            new() { ElectionId = elPres.Id,    Name = "Tobias Weiss",             Description = "Final-year Computer Science student. Pushing for improved digital infrastructure.",                ImageUrl = photoUrls[3] },
            new() { ElectionId = elProject.Id, Name = "Team Alpha — SmartCampus", Description = "An IoT system that monitors room occupancy and reduces energy consumption across campus.",         ImageUrl = photoUrls[4] },
            new() { ElectionId = elProject.Id, Name = "Team Beta — MindBridge",   Description = "A mental health check-in app that connects students anonymously with counsellors.",               ImageUrl = photoUrls[5] },
            new() { ElectionId = elProject.Id, Name = "Team Gamma — VoteRight",   Description = "A transparent, open-source digital voting platform designed for student organisations.",          ImageUrl = photoUrls[6] },
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

        // ── Votes — all 100 users participate ────────────────────────────
        var votes = new List<Vote>();
        var voters = users;

        var textResponses = new[]
        {
            "More 24-hour study spaces during exam season would make a huge difference.",
            "The Wi-Fi in the main lecture hall drops constantly. Better internet infrastructure is a must.",
            "Affordable healthy food options in the cafeteria. The current choices are either expensive or unhealthy.",
            "A dedicated quiet zone in the library with stricter noise policies would help everyone focus.",
            "Better public transport links to campus — the last bus leaves too early.",
            "More mental health counsellors and shorter waiting times for appointments.",
            "Improved recycling facilities across campus buildings.",
            "Longer opening hours for the sports centre, especially on weekends.",
            "A peer tutoring scheme where experienced students help those in earlier years.",
            "Faster Wi-Fi and more power sockets in the main study areas.",
        };

        for (int i = 0; i < voters.Count; i++)
        {
            var v = voters[i];

            // Election 1 — Student Union President
            votes.Add(new Vote { ElectionId = elPres.Id,    UserId = v.Id, CandidateId = presCands[i % presCands.Count].Id, SubmittedAt = now.AddDays(-20).AddHours(i) });
            // Election 2 — Multichoice
            var selected = string.Join(",", progOptions.Where((_, idx) => (i + idx) % 3 == 0).Select(o => o.Id.ToString()));
            if (string.IsNullOrEmpty(selected)) selected = progOptions[0].Id.ToString();
            votes.Add(new Vote { ElectionId = elProg.Id,    UserId = v.Id, SelectedOptions = selected, SubmittedAt = now.AddDays(-18).AddHours(i) });
            // Election 3 — Rating
            votes.Add(new Vote { ElectionId = elLib.Id,     UserId = v.Id, RatingValue = (i % 5) + 1,  SubmittedAt = now.AddDays(-16).AddHours(i) });
            // Election 4 — Text (first 10 voters submit a response)
            if (i < 10)
                votes.Add(new Vote { ElectionId = elCampus.Id, UserId = v.Id, TextResponse = textResponses[i], SubmittedAt = now.AddDays(-14).AddHours(i) });
            // Election 5 — Ranking (closed)
            var ranked = string.Join(",", facOptions.OrderBy(_ => (i * 7 + facOptions.IndexOf(_) * 3) % 11).Select(o => o.Id.ToString()));
            votes.Add(new Vote { ElectionId = elFac.Id,     UserId = v.Id, RankingOrder = ranked,       SubmittedAt = now.AddDays(-35).AddHours(i) });
            // Election 6 — Best Project (closed)
            votes.Add(new Vote { ElectionId = elProject.Id, UserId = v.Id, CandidateId = projCands[i % projCands.Count].Id, SubmittedAt = now.AddDays(-32).AddHours(i) });
        }

        db.Votes.AddRange(votes);
        await db.SaveChangesAsync();
    }

    // Downloads each candidate portrait from randomuser.me once; serves local path thereafter.
    private static async Task<string[]> DownloadCandidatePhotosAsync(IWebHostEnvironment env)
    {
        var uploadsDir = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsDir);

        var urls = new string[CandidatePhotos.Length];
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

        for (int i = 0; i < CandidatePhotos.Length; i++)
        {
            var (remoteUrl, fileName) = CandidatePhotos[i];
            var localPath = Path.Combine(uploadsDir, fileName);

            if (!File.Exists(localPath))
            {
                try
                {
                    var bytes = await http.GetByteArrayAsync(remoteUrl);
                    await File.WriteAllBytesAsync(localPath, bytes);
                }
                catch
                {
                    // Photo unavailable — leave ImageUrl empty; UI will show the 👤 fallback
                    urls[i] = "";
                    continue;
                }
            }

            urls[i] = $"/uploads/{fileName}";
        }

        return urls;
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
