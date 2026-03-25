namespace VotingSystem.Domain;

public class AppUser
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; } = "User"; // "Admin" or "User"
    public string Avatar { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public bool RequestedAdmin { get; set; } = false;
    public string AdminRequestReason { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Election> Elections { get; set; } = new List<Election>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
