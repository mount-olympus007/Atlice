using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    public class MyPinModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MyPinModel(IDataRepository dataRepository, SignInManager<ApplicationUser> signInManager)
        {
            repository = dataRepository;
            _signInManager = signInManager;
        }
        [BindProperty]
        public string? Id { get; set; }
        [BindProperty]
        public string? Pin { get; set; }
        public string? TapID { get; set; }
        public void OnGet(string userid, string tapid)
        {
            Id = userid;
            if(tapid != null)
            {
                TapID = tapid;
            }
        }

        public async Task<IActionResult> OnPost(string id, int pin, string tapID)
        {
            ApplicationUser? user = repository.Users.FirstOrDefault(x => x.Id.ToString() == id);
            if(user is not null)
            {
                if (user.Secret == pin.ToString())
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    if (tapID != null)
                    {
                        return RedirectToPage("/account/deviceselect", new { tapid = tapID });

                    }
                    return RedirectToPage("/Homeboard/Index");
                }
                else
                {
                    return Page();
                }
            }
            return Page();
        }

    }
}
