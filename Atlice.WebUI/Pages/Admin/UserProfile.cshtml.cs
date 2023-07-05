using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shippo;
using System.Data;
using Order = Atlice.Domain.Entities.Order;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class UserProfileModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public UserProfileModel(IServices _services, IDataRepository repo, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signinManager)
        {
            _signinManager = signinManager;
            services = _services;
            repository = repo;
            _userManager = userManager;
        }
        [ViewData]
        [BindProperty]
        public UserModel UserProfile { get; set; } = new UserModel();

        public class UserModel
        {
            public ApplicationUser User { get; set; } = new ApplicationUser();
            public string Role { get; set; } = string.Empty;
            public List<ContactPage> Pages { get; set; } = new List<ContactPage>();
            public List<AtliceTap> Taps { get; set; } = new List<AtliceTap>();
            public List<Order> Orders { get; set; } = new List<Order>();
            public List<Order> DeviceOrders { get; set; } = new List<Order>();
            public ApplicationUser? Affiliate { get; set; } = new ApplicationUser();
            public List<AdminNote>? Notes { get; set; } = new List<AdminNote>();
            public Chat? Chat { get; set; } = new Chat();
            public YouLoveProfile? YouLoveProfile { get; set; }
            public List<ContactList> ContactLists { get; set; } = new List<ContactList>();
            public RewardTracker? RewardTracker { get; set; }
            public Passport? Passport { get; set; }
            public int ErrorFeedbacks { get; set; }
        }

        [ViewData]
        public List<Event> Events { get; set; } = new List<Event>();

        public async Task OnGet(string id)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {

                Events = repository.Events.Where(x => x.Who == user.UserName).ToList();
                var roles = await _userManager.GetRolesAsync(user);
                UserProfile = new UserModel
                {
                    User = user,
                    Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                    Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                    DeviceOrders = new List<Order>(),
                    Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                    Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                    Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                    ContactLists = repository.ContactLists.Where(x => x.UserId == user.Id).ToList(),
                    Passport = repository.Passports.FirstOrDefault(x=>x.UserId == user.Id),
                    ErrorFeedbacks = repository.ErrorFeedbacks.Count(x=>x.Email == user.Email)
                };
                YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                UserProfile.YouLoveProfile = you;
                string? ro = roles.FirstOrDefault();
                if (ro is not null)
                {
                    UserProfile.Role = ro;
                }
                foreach (var tap in UserProfile.Taps)
                {
                    Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                    if (dbOrder is not null)
                    {
                        UserProfile.DeviceOrders.Add(dbOrder);

                    }
                }
            }


        }
        public async Task<IActionResult> OnPostCreateProfile(Guid id)
        {

            var user = await repository.CreateProfile(id);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Created a profile for user with id: " + user.Id , EventType.Admin, "CreatedProfile", false));

            return Content(user.ToString());

        }
        public async Task<IActionResult> OnPostStartBeta()
        {
            await _signinManager.SignOutAsync();
            return RedirectToPage("/Betaask/credentials_prospect");
        }
        public async Task<IActionResult> OnPostChangeEmail(string email, string UserId)
        {
            ApplicationUser? u = await _userManager.FindByIdAsync(UserId);
            if (u != null)
            {
                u.Email = email;
                u.UserName = email;
                var result = await _userManager.UpdateAsync(u);
                if (result.Succeeded)
                {
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed email for user with id: " + u.Id, EventType.Admin, "ChangeEmail", false));

                    return Content(email);

                }
                else
                {
                    var errors = result.Errors.ToList()[0];
                    if (errors is not null)
                    {
                        return Content(errors.Description);
                    }
                    return Content("Error changing email");

                }
            }
            return Content("Error changing email");

        }
        public async Task<IActionResult> OnPostChangeRole(string UserId, string role)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return Content("Not Found");
            }
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, role);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed role for user with id: " + user.Id, EventType.Admin, "ChangedRole", false));

            return Content("Role:" + role);

        }

        public async Task<IActionResult> OnPostBookMarkem(string UserId, bool bookmarked)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return Content("Not Found");
            }
            user.Bookmarked = bookmarked;
            await _userManager.UpdateAsync(user);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Bookmarked user with id: " + user.Id, EventType.Admin, "Bookmarked", false));

            string view;
            if (user.Bookmarked == true)
            {
                view = "<input type='submit' class='hidden' /><input type='hidden' value='false' name='bookmarked' checked='checked' /><a onclick='javascript: { $(this).prev().click(); $(this).prev().prev().click(); }' data-w-id='ebf40d32-69b4-db1c-da09-bc2efda9fcc6' href='#' class='bookmark w-inline-block' style='background-color: rgb(45, 179, 247);'></a>";
            }
            else
            {
                view = "<input type='submit' class='hidden' /><input type='hidden' value='true' name='bookmarked' /><a onclick='javascript: { $(this).prev().click(); $(this).prev().prev().click(); }' data-w-id='ebf40d32-69b4-db1c-da09-bc2efda9fcc6' href='#' class='bookmark w-inline-block'></a>";
            }
            return Content(view);
        }

        public async Task<IActionResult> OnPostSubmitAdminNote(Guid UserId, string who, string what)
        {
            AdminNote n = new(UserId, who, what);
            n = await repository.SaveAdminNote(n);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added admin note to user with id: " + UserId, EventType.Admin, "AddAdminNote", false));


            string view = "<strong>[" + n.Who + "]: </strong>" + n.What + "<br /><br /><div id='adminNote'></div>";
            return Content(view);
        }

        public async Task<IActionResult> OnPostDeleteDevice(Guid deviceid)
        {
            AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == deviceid);
            var oldKey = t.SNumber;
            if (t is not null)
            {
                var UserId = t.UserId;

                t.ContactPage = null;
                t.UserId = null;
                t.SNumber = Guid.NewGuid().ToString();
                t.Locked = true;
                t.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                t.Location = null;
                t.Note = "Device Wiped from key "+ oldKey +" and changed new key issued "+ t.SNumber +" ; ";
                await repository.SaveTap(t);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Removed device from user with id: " + UserId, EventType.Admin, "DeleteDevice", false));

                return RedirectToPage("UserProfile", new { id = UserId });
            }
            return RedirectToPage("/Admin/Users");

        }

        public async Task<IActionResult> OnPostUpdateTapPage(string pagetype, Guid userid, Guid deviceid)
        {
            ContactPage? p = repository.ContactPages.FirstOrDefault(x => x.PageType == (PageType)Enum.Parse(typeof(PageType), pagetype, true) && x.UserId == userid);
            AtliceTap? t = repository.Taps.FirstOrDefault(x => x.Id == deviceid);
            if (p is not null && t is not null)
            {
                t.ContactPage = p;
                await repository.SaveTap(t);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed Linked page for device with id: " + t.Id, EventType.Admin, "UpdateTapPage", false));

            }
            ApplicationUser? user = await _userManager.FindByIdAsync(userid.ToString());

            if (user is not null)
            {
                YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);

                var roles = await _userManager.GetRolesAsync(user);
                UserProfile = new UserModel
                {
                    User = user,
                    Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                    Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                    DeviceOrders = new List<Order>(),
                    Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                    Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                    Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                };
                UserProfile.YouLoveProfile = you;
                string? role = roles.FirstOrDefault();
                if (role is not null)
                {
                    UserProfile.Role = role;
                }
                foreach (var tap in UserProfile.Taps)
                {
                    Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                    if (dbOrder is not null)
                    {
                        UserProfile.DeviceOrders.Add(dbOrder);

                    }
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendEmail(string UserId, string emailType)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(UserId);
            if (user is not null && user.PhoneNumber is not null && user.Email is not null && user.FirstName is not null)
            {
               
                if (emailType == "LetterfromAlex")
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                        List<string> letterModel = new()
                        {
                            user.FirstName,
                            roles.First().ToString()
                        };
                        var em = await services.RenderToString("/pages/shared/emails/letterao.cshtml", letterModel);
                        await services.SendEmailAsync(user.Email, "A Letter from Alexander Oliver (Founder at Atlice Tap)", em);
                        List<string> data = new()
                        {
                            user.FirstName + " " + user.LastName,
                            user.PhoneNumber,
                            user.Email,
                            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToShortDateString(),
                            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).AddDays(10).ToShortDateString(),
                        };
                        if (await _userManager.IsInRoleAsync(user, "Citizen"))
                        {
                            data.Add("Citizen");
                        }
                        else
                        {
                            data.Add("Tourist");
                        }
                        var c = await services.RenderToString("/pages/shared/emails/EnrollmentCredentialsCitizenEmail.cshtml", data);
                        await services.SendEmailAsync(user.Email, "Your Enrollment Credentials for Atlice Tap V3 Beta", c);
                        await services.SendTextAsync(user.PhoneNumber, "Alexander Oliver has invited you to the Atlice Tap V3 beta. You should have recieved two emails, Enrollment Credentials and a Letter. Please check your junk mail folder.");
                        await repository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Enrollment Start Text sent from settings"));

                        user.Created = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
                        await _userManager.UpdateAsync(user);
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent: "+ emailType+" to user with id " + user.Id, EventType.Admin, "SentEmail", false));

                        return Content("Success");
                    }


                }
       
                if (emailType == "homeboard")
                {
                    var c = await services.RenderToString("/pages/shared/emails/v3guidhomeboard.cshtml", "");


                    await services.SendEmailAsync(user.Email, "V3 Guide: What’s New in Atlice Tap V3? / Home Board", c);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent: " + emailType + " to user with id " + user.Id, EventType.Admin, "SentEmail", false));

                    return Content("Success");
                }
                if (emailType == "takecontrol")
                {
                    var c = await services.RenderToString("/pages/shared/emails/v3guidetakecontrol.cshtml", "");


                    await services.SendEmailAsync(user.Email, "V3 Guide: Take Control with the Home Board", c);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent: " + emailType + " to user with id " + user.Id, EventType.Admin, "SentEmail", false));

                    return Content("Success");
                }
                if (emailType == "stats")
                {
                    List<ContactPage> pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList();
                    int linkclicks = 0;
                    int totalvisitors = 0;
                    int uniquevisitors = 0;
                    foreach (ContactPage page in pages)
                    {
                        foreach(var pageVisit in page.Visits)
                        {
                            linkclicks += repository.LinkClicks.Where(x =>x.PageVisitId == pageVisit.Id && x.TimeStamp.Month == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).AddMonths(-1).Month && x.TimeStamp.Year == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).Year).Count();
                            uniquevisitors++;
                            totalvisitors += pageVisit.Counter;
                        }
                        
                    }
                    var contactlist = repository.ContactLists.FirstOrDefault(x => x.UserId == user.Id);
                    if (contactlist != null)
                    {
                        List<string> data = new()
                {
                    pages[0].ProImage,
                    user.FirstName + " " + user.LastName,
                    totalvisitors.ToString(),
                    uniquevisitors.ToString(),
                    linkclicks.ToString(),
                    contactlist.Contacts.Count.ToString(),
                };
                        if (user.Location is not null)
                        {
                            data.Append(user.Location.Name);
                        }
                        var c = await services.RenderToString("/pages/shared/emails/stats.cshtml", data);


                        await services.SendEmailAsync(user.Email, "Your Atlice “Tap-tivity” - Stats for "+user.FirstName, c);
                        await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent: " + emailType + " to user with id " + user.Id, EventType.Admin, "SentEmail", false));

                        return Content("Success");
                    }

                }
                if(emailType == "premium")
                {
                    var em = await services.RenderToString("/pages/shared/emails/citizenfeatures.cshtml", user.FirstName);
                    await services.SendEmailAsync(user.Email, user.FirstName + ", You've Been Granted Premium Features!", em);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Sent: " + emailType + " to user with id " + user.Id, EventType.Admin, "SentEmail", false));

                    return Content("Success");
                }
            }


            return Content("Error");
        }

        public async Task<IActionResult> OnPostDeletePage(Guid pageid)
        {
            ContactPage? c = repository.ContactPages.FirstOrDefault(x => x.Id == pageid);
            var ts = repository.Taps.Where(x => x.ContactPage == c).ToList();
            if (ts is not null)
            {
                foreach (var t in ts)
                {
                    t.ContactPage = null;
                    await repository.SaveTap(t);
                }

            }

            if (c is not null)
            {
                var userid = c.UserId.ToString();
                await repository.DeleteContactPage(c);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Deleted Contact page for user with id " + userid, EventType.Admin, "DeletePage", false));

                if (userid != null)
                {
                    ApplicationUser? user = await _userManager.FindByIdAsync(userid);
                    if (user is not null)
                    {

                        var roles = await _userManager.GetRolesAsync(user);
                        UserProfile = new UserModel
                        {
                            User = user,
                            Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                            Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                            DeviceOrders = new List<Order>(),
                            Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                            Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                            Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                        };
                        YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                        UserProfile.YouLoveProfile = you;
                        string? role = roles.FirstOrDefault();
                        if (role is not null)
                        {
                            UserProfile.Role = role;
                        }
                        foreach (var tap in UserProfile.Taps)
                        {
                            Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                            if (dbOrder is not null)
                            {
                                UserProfile.DeviceOrders.Add(dbOrder);

                            }
                        }
                    }
                }



            }


            return Page();
        }

        public async Task<IActionResult> OnPostDeleteContactList(Guid listid)
        {
            ContactList? c = repository.ContactLists.FirstOrDefault(x => x.Id == listid);

            if (c is not null)
            {
                var userid = c.UserId.ToString();
                if (userid != null)
                {
                    ApplicationUser? user = await _userManager.FindByIdAsync(userid);

                    foreach (var contact in c.Contacts)
                    {
                        await repository.DeleteContact(contact);
                    }
                    await repository.DeleteContactList(c);


                    if (user is not null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        UserProfile = new UserModel
                        {
                            User = user,
                            Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                            Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                            DeviceOrders = new List<Order>(),
                            Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                            Affiliate = _userManager.Users.FirstOrDefault(x => x.InviteCode == user.InviteCode),
                            Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                        };
                        YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                        UserProfile.YouLoveProfile = you;
                        string? role = roles.FirstOrDefault();
                        if (role is not null)
                        {
                            UserProfile.Role = role;
                        }
                        foreach (var tap in UserProfile.Taps)
                        {
                            Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                            if (dbOrder is not null)
                            {
                                UserProfile.DeviceOrders.Add(dbOrder);

                            }
                        }
                    }

                }

            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUser(Guid UserId)
        {

            await services.DeleteAccount(UserId);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Deleted account of user with id " + UserId, EventType.Admin, "DeleteUser", false));

            return RedirectToPage("/Admin/Index");
        }

        public async Task<IActionResult> OnPostAttachToOrder(string orderid, string userid)
        {
            Order? o = repository.Orders.FirstOrDefault(x => x.Id.ToString() == orderid);
            List<Order> existingOrders = repository.Orders.Where(x => x.UserId.ToString() == userid).ToList();
            existingOrders.Remove(o);
            foreach(var order in existingOrders)
            {
                foreach(var device in order.Taps)
                {
                    order.Taps.Remove(device);
                    await repository.SaveOrder(order);
                    o.Taps.Add(device);
                    await repository.SaveOrder(o);
                }
            }
            return RedirectToPage("UserProfile", new { id = userid });
        }

        public async Task<IActionResult> OnPostAddPhone(Guid userid, string phone)
        {
            ApplicationUser? user = _userManager.Users.FirstOrDefault(x => x.Id == userid);

            if(user is not null)
            {
                ApplicationUser? phoneCheck = _userManager.Users.FirstOrDefault(x => x.PhoneNumber == phone);
                if(phoneCheck is null)
                {
                    user.PhoneNumber = phone;
                    await _userManager.UpdateAsync(user);
                    await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added Phone to user with id " + user.Id, EventType.Admin, "AddPhone", false));

                    if (user is not null)
                    {

                        var roles = await _userManager.GetRolesAsync(user);
                        UserProfile = new UserModel
                        {
                            User = user,
                            Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                            Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                            DeviceOrders = new List<Order>(),
                            Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                            Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                            Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                            ContactLists = repository.ContactLists.Where(x => x.UserId == user.Id).ToList(),
                            Passport = repository.Passports.FirstOrDefault(x => x.UserId == user.Id)
                        };
                        YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                        UserProfile.YouLoveProfile = you;
                        string? ro = roles.FirstOrDefault();
                        if (ro is not null)
                        {
                            UserProfile.Role = ro;
                        }
                        foreach (var tap in UserProfile.Taps)
                        {
                            Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                            if (dbOrder is not null)
                            {
                                UserProfile.DeviceOrders.Add(dbOrder);

                            }
                        }
                    }
                    return Page();
                }
            }
            TempData["Error"] = "Phone number in use already!";
            return Page();
        }

        public async Task<IActionResult> OnPostAddRemoveError(Guid userid, string value)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userid.ToString());
            if(value == "1")
            {
                ErrorFeedback errorFeedback = new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Comment = "Admin Generated",
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    RequestId = "000",
                    TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime()
                };
                await repository.SaveErrorFeedback(errorFeedback);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added Bounty to user with id " + user.Id, EventType.Admin, "Bounty", false));

                int counter = repository.ErrorFeedbacks.Count(x => x.Email == user.Email);
            }
            else
            {
                ErrorFeedback? errorFeedback = repository.ErrorFeedbacks.OrderByDescending(x=>x.TimeStamp).FirstOrDefault(x => x.Email == user.Email);
                if (errorFeedback is not null)
                    await repository.DeleteErrorFeedback(errorFeedback.Id);
                int counter = repository.ErrorFeedbacks.Count(x => x.Email == user.Email);
            }
            var roles = await _userManager.GetRolesAsync(user);
            UserProfile = new UserModel
            {
                User = user,
                Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                DeviceOrders = new List<Order>(),
                Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                ContactLists = repository.ContactLists.Where(x => x.UserId == user.Id).ToList(),
                Passport = repository.Passports.FirstOrDefault(x => x.UserId == user.Id),
                ErrorFeedbacks = repository.ErrorFeedbacks.Count(x => x.Email == user.Email)
            };
            YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
            UserProfile.YouLoveProfile = you;
            string? ro = roles.FirstOrDefault();
            if (ro is not null)
            {
                UserProfile.Role = ro;
            }
            foreach (var tap in UserProfile.Taps)
            {
                Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                if (dbOrder is not null)
                {
                    UserProfile.DeviceOrders.Add(dbOrder);

                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddStamp(Guid userid, string stampName)
        {
            Passport? pp = repository.Passports.FirstOrDefault(x=>x.UserId==userid);
            if(pp is not null)
            {
                pp.Stamps.Add(await repository.SaveStamp(new Stamp(stampName, Platform.Atlice, pp.Id)));
                await repository.SavePassport(pp);
                await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added Stamp to Passport of user with id " + userid, EventType.Admin, "AddStamp", false));

            }

            ApplicationUser? user = await _userManager.FindByIdAsync(userid.ToString());

            if (user is not null)
            {

                var roles = await _userManager.GetRolesAsync(user);
                UserProfile = new UserModel
                {
                    User = user,
                    Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                    Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                    DeviceOrders = new List<Order>(),
                    Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                    Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                    Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                    ContactLists = repository.ContactLists.Where(x => x.UserId == user.Id).ToList(),
                    Passport = repository.Passports.FirstOrDefault(x => x.UserId == user.Id),
                    ErrorFeedbacks = repository.ErrorFeedbacks.Count(x => x.Email == user.Email)
                };
                YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                UserProfile.YouLoveProfile = you;
                string? ro = roles.FirstOrDefault();
                if (ro is not null)
                {
                    UserProfile.Role = ro;
                }
                foreach (var tap in UserProfile.Taps)
                {
                    Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                    if (dbOrder is not null)
                    {
                        UserProfile.DeviceOrders.Add(dbOrder);

                    }
                }
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAddContactPage(Guid userid, PageType pageType)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userid.ToString());
            ContactPage business = new() { ProImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", PreImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", UserId = userid, Name = user.FirstName, PhoneNumber = user.PhoneNumber, Email = user.Email, PageType = pageType, EmailPreview = true, PhonePreview = true, WebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite), SubConpreview = false, SubmitContact = false, SaveToContacts = true, SaveToContactsPreview = true, NoteToSelf = false, NotetoSelfPreview = true, VPhonePreview = true, VEmailPreview = true, VWebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite) };
            await repository.SaveContactPage(business);
            await repository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Added Contact page to user with id " + user.Id, EventType.Admin, "AddContactPage", false));

            var roles = await _userManager.GetRolesAsync(user);
            UserProfile = new UserModel
            {
                User = user,
                Pages = repository.ContactPages.Where(x => x.UserId == user.Id).ToList(),
                Orders = repository.Orders.Where(x => x.UserId == user.Id).ToList(),
                DeviceOrders = new List<Order>(),
                Taps = repository.Taps.Where(x => x.UserId == user.Id).ToList(),
                Affiliate = _userManager.Users.FirstOrDefault(x => x.Id == user.AffiliateId),
                Notes = repository.AdminNotes.Where(x => x.UserId == user.Id).ToList(),
                ContactLists = repository.ContactLists.Where(x => x.UserId == user.Id).ToList(),
                Passport = repository.Passports.FirstOrDefault(x => x.UserId == user.Id),
                ErrorFeedbacks = repository.ErrorFeedbacks.Count(x => x.Email == user.Email)
            };
            YouLoveProfile? you = repository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
            UserProfile.YouLoveProfile = you;
            string? ro = roles.FirstOrDefault();
            if (ro is not null)
            {
                UserProfile.Role = ro;
            }
            foreach (var tap in UserProfile.Taps)
            {
                Order? dbOrder = repository.Orders.FirstOrDefault(x => x.Taps.Contains(tap));
                if (dbOrder is not null)
                {
                    UserProfile.DeviceOrders.Add(dbOrder);

                }
            }
            return Page();
        }
    }
}
