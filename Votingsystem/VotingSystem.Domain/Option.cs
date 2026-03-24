namespace VotingSystem.Domain;

public class Option
{
    public int Id { get; set; }
    public int ElectionId { get; set; }
    public string Text { get; set; } = "";

    public Election Election { get; set; } = null!;
}
