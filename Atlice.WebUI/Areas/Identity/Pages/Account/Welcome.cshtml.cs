using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class WelcomeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository repository;
        public WelcomeModel(IDataRepository repo, UserManager<ApplicationUser> userManager)
        {
            repository = repo;
            _userManager = userManager;

        }
        [ViewData]
        public string? TapID { get; set; }
        public async Task <IActionResult> OnGet()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                AtliceTap? tap = repository.Taps.FirstOrDefault(x => x.UserId == user.Id && x.TapType == TapType.Virtual);
                if (tap != null)
                {
                    TapID = tap.SNumber[..8];
                    return Page();
                }
            }
            
            return RedirectToPage("/Account/Login");
            
        }

        
    }
}
