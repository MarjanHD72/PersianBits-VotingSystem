namespace VotingSystem.Domain;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ElectionId { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public AppUser User { get; set; } = null!;
    public Election Election { get; set; } = null!;
}
