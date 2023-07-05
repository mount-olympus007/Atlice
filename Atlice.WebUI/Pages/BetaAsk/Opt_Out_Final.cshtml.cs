using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class Opt_Out_FinalModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IServices services;
        private readonly IDataRepository repository;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public Opt_Out_FinalModel(IDataRepository _repository, IServices _services, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> manager)
        {
            repository = _repository;
            services = _services;
            signInManager = _signInManager;
            userManager = manager;
        }
        public async Task<IActionResult> OnGet()
        {
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Clicked opt Out", EventType.User, "OptOut", false));

            return Page();
        }
        public async Task<IActionResult> OnPostOptOut(string idea, string contact)
        {
            if(User.Identity is not null && User.Identity.Name is not null)
            {
                ApplicationUser? u = await userManager.FindByNameAsync(User.Identity.Name);

                if (u != null)
                {
                    if (contact == "Yes")
                    {
                        u.SmsAlerts = true;
                    }
                    else
                    {
                        u.SmsAlerts = false;
                    }
                    u.AboutMe = idea;

                    var roles = await userManager.GetRolesAsync(u);
                    await userManager.RemoveFromRolesAsync(u, roles);
                    await userManager.AddToRoleAsync(u, "TryLater");
                    await userManager.UpdateAsync(u);
                    string path = "https://atlicemedia.blob.core.windows.net/atliceapp/try_later_reminder.html";
                    string contents;
                    var wc = new HttpClient();
                    contents = await wc.GetStringAsync(path);
                    contents = contents.Replace("[name]", u.FirstName).Replace("https://Atlice.azurewebsites.net", "https://atlice.com");
                    if(!string.IsNullOrEmpty(u.Email))
                    {
                        await services.SendEmailAsync(u.Email, "Try Later Reminder", contents);
                        await repository.SaveEvent(new Event(User.Identity.Name, u.FirstName + u.LastName +  "Opt-out of the Beta Ask on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(), EventType.User, "OptOut", false));

                    }


                    await signInManager.SignOutAsync();
                    return Content("Thank you for your time! We will contact you soon!");
                }
                return Content("Thank you for your time!");
            }
            return Content("Thank you for your time!");
        }
    }
}
