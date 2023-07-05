using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.BetaAsk
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class Opt_In_FinalModel : PageModel
    {
        private readonly IDataRepository repository;
        public Opt_In_FinalModel(IDataRepository _repository)
        {
            repository= _repository;
        }
        public async Task<IActionResult> OnGet()
        {
            ApplicationUser? u = repository.Users.FirstOrDefault(x=> x.UserName == User.Identity.Name);
            if(u is not null)
            {
                RewardTracker? t = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == u.Id);
                if(t is not null) 
                {
                    if (!t.PlacedOrder)
                    {
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Skipped placing an order", EventType.User, "AlreadyHaveDevices", false));

                    }

                }
            }
            return Page();
        }
    }
}
