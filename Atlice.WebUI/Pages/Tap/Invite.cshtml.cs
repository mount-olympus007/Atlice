using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Tap
{
    [Authorize(Roles = "Adminis,Citizen")]
    public class InviteModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository _repository;

        public InviteModel(UserManager<ApplicationUser> userManager, IDataRepository repository)
        {
            _repository= repository;
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGet(string mobile)
        {
            if(!string.IsNullOrEmpty(mobile))
            {
                ApplicationUser? prospect = _userManager.Users.FirstOrDefault(x=>x.PhoneNumber == mobile);
                ApplicationUser? citizen = await _userManager.GetUserAsync(User);

                if (prospect != null && prospect.PhoneNumber is not null && citizen != null)
                {
                    var roles = await _userManager.GetRolesAsync(prospect);
                    if (roles.First() == "Lead")
                    {
                        await _userManager.RemoveFromRolesAsync(prospect,roles);
                        await _userManager.AddToRoleAsync(prospect, "Prospect");
                        Passport pp = await _repository.SavePassport(new Passport(prospect.Id));
                        pp.Stamps.Add(await _repository.SaveStamp(new Stamp("Citizen's Stamp", Platform.Atlice, pp.Id)));
                        if (User.IsInRole("Adminis"))
                        {
                            pp.Stamps.Add(await _repository.SaveStamp(new Stamp("Founder's Stamp", Platform.Atlice, pp.Id)));
                            await _repository.SaveEvent(new Event("Ip: " + HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), "Founders Stamp added for "+prospect.Email+" by " + citizen.Email, EventType.User, "Invite", false));

                        }
                        await _repository.SavePassport(pp);
                        //send promote email
                        await _repository.SaveEvent(new Event("Ip: " + HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), prospect.Email + " Promoted to Citizen", EventType.User, "Invite", false));

                    }
                    return RedirectToPage("/Homeboard/Index");

                }
                else
                {
                    return RedirectToPage("/Tap/Locked");
                }
            }
            return RedirectToPage("/Tap/Locked");
        }
    }
}
