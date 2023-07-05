using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.X509;
using Shippo;
using System.Collections;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class CustomerProfileModel : PageModel
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerProfileModel(IDataRepository repo, IServices _services, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            services = _services;
            repository = repo;
        }

        [ViewData]
        public CustomerModel Customer { get; set; } = new CustomerModel();
        public class CustomerModel
        {
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public List<Domain.Entities.Order> Orders { get; set; } = new List<Domain.Entities.Order>();
            public List<AdminNote> Notes { get; set; } = new List<AdminNote>();
            public List<ApplicationUser> Linked { get; set; } = new List<ApplicationUser>();
            public List<AtliceTap> Taps { get; set; } = new List<AtliceTap>();
        }
        public async Task<IActionResult> OnGet(Guid id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["message"] = "Deleted User";
                return RedirectToPage("/admin/fullfillment");
            }
            CustomerModel model = new()
            {
                User = user,
                Notes = repository.AdminNotes.Where(x => x.UserId == id).ToList(),
                Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList()
            };
            Customer = model;
            Customer.Taps = new List<AtliceTap>();
            Customer.Linked = new List<ApplicationUser>();
            foreach (var order in Customer.Orders)
            {
                foreach (var device in order.Taps)
                {
                    if (!Customer.Taps.Contains(device))
                    {
                        Customer.Taps.Add(device);
                    }
                }
            }
            foreach (var tap in Customer.Taps.Where(x => x.UserId != null))
            {
                var uid = tap.UserId.ToString();
                if (uid != null)
                {
                   
                    ApplicationUser? u = await _userManager.FindByIdAsync(uid);
                    if(u is not null)
                    {
                        Customer.Linked.Add(u);
                    }
                    
                }
                
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateInfo(Guid orderid, string phone, string email, string address1, string address2, string city, string state, string zip)
        {
            Domain.Entities.Order? o = repository.Orders.FirstOrDefault(x => x.Id == orderid);
            if(o is not null)
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(o.UserId.ToString());
                if (user is not null)
                {
                    if (repository.Users.FirstOrDefault(x => x.PhoneNumber == phone) != null)
                    {
                        
                        o.Phone = phone;
                        o.Email = email;
                        o.ShipAddressLine1 = address1;
                        o.ShipAddressLine2 = address2;
                        o.ShipCity = city;
                        o.ShipState = state;
                        o.ShipCode = zip;
                        await repository.SaveOrder(o);
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " updated the customer info for user: "+user.Id, EventType.Admin, "UpdateInfo", false));
                        return RedirectToPage("CustomerProfile", new { id = user.Id });
                    }
                    

                return RedirectToPage("CustomerProfile", new { id = user.Id });
                }
            }

            return RedirectToPage("/Admin/Index");
        }

        public async Task<IActionResult> OnPostCustomerNote(string Note, Guid UserId)
        {
            await repository.SaveAdminNote(new AdminNote(UserId, "Admin", Note));
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " add an Admin Note for user: " + UserId, EventType.Admin, "CustomerNote", false));

            string view = "<li>-" + Note + "</li><li id='noteDiv'></li>";
            return Content(view);
        }
        public async Task<IActionResult> OnPostAddProduct(string UserId, string devid, string note)
        {
            ApplicationUser? u = await _userManager.FindByIdAsync(UserId);
            AtliceTap? t = repository.Taps.FirstOrDefault(x=>x.UserId == null && x.Note == null && x.Sku == (SKU)System.Enum.Parse(typeof(SKU), devid));
            if(t == null)
            {
                t = new()
                {
                    LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                    UserId = null,
                    ContactPage = null,
                    Location = null,
                    Locked = true,
                    Note = "New Inventory Device; " + note + "; " + "Created on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ",
                    //same sku as webflow
                    Sku = (SKU)System.Enum.Parse(typeof(SKU), devid),
                    //new random serial
                    SNumber = Guid.NewGuid().ToString(),
                };
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " added a device that was not in the database to a new order, deviceId: " + t.Id, EventType.Admin, "AddProduct/NewOrder", false));

            }


            if (t.Sku == SKU.LGYCSTLGCD1)
            {
                t.TapType = TapType.Card;

            }
            if (t.Sku == SKU.CLSTG25GRY1)
            {
                t.TapType = TapType.Tag;

            }
            if (t.Sku == SKU.CLSTG30GRY1)
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


            if(u is not null && u.Email is not null)
            {
                Domain.Entities.Order newOrder = new(u.FirstName + " " + u.LastName, u.FirstName + " " + u.LastName, "not complete", "not complete", "not complete", "not complete", "not complete", u.Email, u.Id, u.Id.ToString())
                {
                    Id = Guid.NewGuid(),
                    Brand = "Atlice",
                    ChargeId = "Free",
                    Comments = "Created by Admin: " + note,
                    CustomerPaid = "0.00",
                    Last4 = "9999",
                    Phone = u.PhoneNumber,
                    Email = u.Email,
                    Taps = new List<AtliceTap>(),
                    Status = OrderStatus.NewOrder

                };
                newOrder.Taps.Add(t);

                await repository.SaveOrder(newOrder);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " added an order to user : " + newOrder.UserId, EventType.Admin, "AddProduct/NewOrder", false));

            }


            return RedirectToPage("Fullfillment");


        }

        public IActionResult OnPostGetQR(Guid deviceID)
        {
            AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == deviceID);
            if(t is not null)
            {
                var url = "https://atlice.com/tap/" + t.SNumber[..8];
                byte[] data = services.BitmapToBytesCode(services.GenerateQR(200, 200, url));
                string view = "<img src='" + string.Format("data:image/png;base64,{0}", Convert.ToBase64String(data)) + "' loading='lazy' width='200' alt='' class='account-user-icon'>";
                return Content(view);
            }
            return Content("Error Device Not Found");

        }

        public async Task<IActionResult> OnPostReadyShip(Guid tapID)
        {
            AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == tapID);
            if(t is not null)
            {
                t.Note = t.Note + "Created on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                await repository.SaveTap(t);
                Domain.Entities.Order? order = repository.Orders.FirstOrDefault(x => x.Taps.Contains(t));
                if(order is not null)
                {
                   
                    if (order.Taps.All(x => x.Note is not null && x.Note.Contains("Created on")))
                    {
                        order.Status = OrderStatus.PendingShipment;
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " completed making devices for order number: " + order.Id, EventType.Admin, "ReadyToShip", false));

                    }
                    else
                    {
                        order.Status = OrderStatus.PartiallyComplete;
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " partially completed order number: " + order.Id, EventType.Admin, "ReadyToShip", false));

                    }

                    await repository.SaveOrder(order);
                    return Content(t.Note);
                }
            }
            return Content("Device Not Found");
        }
        public async Task<IActionResult> OnPostCreateLabel(Guid orderid)
        {
            Domain.Entities.Order? order = repository.Orders.FirstOrDefault(x => x.Id == orderid);

            if (order.Status == OrderStatus.PendingShipment)
            {
                APIResource resource = new APIResource("shippo_live_3b19b7a30792f300c800c5457a9a1fdab2b99755\r\n\r\n");
                ShippoCollection<CarrierAccount> filteredCarrierAccounts = resource.AllCarrierAccount();
                Hashtable toAddressTable = new Hashtable();
                toAddressTable.Add("name", order.Name);
                if (!string.IsNullOrEmpty(order.Company))
                    toAddressTable.Add("company", order.Company);
                toAddressTable.Add("street1", order.ShipAddressLine1);
                if (!string.IsNullOrEmpty(order.ShipAddressLine2))
                    toAddressTable.Add("street2", order.ShipAddressLine2);

                toAddressTable.Add("city", order.ShipCity);
                if (order.ShipState.Length != 2)
                {
                    toAddressTable.Add("state", "NY");
                }
                else
                {
                    toAddressTable.Add("state", order.ShipState);
                }
                toAddressTable.Add("zip", order.ShipCode);
                toAddressTable.Add("country", "US");

                toAddressTable.Add("phone", "+1 " + order.Phone.Substring(0, 3) + " " + order.Phone.Substring(3, 3) + " " + order.Phone.Substring(6, 4));
                toAddressTable.Add("email", order.Email);

                // from address
                Hashtable fromAddressTable = new Hashtable();
                fromAddressTable.Add("name", "Alexander Oliver");
                fromAddressTable.Add("company", "SOSKYHIGH Media");
                fromAddressTable.Add("street1", "580 Flatbush ave");
                fromAddressTable.Add("street2", "17L");
                fromAddressTable.Add("city", "Brooklyn");
                fromAddressTable.Add("state", "NY");
                fromAddressTable.Add("zip", "11225");
                fromAddressTable.Add("country", "US");
                fromAddressTable.Add("email", "atlicetap@atlice.com");
                fromAddressTable.Add("phone", "+1 646 673 2325");
                //fromAddressTable.Add("metadata", "Customer ID 123456");

                // parcel
                Hashtable parcelTable = new Hashtable();
                parcelTable.Add("length", "19");
                parcelTable.Add("width", "10");
                parcelTable.Add("height", "1");
                parcelTable.Add("distance_unit", "cm");
                parcelTable.Add("weight", "1");
                parcelTable.Add("mass_unit", "oz");

                // shipment
                Hashtable shipmentTable = new Hashtable();
                shipmentTable.Add("address_to", toAddressTable);
                shipmentTable.Add("address_from", fromAddressTable);
                shipmentTable.Add("parcels", parcelTable);

                Console.WriteLine("Getting shipping label..");
                Hashtable transactionParameters = new Hashtable();
                transactionParameters.Add("shipment", shipmentTable);
                transactionParameters.Add("servicelevel_token", "usps_first");
                transactionParameters.Add("carrier_account", "16f46afaa46a4494b200d6d97f25b274");
                Transaction transaction = resource.CreateTransaction(transactionParameters);

                if (((String)transaction.Status).Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " created label for order number: " + order.Id, EventType.Admin, "CreateLabel", false));

                    Console.WriteLine("Label url : " + transaction.LabelURL);
                    Console.WriteLine("Tracking number : " + transaction.TrackingNumber);
                    order.Tracking = transaction.TrackingNumber.ToString();
                    order.Comments = order.Comments + "Shipping URL: " + transaction.LabelURL + " ;";
                }
                await repository.SaveOrder(order);

            }
            return RedirectToPage("CustomerProfile", new {id = order.UserId});
        }
        public async Task<IActionResult> OnPostHandDeliver(Guid tapID)
        {
            AtliceTap? t = repository.Taps.FirstOrDefault(x=>x.Id== tapID);
            Domain.Entities.Order? o = repository.Orders.FirstOrDefault(x => x.Taps.Contains(t));
            if(t != null && o is not null)
            {
                if(o is not null)
                {
                    o.Status = OrderStatus.HandDeliver;
                    await repository.SaveOrder(o);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " switched to hand delivery for order number: " + o.Id, EventType.Admin, "HandDeliver", false));

                    return RedirectToPage("CustomerProfile", new { id = o.Id });

                }

            }
            return RedirectToPage("CustomerProfile", new { id = o.Id });
        }
    }
}
