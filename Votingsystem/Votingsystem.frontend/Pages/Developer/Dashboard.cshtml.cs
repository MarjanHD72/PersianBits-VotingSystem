using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Developer;

[Authorize(Roles = "Developer")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    public DashboardModel(AppDbContext db) => _db = db;

    // System KPIs
    public int TotalUsers { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalDevelopers { get; set; }
    public int PendingRequests { get; set; }
    public int TotalElections { get; set; }
    public int TotalVotes { get; set; }
    public int ActiveElections { get; set; }

    // Admin request inbox
    public List<AppUser> AdminRequests { get; set; } = new();

    // All users
    public List<AppUser> AllUsers { get; set; } = new();

    public async Task OnGetAsync()
    {
        var allUsers = await _db.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();

        AllUsers        = allUsers;
        TotalUsers      = allUsers.Count;
        TotalAdmins     = allUsers.Count(u => u.Role == "Admin");
        TotalDevelopers = allUsers.Count(u => u.Role == "Developer");
        PendingRequests = allUsers.Count(u => u.RequestedAdmin && u.Role == "User");

        AdminRequests = allUsers
            .Where(u => u.RequestedAdmin && u.Role == "User")
            .OrderByDescending(u => u.CreatedAt)
            .ToList();

        TotalElections  = await _db.Elections.CountAsync();
        ActiveElections = await _db.Elections.CountAsync(e => e.Status == "running");
        TotalVotes      = await _db.Votes.CountAsync();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.Role = "Admin";
            user.RequestedAdmin = false;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDenyAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            user.RequestedAdmin = false;
            user.AdminRequestReason = "";
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSetRoleAsync(int id, string role)
    {
        if (role != "User" && role != "Admin" && role != "Developer")
            return RedirectToPage();

        var currentDevId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(id);
        if (user != null && user.Id != currentDevId)
        {
            user.Role = role;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var currentDevId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _db.Users.FindAsync(id);
        if (user != null && user.Id != currentDevId)
        {
            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
