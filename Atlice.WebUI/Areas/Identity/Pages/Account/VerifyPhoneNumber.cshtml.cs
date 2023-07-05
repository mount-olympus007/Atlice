using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Atlice.WebUI.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace Atlice.Areas.Identity.Pages.Account
{
    public class VerifyPhoneNumberModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDataRepository _dataRepository;
        private readonly IConfiguration _config;

        public VerifyPhoneNumberModel(SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            IDataRepository dataRepository,
           UserManager<ApplicationUser> userManager)
        {
            _config = config;
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
            [Display(Name = "Code")]
            public string? Code { get; set; }
            public string? Tapid { get; set; }
            public string? ReturnUrl { get; set; }

        }
        public void OnGet(string phone, string? tapid = null, string? returnurl = null)
        {
            Input = new InputModel
            {
                PhoneNumber = phone,
                Tapid = tapid,
                ReturnUrl = returnurl
            };
            var tid = TempData["tapid"];
            if (tid is not null && !string.IsNullOrEmpty(tid.ToString()))
            {
                Input.Tapid = tid.ToString();
            }

        }

        public async Task<IActionResult> OnGetSubmit(string PhoneNumber, string Code, string? tapid = null, string? returnurl = null)
        {
            ApplicationUser? u = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == PhoneNumber);
            if (u == null)
            {
                TempData["ERROR"] = "Invalid login attempt.";
                return Page();
            }
            var result = await _userManager.ChangePhoneNumberAsync(u, PhoneNumber, Code);
            if (result.Succeeded)
            {
                if (u.TwoFactorEnabled)
                {
                    string url = "/identity/Account/MyPin?userid=" + u.Id.ToString() + "&tapid=" + tapid;
                    return new JsonResult(new { url });
                }
                else
                {


                    await _signInManager.SignInAsync(u, isPersistent: true);

                    RewardTracker? t = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == u.Id);
                    if (t is not null)
                    {
                        if(!string.IsNullOrEmpty(tapid))
                        {
                            TempData["tapid"] = tapid;
                        }
                        if (!t.EligibilityForm)
                        {
                            string ul = "/BetaAsk/Eligibility_Form";
                            return new JsonResult(new { url = ul });
                        }
                        if (!t.OnboardingStep2)
                        {
                            string ul = "/identity/account/onboarding_step_2?email=" + u.Email;
                            return new JsonResult(new { url = ul });
                        }
                        if (!t.Terms)
                        {
                            string ul = "/identity/account/termsandconditions?email=" + u.Email;
                            return new JsonResult(new { url = ul });
                        }
                        if (!t.DeviceSelect)
                        {
                            if (!string.IsNullOrEmpty(tapid))
                            {
                                if (tapid == "0")
                                {
                                    Order? o = _dataRepository.Orders.OrderByDescending(x => x.OrderShipped).FirstOrDefault(x => x.UserId == u.Id && x.Status == OrderStatus.Shipped);
                                    if (o is not null)
                                    {
                                        if (o.Taps is not null)
                                        {
                                            tapid = o.Taps.First().SNumber[..8];
                                        }

                                    }
                                    string ul = "/identity/account/deviceselect?tapid=" + tapid;
                                    return new JsonResult(new { url = ul });
                                }
                                else
                                {
                                    string ul = "/identity/account/deviceselect?tapid=" + tapid;
                                    return new JsonResult(new { url = ul });
                                }
                            }
                            
                        }
                        if (!t.OnboardingStep7)
                        {
                            string ul = "/identity/account/onboarding-setup-contact-page";
                            return new JsonResult(new { url = ul });
                        }
                        if (!string.IsNullOrEmpty(tapid))
                        {
                            AtliceTap? alreadyActivatedDevice = _dataRepository.Taps.FirstOrDefault(x => x.SNumber.StartsWith(tapid) && x.UserId is not null);
                            if (alreadyActivatedDevice != null)
                            {
                                AtliceTap? virtualDev = _dataRepository.Taps.FirstOrDefault(x => x.UserId == t.UserId);
                                if (virtualDev != null)
                                {
                                    return new JsonResult(new { url = "/identity/account/deviceselect?tapid=" + virtualDev.SNumber[..8] });
                                }
                                
                            }
                            Order? o = _dataRepository.Orders.OrderByDescending(x => x.OrderShipped).FirstOrDefault(x => x.UserId == u.Id && x.Status == OrderStatus.Shipped);
                            if (o is not null)
                            {
                                if (o.Taps is not null)
                                {
                                    tapid = o.Taps.First().SNumber[..8];
                                }

                            }
                            string ul = "/identity/account/deviceselect?tapid=" + tapid;
                            return new JsonResult(new { url = ul });
                        }

                        if (!string.IsNullOrEmpty(returnurl))
                        {
                            if (returnurl.Contains("/tap/invite"))
                            {
                                return LocalRedirect(returnurl);
                            }
                        }
                        string url = "/homeboard/index";
                        return new JsonResult(new { url });
                    }
                }
            }
            TempData["ERROR"] = "Failed to verify phone";
            return Page();
        }

    }
}
