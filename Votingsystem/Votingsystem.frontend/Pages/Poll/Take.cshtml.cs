using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VotingSystem.Frontend.Pages.Poll;

public class TakeModel : PageModel
{
    /*
        TAKE POLL PAGE MODEL (Frontend only)
        -----------------------------------
       

        Right now:
        - We do not load questions from a database.
        - We do not save answers.
        - OnGet() is empty because the page is static UI.

        Later (backend work your teammates will do):
        - Load the poll by ID (from route like /Poll/Take?id=123)
        - Load questions/options from DB
        - OnPost() to receive answers and validate them
        - Save answers and redirect to /Poll/Thanks
    */

    public void OnGet()
    {
        
    }
}