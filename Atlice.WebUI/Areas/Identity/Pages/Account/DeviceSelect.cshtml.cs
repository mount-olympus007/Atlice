using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class DeviceSelectModel : PageModel
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeviceSelectModel(UserManager<ApplicationUser> userManager, IDataRepository repo, IServices _services)
        {
            _userManager = userManager;
            repository = repo;
            services = _services;
        }
        [ViewData]
        public List<AtliceTap> Taps { get; set; } = new List<AtliceTap>();
        public async Task<IActionResult> OnGet(string? tapid)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user is not null)
            {
                RewardTracker? r = repository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                if (r == null || r.Terms == false)
                {
                    return RedirectToPage("/account/Termsandconditions");
                }
                Taps = new List<AtliceTap>();
                if (string.IsNullOrEmpty(tapid))
                {
                    var vir = repository.Taps.FirstOrDefault(x => x.TapType == TapType.Virtual && x.UserId == user.Id);
                    if (vir != null)
                    {
                        vir.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                        vir.Note = vir.Note + "Activated: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + string.Concat("Activated Device ", vir.SNumber.AsSpan(0, 8)), EventType.User, "DeviceSelect", false));

                        await repository.SaveTap(vir);
                        Taps.Add(vir);
                    }
                    else
                    {
                        return RedirectToPage("/identity/account/onboarding_step_2");
                    }

                }
                else
                {

                    AtliceTap? t = repository.Taps.FirstOrDefault(x => x.SNumber[..8] == tapid.ToString());
                    if (t is not null)
                    {
                        Taps.Add(t);
                        if (t.TapType != TapType.Virtual)
                        {
                            Order? o = repository.Orders.FirstOrDefault(x => x.Taps.Contains(t));
                            if (o != null)
                            {
                                if (o.UserId == user.Id && o.Status == OrderStatus.Shipped)
                                {
                                    foreach (var tap in o.Taps.ToList())
                                    {
                                        tap.UserId = user.Id;
                                        tap.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                                        tap.Note = tap.Note + "Activated: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + string.Concat("Activated Device ", tap.SNumber.AsSpan(0, 8)), EventType.User, "DeviceSelect", false));
                                        await repository.SaveTap(tap);
                                        Taps.Add(tap);
                                    }
                                    o.Status = OrderStatus.Activated;
                                    user.Taps.Union(o.Taps);
                                    await _userManager.UpdateAsync(user);
                                    await repository.SaveOrder(o);
                                    await services.SendTextAsync(user.PhoneNumber, "Thank you for activating your Atlice Tap devices." + "\n\n Add Atlice Tap to your contacts with this link: https://atlice.com/cards?email=atlicetap@atlice.com&contactid=0");

                                }
                                if (o.Taps is not null)
                                {
                                    Taps = o.Taps.ToList();
                                }

                            }


                        }
                    }

                }


                if (Taps.FirstOrDefault() is not null)
                {

                    return Page();
                }
                else
                {
                    return RedirectToPage("/identity/account/onboarding_step_2");
                }
            }
            return RedirectToPage("/identity/account/onboarding_step_2");
        }
    }
}
