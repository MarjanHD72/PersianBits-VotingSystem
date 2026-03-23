using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin.Builders;

[Authorize(Roles = "Admin")]
public class MultichoiceModel : PageModel
{
    private readonly AppDbContext _db;
    public MultichoiceModel(AppDbContext db) => _db = db;

    [BindProperty] public string Title { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";
    [BindProperty] public List<string> OptionList { get; set; } = new();

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
            Type = "multichoice",
            Status = "draft",
            CreatorId = userId
        };

        foreach (var opt in (OptionList ?? new()).Where(o => !string.IsNullOrWhiteSpace(o)))
            election.Options.Add(new Option { Text = opt.Trim() });

        _db.Elections.Add(election);
        await _db.SaveChangesAsync();
        return RedirectToPage("/Admin/Dashboard");
    }
}
