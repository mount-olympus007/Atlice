using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class EmailsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
