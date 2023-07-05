using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class RegisterModel : PageModel
    {
        private readonly IDataRepository repository;
        public RegisterModel(IDataRepository _repository)
        {
            repository = _repository;
        }

        public async Task<IActionResult> OnGet()
        {
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " made it to registration from betaask", EventType.User, "Register", false));
            return Page();

        }


    }
}
