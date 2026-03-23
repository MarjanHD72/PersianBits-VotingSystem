using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly AppDbContext _db;
    public UsersModel(AppDbContext db) => _db = db;

    public List<AppUser> UserList { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string? Search { get; set; }

    public async Task OnGetAsync()
    {
        var query = _db.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var s = Search.Trim().ToLower();
            query = query.Where(u => u.FullName.ToLower().Contains(s) || u.Email.ToLower().Contains(s));
        }

        UserList = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostDisableAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null) { user.IsActive = !user.IsActive; await _db.SaveChangesAsync(); }
        return RedirectToPage();
    }
}
