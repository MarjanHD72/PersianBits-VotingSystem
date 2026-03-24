using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VotingSystem.Domain;
using static System.Collections.Specialized.BitVector32;

namespace Votingsystem.frontend.Pages.User;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    public DashboardModel(AppDbContext db) => _db = db;

    public List<Election> AvailableElections { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Show running elections the user hasn't voted in yet
        var votedIds = await _db.Votes
            .Where(v => v.UserId == userId)
            .Select(v => v.ElectionId)
            .ToListAsync();

        AvailableElections = await _db.Elections
            .Where(e => e.Status == "running" && !votedIds.Contains(e.Id))
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }
}
