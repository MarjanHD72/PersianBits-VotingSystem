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
    public class RatingResult { public int Stars { get; set; } public int Count { get; set; } public double Percentage { get; set; } }
    public class TextResult { public string UserName { get; set; } = ""; public string Text { get; set; } = ""; public DateTime SubmittedAt { get; set; } }
    public class OptionResult { public string Text { get; set; } = ""; public int Count { get; set; } public double Percentage { get; set; } }
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
            CandidateResults = Election.Candidates
                .Select(c => new CandidateResult
                {
                    Name = c.Name,
                    ImageUrl = c.ImageUrl ?? "",
                    Votes = Election.Votes.Count(v => v.CandidateId == c.Id),
                    Percentage = TotalVotes > 0 ? Math.Round(Election.Votes.Count(v => v.CandidateId == c.Id) * 100.0 / TotalVotes, 1) : 0
                }).OrderByDescending(c => c.Votes).ToList();
        }
        else if (Election.Type == "rating")
        {
            for (int i = 5; i >= 1; i--)
            {
                var count = Election.Votes.Count(v => v.RatingValue == i);
                RatingResults.Add(new RatingResult
                {
                    Stars = i,
                    Count = count,
                    Percentage = TotalVotes > 0 ? Math.Round(count * 100.0 / TotalVotes, 1) : 0
                });
            }
            AverageRating = TotalVotes > 0 ? Math.Round(Election.Votes.Where(v => v.RatingValue.HasValue).Average(v => v.RatingValue!.Value), 1) : 0;
        }
        else if (Election.Type == "text")
        {
            TextResponses = Election.Votes.Where(v => v.TextResponse != null)
                .OrderByDescending(v => v.SubmittedAt)
                .Select(v => new TextResult { UserName = v.User.FullName, Text = v.TextResponse!, SubmittedAt = v.SubmittedAt }).ToList();
        }
        else if (Election.Type == "multichoice")
        {
            foreach (var opt in Election.Options)
            {
                var count = Election.Votes.Count(v => v.SelectedOptions != null && v.SelectedOptions.Split(',').Contains(opt.Id.ToString()));
                OptionResults.Add(new OptionResult
                {
                    Text = opt.Text,
                    Count = count,
                    Percentage = TotalVotes > 0 ? Math.Round(count * 100.0 / TotalVotes, 1) : 0
                });
            }
            OptionResults = OptionResults.OrderByDescending(o => o.Count).ToList();
        }
        else if (Election.Type == "ranking")
        {
            var optCount = Election.Options.Count;
            foreach (var opt in Election.Options)
            {
                int totalPoints = 0; int appearances = 0;
                foreach (var v in Election.Votes.Where(v => !string.IsNullOrEmpty(v.RankingOrder)))
                {
                    var order = v.RankingOrder!.Split(',');
                    var idx = Array.IndexOf(order, opt.Id.ToString());
                    if (idx >= 0) { totalPoints += (optCount - idx); appearances++; }
                }
                RankingItemResults.Add(new RankingItemResult
                {
                    Text = opt.Text,
                    Points = totalPoints,
                    AvgPosition = appearances > 0 ? Math.Round((double)totalPoints / appearances, 1) : 0
                });
            }
            RankingItemResults = RankingItemResults.OrderByDescending(r => r.Points).ToList();
        }

        return Page();
    }
}
