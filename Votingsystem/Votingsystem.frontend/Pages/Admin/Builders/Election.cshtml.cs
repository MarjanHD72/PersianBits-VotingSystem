using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Votingsystem.frontend.Pages.Admin.Builders
{
    public class ElectionModel : PageModel
    {
        [Microsoft.AspNetCore.Mvc.BindProperty] public string Title { get; set; } = "";
        [Microsoft.AspNetCore.Mvc.BindProperty] public string Description { get; set; } = "";

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