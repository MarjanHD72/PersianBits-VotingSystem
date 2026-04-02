using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Auth.Register;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly EmailService _email;

    public IndexModel(AppDbContext db, EmailService email)
    {
        _db = db;
        _email = email;
    }

    [BindProperty] public string FullName { get; set; } = "";
    [BindProperty] public string DateOfBirth { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    [BindProperty] public string Password2 { get; set; } = "";
    [BindProperty] public bool RequestAdmin { get; set; }
    [BindProperty] public string AdminReason { get; set; } = "";

    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName))
        { Error = "Full name is required."; return Page(); }

        if (!DateTime.TryParse(DateOfBirth, out var dob))
        { Error = "Please enter a valid date of birth."; return Page(); }

        if ((DateTime.Today - dob).TotalDays < 365 * 18)
        { Error = "You must be at least 18 years old to register."; return Page(); }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        { Error = "Please enter a valid email address."; return Page(); }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8)
        { Error = "Password must be at least 8 characters."; return Page(); }

        if (Password != Password2)
        { Error = "Passwords do not match."; return Page(); }

        var normalizedEmail = Email.Trim().ToLower();
        if (await _db.Users.AnyAsync(u => u.Email == normalizedEmail))
        { Error = "An account with this email already exists."; return Page(); }

        var user = new AppUser
        {
            FullName = FullName.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCryptHelper.Hash(Password),
            DateOfBirth = dob,
            Role = "User",
            Avatar = "😀",
            IsActive = true,
            RequestedAdmin = RequestAdmin,
            AdminRequestReason = RequestAdmin ? AdminReason?.Trim() ?? "" : ""
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (RequestAdmin)
        {
            await _email.SendAdminRequestAsync(
                user.FullName,
                user.Email,
                dob.ToString("yyyy-MM-dd"),
                user.AdminRequestReason
            );
        }

        return RedirectToPage("/Auth/Login", new { registered = true });
    }
}
