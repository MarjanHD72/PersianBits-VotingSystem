namespace VotingSystem.Domain;

public class Vote
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public int UserId { get; set; }
    public int? CandidateId { get; set; }       // for election type
    public int? RatingValue { get; set; }        // for rating type (1-5)
    public string? TextResponse { get; set; }    // for text type
    public string? SelectedOptions { get; set; } // for multichoice (comma-separated option IDs)
    public string? RankingOrder { get; set; }    // for ranking (comma-separated option IDs in order)
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public Election Election { get; set; } = null!;
    public AppUser User { get; set; } = null!;
    public Candidate? Candidate { get; set; }
}
