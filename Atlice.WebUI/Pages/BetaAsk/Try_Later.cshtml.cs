using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    public class Try_laterModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IServices services;
        public Try_laterModel(IServices _services, IDataRepository _repository, UserManager<ApplicationUser> _userManager)
        {
            services = _services;
            repository = _repository;
            userManager = _userManager;
        }
        public IActionResult OnGet()
        {
            return Page();
        }
        public async Task<IActionResult> OnPostTryLater(string phone)
        {
            if (repository.Users.FirstOrDefault(x => x.PhoneNumber == phone) != null)
            {
                ApplicationUser? user = userManager.Users.FirstOrDefault(x => x.PhoneNumber == phone);
                if (user != null && user.Email is not null)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    await userManager.RemoveFromRolesAsync(user, roles);
                    await userManager.AddToRoleAsync(user, "TryLater");

                    await userManager.UpdateAsync(user);
                    string path = "https://atlicemedia.blob.core.windows.net/atliceapp/try_later_reminder.html";
                    string contents;
                    HttpClient httpClient = new HttpClient();
                    contents = await httpClient.GetStringAsync(path);
                    contents = contents.Replace("[name]", user.Email).Replace("https://Atlice.azurewebsites.net", "https://atlice.com");
                    await services.SendEmailAsync(user.Email, "Try Later Reminder", contents);
                }
                
            }
            return Content("Thank you for your time!");
        }
    }
}
