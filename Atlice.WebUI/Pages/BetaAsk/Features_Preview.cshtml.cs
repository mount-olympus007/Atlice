using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class Features_PreviewModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
