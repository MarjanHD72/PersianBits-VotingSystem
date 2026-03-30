namespace VotingSystem.Domain;

public class Candidate
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string ImageUrl { get; set; } = "";

    public Election Election { get; set; } = null!;
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
