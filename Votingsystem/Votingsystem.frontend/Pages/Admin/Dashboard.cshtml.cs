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

    public List<Election> Elections { get; set; } = new();
    public int TotalPolls { get; set; }
    public int RunningCount { get; set; }
    public int TodayVotes { get; set; }

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

        TotalPolls = Elections.Count;
        RunningCount = Elections.Count(e => e.Status == "running");
        TodayVotes = Elections.Sum(e => e.Votes.Count(v => v.SubmittedAt.Date == DateTime.UtcNow.Date));
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
        var el = await _db.Elections.Include(e => e.Votes).Include(e => e.Candidates).FirstOrDefaultAsync(e => e.Id == id);
        if (el != null) { _db.Elections.Remove(el); await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }
}
