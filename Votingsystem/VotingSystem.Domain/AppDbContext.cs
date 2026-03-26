using Microsoft.EntityFrameworkCore;

namespace VotingSystem.Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Election> Elections => Set<Election>();
    public DbSet<Candidate> Candidates => Set<Candidate>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<AppUser>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        mb.Entity<Election>(e =>
        {
            e.HasIndex(el => el.SessionId).IsUnique();
            e.HasOne(el => el.Creator)
             .WithMany(u => u.Elections)
             .HasForeignKey(el => el.CreatorId);
        });

        mb.Entity<Candidate>(e =>
        {
            e.HasOne(c => c.Election)
             .WithMany(el => el.Candidates)
             .HasForeignKey(c => c.ElectionId);
        });

        mb.Entity<Option>(e =>
        {
            e.HasOne(o => o.Election)
             .WithMany(el => el.Options)
             .HasForeignKey(o => o.ElectionId);
        });

        mb.Entity<Vote>(e =>
        {
            e.HasOne(v => v.Election)
             .WithMany(el => el.Votes)
             .HasForeignKey(v => v.ElectionId);

            e.HasOne(v => v.User)
             .WithMany(u => u.Votes)
             .HasForeignKey(v => v.UserId);

            e.HasOne(v => v.Candidate)
             .WithMany(c => c.Votes)
             .HasForeignKey(v => v.CandidateId)
             .IsRequired(false);
        });

        // Seed a default admin
        mb.Entity<AppUser>().HasData(new AppUser
        {
            Id = 1,
            FullName = "Admin",
            Email = "admin@persianbits.com",
            PasswordHash = BCryptHelper.Hash("admin123"),
            DateOfBirth = new DateTime(2000, 1, 1),
            Role = "Admin",
            Avatar = "🔥",
            IsActive = true,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
