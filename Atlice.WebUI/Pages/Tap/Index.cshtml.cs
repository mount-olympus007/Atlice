using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Atlice.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp;
using System.Drawing;
using Contact = Atlice.Domain.Entities.Contact;

namespace Atlice.WebUI.Pages.Tap
{
    public class IndexModel : PageModel
    {
        private readonly IDataRepository repository;
        private readonly IServices services;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _context;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public IndexModel(IDataRepository repo, IServices _services, UserManager<ApplicationUser> userManager, IHttpContextAccessor context)
        {
            _context = context;
            _userManager = userManager;
            repository = repo;
            services = _services;
            _context = context;
        }
        [ViewData]
        public ContactPage ContactPage { get; set; } = new ContactPage();
        [ViewData]
        public PageVisit PageVisit { get; set; } = new PageVisit();
        [ViewData]
        public string JP { get; set; } = "";

        public class ContactSubmission
        {
            public string SubmissionDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime().ToShortDateString();
            public string PageType { get; set; } = Domain.Entities.PageType.Business.ToString();
            public string? Location { get; set; }
            public string? ContactName { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public string? ContactWeb { get; set; }
            public string? Note { get; set; }
            public string? Vcard { get; set; }
            public string? Role { get; set; }
            public string? PageOwnerRole { get; set; }
            public bool CitizenTrack { get; set; }
        }

        public class ContactSubmissionConfirmation
        {
            public string? WhosPage { get; set; }
            public string? WhosVcard { get; set; }
            public string? Reedem { get; set; }
            public string? Image { get; set; }
            public string? Note { get; set; }
            public string? Location { get; set; }
            public string? Role { get; set; }
            public string? PageOwnerRole { get; set; }
            public bool CitizenTrack { get; set; }
        }

        public class StatusModel
        {
            public string? Color { get; set; }
            public string? Status { get; set; }
        }

        public async Task<IActionResult> OnGet(string id, int pvid = 0)
        {

            PageVisit? visit = new PageVisit();
            //byte[] byteArr = Array.Empty<byte>();
            if (_context.HttpContext is not null && _context.HttpContext.Connection.RemoteIpAddress is not null)
            {
                visit.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                visit.Ip = _context.HttpContext.Connection.RemoteIpAddress.ToString();
                visit.TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
                HttpContext.Session.SetString("visit", Newtonsoft.Json.JsonConvert.SerializeObject(visit));
            }
            else
            {
                await repository.SaveEvent(new Event("Anonymous","Anonymous request with no context or ip", EventType.Anonymous, "Tap", true));

                return RedirectToPage("/Tap/Locked");
            }

            if (id.Length == 8)
            {
                var t = repository.Taps.FirstOrDefault(x => x.SNumber.ToLower().StartsWith(id.ToLower()));

                if (t != null)
                {
                    if (!t.Locked)
                    {
                        if (t.ContactPage == null)
                        {
                            var o = repository.Orders.FirstOrDefault(x => x.Taps.Contains(t));
                            if (o == null)
                            {
                                await repository.SaveEvent(new Event("Ip: "+visit.Ip+" UserAgent: "+ visit.UserAgent, "Tapped Locked device not connected to an order", EventType.Anonymous, "Tap", true));

                                return RedirectToPage("/Tap/Locked");
                            }
                            else
                            {
                                await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Tapped Locked device connected to order with id:" + o.Id, EventType.Anonymous, "Tap", false));

                                return LocalRedirect("/Identity/Account/login?tapid=" + id);

                            }


                        }
                        if (t.Bypass && t.BypassURL != null)
                        {
                            await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Bypass Successful to:" + t.BypassURL, EventType.Anonymous, "Tap", false));

                            return Redirect(t.BypassURL);
                        }
                        ContactPage? c = repository.ContactPages.FirstOrDefault(x => x.Id == t.ContactPage.Id);
                        if (c is not null)
                        {
                            ContactPage = c;
                            if (HttpContext.Connection.RemoteIpAddress is not null)
                            {

                                var dbvisit = c.Visits.FirstOrDefault(x => x.Ip == visit.Ip && x.UserAgent == visit.UserAgent);
                                if (dbvisit == null)
                                {
                                    visit.ContactPageId = t.ContactPage.Id;
                                    visit.Counter++;
                                    visit = await repository.SavePageVisit(visit);
                                    c.Visits.Add(visit);
                                    await repository.SaveContactPage(c);
                                    await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "New Page Visit" , EventType.Anonymous, "Tap", false));
                                    PageVisit = visit;

                                }
                                else
                                {
                                    dbvisit.Counter++;
                                    visit = await repository.SavePageVisit(dbvisit);
                                    await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Repeat Page Visit", EventType.Anonymous, "Tap", false));

                                    PageVisit = dbvisit;
                                }

                                t.Hits++;
                                await repository.SaveTap(t);
                                HttpContext.Session.Remove("visit");
                                return Page();
                            }
                            return RedirectToPage("/Tap/Locked");
                        }
                        await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Contact Page not found", EventType.Anonymous, "Tap", false));

                        return RedirectToPage("/Tap/Locked");
                    }
                    else
                    {
                        await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Visited Locked Page", EventType.Anonymous, "Tap", false));

                        return RedirectToPage("/Tap/Locked");
                    }
                }
                else
                {
                    await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, "Device not found", EventType.Anonymous, "Tap", false));

                    return RedirectToPage("/Tap/Locked");
                }
            }
            else
            {
                ContactPage? p = repository.ContactPages.FirstOrDefault(x => x.Id == Guid.Parse(id));
                if (p != null)
                {
                    ContactPage = p;
                    var jp = TempData["JustPublished"];
                    if (jp is not null)
                    {

                        if (jp.ToString() == "true")
                        {
                            JP = "true";
                            await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, User.Identity.Name + " Published Page Visit not logged", EventType.Anonymous, "Tap", false));

                        }
                        else
                        {
                            JP = "false";
                        }
                    }
                    HttpContext.Session.Remove("visit");

                    return Page();


                }
                await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, User.Identity.Name + " page could not be found", EventType.Anonymous, "Tap", false));

                return RedirectToPage("/Tap/Locked");

            }
        }

        public async Task<IActionResult> OnGetDownloadvCard(Guid id, int pvid = 0, bool dl = false)
        {
            ContactPage? meetYou = repository.ContactPages.FirstOrDefault(x => x.Id == id);

            
            var vcard = new VCard
            {
                Links = new List<TapLink>(),
            };
            if (meetYou is not null)
            {
                string fname = "";
                string lname = "";
                if (meetYou.Name is null)
                {
                    ApplicationUser? u = repository.Users.FirstOrDefault(x => x.Id == meetYou.Id);
                    if (u != null)
                    {
                        fname = u.FirstName ??= "New";
                        lname = u.LastName ??= "User";
                    }
                }
                else
                {
                    string[] names = meetYou.Name.Split(' ');
                    fname = names[0];
                    lname = "";
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
                }



                vcard.FirstName = fname;
                vcard.LastName = lname;

                if (!string.IsNullOrEmpty(meetYou.ProImage))
                {
                    HttpClient httpClient = new();
                    vcard.Image = await httpClient.GetByteArrayAsync(meetYou.ProImage);

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
                foreach (var link in meetYou.TapLinks.Where(x => x.SocialProviderMainUrl != null && x.VCard == true))
                {
                    vcard.Links.Add(link);

                }
                if (pvid != 0 && dl == true && meetYou is not null)
                {
                    PageVisit? visit = meetYou.Visits.FirstOrDefault(x => x.Id == pvid);
                    if (visit is not null)
                    {
                        visit.ContactDownloaded = true;
                        await repository.SavePageVisit(visit);
                        await repository.SaveEvent(new Event("Ip: " + visit.Ip + " UserAgent: " + visit.UserAgent, User.Identity.Name + " Downloaded Vcard", EventType.Anonymous, "DownloadVcard", false));

                    }
                }

                return new vCardActionResult(vcard);
            }

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostNoteToSelf(string pageId, string note, string email)
        {

            ContactPage? c = repository.ContactPages.FirstOrDefault(x => x.Id == Guid.Parse(pageId));
            if (string.IsNullOrEmpty(note) || string.IsNullOrEmpty(email))
            {
                StatusModel m = new()
                {
                    Color = "red",
                    Status = "Invalid email or note"
                };
                return new JsonResult(new { view = await services.RenderToString("/pages/tap/status.cshtml", m) });

            }
            if (c is not null)
            {
                ApplicationUser? pageOwner = repository.Users.FirstOrDefault(x => x.Id == c.UserId);
                if (pageOwner is not null)
                {
                    ContactSubmissionConfirmation csc = new()
                    {
                        WhosPage = c.Name ??= "New Contact",
                        WhosVcard = "https://atlice.com/cards?id=" + c.Id + "&email=" + pageOwner.Email,
                        Image = c.ProImage,
                        Note = note,
                        Reedem = "https://atlice.com/tap/tap_try_later?invitecode=" + pageOwner.Id + "&email=" + email
                    };
                    if (!string.IsNullOrEmpty(c.Location))
                    {
                        csc.Location = c.Location;
                    }

                    ApplicationUser? userViaEmail = repository.Users.FirstOrDefault(x => x.Email == email);
                    if (userViaEmail is null)
                    {
                        userViaEmail = new ApplicationUser
                        {
                            Email = email,
                            AboutMe = note,
                            FirstName = "New",
                            LastName = "User",
                            UserName = email,
                            AffiliateId = pageOwner.Id,
                            PhoneNumber = string.Empty

                        };
                        await services.CreateAtliceAccount(userViaEmail, "Lead");
                    }

                    string htm = await services.RenderToString("/pages/shared/emails/notetoself.cshtml", csc);
                    await services.SendEmailAsync(email, "Your Note to Self from " + csc.WhosPage, htm);
                    StatusModel mo = new()
                    {
                        Color = "green",
                        Status = "Email Sent!"
                    };
                    
                    await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Completed Note to Self, received invite from user with id: "+pageOwner.Id, EventType.Anonymous, "NoteToSelf", false));

                    return Content(await services.RenderToString("/pages/tap/status.cshtml", mo));
                }
                StatusModel mod = new()
                {
                    Color = "red",
                    Status = "Error Sending Email"
                };
                await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Error Completing Note to Self, page owner not found", EventType.Anonymous, "NoteToSelf", true));

                return Content(await services.RenderToString("/pages/tap/status.cshtml", mod));
            }
            else
            {
                StatusModel m = new()
                {
                    Color = "red",
                    Status = "Error Sending Email"
                };
                await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Error Completing Note to Self, could not find page", EventType.Anonymous, "NoteToSelf", true));

                return Content(await services.RenderToString("/pages/tap/status.cshtml", m));
            }
        }

        public async Task<IActionResult> OnPostSubmitContact(string name, string? website, string? note, string pageId, string email, string phone)
        {
            StatusModel m = new();
            if (phone == null && email == null)
            {
                m.Color = "red";
                m.Status = "An email address and/or phone number is required!";
                return new JsonResult(new { view = await services.RenderToString("/pages/tap/status.cshtml", m) });

            }
            ContactPage? c = repository.ContactPages.FirstOrDefault(x => x.Id == Guid.Parse(pageId));
            if (c == null)
                return RedirectToPage("/Tap/Locked");
            ApplicationUser? pageOwner = repository.Users.FirstOrDefault(x => x.Id == c.UserId);
            ContactList? l = repository.ContactLists.FirstOrDefault(x => x.UserId == c.UserId);
            if (pageOwner is null || l is null)
                return RedirectToPage("/Tap/Locked");
            ApplicationUser? visitor = repository.Users.FirstOrDefault(x => x.PhoneNumber == phone);
            if (visitor is null)
            {
                if (string.IsNullOrEmpty(email))
                {
                    email = Guid.NewGuid() + "@atlicetap.com";
                }
                visitor = await _userManager.FindByEmailAsync(email);
                if (visitor == null)
                {
                    var names = name.Split();
                    string lname = string.Empty;
                    string fname = string.Empty;
                    if (names.Length > 2)
                    {
                        fname = names[0];
                        foreach (string d in names.Skip(1))
                        {
                            lname = lname + " " + d;
                        }
                    }
                    else
                    {
                        if (names.Length < 2)
                        {
                            fname = names[0];
                            lname = "";
                        }
                        else
                        {
                            fname = names[0];
                            lname = names[1];

                        }
                    }
                    
                    visitor = new ApplicationUser
                    {
                        Email = email,
                        PhoneNumber = phone,
                        AboutMe = note,
                        FirstName = fname,
                        LastName = lname,
                        MyWebsite = website,
                        UserName = email,
                        AffiliateId = pageOwner.Id
                    };
                    visitor = await services.CreateAtliceAccount(visitor, "Lead");
                    await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Converted to lead, new user created", EventType.Anonymous, "SubmitContact", true));

                }
            }
            Contact contact = new()
            {
                ApplicationUserID = visitor.Id.ToString(),
                Note = note,
                Name = name,
                Website = website,
                Email = email,
                LinkedPage = c
            };
            if (!string.IsNullOrEmpty(phone))
            {
                contact.Phone = phone;
            }
            if (l.Contacts.FirstOrDefault(x => x.Email == visitor.Email) == null)
            {
                contact = await repository.SaveContact(contact);
                l.Contacts.Add(contact);
                await repository.SaveContactList(l);
                ContactSubmission cs = new()
                {
                    ContactName = contact.Name,
                    SubmissionDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToString("f"),
                    ContactEmail = contact.Email,
                    ContactPhone = contact.Phone,
                    ContactWeb = contact.Website,
                    Note = contact.Note,
                    PageType = c.PageType.ToString(),
                    Vcard = "https://atlice.com/cards?contactid=" + contact.Id + "&email=" + visitor.Email
                };
                if (contact.Location != null)
                {
                    cs.Location = contact.Location.Name;
                }
                var htm = await services.RenderToString("/pages/shared/emails/contactsubmission.cshtml", cs);
                if (pageOwner.Email is not null)
                {
                    await services.SendEmailAsync(pageOwner.Email, "New Contact Submission from " + cs.ContactName, htm);

                }
                ContactSubmissionConfirmation csc = new()
                {
                    WhosVcard = "https://atlice.com/cards?id=" + c.Id + "&email=" + visitor.Email,
                    Reedem = "https://atlice.com/tap/tap_try_later?invitecode=" + pageOwner.Id + "&email=" + visitor.Email
                };
                if (contact.LinkedPage is not null)
                {
                    csc.Image = contact.LinkedPage.ProImage;
                    csc.WhosPage = contact.LinkedPage.Name;
                }
                var confirmhtm = await services.RenderToString("/pages/shared/emails/contactsubmissionconfirm.cshtml", csc);
                if (visitor.Email is not null)
                {
                    await services.SendEmailAsync(visitor.Email, "Your Contact Submission to " + csc.WhosPage, confirmhtm);
                }
                m.Color = "green";
                m.Status = "Submitted!";
                await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Contact Submitted Successfully", EventType.Anonymous, "SubmitContact", false));

                return Content(await services.RenderToString("/pages/tap/status.cshtml", m));
            }
            else
            {
                m.Color = "orange";
                m.Status = "You have already submitted your contact information!";
                await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " attemped to resubmit contact", EventType.Anonymous, "SubmitContact", true));

                return Content(await services.RenderToString("/pages/tap/status.cshtml", m));
            }

        }

        public async Task<IActionResult> OnGetAddLinkClick(string url, int pagevisitid, string linkClickType)
        {
            PageVisit? v = repository.PageVisits.FirstOrDefault(x => x.Id == pagevisitid);
            if (v is not null)
            {
                Enum.TryParse(linkClickType, out LinkClickType myStatus);
                
                LinkClick c = new LinkClick(myStatus);
                c.PageVisitId = pagevisitid;
                c = await repository.SaveLinkClick(c);
                await repository.SaveEvent(new Event("Ip: " + _context.HttpContext.Connection.RemoteIpAddress.ToString() + " UserAgent: " + HttpContext.Request.Headers["User-Agent"].ToString(), " Clicked link "+linkClickType+" for Page Visit Id:" +v.Id, EventType.Anonymous, "LinkClick", false));

                v.LinkClicks.Add(c);
                await repository.SavePageVisit(v);

            }
            return new JsonResult(new { url });
        }
    }
}
