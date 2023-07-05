using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis,Prospect")]
    public class Eligibility_FormModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public Eligibility_FormModel(UserManager<ApplicationUser> manager, IDataRepository dataRepository, SignInManager<ApplicationUser> signInManager)
        {
            userManager = manager;
            repository = dataRepository;
            _signInManager = signInManager;
        }

        [ViewData]
        public YouLoveProfile YouLoveProfile { get; set; } = new YouLoveProfile { Id = Guid.NewGuid(), StateofMind = "Relaxed", SocialMediaUse = "Personal" };
        public async Task< IActionResult> OnGet()
        {
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if(user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                if (r == null || r.Credentials == false)
                {
                    return RedirectToPage("/BetaAsk/credentials_prospect");
                }
            }
            
            return Page();
        }

        

        public async Task<IActionResult> OnPostEligibility(string StateofMind,
            bool Entrepreneur, bool Creator, bool BrandOwner, bool TeamLeader, bool IndependentContractor, bool Employee, bool Cards,
            bool Payments, bool Paypal, bool Cashapp, bool Venmo, bool Stripe, bool Apple, bool Google, bool Other, bool SocialMedia,
            string SocialMediaUse, bool BusinessPage, bool Zoom, bool ContentCreator, bool Write, bool Video, bool Photographer,
            bool Design, bool Artwork, bool Music, bool Otherjob, bool ChainAssets, bool StoreAssets)
        {
        
            ApplicationUser? user = await userManager.GetUserAsync(User);
            if (user is not null)
            {
                YouLoveProfile? y = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                if (y != null)
                {
                    y = await repository.SaveYouLoveProfile(new YouLoveProfile { Id = Guid.NewGuid(), StateofMind = "Relaxed", SocialMediaUse = "Personal" });
                    user.YouLoveProfileId = y.Id;
                    StateofMind ??= "";
                    y.StateofMind = StateofMind;
                    y.Entrepreneur = Entrepreneur;
                    y.Creator = Creator;
                    y.BrandOwner = BrandOwner;
                    y.TeamLeader = TeamLeader;
                    y.IndependentContractor = IndependentContractor;
                    y.Employee = Employee;
                    y.Cards = Cards;
                    y.Payments = Payments;
                    y.Paypal = Paypal;
                    y.Cashapp = Cashapp;
                    y.Venmo = Venmo;
                    y.Stripe = Stripe;
                    y.Apple = Apple;
                    y.Google = Google;
                    y.Other = Other;
                    y.SocialMedia = SocialMedia;
                    SocialMediaUse ??= "";
                    y.SocialMediaUse = SocialMediaUse;
                    y.BusinessPage = BusinessPage;
                    y.Zoom = Zoom;
                    y.ContentCreator = ContentCreator;
                    y.Write = Write;
                    y.Video = Video;
                    y.Photographer = Photographer;
                    y.Design = Design;
                    y.Artwork = Artwork;
                    y.Music = Music;
                    y.Other = Otherjob;
                    y.ChainAssets = ChainAssets;
                    y.StoreAssets = StoreAssets;
                    await repository.SaveYouLoveProfile(y);
                    var roles = await userManager.GetRolesAsync(user);
                    if (!await userManager.IsInRoleAsync(user, "Adminis"))
                    {

                        if (y.BusinessPage || y.Zoom || y.ContentCreator || y.SocialMedia && y.SocialMediaUse == "All" || y.SocialMedia && y.SocialMediaUse == "Business")
                        {
                            await userManager.RemoveFromRolesAsync(user, roles);
                            await userManager.AddToRoleAsync(user, "Citizen");
                            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Completed Eligibility form as Citizen", EventType.User, "EligibilityForm", false));

                        }
                        else
                        {
                            await userManager.RemoveFromRolesAsync(user, roles);

                            await userManager.AddToRoleAsync(user, "Tourist");
                            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Completed Eligibility form as Tourist", EventType.User, "EligibilityForm", false));

                        }
                    }
                    await userManager.UpdateAsync(user);
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(user, true);

                    RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                    if (r != null)
                    {
                        r.EligibilityForm = true;
                        await repository.SaveRewardTracker(r);
                        return RedirectToPage("/BetaAsk/features_preview");
                    }
                }

            }
           
            return RedirectToPage("/BetaAsk/credentials_prospect");
        }
    }
}
