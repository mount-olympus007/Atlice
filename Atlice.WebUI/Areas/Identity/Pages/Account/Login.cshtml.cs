// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wangkanai.Detection.Services;
using Atlice.Domain.Abstract;
using Atlice.WebUI.Hubs;
using AngleSharp.Dom;
using static QRCoder.PayloadGenerator;

namespace Atlice.WebUI.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServices services;
        private readonly IDataRepository _dataRepository;
        public LoginModel(IServices _services, UserManager<ApplicationUser> userManager, IDataRepository dataRepository)
        {
            _dataRepository= dataRepository;
            services = _services;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        [TempData]
        public string ErrorMessage { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Please enter your mobile number to continue.")]
            [Phone]
            public string PhoneNumber { get; set; }
            public string TapID { get; set; }
        }
        public async Task OnGetAsync(string tapid = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            if (!string.IsNullOrEmpty(tapid))
            {
                Input.TapID = tapid;
            }
        }

        public async Task<IActionResult> OnPostLogIn()
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                ApplicationUser user = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == Input.PhoneNumber);
                
                if (user == null && !string.IsNullOrEmpty(Input.TapID))
                {
                    AtliceTap T = _dataRepository.Taps.FirstOrDefault(x=>x.SNumber.StartsWith(Input.TapID));
                    Order o = _dataRepository.Orders.FirstOrDefault(x => x.Taps.Contains(T));
                    ApplicationUser orderMaker = await _userManager.FindByIdAsync(o.UserId.ToString());
                    if (T is not null && o is not null && orderMaker is not null)
                    {
                        Gift gift = _dataRepository.Gifts.FirstOrDefault(x=>x.To == user.Id && x.From == orderMaker.Id && x.GiftId == T.Id);
                        if(gift == null)
                        {
                            TempData["ERROR"] = "Device Locked by Owner";
                            return Page();
                        }
                        ApplicationUser u = new ApplicationUser
                        {
                            Email = T.SNumber + "@atlicetap.com",
                            AboutMe = "Grandfathered in",
                            FirstName = "New",
                            LastName = "User",
                            UserName = T.SNumber + "@atlicetap.com",
                            AffiliateId = orderMaker.Id,
                            PhoneNumber = Input.PhoneNumber
                        };
                        u = await services.CreateAtliceAccount(u, "Tourist");
                    }
                    var returnpage = "/identity/account/onboarding-step-2";
                    return RedirectToPage("VerifyPhoneNumber", new { phone = Input.PhoneNumber, tapid = Input.TapID, returnpage });
                }
                if(await _userManager.IsInRoleAsync(user, "Deleted"))
                {
                    TempData["ERROR"] = "Invalid login attempt.";
                    return Page();
                }
           
                var codePhone = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                await services.SendTextAsync(Input.PhoneNumber, "Your security code is: " + codePhone);
                var returnurl = Request.GetTypedHeaders().Referer.AbsoluteUri;
                if (returnurl.Contains("betaask/credentials_prospect") || returnurl.Contains("betaask/eligibility_form") || returnurl.Contains("betaask/onboarding") || returnurl.Contains("account/onboarding-step-2") || returnurl.Contains("account/verifystep") || returnurl.Contains("account/termsandconditions") || returnurl.Contains("account/deviceselect") || returnurl.Contains("account/onboarding-setup-contact-page") || returnurl.Contains("account/onboarding-step-7") || returnurl.Contains("tap/invite"))
                {
                    return RedirectToPage("VerifyPhoneNumber", new { phone = Input.PhoneNumber, tapid = Input.TapID, returnurl });
                }
                else
                {
                    return RedirectToPage("VerifyPhoneNumber", new { phone = Input.PhoneNumber, tapid = Input.TapID });
                }
                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
