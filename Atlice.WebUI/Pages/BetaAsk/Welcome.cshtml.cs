using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    public class WelcomeModel : PageModel
    {
        

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
