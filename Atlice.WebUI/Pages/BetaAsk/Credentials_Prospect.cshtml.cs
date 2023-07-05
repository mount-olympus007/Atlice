using Microsoft.AspNetCore.Mvc;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wangkanai.Detection.Services;
using Atlice.Domain.Abstract;

namespace Atlice.WebUI.Pages.BetaAsk
{
    public class Credentials_ProspectModel : PageModel
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDataRepository repository;
        private readonly IServices services;
        public Credentials_ProspectModel(IDataRepository dataRepository, IServices _services, UserManager<ApplicationUser> manager)
        {
            userManager = manager;
            services = _services;
            repository = dataRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public class InputModel
        {
            public string? Number { get; set; }
            public string? Email { get; set; }
            public Guid? Invitation { get; set; }
            public string? Error { get; set; }
        }

        public void OnGet(Guid al, Guid nu)
        {
            var pageOwner = userManager.Users.FirstOrDefault(x => x.InviteCode == al);
            var newUser = userManager.Users.FirstOrDefault(x => x.Id == nu);

            if (newUser != null && pageOwner != null)
            {
                Input = new InputModel
                {
                    Email = newUser.Email,
                    Number = newUser.PhoneNumber,
                    Invitation = al
                };
            }
            else
            {
                Input = new InputModel
                {
                    Email = "",
                    Number = "",
                    Invitation = Guid.NewGuid()
                };
            }
        }

        public async Task<IActionResult> OnGetProsCred(string number, string email)
        {
            ApplicationUser? user = userManager.Users.FirstOrDefault(x => x.PhoneNumber == number && x.Email == email);
            if (user != null && user.PhoneNumber is not null)
            {
                if (await userManager.IsInRoleAsync(user, "Citizen") || await userManager.IsInRoleAsync(user, "Tourist") || await userManager.IsInRoleAsync(user, "Adminis"))
                {
                   
                    var codePhone = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                    await services.SendTextAsync(number, "Your security code is: " + codePhone);
                    //Atlice token
                    string url = "/BetaAsk/VerifyPhoneNumber?phone=" + user.PhoneNumber;
                    return new JsonResult(new { url });

                }
                if (await userManager.IsInRoleAsync(user, "Prospect") || await userManager.IsInRoleAsync(user, "Lead"))
                {
                    var codePhone = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                    await services.SendTextAsync(number, "Your security code is: " + codePhone);
                    //Atlice token
                    
                    string url = "/BetaAsk/VerifyPhoneNumber?phone=" + user.PhoneNumber + "&email=" + email;
                    return new JsonResult(new { url });
                }

                return new JsonResult(new { error = "Invalid login attempt" });

            }
            else
            {
                return new JsonResult(new { error = "Invalid login attempt" });

            }
        }
    }
}
