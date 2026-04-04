using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Poll;

[Authorize]
public class ResultsModel : PageModel
{
    private readonly AppDbContext _db;
    public ResultsModel(AppDbContext db) => _db = db;

    public Election? Election { get; set; }
    public Vote? UserVote { get; set; }
    public int TotalVotes { get; set; }
    public double AverageRating { get; set; }

    public List<CandidateResult>    CandidateResults    { get; set; } = new();
    public List<RatingResult>       RatingResults       { get; set; } = new();
    public List<TextResult>         TextResponses       { get; set; } = new();
    public List<OptionResult>       OptionResults       { get; set; } = new();
    public List<RankingItemResult>  RankingItemResults  { get; set; } = new();

    public class CandidateResult   { public string Name { get; set; } = ""; public string ImageUrl { get; set; } = ""; public int Votes { get; set; } public double Percentage { get; set; } }
    public class RatingResult      { public int Stars { get; set; } public int Count { get; set; } public double Percentage { get; set; } }
    public class TextResult        { public string UserName { get; set; } = ""; public string Text { get; set; } = ""; public DateTime SubmittedAt { get; set; } }
    public class OptionResult      { public string Text { get; set; } = ""; public int Count { get; set; } public double Percentage { get; set; } }
    public class RankingItemResult { public string Text { get; set; } = ""; public int Points { get; set; } }

    public async Task<IActionResult> OnGetAsync(string? session)
    {
        if (string.IsNullOrWhiteSpace(session)) return RedirectToPage("/User/Dashboard");

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        Election = await _db.Elections
            .Include(e => e.Candidates)
            .Include(e => e.Options)
            .Include(e => e.Votes).ThenInclude(v => v.User)
            .FirstOrDefaultAsync(e => e.SessionId == session.Trim().ToUpper());

        if (Election == null) return RedirectToPage("/User/Dashboard");

        // Only participants can see results
        UserVote = Election.Votes.FirstOrDefault(v => v.UserId == userId);
        if (UserVote == null) return RedirectToPage("/User/Dashboard");

        // Load candidate nav for election type
        if (UserVote.CandidateId.HasValue)
            UserVote = await _db.Votes.Include(v => v.Candidate)
                .FirstOrDefaultAsync(v => v.ElectionId == Election.Id && v.UserId == userId);

        TotalVotes = Election.Votes.Count;

        if (Election.Type == "election")
        {
            var tally = VoteTally.ForCandidates(Election.Candidates);
            foreach (var v in Election.Votes) tally.RecordSingleVote(v.CandidateId);
            CandidateResults = tally.GetRankedResults()
                .Select(e => new CandidateResult { Name = e.Label, ImageUrl = e.ImageUrl, Votes = e.Score, Percentage = e.Percentage })
                .ToList();
        }
        else if (Election.Type == "rating")
        {
            var tally = VoteTally.ForRatingScale();
            foreach (var v in Election.Votes) tally.RecordRating(v.RatingValue);
            AverageRating = tally.WeightedAverage();
            RatingResults = tally.GetRankedResults()
                .OrderByDescending(e => e.Id)
                .Select(e => new RatingResult { Stars = e.Id, Count = e.Score, Percentage = e.Percentage })
                .ToList();
        }
        else if (Election.Type == "text")
        {
            TextResponses = Election.Votes
                .Where(v => v.TextResponse != null)
                .OrderByDescending(v => v.SubmittedAt)
                .Select(v => new TextResult { UserName = v.User.FullName, Text = v.TextResponse!, SubmittedAt = v.SubmittedAt })
                .ToList();
        }
        else if (Election.Type == "multichoice")
        {
            var tally = VoteTally.ForOptions(Election.Options);
            foreach (var v in Election.Votes) tally.RecordMultiChoice(v.SelectedOptions);
            OptionResults = tally.GetRankedResults()
                .Select(e => new OptionResult { Text = e.Label, Count = e.Score, Percentage = e.Percentage })
                .ToList();
        }
        else if (Election.Type == "ranking")
        {
            var tally = VoteTally.ForOptions(Election.Options);
            foreach (var v in Election.Votes) tally.RecordBordaRanking(v.RankingOrder);
            RankingItemResults = tally.GetRankedResults()
                .Select(e => new RankingItemResult { Text = e.Label, Points = e.Score })
                .ToList();
        }

        return Page();
    }
}
