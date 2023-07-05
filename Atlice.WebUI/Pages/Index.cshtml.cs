using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDataRepository _dataRepository;
        public IndexModel(IDataRepository dataRepository, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var us = _userManager.Users.FirstOrDefault();
            await _signInManager.SignInAsync(us, true);
            if (User.Identity == null)
            {
                return Page();
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("Adminis"))
                    {
                        return RedirectToPage("/admin/index");

                    }
                    ApplicationUser? u = _dataRepository.Users.FirstOrDefault(x=>x.UserName == User.Identity.Name);
                    
                    if (u != null)
                    {
                        RewardTracker? t = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == u.Id);
                        if(t is not null)
                        {
                            if (!t.EligibilityForm)
                            {
                                return RedirectToPage("/BetaAsk/Eligibility_Form");
                            }

                            if (!t.OnboardingStep2)
                            {
                                return RedirectToPage("/account/onboarding-step-2", new { area = "Identity", email = u.Email });

                            }
                            if (!t.Terms)
                            {
                                return RedirectToPage("/account/termsandconditions", new { area = "Identity", email = u.Email });
                            }
                            if (!t.DeviceSelect)
                            {
                                return RedirectToPage("/account/deviceselect", new { area = "Identity" });
                            }
                            if (!t.OnboardingStep7)
                            {
                                string ul = "/identity/account/onboarding-setup-contact-page";
                                return new JsonResult(new { url = ul });
                            }
                            return RedirectToPage("/homeboard/index");
                        }
                        
                    }
                    
                }
            }
            return Page();

        }
    }
}