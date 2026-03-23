using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin.Builders;

[Authorize(Roles = "Admin")]
public class RatingScaleModel : PageModel
{
    private readonly AppDbContext _db;
    public RatingScaleModel(AppDbContext db) => _db = db;

    [BindProperty] public string Title { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Title)) { ModelState.AddModelError("", "Title is required."); return Page(); }
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var election = new Election
        {
            Title = Title.Trim(),
            Description = Description?.Trim() ?? "",
            SessionId = SessionIdGenerator.Generate(),
            Type = "rating",
            Status = "draft",
            CreatorId = userId
        };
        _db.Elections.Add(election);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Admin/Dashboard");
    }
}
