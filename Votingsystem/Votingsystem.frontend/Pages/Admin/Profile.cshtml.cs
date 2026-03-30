using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ProfileModel : PageModel
{
    private readonly AppDbContext _db;
    public ProfileModel(AppDbContext db) => _db = db;

    public AppUser? Profile { get; set; }
    public int ElectionCount { get; set; }
    public int TotalVotes { get; set; }
    public int ActiveCount { get; set; }

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Profile = await _db.Users.FindAsync(userId);

        var elections = await _db.Elections.Where(e => e.CreatorId == userId).Include(e => e.Votes).ToListAsync();
        ElectionCount = elections.Count;
        TotalVotes = elections.Sum(e => e.Votes.Count);
        ActiveCount = elections.Count(e => e.Status == "running");
    }

    public async Task<IActionResult> OnPostAvatarAsync(string avatar)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.Avatar = avatar; await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }
}