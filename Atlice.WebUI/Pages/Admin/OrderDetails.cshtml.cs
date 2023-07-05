using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class OrderDetailsModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public OrderDetailsModel(IDataRepository repo, IServices service)
        {
            services = service;
            repository = repo;
        }

        [ViewData]
        public OrderSheet Order { get; set; } = new OrderSheet();
        public class OrderSheet
        {
            public Order Order { get; set; } = new Order();
            public List<string> qrs { get; set; } = new List<string>();
            public List<string> downloads = new List<string>();
        }
        public async Task<IActionResult> OnGet(Guid id, byte[] img = null)
        {
            if(img is not null)
            {
                string filename = "QR" + DateTime.Now + ".jpg";
                return File(img, "image/jpeg", filename);
            }
            Order? order = repository.Orders.FirstOrDefault(x => x.Id == id);
            if(order == null)
            {
                return RedirectToPage("Index");
            }
            else
            {
                Order = new OrderSheet
                {
                    Order = order,
                    qrs = new List<string>(),
                    downloads = new List<string>()
                };
                foreach (var t in Order.Order.Taps)
                {
                    var url = "https://atlice.com/tap/" + t.SNumber.Substring(0, 8);
                    byte[] data = services.BitmapToBytesCode(services.GenerateQR(200, 200, url));
                    string view = "<img src='" + String.Format("data:image/png;base64,{0}", Convert.ToBase64String(data)) + "' loading='lazy' width='200' alt='' class='account-user-icon'>";
                    Order.qrs.Add(view);
                    Order.downloads.Add(url);
                }
                return Page();
            }
            
        }

        public async Task<IActionResult> OnGetDownloadQR(string img)
        {
            byte[] data = services.BitmapToBytesCode(services.GenerateQR(200, 200, img));
           
            string filename = "QR" +DateTime.Now  + ".jpg";
            
            return File(data, "image/jpeg", filename);            
        }

        public async Task<IActionResult> OnPostChangeStatus(Guid id, string status)
        {
            Order? order = repository.Orders.FirstOrDefault(x=>x.Id == id);
            if(order == null)
            {
                return Page();
            }
            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), status);
            await repository.SaveOrder(order);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed the status of order with id: " + order.Id, EventType.Admin, "ChangeStatus", false));

            Order = new OrderSheet
            {
                Order = order,
                qrs = new List<string>(),
                downloads = new List<string>()
            };
            foreach (var t in Order.Order.Taps)
            {
                var url = "https://atlice.com/tap/" + t.SNumber.Substring(0, 8);
                byte[] data = services.BitmapToBytesCode(services.GenerateQR(200, 200, url));
                string view = "<img src='" + String.Format("data:image/png;base64,{0}", Convert.ToBase64String(data)) + "' loading='lazy' width='200' alt='' class='account-user-icon'>";
                Order.qrs.Add(view);
                Order.downloads.Add(url);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddDevice(Guid id, string device)
        {
            Order? order = repository.Orders.FirstOrDefault(x => x.Id == id);
            if (order == null)
            {
                return Page();
            }
            AtliceTap? card = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == (SKU)Enum.Parse(typeof(SKU), device));
            if (card != null)
            {
                card.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                card.Note = card.Note + "Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                card = await repository.SaveTap(card);
                order.Taps.Add(card);
            }
            else
            {
                card = new AtliceTap
                {
                    UserId = null,
                    Locked = true,
                    TapType = TapType.Card,
                    Note = "AutoGenerated, No Inventory; Assigned on: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ",
                    Sku = SKU.LGYCSTLGCD1
                };
                card = await repository.SaveTap(card);
                order.Taps.Add(card);

            }
            await repository.SaveOrder(order);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added device with id: " + card.Id +" to order id: "+ order.Id, EventType.Admin, "Created Device", false));

            Order = new OrderSheet
            {
                Order = order,
                qrs = new List<string>(),
                downloads = new List<string>()
            };
            foreach (var t in Order.Order.Taps)
            {
                var url = "https://atlice.com/tap/" + t.SNumber.Substring(0, 8);
                byte[] data = services.BitmapToBytesCode(services.GenerateQR(200, 200, url));
                string view = "<img src='" + String.Format("data:image/png;base64,{0}", Convert.ToBase64String(data)) + "' loading='lazy' width='200' alt='' class='account-user-icon'>";
                Order.qrs.Add(view);
                Order.downloads.Add(url);
            }
            return Page();
        }
    }
}
