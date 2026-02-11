using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VotingSystem.Frontend.Pages.Admin.Builders
{
    public class ElectionModel : PageModel
    {
        public void OnGet()
        {
            // Frontend-only prototype.
            // Later backend will:
            // - Validate user is approved creator/admin
            // - Save election + candidates to DB
            // - Generate real session ID + share link
        }
    }
}