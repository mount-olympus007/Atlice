using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class Product_featuresModel : PageModel
    {
        public IActionResult OnGet()
        {
            return Page();  
        }
    }
}
