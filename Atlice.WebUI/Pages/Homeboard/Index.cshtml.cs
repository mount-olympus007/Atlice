using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Atlice.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Nethereum.ABI.Encoders;
using Newtonsoft.Json;
using System.Drawing;
using Wangkanai.Extensions;
using Location = Atlice.Domain.Entities.Location;

namespace Atlice.WebUI.Pages.Homeboard
{
    [Authorize(Roles = "Citizen,Tourist,Adminis")]
    public class IndexModel : PageModel
    {
        private readonly IDataRepository _dataRepository;
        private readonly IServices _services;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public IndexModel(IDataRepository dataRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IServices services, IHttpContextAccessor httpContextAccessor)
        {
            _services = services;
            _userManager = userManager;
            _signInManager = signInManager;
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [ViewData]
        public ApplicationUser Me { get; set; } = new ApplicationUser();
        [ViewData]
        public List<ContactPage> ContactPages { get; set; } = new List<ContactPage>();
        [ViewData]
        public QRModel QRCodeModel { get; set; } = new QRModel();
        [ViewData]
        public int TotalVisits { get; set; } = 0;
        [ViewData]
        public int UniqueVisits { get; set; } = 0;
        [ViewData]
        public int LinkClicks { get; set; } = 0;
        [ViewData]
        public List<AtliceTap> AtliceTaps { get; set; } = new List<AtliceTap>();
        [ViewData]
        public Chat Chat { get; set; } = new Chat();
        [ViewData]
        public Passport? Passport { get; set; } = new Passport(Guid.NewGuid());
        [ViewData]
        public List<Notification> Notifications { get; set; } = new List<Notification>();
        [ViewData]
        public List<News> NewsStories { get; set; } = new List<News>();
        [ViewData]
        public List<ContactModel> ContactList { get; set; } = new List<ContactModel>();
        [ViewData]
        public List<ErrorFeedback> errorFeedbacks { get; set; } = new List<ErrorFeedback>();
        [ViewData]
        public List<Order> NotActiveOrders { get; set; } = new List<Order>();
        

        public class ContactModel
        {
            public Contact Contact { get; set; } = new Contact();
            public ApplicationUser? User { get; set; }
            public string? Role { get; set; }
            public bool CitizenTrack { get; set; }
        }

        [ViewData]
        public List<TapViewModel> TapViewModels { get; set; } = new List<TapViewModel>();

        public class TapViewModel
        {
            public AtliceTap AtliceTap { get; set; }
            public byte[]? QR { get; set; }
        }

        public class QRModel
        {
            public ContactPage ContactPage { get; set; } = new ContactPage();
            public byte[] QR { get; set; } = Array.Empty<byte>();
        }
        public class OpenTapLinkModel
        {
            public LinkType LinkType { get; set; }
            public SocialProvider SocialProvider { get; set; }
            public string? ImageUrl { get; set; }
            public Guid? PageId { get; set; }
        }
        public async Task<IActionResult> OnGet()
        {
            var me = await _userManager.GetUserAsync(User);
            if (me is not null)
                Me = me;
            if (me.Email.ToLower().Contains("atlicetap"))
            {
                me.Email = "";
            }
            ContactPages = new List<ContactPage>();
            Passport = _dataRepository.Passports.FirstOrDefault(x => x.UserId == me.Id);
            List<ContactPage>? pages = new(_dataRepository.ContactPages.Where(x => x.UserId == Me.Id));
            List<AtliceTap>? taps = new(_dataRepository.Taps.Where(x => x.UserId == Me.Id));
            ContactPage? personal = pages.FirstOrDefault(x => x.PageType == PageType.Personal);
            ContactPage? business = pages.FirstOrDefault(x => x.PageType == PageType.Business);
            ContactPage? professional = pages.FirstOrDefault(x => x.PageType == PageType.Professional);
            ContactList? contactList = _dataRepository.ContactLists.FirstOrDefault(x => x.UserId == Me.Id);
            errorFeedbacks = _dataRepository.ErrorFeedbacks.Where(x => x.Email == me.Email).ToList();
            Chat? chat = _dataRepository.Chats.FirstOrDefault(x => x.UserId == Me.Id);
            NotActiveOrders = _dataRepository.Orders.Where(x => x.Status == OrderStatus.Shipped && x.UserId == me.Id && (DateTime.Now - x.OrderShipped).Days >= 10).ToList();
            if (personal is not null && business is not null && professional is not null && contactList is not null && taps is not null && chat is not null)
            {                
                ContactPages.Add(personal);
                ContactPages.Add(business);
                ContactPages.Add(professional);
                if (pages.Count() > 3)
                {
                    foreach(var p in pages)
                    {
                        if (!ContactPages.Contains(p))
                        {
                            ContactPages.Add(p);
                        }
                    }
                }
                
                TotalVisits = professional.Visits.Count + business.Visits.Count + personal.Visits.Count;
                UniqueVisits = professional.Visits.Select(x => x.Ip).Distinct().ToList().Count + business.Visits.Select(x => x.Ip).Distinct().ToList().Count + personal.Visits.Select(x => x.Ip).Distinct().ToList().Count;
                foreach (var p in ContactPages)
                {
                    foreach (var l in p.TapLinks)
                    {
                        LinkClicks += l.LinkClicks;
                    }
                }
                foreach(var contact in contactList.Contacts)
                {
                    if(contact.ApplicationUserID is not null)
                    {
                        ApplicationUser? u = await _userManager.FindByIdAsync(contact.ApplicationUserID);                      
                        if (u != null)
                        {
                            var roles = await _userManager.GetRolesAsync(u);
                            bool citizentrack = _dataRepository.AdminNotes.Any(x => x.Who == u.PhoneNumber && x.What == "Promoted this mobile device to Citizenship");
                            ContactList.Add(new ContactModel
                            {
                                Contact = contact,
                                User = await _userManager.FindByIdAsync(contact.ApplicationUserID),
                                Role = roles.FirstOrDefault(),
                                CitizenTrack = citizentrack
                            });
                        }
                    }
                    
                    
                }
                AtliceTaps = taps;
                if (AtliceTaps.Count > 0)
                {
                    TempData["deviceid"] = taps[0].Id;
                }
                foreach(var tap in AtliceTaps)
                {
                    TapViewModel tapViewModeltapViewModel = new()
                    {
                        AtliceTap= tap
                    };
                    if(tap.TapType == TapType.Virtual)
                    {
                        tapViewModeltapViewModel.QR = _services.BitmapToBytesCode(_services.GenerateQR(200, 200, "https://atlice.com/tap/" + tap.SNumber[..8]));
                    }
                    TapViewModels.Add(tapViewModeltapViewModel);
                }
                
                Chat = chat;
                Notifications = _dataRepository.Notifications.Where(x => x.UserId == Me.Id).ToList();
                NewsStories = _dataRepository.News.Take(3).ToList();
                return Page();
            }
            return RedirectToPage("Error");




        }
        public async Task<IActionResult> OnGetUpdateContactList()
        {
            var me = await _userManager.GetUserAsync(User);
            ContactList? dbcontactList = _dataRepository.ContactLists.FirstOrDefault(x => x.UserId == me.Id);

            List<ContactModel> contactList = new List<ContactModel>();
            foreach (var contact in dbcontactList.Contacts)
            {
                if (contact.ApplicationUserID is not null)
                {
                    ApplicationUser? u = await _userManager.FindByIdAsync(contact.ApplicationUserID);
                    if (u != null)
                    {
                        var roles = await _userManager.GetRolesAsync(u);
                        bool citizentrack = _dataRepository.AdminNotes.Any(x => x.Who == u.PhoneNumber && x.What == "Promoted this mobile device to Citizenship");
                        contactList.Add(new ContactModel
                        {
                            Contact = contact,
                            User = await _userManager.FindByIdAsync(contact.ApplicationUserID),
                            Role = roles.FirstOrDefault(),
                            CitizenTrack = citizentrack
                        });
                    }
                }


            }
            string view = await _services.RenderToString("/pages/homeboard/contactlistpartial.cshtml",contactList);
            return new JsonResult(new { view });
        }
        public async Task<IActionResult> OnGetUpdateAccount(string value)
        {
            string view = await _services.RenderToString("/pages/homeboard/modal_update_account.cshtml", value);
            view = view.Replace(System.Environment.NewLine, string.Empty);

            return new JsonResult(new { view });
        }
        public async Task<IActionResult> OnGetSendText(string value)
        {
            TempData["mobiletoconfirm"] = value;
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user is not null && user.PhoneNumber is not null)
            {
                var codePhone = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

                await _services.SendTextAsync(value, "Your security code is: " + codePhone);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Attempting to change mobile number", EventType.User, "SendConfirmPhoneText", false));

                string view = await _services.RenderToString("/pages/homeboard/confirm_auth_partial.cshtml", "mobile");

                view = view.Replace(System.Environment.NewLine, string.Empty);

                return new JsonResult(new { view });
            }
            return new JsonResult(new { view = "Error" });

        }
        public async Task<IActionResult> OnGetSendEmail(string value)
        {
            TempData["emailtoconfirm"] = value;
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user is not null && user.PhoneNumber is not null)
            {
                var codeEmail = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                await _services.SendEmailAsync(value, "Confirm Your New Email Address", "Your security code is: " + codeEmail);
                await _dataRepository.SaveNotification(new Notification
                {
                    Created= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time),
                    Message = "Email Confimation sent to you inbox at " + value,
                    UserId= user.Id,
                    Id = Guid.NewGuid(),
                    Type = NotificationType.AccountUpdate
                });
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Attempting to change email address", EventType.User, "SendConfirmEmailMessage", false));

                string view = await _services.RenderToString("/pages/homeboard/confirm_auth_partial.cshtml", "email");

                view = view.Replace(Environment.NewLine, string.Empty);

                return new JsonResult(new { view });
            }
            return new JsonResult(new { view = "No User" });
        }
        public async Task<IActionResult> OnGetSaveAccountState(string value, string name)
        {
            ApplicationUser? applicationUser = await _userManager.GetUserAsync(User);
            if(applicationUser is not null)
            {
                if (name == "fullname")
                {
                    string[] names = value.Split(' ');
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
                    applicationUser.FirstName = fname;
                    applicationUser.LastName = lname;
                    await _userManager.UpdateAsync(applicationUser);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed their full name", EventType.User, "SaveAccountState", false));

                    return new JsonResult(new { view = applicationUser.FirstName + " " + applicationUser.LastName });

                }
                if (name == "codephone")
                {
                    var m = TempData["mobiletoconfirm"];
                    if (m is not null)
                    {
                        var temp = m.ToString();
                        if (temp is not null)
                        {
                            var result = await _userManager.ChangePhoneNumberAsync(applicationUser, temp, value);
                            if (result.Succeeded)
                            {
                                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Changed mobile number and confirmed", EventType.User, "SaveAccountState", false));

                                return new JsonResult(new { view = applicationUser.PhoneNumber });

                            }
                            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Failed mobile confirmation", EventType.User, "SaveAccountState", true));

                            return new JsonResult(new { view = applicationUser.PhoneNumber + " Phone number not confirmed" });
                        }
                    }
                    else
                    {
                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Failed mobile confirmation", EventType.User, "SaveAccountState", true));

                        return new JsonResult(new { view = applicationUser.PhoneNumber + " Phone number not confirmed" });
                    }
                }
                if (name == "codeEmail")
                {
                    var e = TempData["emailtoconfirm"];
                    if (e is not null && applicationUser.PhoneNumber is not null)
                    {
                        var result = await _userManager.ChangePhoneNumberAsync(applicationUser, applicationUser.PhoneNumber, value);
                        if (result.Succeeded)
                        {
                            applicationUser.Email = e.ToString();
                            await _userManager.UpdateAsync(applicationUser);
                            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Changed email address and confirmed", EventType.User, "SaveAccountState", false));

                            return new JsonResult(new { view = applicationUser.Email });
                        }
                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Failed email confirmation", EventType.User, "SaveAccountState", true));

                        return new JsonResult(new { view = applicationUser.Email + " Email not confirmed" });
                    }
                    else
                    {
                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Failed email confirmation", EventType.User, "SaveAccountState", true));

                        return new JsonResult(new { view = applicationUser.Email + " Email not confirmed" });

                    }
                }
                if (name == "SMS")
                {
                    applicationUser.SmsAlerts = value == "Yes";
                    await _userManager.UpdateAsync(applicationUser);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Changed sms alert status", EventType.User, "SaveAccountState", false));

                    return value == "Yes" ? new JsonResult(new { view = "SMS alerts enabled" }) : new JsonResult(new { view = "SMS alerts disabled" });

                }
            }
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Failed to save account state", EventType.User, "SaveAccountState", true));

            return new JsonResult(new { view = "" });


        }
        public async Task<IActionResult> OnGetPublish(Guid id)
        {
            ContactPage? page = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if (page != null)
            {
                page.PhonePublished = page.PhonePreview;
                page.EmailPublished = page.EmailPreview;
                page.WebsitePublished = page.WebsitePreview;
                page.SmsAlerts = page.SmsPreview;
                page.SubmitContact = page.SubConpreview;
                page.NoteToSelf = page.NotetoSelfPreview;
                page.SaveToContacts = page.SaveToContactsPreview;
                page.VPhone = page.VPhonePreview;
                page.VEmail = page.VEmailPreview;
                page.VWebsite = page.VWebsitePreview;
                page.LocationPublished = page.LocationPreview;
                page.Grid = page.GridPreview;
                page.VPhone = page.VPhonePreview;
                page.VEmail = page.VEmailPreview;
                page.VWebsite = page.VWebsitePreview;
                page.ProImage = page.PreImage;
                foreach (var link in page.TapLinks)
                {
                    if (link.ShowPreview)
                    {
                        link.ContactPage = true;
                    }
                    else
                    {
                        link.ContactPage = false;
                    }
                    if (link.VPreview)
                    {
                        link.VCard = true;
                    }
                    else
                    {
                        link.VCard = false;
                    }
                    await _dataRepository.SaveTapLink(link);
                }
                page = await _dataRepository.SaveContactPage(page);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Published "+page.PageType+" page", EventType.User, "Publish", false));

                TempData["JustPublished"] = "true";
                return RedirectToPage("/Tap/Index", new { id = page.Id });
            }
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user is not null && user.UserName is not null)
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Error Publishing " + page.PageType + " page", EventType.User, "Publish", true));

            TempData["Error"] = "Error Publishing Page, Admin Notified!";
            return Page();

        }
        public async Task<IActionResult> OnGetPreview(Guid id)
        {
            ContactPage? page = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if (page != null)
            {
                string view = await _services.RenderToString("/pages/homeboard/preview_wrapper.cshtml", page);

                view = view.Replace(System.Environment.NewLine, string.Empty);

                return new JsonResult(new
                {
                    view
                });
            }
            
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + "Error loading " + page.PageType + " page", EventType.User, "Preview", true));

            TempData["Error"] = "Error Loading preview, Admin Notified!";
            return Page();
        }
        public async Task<IActionResult> OnGetTapPageSelect(Guid pageid, Guid tapid)
        {
            AtliceTap? atliceTap = _dataRepository.Taps.FirstOrDefault(x => x.Id == tapid);
            ContactPage? contactPage = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == pageid);
            if (atliceTap is null || contactPage is null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user is not null && user.UserName is not null)
                {
                    await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Error selecting page for tap"));

                }

                TempData["Error"] = "Error selecting page for tap, Admin Notified!";
                return Page();
            }
            else
            {
                atliceTap.Bypass = false;
                atliceTap.BypassURL = null;
                atliceTap.BypassSocialProvider = "";
                atliceTap.BypassImage = "";
                atliceTap.ContactPage = contactPage;
                await _dataRepository.SaveTap(atliceTap);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Swapped to " + contactPage.PageType + " page for device with id: "+atliceTap.Id, EventType.User, "DevicePageSelect", false));

                TapViewModel t = new TapViewModel
                {
                    AtliceTap = atliceTap,
                };
                if(atliceTap.TapType == TapType.Virtual)
                {
                    t.QR = _services.BitmapToBytesCode(_services.GenerateQR(200, 200, "https://atlice.com/tap/" + atliceTap.SNumber[..8]));
                }
                string view = await _services.RenderToString("/pages/homeboard/device_partial.cshtml", t);
                view = view.Replace(Environment.NewLine, string.Empty);
                return new JsonResult(new { view });
            }


        }
        public async Task<IActionResult> OnGetLoadBypassLinks(Guid pageid, Guid tapid)
        {
            AtliceTap? atliceTap = _dataRepository.Taps.FirstOrDefault(x => x.Id == tapid);
            ContactPage? contactPage = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == pageid);
            if (atliceTap is null || contactPage is null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user != null && user.UserName is not null)
                {
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Error loading bypass links bypass for device with id: " + atliceTap.Id, EventType.User, "LoadBypassLnks", true));

                }
                TempData["Error"] = "Error loading bypass links, Admin Notified!";
                return Page();
            }
            else
            {
                TempData["deviceid"] = atliceTap.Id;
                string view = await _services.RenderToString("/pages/homeboard/bypass_links.cshtml", contactPage);
                view = view.Replace(Environment.NewLine, string.Empty);
                return new JsonResult(new
                {
                    view,
                    tapid
                });
            }

        }
        public async Task<IActionResult> OnGetBypass(Guid deviceid, Guid linkid)
        {
            AtliceTap? atliceTap = _dataRepository.Taps.FirstOrDefault(x => x.Id == deviceid);
            TapLink? tapLink = _dataRepository.TapLinks.FirstOrDefault(x => x.Id == linkid);
            if(atliceTap is null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if(user is not null && user.UserName is not null)
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Error loading device for bypass with id: " + atliceTap.Id, EventType.User, "Bypass", true));

                TempData["Error"] = "Error loading device, Admin Notified!";
                return Page();
            }
            else
            {
                if (tapLink is null)
                {
                    atliceTap.Bypass = false;
                    atliceTap.BypassURL = null;
                    atliceTap.BypassSocialProvider = "";
                    atliceTap.BypassImage = "";
                    atliceTap = await _dataRepository.SaveTap(atliceTap);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Removed bypass for device with id: " + atliceTap.Id, EventType.User, "Bypass", false));

                    string view = await _services.RenderToString("/pages/homeboard/indicator_block.cshtml", atliceTap);
                    view = view.Replace(Environment.NewLine, string.Empty);
                    return new JsonResult(new { view });
                }
                else
                {
                    atliceTap.Bypass = true;
                    atliceTap.BypassURL = tapLink.SocialProviderMainUrl;
                    atliceTap.BypassSocialProvider = tapLink.SocialProvider.ToString();
                    atliceTap.BypassImage = tapLink.GetLogo(tapLink.SocialProvider);
                    atliceTap = await _dataRepository.SaveTap(atliceTap);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Bypass to "+tapLink.SocialProvider+" for device with id: " + atliceTap.Id, EventType.User, "Bypass", false));

                    string view = await _services.RenderToString("/pages/homeboard/indicator_block.cshtml", atliceTap);
                    view = view.Replace(Environment.NewLine, string.Empty);
                    return new JsonResult(new { view });
                }
            }
        }
        public async Task<IActionResult> OnGetLockAllDevices(string value)
        {
            List<AtliceTap> taps = new();
            ApplicationUser? u = await _userManager.GetUserAsync(User);
            if(u is not null)
            {
                taps = _dataRepository.Taps.Where(x => x.UserId == u.Id).ToList();
            }
            foreach (var tap in taps)
            {
                if (value.Contains("w--redirected-checked"))
                {
                    tap.Locked = false;
                }
                else
                {
                    tap.Locked = true;
                }
                await _dataRepository.SaveTap(tap);
            }
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Locked all devices", EventType.User, "LockAllDevices", false));

            string view = await _services.RenderToString("/pages/homeboard/locksUpdate.cshtml", taps);

            return new JsonResult(new { view });
        }
        public async Task<IActionResult> OnGetLockDevice(Guid id)
        {
            AtliceTap? atliceTap = _dataRepository.Taps.FirstOrDefault(x => x.Id == id);
            if (atliceTap == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user is not null && user.UserName is not null)
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Tried locking device with id: "+atliceTap.Id, EventType.User, "LockDevice", true));

                TempData["Error"] = "Device not found, Admin Notified!";
                return Page();
            }
            if (atliceTap.Locked)
            {
                atliceTap.Locked = false;
                await _dataRepository.SaveTap(atliceTap);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Unlocked device with id: " + atliceTap.Id, EventType.User, "LockDevice", false));

                string view = await _services.RenderToString("/pages/homeboard/indicator_block.cshtml", atliceTap);
                view = view.Replace(Environment.NewLine, string.Empty);
                return new JsonResult(new { view});

            }
            else
            {
                atliceTap.Locked = true;
                await _dataRepository.SaveTap(atliceTap);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Locked device with id: " + atliceTap.Id, EventType.User, "LockDevice", false));

                string view = await _services.RenderToString("/pages/homeboard/indicator_block.cshtml", atliceTap);
                view = view.Replace(Environment.NewLine, string.Empty);
                return new JsonResult(new { view});

            }
        }
        public async Task<IActionResult> OnGetEditDeviceState(Guid id, string propname, string value)
        {
            AtliceTap? tap = _dataRepository.Taps.FirstOrDefault(x => x.Id == id);
            if (tap == null)
            {
                
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Tried editing device with id: " + tap.Id+" but it was not found", EventType.User, "EditDeviceState", true));

                TempData["Error"] = "Error editing device, Admin Notified!";
                return Page();
            }
            else
            {
                if (propname == "CustomName")
                {
                    tap.CustomName = !string.IsNullOrEmpty(value) ? value : tap.GetOfficialName(tap.Sku);
                    await _dataRepository.SaveTap(tap);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Edited device "+propname+" with id: " + tap.Id , EventType.User, "EditDeviceState", false));

                    return new JsonResult(new { status = tap.CustomName });
                }
                var prop = tap.GetType().GetProperty(propname);
                if(prop is not null)
                {
                    prop.SetValue(value, true);
                    await _dataRepository.SaveTap(tap);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Edited device " + propname + " with id: " + tap.Id , EventType.User, "EditDeviceState", false));

                    return new JsonResult(new { status = value });
                }
                else
                {
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error editing device with id: " + tap.Id , EventType.User, "EditDeviceState", true));

                    TempData["Error"] = "Error editing device, Admin Notified!";
                    return Page();
                }
            }
        }
        public async Task<IActionResult> OnGetPageQR(Guid id, Guid? virtualid)
        {
            if(virtualid is not null)
            {
                AtliceTap t = _dataRepository.Taps.FirstOrDefault(x=>x.Id == virtualid);
                QRModel model = new()
                {
                    ContactPage = t.ContactPage,
                    QR = _services.BitmapToBytesCode(_services.GenerateQR(200, 200, "https://atlice.com/tap/" + t.SNumber[..8]))
                };
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Displayed QR Code for virtual device ", EventType.User, "GetQRCode", false));
                string view = await _services.RenderToString("/pages/homeboard/qr_wrapper.cshtml", model);
                return new JsonResult(new { view });
            }
            ContactPage? c = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if(c == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user is not null && user.UserName is not null)
                    await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Error loading QR code"));
                TempData["Error"] = "Error loading QR code, Admin Notified!";
                return Page();
            }
            else
            {
                QRModel model = new()
                {
                    ContactPage = c,
                    QR = _services.BitmapToBytesCode(_services.GenerateQR(200, 200, "https://atlice.com/tap/" + c.Id))
                };
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Displayed QR Code for " +c.PageType+" page", EventType.User, "GetQRCode", false));
                string view = await _services.RenderToString("/pages/homeboard/qr_wrapper.cshtml", model);
                return new JsonResult(new { view });
            }
            
        }
        public async Task<IActionResult> OnGetLoadEditor(Guid id)
        {
            ContactPage? c = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if( c == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user is not null && user.UserName is not null)
                    await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Error loading editor"));
                TempData["Error"] = "Error loading editor, Admin Notified!";
                return Page();
            }
            else
            {
                string view = await _services.RenderToString("/pages/homeboard/editor.cshtml", c);
                string preview = await _services.RenderToString("/pages/homeboard/preview_wrapper.cshtml", c);
                return new JsonResult(new { view, preview });
            }
            
        }
        public async Task<IActionResult> OnPostUpdateImage(Guid pid, byte[] data)
        {
            ContactPage? p = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == pid);
            if(p == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user is not null && user.UserName is not null)
                    await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Error updating image"));
                TempData["Error"] = "Error updating image, Admin Notified!";
                return Page();
            }
            string filename = p.PageType.ToString() + p.UserId + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime().Second+ "secs.jpg";
            p.PreImage = await _services.UploadPhotoStreamToCloud(data, filename);
            await _dataRepository.SaveContactPage(p);
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Updated the image for their "+p.Id+" page", EventType.User, "GetQRCode", true));

            return Content("Saved!");
        }
        public async Task<IActionResult> OnGetLayoutSwap(Guid id, string choice)
        {
            ContactPage? contactPage = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if(contactPage == null)
            {
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error loading their contact page, it was not found", EventType.User, "ChangeLayout", true));
                TempData["Error"] = "Error loading editor, Admin Notified!";
                return Page();
            }
            else
            {
                if (choice == "true")
                {
                    contactPage.GridPreview = true;
                    await _dataRepository.SaveContactPage(contactPage);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Switched preview to grid", EventType.User, "ChangeLayout", false));

                    string view = await _services.RenderToString("/pages/homeboard/grid_partial.cshtml", contactPage);
                    view = view.Replace(Environment.NewLine, string.Empty);
                    return new JsonResult(new { view });
                }
                else
                {
                    contactPage.GridPreview = false;
                    await _dataRepository.SaveContactPage(contactPage);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Switched preview to list", EventType.User, "ChangeLayout", false));

                    string view = await _services.RenderToString("/pages/homeboard/list_partial.cshtml", contactPage);
                    view = view.Replace(Environment.NewLine, string.Empty);
                    return new JsonResult(new { view });
                }
            }
            
        }
        public async Task<IActionResult> OnGetUpdateVcardLinks(Guid pageId)
        {
            ContactPage? contactPage = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == pageId);
            if(contactPage == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if(user is not null && user.UserName is not null)
                    await _dataRepository.SaveAdminNote(new AdminNote(user.Id, user.UserName, "Error updating vcard"));
                TempData["Error"] = "Error updating vcard, Admin Notified!";
                return Page();
            }
            else
            {
                string view = await _services.RenderToString("/pages/homeboard/vcard_partial.cshtml", contactPage);
                view = view.Replace(Environment.NewLine, string.Empty);
                return new JsonResult(new { view });
            }
            
        }
        public async Task<IActionResult> OnGetAddTapLink(LinkType linkType, SocialProvider socialProvider, Guid pageId)
        {
            ContactPage? p = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == pageId);
            if (ModelState.IsValid && p is not null)
            {
                TapLink t = new()
                {
                    Id = Guid.NewGuid(),
                    LinkType = linkType,
                    SocialProvider = socialProvider
                };
                p.TapLinks.Add(await _dataRepository.SaveTapLink(t));
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Added a tap link for "+ socialProvider, EventType.User, "AddTapLink", false));

                await _dataRepository.SaveContactPage(p);
                string view = "";
                if (linkType == LinkType.Flex)
                {
                    view = await _services.RenderToString("/pages/homeboard/flex_link_partial.cshtml", t);

                }
                if (linkType == LinkType.Pay)
                {
                    view = await _services.RenderToString("/pages/homeboard/pay_link_partial.cshtml", t);

                }
                if (linkType == LinkType.Blockchain)
                {
                    view = await _services.RenderToString("/pages/homeboard/chain_link_partial.cshtml", t);

                }
                if (linkType == LinkType.Connect)
                {
                    if (p.GridPreview)
                    {
                        view = await _services.RenderToString("/pages/homeboard/pay_link_partial.cshtml", t);
                        view = view.Replace("PayStatus", "ConnectStatus").Replace("OpenPayList", "OpenConnectList");

                    }
                    else
                    {
                        t.Title = "Connect on " + t.SocialProvider;
                        await _dataRepository.SaveTapLink(t);
                        view = await _services.RenderToString("/pages/homeboard/flex_link_partial.cshtml", t);
                        view = view.Replace("FlexStatus", "ConnectStatus").Replace("OpenFlexList", "OpenConnectList");

                    }
                }
                if (linkType == LinkType.Shop)
                {
                    view = await _services.RenderToString("/pages/homeboard/shop_link_partial.cshtml", t);

                }
                view = view.Replace(Environment.NewLine, string.Empty);

                return new JsonResult(new { view });

            }
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error adding a tap link for " + socialProvider, EventType.User, "AddTapLink", true));

            return new JsonResult(new { view = "Error adding taplink, try reloading your browser" });

        }
        public async Task<IActionResult> OnGetSavePreviewState(Guid id, string value, string name)
        {
            ContactPage? contactPage = _dataRepository.ContactPages.FirstOrDefault(x => x.Id == id);
            if (contactPage == null)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if(user is not null && user.UserName is not null)
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Got an error updating preview state, page not found" , EventType.User, "SavePreviewState", false));

                TempData["Error"] = "Error updating preview, Admin Notified!";
                return Page();
            }
            else
            {
                var propName = contactPage.GetType().GetProperty(name);
                if(propName is null)
                {
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Got an error updating preview state, property not found", EventType.User, "SavePreviewState", true));
                    TempData["Error"] = "Error updating preview, Admin Notified!";
                    return Page();
                }
                else
                {
                    var propString = propName.ToString();
                    if (string.IsNullOrEmpty(propString))
                    {
                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Got an error updating preview state, property string not found", EventType.User, "SavePreviewState", true));

                        TempData["Error"] = "Error updating preview, Admin Notified!";
                        return Page();
                    }
                    else
                    {
                        if (propString.ToLower().Contains("preview") && !propString.Contains("Image"))
                        {
                            var pageProp = contactPage.GetType().GetProperty(name);
                            if(pageProp is null)
                            {
                                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Got an error updating preview state, page property not found", EventType.User, "SavePreviewState", true));

                                return Page();
                            }
                            else
                            {
                                if (!value.Contains("w--redirected-checked"))
                                {
                                    pageProp.SetValue(contactPage, true);
                                    await _dataRepository.SaveContactPage(contactPage);
                                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Added to preview "+ name, EventType.User, "SavePreviewState", false));
                                    return new JsonResult(new { status = name.Replace("Preview", "") + " saved!" });
                                }
                                else
                                {
                                    pageProp.SetValue(contactPage, false);
                                    await _dataRepository.SaveContactPage(contactPage);
                                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Removed from preview " + name, EventType.User, "SavePreviewState", false));

                                    return new JsonResult(new { status = name.Replace("Preview", "") + " removed from preview!" });
                                }
                            }
                            
                        }
                        else
                        {
                            if (name == "Location" && value != null && value != "")
                            {
                                Location? location = _dataRepository.Locations.FirstOrDefault(x => x.Name.ToLower() == value.ToLower());
                                if (location != null)
                                {
                                    contactPage.Location = location.Name;
                                }
                                else
                                {
                                    value = value.Replace(" ", "");
                                    var uri = new Uri("https://maps.googleapis.com/maps/api/place/textsearch/json?query=" + value + "&key=AIzaSyB0Ot5RxBmQCUxUhbQr3iYEw7ZK8eeoDkk");
                                    HttpClient? client = new();
                                    string responseBody = await client.GetStringAsync(uri);
                                    dynamic? jObj = JsonConvert.DeserializeObject(responseBody);
                                    if(jObj is not null)
                                    {
                                        location = new Location
                                        {
                                            Id = Guid.NewGuid(),
                                            Name = jObj.results[0].name,
                                            GoogleID = jObj.results[0].place_id,
                                            Latitude = jObj.results[0].geometry.location.lat,
                                            Longitude = jObj.results[0].geometry.location.lng,
                                            City = jObj.results[0].name
                                        };
                                        var l = await _dataRepository.SaveLocation(location);
                                        contactPage.Location = l.Name;
                                        await _dataRepository.SaveContactPage(contactPage);
                                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Changed Location to " + location.Name, EventType.User, "SavePreviewState", false));

                                        return new JsonResult(new { status = name + " saved for preview!" });
                                    }
                                    else
                                    {
                                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Got an error saving location ", EventType.User, "SavePreviewState", false));

                                        return new JsonResult(new { status = "Error saving location" });
                                    }


                                }
                            }
                            propName.SetValue(contactPage, value);
                            await _dataRepository.SaveContactPage(contactPage);
                            return new JsonResult(new { status = name + " saved for preview!" });
                        }
                    }
                   
                }
                
            }
            


        }
        public async Task<IActionResult> OnGetSaveTapLinkState(Guid id, string value, string name, string? pageid)
        {
            TapLink? tapLink = _dataRepository.TapLinks.FirstOrDefault(x => x.Id == id);
            if (tapLink == null)
            {
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Error loading tap link with id " + tapLink.Id, EventType.User, "SaveTapLinkState", true));

                TempData["Error"] = "Error loading saving taplink, Admin Notified!";
                return Page();
            }
            else
            {
                ContactPage? p = _dataRepository.ContactPages.FirstOrDefault(x => x.TapLinks.Contains(tapLink));
                var prop = tapLink.GetType().GetProperty(name);
                if(prop is null)
                {
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Error finding property for taplink with id " + tapLink.Id, EventType.User, "SaveTapLinkState", true));

                    TempData["Error"] = "Error loading saving taplink, Admin Notified!";
                    return Page();
                }
                else
                {
                    if (name.Contains("ShowPreview"))
                    {
                        if (!value.Contains("w--redirected-checked"))
                        {
                            if (p is not null && tapLink.LinkType == LinkType.Connect && string.IsNullOrEmpty(tapLink.Title) && !p.GridPreview || tapLink.LinkType == LinkType.Flex && string.IsNullOrEmpty(tapLink.Title) || tapLink.LinkType == LinkType.Shop && string.IsNullOrEmpty(tapLink.Title) || tapLink.LinkType == LinkType.Shop && string.IsNullOrEmpty(tapLink.CustomImage))
                            {

                                return new JsonResult(new { status = "<p style='color:red;'>A title and/or image is required for preview</p>" });

                            }
                            else
                            {
                                prop.SetValue(tapLink, true);
                                await _dataRepository.SaveTapLink(tapLink);
                                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Added " + tapLink.SocialProvider+ " to preview", EventType.User, "SaveTapLinkState", false));

                                return new JsonResult(new { status = "<p style='color:green;'>" + tapLink.SocialProvider + " saved for preview!</p>" });

                            }

                        }
                        else
                        {
                            if (p is not null && tapLink.LinkType == LinkType.Connect && string.IsNullOrEmpty(tapLink.Title) && !p.GridPreview || tapLink.LinkType == LinkType.Flex && string.IsNullOrEmpty(tapLink.Title) || tapLink.LinkType == LinkType.Shop && string.IsNullOrEmpty(tapLink.Title) || tapLink.LinkType == LinkType.Shop && string.IsNullOrEmpty(tapLink.CustomImage))
                            {
                                return new JsonResult(new { status = "<p style='color:red;'>A title and/or image is required for preview</p>" });

                            }
                            else
                            {
                                prop.SetValue(tapLink, false);
                                await _dataRepository.SaveTapLink(tapLink);
                                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Removed " + tapLink.SocialProvider + " from preview", EventType.User, "SaveTapLinkState", false));

                                return new JsonResult(new { status = "<p style='color:green;'>" + tapLink.SocialProvider + " removed from preview!</p>" });
                            }
                            
                        }
                    }
                    if (name.Contains("VPreview"))
                    {
                        if (!value.Contains("w--redirected-checked"))
                        {
                            prop.SetValue(tapLink, true);
                            await _dataRepository.SaveTapLink(tapLink);
                            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Added " + tapLink.SocialProvider + " to vcard", EventType.User, "SaveTapLinkState", false));

                            return new JsonResult(new { status = "<p style='color:green;'>" + tapLink.SocialProvider + " saved for vcard!</p>" });
                        }
                        else
                        {
                            prop.SetValue(tapLink, false);
                            await _dataRepository.SaveTapLink(tapLink);
                            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Removed " + tapLink.SocialProvider + " from vcard", EventType.User, "SaveTapLinkState", false));

                            return new JsonResult(new { status = "<p style='color:green;'>" + tapLink.SocialProvider + " removed from vcard!</p>" });
                        }
                    }
                    else
                    {
                        prop.SetValue(tapLink, value);
                        await _dataRepository.SaveTapLink(tapLink);
                        await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Edited "+name+" for " + tapLink.SocialProvider + " for preview", EventType.User, "SaveTapLinkState", false));

                        return new JsonResult(new { status = "<p style='color:green;'>" + tapLink.SocialProvider + " saved for preview!</p>" });
                    }
                }
                
            }
           

        }
        public async Task<IActionResult> OnGetDeleteTapLink(Guid id)
        {
            TapLink? tapLink = _dataRepository.TapLinks.FirstOrDefault(x => x.Id == id);
            if(tapLink is not null)
            {
                ContactPage? page = _dataRepository.ContactPages.FirstOrDefault(x => x.TapLinks.Contains(tapLink));
                if(page is not null)
                {
                    OpenTapLinkModel model = new()
                    {
                        LinkType = tapLink.LinkType,
                        SocialProvider = tapLink.SocialProvider,
                        PageId = page.Id,
                        ImageUrl = tapLink.GetLogo(tapLink.SocialProvider)
                    };
                    await _dataRepository.DeleteTapLink(tapLink);
                    await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " Deleted tap link " + tapLink.SocialProvider, EventType.User, "DeleteTapLink", false));

                    string view = await _services.RenderToString("/pages/homeboard/open_tap_link_partial.cshtml", model);
                    view = view.Replace(Environment.NewLine, string.Empty);

                    return new JsonResult(new { view });
                }
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error deleting tap link " + tapLink.SocialProvider, EventType.User, "DeleteTapLink", true));


                TempData["Error"] = "Error deleting taplink, Admin Notified!";
                return Page();
            }
            else
            {
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error deleting tap link " + tapLink.SocialProvider, EventType.User, "DeleteTapLink", true));

                TempData["Error"] = "Error deleting taplink, Admin Notified!";
                return Page();
            }
        }
        public async Task<IActionResult> OnGetDownloadvCard(Guid id)
        {
            Contact? contact = _dataRepository.Contacts.FirstOrDefault(x => x.Id == id);
            if(contact is null || contact.Name is null)
            {
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " got an error downloading vcard", EventType.User, "DownloadVcard", true));
                TempData["Error"] = "Error downloading vcard, Admin Notified!";
                return Page();
            }
            else
            {
                var vcard = new VCard
                {
                    Links = new List<TapLink>(),
                    FirstName = contact.Name.Split(" ")[0],
                    LastName = contact.Name.Split(" ")[1]
                };
                
                if (!string.IsNullOrEmpty(contact.ApplicationUserID))
                {
                    ContactPage? meetYou = _dataRepository.ContactPages.FirstOrDefault(x => x.UserId.ToString() == contact.ApplicationUserID && x.PageType == PageType.Business);
                    if (meetYou != null)
                    {
                        if (!string.IsNullOrEmpty(meetYou.ProImage))
                        {
                            HttpClient web = new();
                            vcard.Image = await web.GetByteArrayAsync(meetYou.ProImage);

                        }
                        if (meetYou.VLead && meetYou.ProfileLead != null)
                        {
                            vcard.Lead = meetYou.ProfileLead;
                        }
                        if (meetYou.VPhone && meetYou.PhoneNumber != null)
                        {
                            vcard.Phone = meetYou.PhoneNumber;
                        }
                        if (meetYou.VEmail && meetYou.Email != null)
                        {
                            vcard.Email = meetYou.Email;
                        }
                        if (meetYou.VWebsite && meetYou.Website != null)
                        {
                            vcard.Website = meetYou.Website;
                        }
                        if (meetYou.BusinessName != null)
                        {
                            vcard.Business = meetYou.BusinessName;
                        }
                        if (!string.IsNullOrEmpty(meetYou.Location))
                        {
                            vcard.Location = _dataRepository.Locations.FirstOrDefault(x => x.Name == meetYou.Location);
                        }
                        foreach (var link in meetYou.TapLinks.Where(x => x.SocialProviderMainUrl != null && x.VCard == true))
                        {
                            vcard.Links.Add(link);

                        }

                    }
                    else
                    {
                        vcard.Phone = contact.Phone;
                        if (!string.IsNullOrEmpty(contact.Email))
                        {
                            vcard.Email = contact.Email;
                        }
                        vcard.Website = contact.Website;
                        vcard.Lead = contact.Note;
                        var connectedUser = _dataRepository.Users.FirstOrDefault(x => x.Id.ToString() == contact.ApplicationUserID);
                        if(connectedUser != null)
                        {
                            vcard.Image = await new HttpClient().GetByteArrayAsync(connectedUser.CoverUrl);
                        }
                    }
                }


                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " downloaded vcard " + vcard.Email, EventType.User, "DownloadVcard", false));
                return new vCardActionResult(vcard);
            }
            
        }
        public async Task<IActionResult> OnGetLogOut()
        {
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " logged out ", EventType.User, "Logout", false));

            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnGetDeleteAccount()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            if(user is not null)
            {
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " deleted account " , EventType.User, "DeleteAccount", false));

                await _services.DeleteAccount(user.Id);
                await _signInManager.SignOutAsync();
            }
            
            return RedirectToPage("/Index");
        }
        public async Task<IActionResult> OnGetDeleteContact(Guid id)
        {
            Contact? c = _dataRepository.Contacts.FirstOrDefault(x => x.Id == id);
            if(c is not null)
            {
                await _dataRepository.DeleteContact(c);
                await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " deleted contact " + c.Email, EventType.User, "DeleteContact", false));

                return new JsonResult(new { status = "Success" });
            }
            return new JsonResult(new { status = "Failed" });
        }
    }
}
