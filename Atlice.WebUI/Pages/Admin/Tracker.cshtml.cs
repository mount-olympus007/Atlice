using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class TrackerModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public TrackerModel(IDataRepository repo, IServices _services, UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> _signInManager)
        {
            userManager = manager;
            signInManager = _signInManager;
            services = _services;
            repository = repo;
        }

        [ViewData]
        public int CurrentPage { get; set; }
        [ViewData]
        public int NextPage { get; set; }
        [ViewData]
        public int LastPage { get; set; }
        [ViewData]
        public int PageSize { get; set; } = 100;



        [ViewData]
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        [ViewData]
        public int Count { get; set; }

        public class UserModel
        {
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public string? Role { get; set; }
            public int Pages { get; set; }
            public int Taps { get; set; }
            public int Orders { get; set; }
            public List<AdminNote>? Notes { get; set; }
            public RewardTracker? RewardTracker { get; set; }
        }
        public async Task<IActionResult> OnGet(string filter, int pageNo = 0)
        {

            CurrentPage = pageNo;
            NextPage = pageNo + 1;
            LastPage = pageNo - 1;
            if (pageNo == 0)
            {
                LastPage = 0;
            }
            if (filter != null)
            {
                foreach (ApplicationUser user in repository.Users.Skip(PageSize).Take(PageSize).ToList())
                {
                    if (await userManager.IsInRoleAsync(user, filter))
                    {
                        RewardTracker? t = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                        if (t is not null)
                        {
                            var x = new UserModel
                            {
                                User = user,
                                Role = filter,
                                Orders = repository.Orders.Where(x => x.UserId == user.Id).Count(),
                                Taps = repository.Taps.Where(x => x.UserId == user.Id).Count(),
                                Pages = repository.ContactPages.Where(x => x.UserId == user.Id).Count(),
                                Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).OrderBy(x => x.When).Take(3).ToList(),
                                RewardTracker = t
                            };
                            Users.Add(x);
                        }

                    }

                }


                return Page();
            }
            else
            {
                foreach (ApplicationUser user in repository.Users.Skip(CurrentPage * PageSize).Take(PageSize).ToList())
                {
                    var roles = await userManager.GetRolesAsync(user);
                    RewardTracker? t = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                    var x = new UserModel
                    {
                        User = user,
                        RewardTracker = t
                    };
                    if (roles.Count > 0)
                    {
                        x.Role = roles.FirstOrDefault();
                    }
                    Users.Add(x);
                }


                return Page();
            }

        }

        public async Task<IActionResult> OnPostFindUser(string name, string phone)
        {
            List<ApplicationUser> users = new();

            foreach (var user in repository.Users.ToList())
            {
                if (name != null)
                {
                    if (user.FirstName != null && user.LastName != null)
                    {
                        if (user.FirstName.ToLower().Contains(name.ToLower()) || user.LastName.ToLower().Contains(name.ToLower()))
                        {

                            users.Add(user);
                        }
                    }
                }
                if (phone != null)
                {
                    if (!string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        if (user.PhoneNumber.Contains(phone) && !users.Contains(user))
                        {
                            users.Add(user);
                        }
                    }

                }


            }

            var resultUsers = new List<UserModel>();
            foreach (ApplicationUser user in users)
            {
                var x = new UserModel
                {
                    User = user,
                };
                resultUsers.Add(x);
            }
            string view = await services.RenderToString("/pages/admin/UserList.cshtml", resultUsers);
            return Content(view);
        }

        public async Task<IActionResult> OnGetWipeUser(string userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid);
            if (user is not null)
            {
                foreach (var contact in repository.Contacts.Where(x => x.Phone == user.PhoneNumber).ToList())
                {
                    await repository.DeleteContact(contact);
                }
                await userManager.DeleteAsync(user);
            }

            return RedirectToPage("Users");
        }

        public async Task<IActionResult> OnGetStartBeta()
        {
            await signInManager.SignOutAsync();
            return RedirectToPage("/Betaask/credentials_prospect");
        }

        public async Task<IActionResult> OnGetSendCredText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to verify your credentials. https://atlice.com/betaask/credential_prospect" + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetVerifyCred(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.Credentials = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendEdgeText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to fill out your eligibility form. https://atlice.com/betaask/eligibility_form" + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetVerifyEdgeCity(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.EligibilityForm = true;
                    await userManager.AddToRoleAsync(user, "Citizen");
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetVerifyEdgeTour(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.EligibilityForm = true;
                    await userManager.AddToRoleAsync(user, "Tourist");
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendRegText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to complete Atlice Tap Registration. https://atlice.com/identity/account/onboarding-step-2?email=" + user.Email + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetReg(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.OnboardingStep2 = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetMoveThemForward(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.VerifyStep = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendTermsText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to accept the Terms and Conditions of Atlice Tap. https://atlice.com/identity/account/termsandconditions" + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetTerms(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.Terms = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendDeviceText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to setup your Atlice Tap device. https://atlice.com/identity/account/deviceselect" + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSetDevice(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);
                if (r != null)
                {
                    r.DeviceSelect = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendAttachPageText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to setup your Atlice Tap device. https://atlice.com/identity/account/onboarding-setup-contact-page" + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSetPage(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                AtliceTap? t = repository.Taps.FirstOrDefault(x => x.UserId == userid);
                ContactPage? cp = repository.ContactPages.FirstOrDefault(x => x.UserId == userid);
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);

                if (cp != null && t is not null && r is not null)
                {
                    t.ContactPage = cp;
                    await repository.SaveTap(t);
                    r.SetupContactPage = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }

                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSendPageSetupText(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null && user.PhoneNumber is not null)
            {
                ContactPage? p = repository.ContactPages.FirstOrDefault(x => x.UserId == userid);
                if (p is not null)
                {
                    await services.SendTextAsync(user.PhoneNumber, "Welcome to Atlice Tap V3 beta. Use this link to setup your first Atlice Tap Contact Page. https://atlice.com/identity/account/onboarding-step-7?pageid=" + p.Id + " \n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");
                    return new JsonResult(new { status = "Success" });
                }
                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }
        public async Task<IActionResult> OnGetSetupPage(Guid userid)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userid.ToString());
            if (user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == userid);

                if (r is not null)
                {
                    r.OnboardingStep7 = true;
                    await repository.SaveRewardTracker(r);
                    return new JsonResult(new { status = "Success" });
                }

                return new JsonResult(new { status = "Failed" });
            }
            return new JsonResult(new { status = "Failed" });
        }

    }
}

