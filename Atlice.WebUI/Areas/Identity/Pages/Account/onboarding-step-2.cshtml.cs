using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class onboarding_step_2Model : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository repository;
        private readonly IServices services;
        public onboarding_step_2Model(IServices _services, UserManager<ApplicationUser> userManager, IEmailSender emailSender, IDataRepository repo)
        {
            services = _services;
            _userManager = userManager;
            repository = repo;
        }
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; } = string.Empty;
            [Required]
            [Display(Name = "First Name")]
            public string? FullName { get; set; }
            public string? TapID { get; set; }
        }
        public async Task<IActionResult> OnGet(string email)
        {
            Input = new InputModel();
            if (User.Identity is not null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user != null && user.Email is not null && user.PhoneNumber is not null)
                    {
                        RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                        if (r == null || r.Credentials == false || r.EligibilityForm == false)
                        {
                            return RedirectToPage("/BetaAsk/credentials_prospect");
                        }
                        Input.FullName = user.FirstName + " " + user.LastName;
                        Input.PhoneNumber = user.PhoneNumber;
                        Input.Email = user.Email;
                        await services.SendTextAsync(Input.PhoneNumber, "You are beginning the onboarding process, which will only take 2 minutes.  Shortly you can begin to share your Contact Page witha QR code.");

                    }
                }
                else
                {
                    Input.Email = email;
                }
            }
            else
            {
                Input.Email = email;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostStep2()
        {
            try
            {
                if(Input.PhoneNumber is null || Input.FullName is null)
                {
                    return Page();
                }
                else
                {
                    await services.SendTextAsync(Input.PhoneNumber, "Hello " + Input.FullName + ". This is a phone number confirmation check from atlice.com.");
                }

            }
            catch
            {
                TempData["message"] = "We could not send a text message to this device";
                return Page();
            }
            string[] names = Input.FullName.Split(' ');
            string fname = names[0];
            string lname = "";
            if (names.Length > 2)
            {
                foreach (string d in names.Skip(1))
                {
                    lname = lname + " " + d;
                }
            }
            else
            {
                if (names.Count() < 2)
                {
                    lname = "";
                }
                else
                {
                    lname = names[1];

                }
            }
            if (ModelState.IsValid)
            {
                if (User.Identity is not null)
                {
                    if (User.Identity.Name is not null && User.Identity.IsAuthenticated)
                    {
                        ApplicationUser? user = await _userManager.FindByNameAsync(User.Identity.Name);
                        if(user is not null)
                        {
                            user.Email = Input.Email;
                            user.PhoneNumber = Input.PhoneNumber;
                            user.FirstName = fname;
                            user.LastName = lname;
                            await _userManager.UpdateAsync(user);
                            if (repository.ContactLists.FirstOrDefault(x => x.UserId == user.Id) == null)
                            {
                                await repository.CreateProfile(user.Id);
                            }
                            ApplicationUser? userRepo = repository.Users.FirstOrDefault(x => x.Id == user.Id);
                            if (userRepo is not null)
                            {
                                await _userManager.UpdateAsync(userRepo);
                            }
                            if (await _userManager.IsInRoleAsync(user, "Citizen") || await _userManager.IsInRoleAsync(user, "Adminis"))
                            {
                                var em = await services.RenderToString("/pages/shared/emails/citizenfeatures.cshtml", user.FirstName);

                                await services.SendEmailAsync(user.Email, "You’ve Been Granted Premium Features!", em);
                            }
                            var e = await services.RenderToString("/pages/shared/emails/welcomeemail.cshtml", "");
                            await services.SendEmailAsync(user.Email, "Welcome to Atlice Tap V3 Private Beta Program", e);
                            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Registered", EventType.User, "OnboardingStep2", false));
                            var codePhone = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                            await services.SendTextAsync(Input.PhoneNumber, "Your security code is: " + codePhone);
                            RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                            if (r != null)
                            {
                                r.OnboardingStep2 = true;
                                r.VerifyStep = true;

                                await repository.SaveRewardTracker(r);
                                return RedirectToPage("/Account/Termsandconditions", new { area = "Identity" });
                            }
                        }
              
                        
                    }


                    return Page();
                }
                return Page();
            }
            return Page();
        }
    }
}
