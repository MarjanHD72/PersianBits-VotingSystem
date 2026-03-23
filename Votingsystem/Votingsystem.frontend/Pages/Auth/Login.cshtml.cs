using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    public LoginModel(AppDbContext db) => _db = db;

    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? Error { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(string role = "User")
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            Error = "Please enter email and password.";
            return Page();
        }

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == Email.Trim().ToLower());
        if (user == null || !BCryptHelper.Verify(Password, user.PasswordHash))
        {
            Error = "Invalid email or password.";
            return Page();
        }

        if (!user.IsActive)
        {
            Error = "Your account has been disabled.";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("Avatar", user.Avatar)
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(identity));

        return Redirect(user.Role == "Admin" ? "/Admin/Dashboard" : "/User/Dashboard");
    }
}