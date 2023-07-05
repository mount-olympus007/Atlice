using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class OutstandingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository _dataRepository;
        private readonly IServices _services;
        public OutstandingModel(UserManager<ApplicationUser> userManager, IDataRepository dataRepository, IServices services)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _services = services;
        }
        [ViewData]
        public List<RewardTrackModel> TrackModels { get; set; } = new List<RewardTrackModel>();

        public class RewardTrackModel
        {
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public RewardTracker Tracker { get; set; } = new RewardTracker();
        }
        public async Task OnGet()
        {
            foreach (var user in _userManager.Users.ToList())
            {

                if (await _userManager.IsInRoleAsync(user, "Adminis") || _dataRepository.RewardsTrackers.Any(x => x.UserId == user.Id && !x.SetupContactPage))
                {
                    RewardTracker? tracker = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                    if (user is not null && tracker is not null)
                    {
                        TrackModels.Add(new RewardTrackModel { User = user, Tracker = tracker });

                    }
                }
            }
        }

        public async Task<IActionResult> OnGetSendText(string textType, string userid)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userid);
            if (user is not null && user.PhoneNumber is not null)
            {
                switch (textType)
                {
                    case "Credentials":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/betaask/credentials_prospect'>here</a> to verify your credentials and start your onboarding process");
                        return new JsonResult(new { status = "Success" });
                    case "EligibilityForm":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/betaask/eligibility_form'>here</a> to complete your eligibility form to potentially unlock premium features.");
                        return new JsonResult(new { status = "Success" });
                    case "PlacedOrder":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/betaask/onboarding'>here</a> to place an order for new devices.");
                        return new JsonResult(new { status = "Success" });
                    case "OnboardingStep2":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/onboarding-step-2?email=" + user.Email + "'>here</a> to finish registering your account.");
                        return new JsonResult(new { status = "Success" });
                    case "VerifyStep":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/verifystep?phone=" + user.PhoneNumber + "'>here</a> to verify your mobile device.");
                        return new JsonResult(new { status = "Success" });
                    case "Terms":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/termsandconditions'>here</a> to agree to our terms and conditions.");
                        return new JsonResult(new { status = "Success" });
                    case "DeviceSelect":
                        await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/deviceselect'>here</a> to accept your account license and set up your devices.");
                        return new JsonResult(new { status = "Success" });
                    case "SetupContactPage":
                        AtliceTap? vir = _dataRepository.Taps.FirstOrDefault(x => x.UserId == user.Id && x.TapType == TapType.Virtual);
                        if (vir is not null)
                        {
                            await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/onboarding-setup-contact-page?id=" + vir.SNumber[..8] + "'>here</a> to link your devices to your contact pages.");
                            return new JsonResult(new { status = "Success" });
                        }
                        break;
                    case "OnboardingStep7":
                        ContactPage? c = _dataRepository.ContactPages.FirstOrDefault(x => x.UserId == user.Id && x.PageType == PageType.Personal);
                        if (c is not null)
                        {
                            await _services.SendTextAsync(user.PhoneNumber, "Hey " + user.FirstName + "! Click <a href='https://atlice.com/identity/account/onboarding-step-7?pageid=" + c.Id + "'>here</a> to finish setting up your default contact page.");
                            return new JsonResult(new { status = "Success" });
                        }
                        break;
                }
            }

            return new JsonResult(new { status = "Failed to Send Text" });

        }

    }
}
