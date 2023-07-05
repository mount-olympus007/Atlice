using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class onboarding_setup_contact_pageModel : PageModel
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        public onboarding_setup_contact_pageModel(IDataRepository repo, UserManager<ApplicationUser> manager)
        {
            repository = repo;
            userManager = manager;
        }
        [ViewData]
        [BindProperty]
        public string? Tapid { get; set; }
        public async Task<IActionResult> OnGet(string id)
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                if (r == null || r.Terms == false)
                {
                    return RedirectToPage("/account/Termsandconditions");
                }
                r.DeviceSelect = true;
                await repository.SaveRewardTracker(r);
                if (id != null)
                {
                    Tapid = id;

                }
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostSetupContactPage(string radio, string tapid)
        {
            if(radio == null)
            {
                TempData["message"] = "Please make a selection";
                return Page();
            }

            ApplicationUser? user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                PageType pageType = new();
                if (PageType.Business.ToString() == radio)
                {
                    pageType = PageType.Business;
                }
                if (PageType.Personal.ToString() == radio)
                {
                    pageType = PageType.Personal;
                }
                if (PageType.Professional.ToString() == radio)
                {
                    pageType = PageType.Professional;
                }
                ContactPage? p = repository.ContactPages.FirstOrDefault(x => x.UserId == user.Id && x.PageType == pageType);
                if (p is not null)
                {
                    List<AtliceTap> taps = repository.Taps.Where(x => x.UserId == user.Id).ToList();
                    foreach (var tap in taps)
                    {
                        tap.ContactPage = p;
                        tap.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        await repository.SaveTap(tap);
                    }
                    if (!string.IsNullOrEmpty(tapid))
                    {
                        AtliceTap? t = repository.Taps.FirstOrDefault(x => x.SNumber.StartsWith(tapid));
                        if (t is not null)
                        {
                            t.UserId = user.Id;
                            t.ContactPage= p;
                            await repository.SaveTap(t);
                        }

                    }
                    RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                    if (r != null && r.DeviceSelect == true)
                    {
                        r.SetupContactPage = true;
                        await repository.SaveRewardTracker(r);
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Successfully set up contact page", EventType.User, "SetUpContactPage", false));

                        return RedirectToPage("onboarding-step-7", new { pageid = p.Id });
                    }

                }
            }
         

            
            return RedirectToPage("/Identity/Account/login");




        }
    }
}
