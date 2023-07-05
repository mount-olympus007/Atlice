using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Net.Http.Headers;
using WebflowSharp.Entities;
using WebflowSharp.Infrastructure;
using WebflowSharp.Services.Order;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class FullfillmentModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public FullfillmentModel(IDataRepository repo, IServices _services, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            services = _services;
            repository = repo;
        }

        public class AtliceOrder
        {
            public Order Order { get; set; } = new Order();
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public string? Role { get; set; } = "";
        }


        [ViewData]
        public List<AtliceOrder> Orders { get; set; } = new List<AtliceOrder>();

        public class Product
        {
            public string slug { get; set; }
            public string name { get; set; }
            public bool _archived { get; set; }
            public bool _draft { get; set; }
        }
      

        public class SKUObject
        {
            public string slug { get; set; }
            public string name { get; set; }
            public bool _archived { get; set; }
            public bool _draft { get; set; }
            public Dictionary<string, string> price = new Dictionary<string, string>();
        }
        

        public async Task OnGet()
        {
            var client = new HttpClient();
           
            


            //Call Webflow site api with our token
            WebflowSharp.Services.Site.SiteService s = new("a82d074525b7a4996d9f87bf6618af8d1d3013f48723bfbb3e5f9efd9eec70f6");

            //gets your published sites
            var sd = await s.GetSites();

            //gets the first site, the shop
            Site? sdd = sd.FirstOrDefault();


    //        var request = new HttpRequestMessage
    //        {
    //            Method = HttpMethod.Post,
    //            RequestUri = new Uri("https://api.webflow.com/sites/"+sdd.Id+"/products?access_token=a82d074525b7a4996d9f87bf6618af8d1d3013f48723bfbb3e5f9efd9eec70f"),
    //            Headers =
    //{
    //    { "accept", "application/json" },
    //},
    //            Content = new StringContent("{\"product\":{\"fields\":{\"slug\":\"my-cloak\",\"name\":\"The Cloak\",\"_archived\":false,\"_draft\":false}},\"sku\":{\"fields\":{\"slug\":\"my-cloak\",\"name\":\"Cloak Of Invisibility Color: Obsidian Black\",\"_archived\":false,\"_draft\":false,\"price\":{\"value\":2000,\"unit\":\"USD\"}}}}")
    //            {
    //                Headers =
    //    {
    //        ContentType = new MediaTypeHeaderValue("application/json")
    //    }
    //            }
    //        };
    //        using (var response = await client.SendAsync(request))
    //        {
    //            response.EnsureSuccessStatusCode();
    //            var body = await response.Content.ReadAsStringAsync();
    //            Console.WriteLine(body);
    //        }









            //Call webflow order api for site
            OrderService orderService = new("a82d074525b7a4996d9f87bf6618af8d1d3013f48723bfbb3e5f9efd9eec70f6");
            if (sdd is not null)
            {
                //get all orders
                var y = await orderService.GetOrders(sdd.Id);
                foreach (OrderModel webfloworder in y.Where(x => x.Status.Contains("pending") || x.Status.Contains("unfulfilled") || x.Status.Contains("fulfilled")))
                {
                    Order? order = repository.Orders.FirstOrDefault(x => x.OrderNumber == webfloworder.OrderId);
                    if (order == null)
                    {
                        ApplicationUser? user;
                        if (webfloworder.CustomData.Count() != 0)
                        {
                            user = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == webfloworder.CustomData[0].TextInput);
                        }
                        else
                        {
                            user = _userManager.Users.FirstOrDefault(x => x.Email == webfloworder.CustomerInfo.Email);
                        }
                        if (user is null)
                        {
                            string[] names = webfloworder.CustomerInfo.FullName.Split(' ');
                            string fname = names[0];
                            string lname = "";
                            if (names.Length > 2)
                            {
                                foreach (string d in names.Skip(1))
                                {
                                    lname = lname + " " + d;
                                }
                            }
                            else
                            {
                                if (names.Length < 2)
                                {
                                    lname = "";
                                }
                                else
                                {
                                    lname = names[1];

                                }
                            }

                            user = new ApplicationUser()
                            {
                                Id = Guid.NewGuid(),
                                FirstName = fname,
                                LastName = lname,
                                UserName = webfloworder.CustomerInfo.Email,
                                SmsAlerts = true,
                                Email = webfloworder.CustomerInfo.Email
                            };
                            if (webfloworder.CustomData.Count() > 0)
                            {
                                user.PhoneNumber = webfloworder.CustomData[0].TextInput;
                            }
                            user = await services.CreateAtliceAccount(user, "Tourist");
                        }


                        order = new Order(webfloworder.CustomerInfo.FullName, webfloworder.CustomerInfo.FullName, webfloworder.ShippingAddress.Line1, webfloworder.ShippingAddress.City, webfloworder.ShippingAddress.State, webfloworder.ShippingAddress.PostalCode, null, webfloworder.CustomerInfo.Email, user.Id, webfloworder.StripeDetails.CustomerId)
                        {
                            OrderNumber = webfloworder.OrderId,

                            OrderRecieved = DateTime.Now
                        };

                        if (webfloworder.StripeCard != null)
                        {
                            order.Last4 = webfloworder.StripeCard.Last4.ToString();
                            order.Brand = webfloworder.StripeCard.Brand;
                            order.ChargeId = webfloworder.StripeDetails.ChargeId;
                            order.CustomerId = webfloworder.StripeDetails.CustomerId;
                            order.OwnerName = webfloworder.StripeCard.OwnerName;
                        }
                        if (webfloworder.CustomData.Count() != 0)
                            order.Phone = webfloworder.CustomData[0].TextInput;

                        if (webfloworder.CustomData.Count() > 1)
                        {
                            order.Comments = webfloworder.CustomData[1].TextArea;
                        }

                        //foreach item on receipt
                        foreach (var item in webfloworder.PurchasedItems)
                        {
                            if (item.VariantSku == "CLSCDTGCMB" || item.VariantSku == "CLSCDWCLSTG21")
                            {
                                AtliceTap? card = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.TapType == TapType.Card);
                                if (card == null)
                                {
                                    await repository.SaveTap(card = new AtliceTap
                                    {
                                        LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                                        UserId = null,
                                        ContactPage = null,
                                        Location = null,
                                        Locked = true,
                                        //same sku as webflow
                                        Sku = SKU.CLSCDWHT,
                                    });

                                }
                                AtliceTap? tag = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.TapType == TapType.Tag);
                                if (tag == null)
                                {
                                    await repository.SaveTap(card = new AtliceTap
                                    {
                                        LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                                        UserId = null,
                                        ContactPage = null,
                                        Location = null,
                                        Locked = true,
                                        //same sku as webflow
                                        Sku = SKU.CLSTG25GRY1,
                                    });

                                }
                                order.Taps.Add(card);
                                order.Taps.Add(tag);
                            }
                            else
                            {
                                AtliceTap? tap = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == (SKU)System.Enum.Parse(typeof(SKU), item.VariantSku));

                                //if found
                                if (tap != null)
                                {
                                    //add blank device to order
                                    order.Taps.Add(tap);
                                }
                                else
                                {
                                    await repository.SaveTap(tap = new AtliceTap
                                    {
                                        LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                                        UserId = null,
                                        ContactPage = null,
                                        Location = null,
                                        Locked = true,
                                        //same sku as webflow
                                        Sku = (SKU)System.Enum.Parse(typeof(SKU), item.VariantSku),
                                    });

                                    order.Taps.Add(tap);
                                }
                            }
                            //find tap with no user id, no note, and matching webflow sku

                            //if not found

                        }
                        order = await repository.SaveOrder(order);

                    }
                    else
                    {
                        order.WebflowStatus = (WebflowStatus)System.Enum.Parse(typeof(WebflowStatus), webfloworder.Status);
                        if (!string.IsNullOrEmpty(webfloworder.FulfilledOn))
                        {
                            order.OrderDelivered = DateTime.Parse(webfloworder.FulfilledOn);
                        }

                        order = await repository.SaveOrder(order);
                    }
                }
            }



            Orders = new List<AtliceOrder>();
            foreach (Order o in repository.Orders.ToList())
            {
                var user = await _userManager.FindByIdAsync(o.UserId.ToString());
                if (user != null)
                {
                    IList<string> roles = await _userManager.GetRolesAsync(user);

                    if (roles.Count > 0)
                    {
                        AtliceOrder atliceOrder = new()
                        {
                            Order = o,
                            User = user,
                            Role = roles.FirstOrDefault()
                        };
                        Orders.Add(atliceOrder);
                    }
                    else
                    {
                        AtliceOrder atliceOrder = new()
                        {
                            Order = o,
                            User = user,
                            Role = "Prospect"
                        };
                        Orders.Add(atliceOrder);

                    }
                }
            }

            Orders = Orders.OrderByDescending(x => x.Order.OrderRecieved).ToList();
        }

        public async Task<IActionResult> OnPostOrderSearch(string? name = null, string? orderNO = null)
        {
            List<Order> orders = new();
            foreach (Order order in repository.Orders.ToList())
            {
                if (order.Name != null && name != null)
                {
                    if (order.Name.ToLower().Contains(name.ToLower()))
                    {
                        orders.Add(order);
                    }
                }
                if (orderNO != null && order.OrderNumber.ToLower().StartsWith(orderNO.ToLower()))
                {
                    orders.Add(order);
                }
            }
            string view = await services.RenderToString("/pages/admin/FullfillmentList.cshtml", orders);
            return Content(view);
        }

        public IActionResult OnPostProcessOrder(Guid id)
        {
            Order? order = repository.Orders.FirstOrDefault(x => x.Id == id);
            if (order is not null)
            {
                return RedirectToPage("CustomerProfile", new { id = order.UserId });
            }
            return RedirectToPage("Fullfillment");
        }

        public async Task<IActionResult> OnPostReadyToShip(Guid id, string tracking)
        {
            Order? order = repository.Orders.FirstOrDefault(x => x.Id == id);

            if (order is not null)
            {
                foreach (var tap in order.Taps)
                {
                    tap.Note = tap.Note + "Shipped: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                    tap.Locked = false;
                    await repository.SaveTap(tap);
                }
                order.Status = OrderStatus.Shipped;
                order.Tracking = tracking;
                //contents = tracking number input replace
                var htm = await services.RenderToString("/pages/shared/emails/ordershipped.cshtml", order);

                order.Tracking = tracking;
                order.OrderShipped = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                if (!string.IsNullOrEmpty(order.Email) && !string.IsNullOrEmpty(order.Phone))
                {
                    await services.SendEmailAsync(order.Email, "Track Your Atlice Tap Shipment", htm);
                    await repository.SaveNotification(new Notification
                    {
                        Created = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time),
                        Message = "Check your inbox for an Order Tracking Email",
                        UserId = order.UserId,
                        Id = Guid.NewGuid(),
                        Type = NotificationType.AccountUpdate
                    });
                    await services.SendTextAsync(order.Phone, "Your Atlice Tap devices have been shipped! Please check your email inbox and junk folder for shipment tracking and next steps." + "\nGo to step-by-step activation guide here https://atlice.com/tap/setup on Atlice");
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent Order Shipped Text sent from fullfillment", EventType.Admin, "OrderShipped", false));

                }
                await repository.SaveOrder(order);
                return RedirectToPage("CustomerProfile", new { id = order.UserId });
            }
            else
            {
                return RedirectToPage("Fullfillment");
            }
        }

        public async Task<IActionResult> OnGetBookmark(Guid id)
        {
            Order o = repository.Orders.FirstOrDefault(x => x.Id == id);
            o.BookMarked = true;
            await repository.SaveOrder(o);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Bookmarked order number: " +o.Id, EventType.Admin, "Bookmarked", false));
            var url = "/admin/fullfillment";
            return new JsonResult(new { url });
        }

        public async Task<IActionResult> OnGetDeleteOrder(string id)
        {
            await repository.DeleteOrder(Guid.Parse(id));
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Deleted order number: " + id, EventType.Admin, "DeleteOrder", false));

            //Call Webflow site api with our token
            WebflowSharp.Services.Site.SiteService s = new("ef9c291b779b815aef5208ab5e1e17fbcdab90ef4d6fe6dd13b0379ccbf02c84");

            //gets your published sites
            var sd = await s.GetSites();

            //gets the first site, the shop
            Site? sdd = sd.FirstOrDefault();

            //Call webflow order api for site
            OrderService orderService = new("ef9c291b779b815aef5208ab5e1e17fbcdab90ef4d6fe6dd13b0379ccbf02c84");
            if (sdd is not null)
            {
                //get all orders
                var y = await orderService.GetOrders(sdd.Id);
                foreach (OrderModel webfloworder in y.Where(x => x.Status.Contains("pending") || x.Status.Contains("unfulfilled")))
                {
                    Order? order = repository.Orders.FirstOrDefault(x => x.OrderNumber == webfloworder.OrderId);
                    if (order == null)
                    {
                        ApplicationUser? user = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == webfloworder.CustomData[0].TextInput);
                        if (user is null)
                        {
                            user = await _userManager.FindByEmailAsync(webfloworder.CustomerInfo.Email);
                            if (user == null)
                            {
                                string[] names = webfloworder.CustomerInfo.FullName.Split(' ');
                                string fname = names[0];
                                string lname = "";
                                if (names.Length > 2)
                                {
                                    foreach (string d in names.Skip(1))
                                    {
                                        lname = lname + " " + d;
                                    }
                                }
                                else
                                {
                                    if (names.Length < 2)
                                    {
                                        lname = "";
                                    }
                                    else
                                    {
                                        lname = names[1];

                                    }
                                }

                                user = new ApplicationUser()
                                {
                                    Id = Guid.NewGuid(),
                                    FirstName = fname,
                                    LastName = lname,
                                    UserName = webfloworder.CustomerInfo.Email,
                                    SmsAlerts = true,
                                    PhoneNumber = webfloworder.CustomData[0].TextInput,
                                    Email = webfloworder.CustomerInfo.Email
                                };
                                user = await services.CreateAtliceAccount(user, "Tourist");
                            }
                        }
                        order = new Order(webfloworder.CustomerInfo.FullName, webfloworder.CustomerInfo.FullName, webfloworder.ShippingAddress.Line1, webfloworder.ShippingAddress.City, webfloworder.ShippingAddress.State, webfloworder.ShippingAddress.PostalCode, webfloworder.CustomData[0].TextInput, webfloworder.CustomerInfo.Email, user.Id, webfloworder.StripeDetails.CustomerId)
                        {
                            OrderNumber = webfloworder.OrderId,
                            Last4 = webfloworder.StripeCard.Last4.ToString(),
                            Brand = webfloworder.StripeCard.Brand,
                            ChargeId = webfloworder.StripeDetails.ChargeId,
                            CustomerId = webfloworder.StripeDetails.CustomerId,
                            OwnerName = webfloworder.StripeCard.OwnerName,
                            Comments = webfloworder.CustomData[1].TextArea
                        };
                        //foreach item on receipt
                        foreach (var item in webfloworder.PurchasedItems)
                        {
                            //find tap with no user id, no note, and matching webflow sku
                            AtliceTap? tap = repository.Taps.FirstOrDefault(x => x.UserId == null && x.Note == null && x.Sku == (SKU)System.Enum.Parse(typeof(SKU), item.VariantSku));

                            //if found
                            if (tap != null)
                            {
                                //add blank device to order
                                order.Taps.Add(tap);
                            }
                            //if not found
                            else
                            {
                                //auto create tap in database
                                tap = new AtliceTap
                                {
                                    LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                                    UserId = null,
                                    ContactPage = null,
                                    Location = null,
                                    Locked = true,
                                    //same sku as webflow
                                    Sku = (SKU)System.Enum.Parse(typeof(SKU), item.VariantSku),
                                };
                                tap = await repository.SaveTap(tap);
                                //add new tap to order
                                order.Taps.Add(tap);
                            }
                        }
                        order = await repository.SaveOrder(order);

                    }
                    else
                    {
                        order.WebflowStatus = (WebflowStatus)System.Enum.Parse(typeof(WebflowStatus), webfloworder.Status);
                        order.BookMarked = true;
                        order = await repository.SaveOrder(order);
                    }
                }
            }



            Orders = new List<AtliceOrder>();
            foreach (Order o in repository.Orders.ToList())
            {
                var user = await _userManager.FindByIdAsync(o.UserId.ToString());
                if (user != null)
                {
                    IList<string> roles = await _userManager.GetRolesAsync(user);

                    if (roles.Count > 0)
                    {
                        AtliceOrder atliceOrder = new()
                        {
                            Order = o,
                            User = user,
                            Role = roles.FirstOrDefault()
                        };
                        Orders.Add(atliceOrder);
                    }
                    else
                    {
                        AtliceOrder atliceOrder = new()
                        {
                            Order = o,
                            User = user,
                            Role = "Prospect"
                        };
                        Orders.Add(atliceOrder);

                    }
                }
            }

            Orders = Orders.OrderByDescending(x => x.Order.OrderRecieved).ToList();
            return Page();
        }
    }
}
