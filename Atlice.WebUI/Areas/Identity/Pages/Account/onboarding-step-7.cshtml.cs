using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class onboarding_step_7Model : PageModel
    {
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServices _services;
        public onboarding_step_7Model(IDataRepository repo, UserManager<ApplicationUser> userManager, IServices services)
        {
            repository = repo;
            _userManager = userManager;
            _services = services;
        }
        [ViewData]
        [BindProperty]
        public ContactPage ContactPage { get; set; } = new ContactPage();
        public async Task<IActionResult> OnGet(Guid? pageid)
        {
            ApplicationUser? u = await _userManager.GetUserAsync(User);
            if(u is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == u.Id);
                if (r == null || r.SetupContactPage == false)
                {
                    return RedirectToPage("/account/onboarding-step-2");
                }
                ContactPage? c = repository.ContactPages.FirstOrDefault(x => x.Id == pageid);
                if (c != null)
                {
                    ContactPage = c;
                    return Page();
                }
                else
                {
                    var cp = repository.ContactPages.FirstOrDefault(x => x.UserId == u.Id);
                    if(cp is not null)
                    {
                        ContactPage = cp;
                    }
                    
                }
            }
            
            return RedirectToPage("/Identity/Account/login");
            
        }
        public async Task<IActionResult> OnPostSetupContactPage()
        {
            if(ContactPage is not null)
            {
                ContactPage? p = repository.ContactPages.FirstOrDefault(x => x.Id == ContactPage.Id);
                ApplicationUser? u = await _userManager.GetUserAsync(User);
                if(p is not null && u is not null && u.PhoneNumber is not null)
                {
                    AtliceTap? t = repository.Taps.FirstOrDefault(x => x.UserId == p.UserId);
                    if (t is not null)
                    {
                        p.Name = ContactPage.Name;
                        p.BusinessName = ContactPage.BusinessName;
                        p.ProfileLead = ContactPage.ProfileLead;
                        p.PhoneNumber = ContactPage.PhoneNumber;
                        p.Email = ContactPage.Email;
                        p.Website = ContactPage.Website;
                        await repository.SaveContactPage(p);
                        t.ContactPage = p;
                        await repository.SaveTap(t);
                        RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == p.UserId);
                        if (r != null)
                        {
                            r.OnboardingStep7 = true;
                            await repository.SaveRewardTracker(r);

                            await _services.SendTextAsync(u.PhoneNumber, "Thank you for completing the Atlice Tap V3 enrollment and onboarding. If you ordered devices, you can add them to your account by activating them when they arrive. Your tracking information is in your shipping confirmation email." + "\n\n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Completed Enrollment", EventType.User, "TapIn", false));

                            return RedirectToPage("/Homeboard/Index");
                        }
                        
                    }
                }
                
            }
            
            return RedirectToPage("/Identity/Account/login");
        }
    }
}
