using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.User;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly AppDbContext _db;
    public ProfileModel(AppDbContext db) => _db = db;

    public AppUser? Profile { get; set; }

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Profile = await _db.Users.FindAsync(userId);
    }

    public async Task<IActionResult> OnPostAvatarAsync(string avatar)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(userId);
        if (user != null) { user.Avatar = avatar; await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }
}