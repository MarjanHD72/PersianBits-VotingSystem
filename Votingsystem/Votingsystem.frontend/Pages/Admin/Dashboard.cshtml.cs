using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    public DashboardModel(AppDbContext db) => _db = db;

    // Elections list
    public List<Election> Elections { get; set; } = new();

    // KPI counts
    public int TotalPolls { get; set; }
    public int RunningCount { get; set; }
    public int DraftCount { get; set; }
    public int ClosedCount { get; set; }
    public int TodayVotes { get; set; }
    public int TotalVotes { get; set; }
    public int TotalUsers { get; set; }
    public int CompletionRate { get; set; }

    // For participation progress bars
    public int MaxVotes { get; set; }

    // Chart data
    public List<DayVote> VotesByDay { get; set; } = new();
    public List<TopElection> TopElections { get; set; } = new();

    public class DayVote   { public string Label { get; set; } = ""; public int Count { get; set; } }
    public class TopElection { public string Title { get; set; } = ""; public int Votes { get; set; } }

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Elections = await _db.Elections
            .Where(e => e.CreatorId == userId)
            .Include(e => e.Votes)
            .Include(e => e.Candidates)
            .Include(e => e.Options)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();

        TotalPolls   = Elections.Count;
        RunningCount = Elections.Count(e => e.Status == "running");
        DraftCount   = Elections.Count(e => e.Status == "draft");
        ClosedCount  = Elections.Count(e => e.Status == "closed");
        TodayVotes   = Elections.Sum(e => e.Votes.Count(v => v.SubmittedAt.Date == DateTime.UtcNow.Date));
        TotalVotes   = Elections.Sum(e => e.Votes.Count);
        MaxVotes     = Elections.Any() ? Elections.Max(e => e.Votes.Count) : 1;
        if (MaxVotes == 0) MaxVotes = 1; // avoid division by zero in view

        CompletionRate = TotalPolls > 0
            ? (int)Math.Round(ClosedCount * 100.0 / TotalPolls)
            : 0;

        TotalUsers = await _db.Users.CountAsync();

        // Votes per day for last 7 days
        var today = DateTime.UtcNow.Date;
        var allVotes = Elections.SelectMany(e => e.Votes).ToList();
        VotesByDay = Enumerable.Range(6, 7)      // 6 days ago → today (index 6..12 → offset 6 down to 0)
            .Select(offset =>
            {
                var day = today.AddDays(-(6 - offset));  // today-6 … today
                return new DayVote
                {
                    Label = day.ToString("MMM d"),
                    Count = allVotes.Count(v => v.SubmittedAt.Date == day)
                };
            })
            .ToList();

        // Top 5 elections by vote count
        TopElections = Elections
            .Where(e => e.Votes.Count > 0)
            .OrderByDescending(e => e.Votes.Count)
            .Take(5)
            .Select(e => new TopElection { Title = e.Title, Votes = e.Votes.Count })
            .ToList();
    }

    public async Task<IActionResult> OnPostStartAsync(int id)
    {
        var el = await _db.Elections.FindAsync(id);
        if (el != null) { el.Status = "running"; await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostStopAsync(int id)
    {
        var el = await _db.Elections.FindAsync(id);
        if (el != null) { el.Status = "closed"; await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var el = await _db.Elections
            .Include(e => e.Votes)
            .Include(e => e.Candidates)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (el != null) { _db.Elections.Remove(el); await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }
}
