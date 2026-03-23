namespace VotingSystem.Domain;

public class Election
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string SessionId { get; set; } = "";
    public string Type { get; set; } = "election"; // "election", "rating", "text", "multichoice", "ranking"
    public string Status { get; set; } = "draft"; // "draft", "running", "closed"
    public int CreatorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser Creator { get; set; } = null!;
    public ICollection<Candidate> Candidates { get; set; } = new List<Candidate>();
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
