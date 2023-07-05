using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class InventoryModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public InventoryModel(IDataRepository repo, IServices _services, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            services = _services;
            repository = repo;
        }

        [ViewData]
        public List<TapModel> Inventory { get; set; } = new List<TapModel>();
        [ViewData]
        public int Count { get; set; }
        public class TapModel
        {
            public AtliceTap Tap { get; set; } = new AtliceTap();
            public ApplicationUser? User { get; set; }
            public Order? Order { get; set; }
        }

        public async Task OnGet(int pageNo = 0)
        {

            int pageSize = 100;
            Count = repository.Taps.Count();
            Inventory = new List<TapModel>();
            foreach (var tap in repository.Taps.OrderByDescending(x => x.LastEdited).Skip(pageSize * pageNo).Take(pageSize).ToList())
            {
                TapModel t = new()
                {
                    Tap = tap
                };
                
                var tu = tap.UserId.ToString();
                if (!string.IsNullOrEmpty(tu))
                {
                    var user = await _userManager.FindByIdAsync(tu);
                    t.User = user;
                }
                if (repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap)) != null)
                {
                    Order? order = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                    t.Order = order;
                    
                }
                Inventory.Add(t);
            }
            Inventory = Inventory.OrderByDescending(x => x.Tap.LastEdited).ToList();
            var createdTaps = Inventory.Where(x=>x.Tap.Note is not null && x.Tap.Note.Contains("Created")).ToList();
            foreach(var t in createdTaps)
                Inventory.Remove(t);
            Inventory = createdTaps.Union(Inventory).ToList();
        }


        public async Task<IActionResult> OnPostFindDevice(string? tapid, string? activated, string? assigned)
        {
            List<AtliceTap> atliceTaps = new();
            if (activated != null)
            {
                foreach (var user in repository.Users.ToList())
                {
                    if (user.FirstName != null && user.LastName != null)
                    {
                        if (user.FirstName.ToLower().Contains(activated.ToLower()) || user.LastName.ToLower().Contains(activated.ToLower()))
                        {
                            List<AtliceTap> taps = new(repository.Taps.Where(x => x.UserId == user.Id));
                            atliceTaps.AddRange(taps);
                        }

                    }


                }
            }

            if (assigned != null)
            {
                foreach (ApplicationUser user in _userManager.Users.ToList())
                {
                    if (user.FirstName != null && user.LastName != null)
                    {
                        if (user.FirstName.ToLower().Contains(assigned.ToLower()) || user.LastName.ToLower().Contains(assigned.ToLower()))
                        {
                            List<AtliceTap> taps = repository.Taps.Where(x => x.UserId == user.Id).ToList();
                            atliceTaps.AddRange(taps);
                        }
                    }



                }
            }

            if (tapid != null)
            {
                List<AtliceTap> taps = repository.Taps.Where(x => x.SNumber.ToLower().StartsWith(tapid.ToLower())).ToList();
                atliceTaps.AddRange(taps);
            }
            var resultTaps = new List<TapModel>();
            foreach (var tap in atliceTaps)
            {
                TapModel t = new()
                {
                    Tap = tap
                };
                var ti = tap.UserId.ToString();
                if (!string.IsNullOrEmpty(ti))
                {
                    var user = await _userManager.FindByIdAsync(ti);
                    t.User = user;
                }
                if (repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap)) != null)
                {
                    Order? order = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                    t.Order = order;

                }
                resultTaps.Add(t);
            }
            string view = await services.RenderToString("/pages/admin/TapList.cshtml", resultTaps);
            return Content(view);
        }

        public async Task<IActionResult> OnPostCreateDevice(string devid)
        {

            AtliceTap t = new()
            {
                LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                UserId = null,
                ContactPage = null,
                Location = null,
                Locked = true,
                Note = "New Inventory Device; ",
                //same sku as webflow
                Sku = (SKU)System.Enum.Parse(typeof(SKU), devid),
                //new random serial
                SNumber = Guid.NewGuid().ToString(),
            };

            if (t.Sku == SKU.LGYCSTLGCD1)
            {
                t.TapType = TapType.Card;

            }
            if (t.Sku == SKU.CLSTG25GRY1)
            {
                t.TapType = TapType.Tag;

            }
            if (t.Sku == SKU.CLSCDWHT)
            {
                t.TapType = TapType.Card;

            }
            if (t.Sku == SKU.Virtual)
            {
                t.TapType = TapType.Virtual;

            }
            await repository.SaveTap(t);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Created a new Inventory device with id: " + t.Id, EventType.Admin, "Created Device", false));


            return RedirectToPage("DeviceDetails", new { id=t.Id});
        }
        

    }
}
