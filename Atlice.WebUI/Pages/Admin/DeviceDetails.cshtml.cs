using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shippo;
using Order = Atlice.Domain.Entities.Order;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class DeviceDetailsModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public DeviceDetailsModel(IDataRepository repository, UserManager<ApplicationUser> userManager)
        {
            this.userManager= userManager;
            this.repository = repository;
        }

        [BindProperty]
        public InputModel Device { get; set; } = new InputModel();

        public class InputModel
        {
            public AtliceTap Tap { get; set; } = new AtliceTap();
            public Guid? UserId { get; set; }
            public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        }

        public async Task<IActionResult> OnGet(Guid id)
        {
            AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == id);
            if(t is not null)
            {
                Device = new InputModel { Tap = t, UserId = t.UserId };
                foreach(var user in userManager.Users.ToList())
                {
                    if(await userManager.IsInRoleAsync(user,"Citizen") || await userManager.IsInRoleAsync(user,"Tourist") || await userManager.IsInRoleAsync(user, "Adminis"))
                    {
                        Device.Users.Add(user);
                    }
                }
                return Page();
            }
            else
            {
                TempData["AddError"] = "Device not found";
                return RedirectToPage("/admin/inventory");
            }
        }

        public async Task<IActionResult> OnPostEditDevice()
        {
            if(Device is not null && Device.Tap is not null)
            {
                AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == Device.Tap.Id);
                if(t is not null)
                {
                    t.Locked = Device.Tap.Locked;
                    if (!string.IsNullOrEmpty(Device.UserId.ToString()))
                    {
                        t.UserId = Device.UserId;
                        ApplicationUser? user = repository.Users.FirstOrDefault(x => x.Id == Device.UserId);
                        if (user != null && user.PhoneNumber is not null && user.Email is not null)
                        {
                            Order order = new(user.FirstName + " " + user.LastName, user.FirstName + " " + user.LastName, "not complete", "not complete", "not complete", "not complete", user.PhoneNumber, user.Email, user.Id, user.Id.ToString())
                            {
                                BookMarked = true
                            };
                            t.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                            t.Note = t.Note + "Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                            t = await repository.SaveTap(t);

                            order.Taps.Add(t);
                            t = await repository.SaveTap(t);
                            order = await repository.SaveOrder(order);
                            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " created an order with id: "+order.Id+", bookmarked and assigned this device to that order, deviceId: " + t.Id, EventType.Admin, "EditDevice/DeviceDetails", false));

                        }



                    }
                    await repository.SaveTap(t);
                    return RedirectToPage("/admin/inventory");
                }
            }
            TempData["AddError"] = "Device not found";
            return RedirectToPage("/admin/inventory");
        }

        public async Task<IActionResult> OnGetWipeDevice(Guid deviceid)
        {
            if(User.Identity is not null)
            {
                AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == deviceid);
                if (t is not null)
                {
                    var UserId = t.UserId;

                    t.ContactPage = null;
                    t.UserId = null;
                    t.Locked = true;
                    t.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                    t.Location = null;
                    t.Note = "Device reset in Admin due to duplicate by: " + User.Identity.Name + "; ";
                    await repository.SaveTap(t);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " reset device with id:" + t.Id, EventType.Admin, "WipeDevice/DeviceDetails", false));

                    Device = new InputModel { Tap = t, UserId = t.UserId };
                    foreach (var user in userManager.Users.ToList())
                    {
                        if (await userManager.IsInRoleAsync(user, "Citizen") || await userManager.IsInRoleAsync(user, "Tourist") || await userManager.IsInRoleAsync(user, "Adminis"))
                        {
                            Device.Users.Add(user);
                        }
                    }
                    return Page();
                }
            }
            
            return RedirectToPage("/admin/inventory");
        }
    }
}
