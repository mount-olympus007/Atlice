using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class TermsAndConditionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository repository;
        public TermsAndConditionsModel(IDataRepository repo, UserManager<ApplicationUser> userManager)
        {
            repository = repo;
            _userManager = userManager;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }


        public class InputModel
        {
            public bool TermsConfirmed { get; set; }
            public Guid Id { get; set; }
        }



        public async Task<IActionResult> OnGet()
        {
            ApplicationUser? u = await _userManager.GetUserAsync(User);
            if(u is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == u.Id);
                if (r == null || r.VerifyStep == false)
                {
                    return RedirectToPage("/account/onboarding-step-2");
                }
                Input.TermsConfirmed = false;
                Input.Id = u.Id;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostTermsConfirm()
        {
            if(Input is not null)
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(Input.Id.ToString());
                if (Input.TermsConfirmed == true && user is not null)
                {
                    user.TermsConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Agreed to terms", EventType.User, "TermsConfirm", false));

                    RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                    if (r != null)
                    {
                        r.Terms = true;
                        await repository.SaveRewardTracker(r);
                        return RedirectToPage("/Account/Welcome");
                    }
                    
                    
                }

                else
                {
                    TempData["message"] = "You must agree to terms and have a valid account to login.";
                    return RedirectToPage();
                }
            }
            TempData["message"] = "You must agree to terms and have a valid account to login.";
            return RedirectToPage();

        }

    }
}
