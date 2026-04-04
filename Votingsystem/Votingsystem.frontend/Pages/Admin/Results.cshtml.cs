using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ResultsModel : PageModel
{
    private readonly AppDbContext _db;
    public ResultsModel(AppDbContext db) => _db = db;

    public Election? Election { get; set; }
    public List<CandidateResult> CandidateResults { get; set; } = new();
    public List<RatingResult> RatingResults { get; set; } = new();
    public List<TextResult> TextResponses { get; set; } = new();
    public List<OptionResult> OptionResults { get; set; } = new();
    public List<RankingItemResult> RankingItemResults { get; set; } = new();
    public int TotalVotes { get; set; }
    public double AverageRating { get; set; }

    public class CandidateResult { public string Name { get; set; } = ""; public string ImageUrl { get; set; } = ""; public int Votes { get; set; } public double Percentage { get; set; } }
    public class RatingResult    { public int Stars { get; set; } public int Count { get; set; } public double Percentage { get; set; } }
    public class TextResult      { public string UserName { get; set; } = ""; public string Text { get; set; } = ""; public DateTime SubmittedAt { get; set; } }
    public class OptionResult    { public string Text { get; set; } = ""; public int Count { get; set; } public double Percentage { get; set; } }
    public class RankingItemResult { public string Text { get; set; } = ""; public double AvgPosition { get; set; } public int Points { get; set; } }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return RedirectToPage("/Admin/Dashboard");

        Election = await _db.Elections
            .Include(e => e.Candidates).Include(e => e.Options)
            .Include(e => e.Votes).ThenInclude(v => v.User)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (Election == null) return RedirectToPage("/Admin/Dashboard");
        TotalVotes = Election.Votes.Count;

        if (Election.Type == "election")
        {
            // BEFORE: O(votes × candidates) — Count() called once per candidate
            // AFTER:  O(votes) — single pass through votes, O(1) slot lookup per vote
            var tally = VoteTally.ForCandidates(Election.Candidates);
            foreach (var v in Election.Votes)
                tally.RecordSingleVote(v.CandidateId);

            CandidateResults = tally.GetRankedResults()
                .Select(e => new CandidateResult
                {
                    Name       = e.Label,
                    ImageUrl   = e.ImageUrl,
                    Votes      = e.Score,
                    Percentage = e.Percentage
                }).ToList();
        }
        else if (Election.Type == "rating")
        {
            // BEFORE: Count() called once per star level (5 passes) + separate Average()
            // AFTER:  single pass; WeightedAverage() computed from the same tally
            var tally = VoteTally.ForRatingScale();
            foreach (var v in Election.Votes)
                tally.RecordRating(v.RatingValue);

            AverageRating = tally.WeightedAverage();

            RatingResults = tally.GetRankedResults()
                .OrderByDescending(e => e.Id)          // display 5★ first
                .Select(e => new RatingResult
                {
                    Stars      = e.Id,
                    Count      = e.Score,
                    Percentage = e.Percentage
                }).ToList();
        }
        else if (Election.Type == "text")
        {
            // Text responses are free-form — no tally needed, kept as-is
            TextResponses = Election.Votes
                .Where(v => v.TextResponse != null)
                .OrderByDescending(v => v.SubmittedAt)
                .Select(v => new TextResult
                {
                    UserName    = v.User.FullName,
                    Text        = v.TextResponse!,
                    SubmittedAt = v.SubmittedAt
                }).ToList();
        }
        else if (Election.Type == "multichoice")
        {
            // BEFORE: O(votes × options) — Count() + Split() called once per option
            // AFTER:  O(votes × avg_selections) — each vote's CSV parsed exactly once
            var tally = VoteTally.ForOptions(Election.Options);
            foreach (var v in Election.Votes)
                tally.RecordMultiChoice(v.SelectedOptions);

            OptionResults = tally.GetRankedResults()
                .Select(e => new OptionResult
                {
                    Text       = e.Label,
                    Count      = e.Score,
                    Percentage = e.Percentage
                }).ToList();
        }
        else if (Election.Type == "ranking")
        {
            // BEFORE: O(votes × options²) — nested loop: per option → per vote → indexOf
            // AFTER:  O(votes × options)  — each ballot's ranking CSV parsed exactly once;
            //         Borda points assigned to all slots in that single pass
            var tally = VoteTally.ForOptions(Election.Options);
            foreach (var v in Election.Votes)
                tally.RecordBordaRanking(v.RankingOrder);

            RankingItemResults = tally.GetRankedResults()
                .Select(e => new RankingItemResult
                {
                    Text        = e.Label,
                    Points      = e.Score,
                    AvgPosition = tally.BallotCount > 0
                                      ? Math.Round((double)e.Score / tally.BallotCount, 1)
                                      : 0
                }).ToList();
        }

        return Page();
    }
}
