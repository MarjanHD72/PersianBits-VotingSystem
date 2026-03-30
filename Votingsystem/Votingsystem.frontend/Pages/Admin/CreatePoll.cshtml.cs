using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Votingsystem.frontend.Pages.Admin;

[Authorize(Roles = "Admin")]
public class CreatePollModel : PageModel
{
    public void OnGet() { }
}
