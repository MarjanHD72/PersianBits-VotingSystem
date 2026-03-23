using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UserDetailsModel : PageModel
{
    private readonly AppDbContext _db;
    public UserDetailsModel(AppDbContext db) => _db = db;

    public AppUser? UserDetail { get; set; }
    public int VoteCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return RedirectToPage("/Admin/Users");
        UserDetail = await _db.Users.FindAsync(id);
        if (UserDetail == null) return RedirectToPage("/Admin/Users");
        VoteCount = await _db.Votes.CountAsync(v => v.UserId == id);
        return Page();
    }
}
