using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Poll;

[Authorize]
public class TakeModel : PageModel
{
    private readonly AppDbContext _db;
    public TakeModel(AppDbContext db) => _db = db;

    public Election? Election { get; set; }
    public bool AlreadyVoted { get; set; }
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(string? session)
    {
        if (string.IsNullOrWhiteSpace(session)) return RedirectToPage("/User/Dashboard");

        Election = await _db.Elections
            .Include(e => e.Candidates)
            .Include(e => e.Options)
            .FirstOrDefaultAsync(e => e.SessionId == session.Trim().ToUpper());

        if (Election == null) { Error = "Election not found."; return Page(); }
        if (Election.Status != "running") { Error = "This election is not currently active."; return Page(); }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        AlreadyVoted = await _db.Votes.AnyAsync(v => v.ElectionId == Election.Id && v.UserId == userId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? session, int? candidateId, int? ratingValue,
        string? textResponse, List<int>? selectedOptions, string? rankingOrder)
    {
        if (string.IsNullOrWhiteSpace(session)) return RedirectToPage("/User/Dashboard");

        var election = await _db.Elections
            .Include(e => e.Candidates)
            .Include(e => e.Options)
            .FirstOrDefaultAsync(e => e.SessionId == session.Trim().ToUpper());

        if (election == null || election.Status != "running")
            return RedirectToPage("/User/Dashboard");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _db.Votes.AnyAsync(v => v.ElectionId == election.Id && v.UserId == userId))
            return RedirectToPage("/Poll/Thanks");

        var vote = new Vote
        {
            ElectionId = election.Id,
            UserId = userId,
            CandidateId = candidateId,
            RatingValue = ratingValue,
            TextResponse = textResponse,
            SelectedOptions = selectedOptions != null ? string.Join(",", selectedOptions) : null,
            RankingOrder = rankingOrder
        };

        _db.Votes.Add(vote);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Poll/Thanks");
    }
}
