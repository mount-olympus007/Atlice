using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class CreateUserModel : PageModel
    {
        private readonly IServices _services;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository _dataRepository;
        public CreateUserModel(IServices services, UserManager<ApplicationUser> userManager, IDataRepository dataRepository)
        {
            _services = services;
            _userManager = userManager;
            _dataRepository = dataRepository;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCreate(string fname, string lname, string email, string mobile)
        {
            char[] arr = mobile.ToCharArray();
            arr = Array.FindAll(arr, char.IsDigit);
            
            string phone = new string(arr);
            var admin = await _userManager.GetUserAsync(User);
            ApplicationUser? user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
            if(user == null && admin is not null)
            {

                user = new()
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = mobile,
                    Email = email,
                    UserName = email,
                    FirstName = fname,
                    LastName = lname,
                    AboutMe = "Created for Beta Test",
                    AffiliateId = admin.Id
                };

                user = await _services.CreateAtliceAccount(user, "Prospect");
                await _dataRepository.SaveEvent(new Event(admin.UserName, admin.FirstName + " Created user with id: " + user.Id.ToString(), EventType.Admin, "Create User", false));
                return RedirectToPage("UserProfile", new { id = user.Id });

            }
            else
            {
                return Content("A user with this mobile/email already exists!");
            }
        }

    }
}
