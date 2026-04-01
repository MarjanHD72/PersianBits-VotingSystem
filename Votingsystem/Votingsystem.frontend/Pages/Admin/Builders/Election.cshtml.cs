using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VotingSystem.Domain;

namespace Votingsystem.frontend.Pages.Admin.Builders;

[Authorize(Roles = "Admin")]
public class ElectionModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    public ElectionModel(AppDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    [BindProperty] public string Title { get; set; } = "";
    [BindProperty] public string Description { get; set; } = "";
    [BindProperty] public List<CandidateInput> CandidateList { get; set; } = new();

    public class CandidateInput
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            ModelState.AddModelError("", "Title is required.");
            return Page();
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var election = new Election
        {
            Title = Title.Trim(),
            Description = Description?.Trim() ?? "",
            SessionId = SessionIdGenerator.Generate(),
            Type = "election",
            Status = "draft",
            CreatorId = userId
        };

        var validCandidates = (CandidateList ?? new())
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .ToList();

        // Handle file uploads
        var files = Request.Form.Files;
        for (int i = 0; i < validCandidates.Count; i++)
        {
            var c = validCandidates[i];
            var imageUrl = "";

            // Look for a file named CandidateImage_N
            var file = files[$"CandidateImage_{i}"];
            if (file != null && file.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsDir);
                var ext = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsDir, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                imageUrl = $"/uploads/{fileName}";
            }

            election.Candidates.Add(new Candidate
            {
                Name = c.Name.Trim(),
                Description = c.Description?.Trim() ?? "",
                ImageUrl = imageUrl
            });
        }

        _db.Elections.Add(election);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Admin/Dashboard");
    }
}
