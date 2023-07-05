using System.ComponentModel.DataAnnotations;
using Atlice.WebUI.Areas.Identity.Pages.Account;
using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    public class VerifyPhoneNumberModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDataRepository _dataRepository;

        public VerifyPhoneNumberModel(SignInManager<ApplicationUser> signInManager,
            IDataRepository dataRepository,
           UserManager<ApplicationUser> userManager)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            [Required]
            [Phone]
            public string? PhoneNumber { get; set; }
            [Required]
            public string? Email { get; set; }
            [Required]
            [Display(Name = "Code")]
            public string? Code { get; set; }

        }
        public IActionResult OnGet(string phone, string email)
        {
            Input = new InputModel
            {
                PhoneNumber = phone,
                Email = email
            };
            if (string.IsNullOrEmpty(email))
            {
                Input.Email = "";
            }
            return Page();
        }

        public async Task<IActionResult> OnGetSubmit (string PhoneNumber, string Code)
        {
            ApplicationUser? u = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == PhoneNumber);
            if (u == null)
            {
                ModelState.AddModelError("", "Failed to verify phone");
                return new JsonResult(new { error = "Failed to verify phone" });
            }
           
            var result = await _userManager.ChangePhoneNumberAsync(u, PhoneNumber, Code);
            if (result.Succeeded)
            {
                await _userManager.UpdateAsync(u);
                await _signInManager.SignInAsync(u, isPersistent: true);
                await _dataRepository.SaveEvent(new Event(u.Email, u.Email + " Prospect found with id " + u.Id, EventType.Anonymous, "VerifyPhoneBetaAsk", false));

                RewardTracker? r = _dataRepository.RewardsTrackers.FirstOrDefault(x=>x.UserId == u.Id);
                if(r != null)
                {
                    r.Credentials = true;
                    await _dataRepository.SaveRewardTracker(r);
                    string url = "/BetaAsk/eligibility_form";
                    return new JsonResult(new { url = url });
                }

                ModelState.AddModelError("", "Failed to verify phone");
                return new JsonResult(new { error = "Failed to verify phone" });
            }
            ModelState.AddModelError("", "Failed to verify phone");
            return new JsonResult(new { error = "Failed to verify phone" });
        }

    }
}
