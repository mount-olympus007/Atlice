using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlice.Domain.Concrete
{
    public class EFDataRepository:IDataRepository
    {
        private readonly EFDbContext _dbContext;
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private static IWebHostEnvironment _env => (IWebHostEnvironment)_httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public EFDataRepository(EFDbContext context)
        {
            _dbContext = context;    
        }
        public IEnumerable<Event> Events { get { return _dbContext.Event; } }
        public IEnumerable<Gift> Gifts { get { return _dbContext.Gift; } }
        public IEnumerable<ConnectedUser> ConnectedUsers { get { return _dbContext.ConnectedUsers; } }
        public IEnumerable<ApplicationUser> Users { get { return _dbContext.Users; } }
        public IEnumerable<Badge> Badges { get { return _dbContext.Badges; } }
        public IEnumerable<News> News { get { return _dbContext.News; } }
        public IEnumerable<Notification> Notifications { get { return _dbContext.Notifications; } }
        public IEnumerable<VCard> VCards { get { return _dbContext.VCards; } }
        public IEnumerable<Order> Orders { get { return _dbContext.Orders.Include(x=>x.Taps); } }
        public IEnumerable<TapLink> TapLinks { get { return _dbContext.TapLink; } }
        public IEnumerable<Contact> Contacts { get { return _dbContext.Contact; } }
        public IEnumerable<ContactList> ContactLists { get { return _dbContext.ContactList.Include(x=>x.Contacts).ThenInclude(x=>x.LinkedPage); } }
        public IEnumerable<AtliceTap> Taps { get { return _dbContext.AtliceTap.Include(x => x.ContactPage); } }
        public IEnumerable<Chat> Chats { get { return _dbContext.Chat.Include(x=>x.Messages); } }
        public IEnumerable<ChatMessage> ChatMessages { get { return _dbContext.ChatMessage; } }
        public IEnumerable<ContactPage> ContactPages { get { return _dbContext.ContactPage.Include(x=>x.TapLinks).Include(x=>x.Visits); } }
        public IEnumerable<Location> Locations { get { return _dbContext.Location; } }
        public IEnumerable<AdminNote> AdminNotes { get { return _dbContext.AdminNote; } }
        public IEnumerable<PageVisit> PageVisits { get { return _dbContext.PageVisit.Include(x=>x.LinkClicks); } }
        public IEnumerable<Photo> Photos { get { return _dbContext.Photo; } }
        public IEnumerable<YouLoveProfile> YouLoveProfiles { get { return _dbContext.YouLoveProfile; } }
        public IEnumerable<State> States { get { return _dbContext.State; } }
        public IEnumerable<Legislator> Legislators { get { return _dbContext.Legislator;} }
        public IEnumerable<RewardTracker> RewardsTrackers { get { return _dbContext.RewardTracker; } }
        public IEnumerable<ErrorFeedback> ErrorFeedbacks { get { return _dbContext.ErrorFeedback;} }
        public IEnumerable<Passport> Passports { get { return _dbContext.Passport.Include(x => x.Stamps) ; } }
        public IEnumerable<Stamp> Stamps { get { return _dbContext.Stamp; } }
        public IEnumerable<LinkClick> LinkClicks { get { return _dbContext.LinkClick; } }
        public IEnumerable<IdentityUserRole<Guid>> UserRoles { get { return _dbContext.UserRoles;} }
        public async Task<Event> SaveEvent(Event e)
        {
            Event? ev = await _dbContext.Event.FindAsync(e.Id);
            if(ev is null)
            {
                ev = e;
                await _dbContext.Event.AddAsync(ev);
                
            }
            await _dbContext.SaveChangesAsync();
            return ev;
        }
        public async Task<Gift> SaveGift(Gift gift)
        {
            Gift? g = await _dbContext.Gift.FindAsync(gift.Id);
            if(g == null)
            {
                g= gift;
                _dbContext.Gift.Add(g);
            }
            await _dbContext.SaveChangesAsync();
            return g;
        }
        public async Task<Notification> SaveNotification(Notification notification)
        {
            Notification? n = await _dbContext.Notifications.FindAsync(notification.Id);
            if(n == null)
            {
                n = notification;
                _dbContext.Notifications.Add(n);
            }
            await _dbContext.SaveChangesAsync();
            return n;
        }

        public async Task<Organization> SaveOrganization(Organization organization)
        {
            Organization? o = await _dbContext.Organization.FindAsync(organization.Id);
            if (o == null)
            {
                o = organization;
                _dbContext.Organization.Add(o);
                await _dbContext.SaveChangesAsync();
                return o;
            }
            else
            {
                
                o.Name = organization.Name;
                o.BusinessType = organization.BusinessType;
                o.Phone = organization.Phone;
                o.Email = organization.Email;
                o.Street = organization.Street;
                o.City = organization.City;
                o.State = organization.State;
                o.Country = organization.Country;
                o.PostalCode = organization.PostalCode;
                o.LogoUrl = organization.LogoUrl;
                o.CreatedDate = organization.CreatedDate;
                o.LastUpdated = DateTime.Now;
                o.Manager = organization.Manager;
                await _dbContext.SaveChangesAsync();
                return o;
            }
        }
        public async Task<LinkClick> SaveLinkClick(LinkClick linkClick)
        {
            LinkClick? dbclick = LinkClicks.FirstOrDefault(x => x.Id == linkClick.Id);
            if (dbclick == null)
            {
                dbclick = linkClick;
                _dbContext.LinkClick.Add(dbclick);
            }
            else
            {
                dbclick.TimeStamp = linkClick.TimeStamp;
                dbclick.LinkClickType = linkClick.LinkClickType;
                dbclick.PageVisitId= linkClick.PageVisitId;
            }
            await _dbContext.SaveChangesAsync();
            return dbclick;
        }

        public async Task<Stamp> SaveStamp(Stamp stamp)
        {
            Stamp? dbstamp = Stamps.FirstOrDefault(x=>x.Id == stamp.Id);
            if(dbstamp == null)
            {
                dbstamp = stamp;
                _dbContext.Stamp.Add(dbstamp);
            }
            else
            {
                dbstamp.Name= stamp.Name;
                dbstamp.Description= stamp.Description;
                dbstamp.Platform= stamp.Platform;
                dbstamp.TimeStamp= stamp.TimeStamp;
            }
            await _dbContext.SaveChangesAsync();
            return dbstamp;
        }
        public async Task<Passport> SavePassport(Passport passport)
        {
            Passport? dbpassport = Passports.FirstOrDefault(x => x.Id == passport.Id);
            if (dbpassport == null)
            {
                dbpassport = passport;
                _dbContext.Passport.Add(dbpassport);
            }
            else
            {
                dbpassport.UserId = passport.UserId;
                dbpassport.LastUpdated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                dbpassport.Stamps = new List<Stamp>(passport.Stamps.Union(passport.Stamps));
            }
            await _dbContext.SaveChangesAsync();
            return dbpassport;
        }
        //public DeleteStamp(Guid stampId) { return Task.CompletedTask; }

        //public DeletePassport(Guid passportId) { return Task.CompletedTask; }
        public async Task DeleteContactPage(ContactPage contactPage)
        {
            ContactPage? c = ContactPages.FirstOrDefault(x => x.Id == contactPage.Id);
            if(c is not null)
            {
                _dbContext.ContactPage.Remove(c);
                await _dbContext.SaveChangesAsync();
            }
           
        }
        public async Task DeleteTap(Guid tapid) 
        {
            AtliceTap? t = await _dbContext.AtliceTap.FindAsync(tapid);
            if(t is not null)
            {
                _dbContext.AtliceTap.Remove(t);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task DeleteOrder(Guid orderid)
        {
            Order? o = Orders.FirstOrDefault(x=>x.Id == orderid);
            if(o is not null)
            {
                foreach(var tap in o.Taps)
                {
                    tap.UserId = null;
                    tap.CustomName = null;
                    tap.ForwardUrl = null;
                    tap.Note = tap.Note + "Order Deleted on " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + "; ";
                    tap.LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
                    tap.Hits = 0;
                    tap.Bypass = false;
                    tap.BypassURL = null;
                    tap.Locked = true;
                    await SaveTap(tap);
                }
                o.Taps = new List<AtliceTap>();
                await _dbContext.SaveChangesAsync();
                
                _dbContext.Orders.Remove(o);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task DeleteContactList(ContactList contactList)
        {
            ContactList? c = ContactLists.FirstOrDefault(x => x.Id == contactList.Id);
            if (c is not null)
            {
                _dbContext.ContactList.Remove(c);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<State> SaveState(State state)
        {
            State? s = States.FirstOrDefault(x=> x.Id == state.Id);
            if (s is null)
            {
                s = state;
                await _dbContext.State.AddAsync(s);
            }
            else
            {
                s.Name= state.Name;
                s.Abbreviation= state.Abbreviation;
                s.Lat = state.Lat;
                s.Lng= state.Lng;
                s.FormsOfId= state.FormsOfId;
                s.VotersWithoutId= state.VotersWithoutId;
                if(s.Legislators is not null && state.Legislators is not null)
                    s.Legislators = new List<Legislator>(s.Legislators.Union(state.Legislators));
                
            }
            await _dbContext.SaveChangesAsync();
            return s;
        }
        public async Task<Legislator> SaveLegislator(Legislator legislator)
        {
            Legislator? l = Legislators.FirstOrDefault(x=>x.Id == legislator.Id);
            if(l is null)
            {
                l = legislator;
                await _dbContext.Legislator.AddAsync(l);
            }
            else
            {
                l.Name = legislator.Name;
                l.Party = legislator.Party;
                l.StateId= legislator.StateId;
                l.Title = legislator.Title;
                l.District= legislator.District;
                l.Tenure= legislator.Tenure;
                l.Image = legislator.Image;
                l.Lat= legislator.Lat;
                l.Long= legislator.Long;
            }
            await _dbContext.SaveChangesAsync();
            return l;
        }
        public async Task<RewardTracker> SaveRewardTracker(RewardTracker rewardTracker)
        {
            RewardTracker? t = RewardsTrackers.FirstOrDefault(x=>x.Id == rewardTracker.Id);
            if(t is null)
            {
                t = rewardTracker;
                await _dbContext.RewardTracker.AddAsync(t);
            }
            else
            {
                t.Credentials= rewardTracker.Credentials;
                t.EligibilityForm = rewardTracker.EligibilityForm;
                t.PlacedOrder = rewardTracker.PlacedOrder;
                t.OnboardingStep2 = rewardTracker.OnboardingStep2;
                t.VerifyStep = rewardTracker.VerifyStep;
                t.Terms = rewardTracker.Terms;
                t.DeviceSelect = rewardTracker.DeviceSelect;
                t.SetupContactPage = rewardTracker.SetupContactPage;
                t.OnboardingStep7 = rewardTracker.OnboardingStep7;
            }
            await _dbContext.SaveChangesAsync();
            return t;
        }
        public async Task<Photo> SavePhoto(Photo photo)
        {
            Photo? dbphoto = Photos.FirstOrDefault(x => x.Id == photo.Id);
            if(dbphoto == null)
            {
                dbphoto = photo;
                await _dbContext.Photo.AddAsync(dbphoto);
            }
            else
            {
                dbphoto.CreatorID = photo.CreatorID;
                dbphoto.FileType = photo.FileType;
                dbphoto.FilePath = photo.FilePath;
                dbphoto.FileName = photo.FileName;
                dbphoto.DateUploaded = photo.DateUploaded;
                dbphoto.Height = photo.Height;
                dbphoto.Width = photo.Width;
            }
            await _dbContext.SaveChangesAsync();
            return dbphoto;
        }
        public async Task<PageVisit> SavePageVisit(PageVisit pageVisit)
        {
            PageVisit? dbpv = PageVisits.FirstOrDefault(x => x.Id == pageVisit.Id);
            if (dbpv == null)
            {
                dbpv = pageVisit;
                await _dbContext.PageVisit.AddAsync(dbpv);
            }
            else
            {
                dbpv.Ip = pageVisit.Ip;
                dbpv.TimeStamp = pageVisit.TimeStamp;
                dbpv.ContactDownloaded = pageVisit.ContactDownloaded;
                dbpv.ContactPageId = pageVisit.ContactPageId;
                dbpv.LinkClicks = new List<LinkClick>(dbpv.LinkClicks.Union(pageVisit.LinkClicks));
            }
            await _dbContext.SaveChangesAsync();
            return dbpv;
        }
        public async Task<Contact> SaveContact(Contact contact)
        {
            Contact? dbContact = Contacts.FirstOrDefault(x => x.Id == contact.Id);
            if (dbContact == null)
            {
                dbContact = contact;
                await _dbContext.Contact.AddAsync(dbContact);
            }
            else
            {
                dbContact.DateMeet = contact.DateMeet;
                dbContact.Note = contact.Note;
                dbContact.Name = contact.Name;
                dbContact.Website = contact.Website;
                dbContact.Email = contact.Email;
                dbContact.Phone = contact.Phone;
                dbContact.LinkedPage = contact.LinkedPage;
                dbContact.Location = contact.Location;
                dbContact.ApplicationUserID = contact.ApplicationUserID;

            }
            await _dbContext.SaveChangesAsync();
            return dbContact;
        }
        public async Task<ConnectedUser> AddConnectedUser(ConnectedUser connectedUser)
        {
            ConnectedUser? dbConnectedUser = await _dbContext.ConnectedUsers.FirstOrDefaultAsync(x => x.UserID == connectedUser.UserID);
            if (dbConnectedUser == null)
            {
                dbConnectedUser = connectedUser;
                await _dbContext.ConnectedUsers.AddAsync(connectedUser);
            }
            await _dbContext.SaveChangesAsync();
            return dbConnectedUser;


        }
        public async Task<ConnectedUser> RemoveConnectedUser(string connectedUserId)
        {
            ConnectedUser? dbConnectedUser = await _dbContext.ConnectedUsers.FirstOrDefaultAsync(x => x.UserID == connectedUserId);
            if(dbConnectedUser == null)
            {
                return new ConnectedUser();
            }
            else
            {
                _dbContext.ConnectedUsers.Remove(dbConnectedUser);
                await _dbContext.SaveChangesAsync();
                return dbConnectedUser;
            }
            
            


        }
        public async Task<ChatMessage> SaveChatMessage(ChatMessage chatMessage)
        {
            ChatMessage? c = ChatMessages.FirstOrDefault(x => x.Id == chatMessage.Id);
            if(c == null)
            {
                c = chatMessage;
                await _dbContext.ChatMessage.AddAsync(c);
            }
            else
            {
                c.Message = chatMessage.Message;
                c.Sender = chatMessage.Sender;
                c.TimeStamp = chatMessage.TimeStamp;
            }
            await _dbContext.SaveChangesAsync();
            return c;
        }
        public async Task<Chat> SaveChat(Chat chat)
        {
            Chat? c = Chats.FirstOrDefault(x => x.Id == chat.Id);
            if(c == null)
            {
                c = chat;
                await _dbContext.Chat.AddAsync(c);
            }
            else
            {
                c.UserId = chat.UserId;
                c.Messages = new List<ChatMessage>( c.Messages.Union(chat.Messages));
            }
            await _dbContext.SaveChangesAsync();
            return c;
        }
        public async Task<ErrorFeedback> SaveErrorFeedback(ErrorFeedback errorFeedback)
        {
            await _dbContext.ErrorFeedback.AddAsync(errorFeedback);
            await _dbContext.SaveChangesAsync();
            return errorFeedback;
        }
        public async Task DeleteErrorFeedback(Guid id)
        {
            ErrorFeedback? f = ErrorFeedbacks.FirstOrDefault(x => x.Id == id);
            if (f != null)
            {
                _dbContext.ErrorFeedback.Remove(f);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<AtliceTap> SaveTap(AtliceTap atliceTap)
        {
            AtliceTap? tap = Taps.FirstOrDefault(x=>x.Id == atliceTap.Id);

            if(tap == null)
            {
                tap = atliceTap;
                await _dbContext.AtliceTap.AddAsync(tap);
            }
            else
            {
                tap.TapType = atliceTap.TapType;
                tap.Sku = atliceTap.Sku;
                tap.CustomName = atliceTap.CustomName;
                tap.Location = atliceTap.Location;
                tap.SNumber = atliceTap.SNumber;
                tap.UserId = atliceTap.UserId;
                tap.ForwardUrl = atliceTap.ForwardUrl;
                tap.Note = atliceTap.Note;
                tap.ContactPage = atliceTap.ContactPage;
                tap.LastEdited = atliceTap.LastEdited;
                tap.Hits = atliceTap.Hits;
                tap.Bypass = atliceTap.Bypass;
                tap.BypassURL = atliceTap.BypassURL;
                tap.BypassSocialProvider = atliceTap.BypassSocialProvider;
                tap.BypassImage = atliceTap.BypassImage;
                tap.Locked = atliceTap.Locked;
            }
            await _dbContext.SaveChangesAsync();
            return tap;
        }
        public async Task<AdminNote> SaveAdminNote(AdminNote adminNote)
        {
            AdminNote? note = AdminNotes.FirstOrDefault(x=>x.Id==adminNote.Id);
            if(note == null)
            {
                note = adminNote;
                await _dbContext.AdminNote.AddAsync(note);
            }
            else
            {
                note.Who = adminNote.Who;
                note.What = adminNote.What;
                note.When = adminNote.When;
            }
            await _dbContext.SaveChangesAsync();
            return note;
        }
        public async Task<Order> SaveOrder(Order order)
        {
            Order? o = Orders.FirstOrDefault(x=>x.Id == order.Id);
            if(o == null)
            {
                o = order;
                await _dbContext.Orders.AddAsync(o);
            }
            else
            {
                o.Name = order.Name;
                o.OrderNumber = order.OrderNumber;
                o.WebflowStatus = order.WebflowStatus;
                o.ShipName = order.ShipName;
                o.ShipAddressLine1 = order.ShipAddressLine1;
                o.ShipAddressLine2 = order.ShipAddressLine2;
                o.ShipCity = order.ShipCity;
                o.ShipCode = order.ShipCode;
                o.Last4 = order.Last4;
                o.Brand = order.Brand;
                o.ChargeId = order.ChargeId;
                o.OwnerName = order.OwnerName;
                o.LogoUrl = order.LogoUrl;
                o.CustomerPaid = order.CustomerPaid;
                o.Comments = order.Comments;
                o.Phone = order.Phone;
                o.Email = order.Email;
                o.UserId = order.UserId;
                o.Status = order.Status;
                o.OrderRecieved = order.OrderRecieved;
                o.BookMarked = order.BookMarked;
                o.OrderShipped = order.OrderShipped;
                o.OrderRecieved = order.OrderRecieved;
                if(o.Taps is not null && order.Taps is not null)
                {
                    o.Taps = new List<AtliceTap>(o.Taps.Union(order.Taps));
                }
            }
            await _dbContext.SaveChangesAsync();
            return o;
        }
        public async Task<ContactPage> SaveContactPage(ContactPage contactPage)
        {
            ContactPage? page =_dbContext.ContactPage.FirstOrDefault(x=>x.Id == contactPage.Id);
            if(page == null)
            {
                page = contactPage;
                await _dbContext.ContactPage.AddAsync(page);
            }
            else
            {
                page.Grid = contactPage.Grid;
                page.GridPreview = contactPage.GridPreview;
                page.Name = contactPage.Name;
                page.ProfileLead = contactPage.ProfileLead;
                page.BusinessName = contactPage.BusinessName;
                page.PageType = contactPage.PageType;
                page.PhoneNumber = contactPage.PhoneNumber;
                page.PhonePreview = contactPage.PhonePreview;
                page.PhonePublished = contactPage.PhonePublished;
                page.Email = contactPage.Email;
                page.EmailPreview = contactPage.EmailPreview;
                page.EmailPublished = contactPage.EmailPublished;
                page.Website = contactPage.Website;
                page.WebsitePreview = contactPage.WebsitePreview;
                page.WebsitePublished = contactPage.WebsitePublished;
                page.SmsAlerts = contactPage.SmsAlerts;
                page.SmsPreview = contactPage.SmsPreview;
                page.SubmitContact = contactPage.SubmitContact;
                page.SubConpreview = contactPage.SubConpreview;
                page.NoteToSelf = contactPage.NoteToSelf;
                page.NotetoSelfPreview = contactPage.NotetoSelfPreview;
                page.SaveToContacts = contactPage.SaveToContacts;
                page.SaveToContactsPreview = contactPage.SaveToContactsPreview;
                page.VName = contactPage.VName;
                page.VLead = contactPage.VLead;
                page.VPhone = contactPage.VPhone;
                page.VPhonePreview = contactPage.VPhonePreview;
                page.VEmail = contactPage.VEmail;
                page.VEmailPreview = contactPage.VEmailPreview;
                page.VWebsite = contactPage.VWebsite;
                page.VWebsitePreview = contactPage.VWebsitePreview;
                page.ProImage = contactPage.ProImage;
                page.PreImage = contactPage.PreImage;
                page.Location = contactPage.Location;
                page.LocationPreview = contactPage.LocationPreview;
                page.LocationPublished = contactPage.LocationPublished;
                page.TapLinks = new List<TapLink>(page.TapLinks.Union(contactPage.TapLinks));
                page.Visits = new List<PageVisit>(page.Visits.Union(contactPage.Visits));

            }
            await _dbContext.SaveChangesAsync();
            return page;
        }
        public async Task<YouLoveProfile> SaveYouLoveProfile(YouLoveProfile youLoveProfile)
        {
            YouLoveProfile? profile = YouLoveProfiles.FirstOrDefault(x => x.Id == youLoveProfile.Id);
            if (profile == null)
            {
                profile = youLoveProfile;
                await _dbContext.YouLoveProfile.AddAsync(youLoveProfile);
            }
            else
            {
                profile.StateofMind = youLoveProfile.StateofMind;
                profile.Entrepreneur = youLoveProfile.Entrepreneur;
                profile.Creator = youLoveProfile.Creator;
                profile.BrandOwner = youLoveProfile.BrandOwner;
                profile.TeamLeader = youLoveProfile.TeamLeader;
                profile.IndependentContractor = youLoveProfile.IndependentContractor;
                profile.Employee = youLoveProfile.Employee;
                profile.Cards = youLoveProfile.Cards;
                profile.Payments = youLoveProfile.Payments;
                profile.Paypal = youLoveProfile.Paypal;
                profile.Cashapp = youLoveProfile.Cashapp;
                profile.Venmo = youLoveProfile.Venmo;
                profile.Stripe = youLoveProfile.Stripe;
                profile.Apple = youLoveProfile.Apple;
                profile.Google = youLoveProfile.Google;
                profile.Other = youLoveProfile.Other;
                profile.SocialMedia = youLoveProfile.SocialMedia;
                profile.SocialMediaUse = youLoveProfile.SocialMediaUse;
                profile.BusinessPage = youLoveProfile.BusinessPage;
                profile.Zoom = youLoveProfile.Zoom;
                profile.ContentCreator = youLoveProfile.ContentCreator;
                profile.Write = youLoveProfile.Write;
                profile.Video = youLoveProfile.Video;
                profile.Photographer = youLoveProfile.Photographer;
                profile.Design = youLoveProfile.Design;
                profile.Artwork = youLoveProfile.Artwork;
                profile.Music = youLoveProfile.Music;
                profile.Otherjob = youLoveProfile.Otherjob;
                profile.ChainAssets = youLoveProfile.ChainAssets;
                profile.StoreAssets = youLoveProfile.StoreAssets;
            }
            await _dbContext.SaveChangesAsync();
            return profile;
        }
        public async Task<ContactList> SaveContactList(ContactList contactList)
        {
            ContactList? list = ContactLists.FirstOrDefault(x => x.Id == contactList.Id);
            if (list == null)
            {
                list = contactList;
                await _dbContext.ContactList.AddAsync(list);
            }
            else
            {
                list.UserId = contactList.UserId;
                list.Contacts = new List<Contact>(list.Contacts.Union(contactList.Contacts));
            }
            await _dbContext.SaveChangesAsync();
            return list;
        }
        public async Task<Location> SaveLocation(Location location)
        {
            Location? loc = Locations.FirstOrDefault(x=>x.GoogleID == location.GoogleID);
            if(loc == null)
            {
                loc = location;
                await _dbContext.Location.AddAsync(loc);
            }
            else
            {
                loc.GoogleID = location.GoogleID;
                loc.Longitude = location.Longitude;
                loc.Latitude = location.Latitude;
                loc.City = location.City;
                loc.Name = location.Name;
            }
            await _dbContext.SaveChangesAsync();
            return loc;
        }
        public async Task<TapLink> SaveTapLink(TapLink tapLink)
        {
            TapLink? link = TapLinks.FirstOrDefault(x => x.Id == tapLink.Id);
            if(link == null)
            {
                link = tapLink;
                await _dbContext.TapLink.AddAsync(link);
            }
            else
            {
                link.Id = tapLink.Id;
                link.LinkType = tapLink.LinkType;
                link.SocialProviderMainUrl = tapLink.SocialProviderMainUrl;
                link.VCard = tapLink.VCard;
                link.VPreview = tapLink.VPreview;
                link.ContactPage = tapLink.ContactPage;
                link.ShowPreview = tapLink.ShowPreview;
                link.LinkClicks = tapLink.LinkClicks;
                link.SocialProvider = tapLink.SocialProvider;
                link.Title = tapLink.Title;
                link.Description = tapLink.Description;
                link.CustomImage = tapLink.CustomImage;
                link.WalletAddress = tapLink.WalletAddress;
            }
            await _dbContext.SaveChangesAsync();
            return link;
        }
        public async Task DeleteTapLink(TapLink tapLink)
        {
            TapLink? link = TapLinks.FirstOrDefault(x=> x.Id == tapLink?.Id);
            if(link != null)
            {
                _dbContext.TapLink.Remove(link);
                await _dbContext.SaveChangesAsync();
            }
            
        }
        public async Task<ApplicationUser> CreateProfile(Guid id)
        {
            ApplicationUser? user = Users.FirstOrDefault(x=>x.Id == id);
            if(user == null)
            {
                return new ApplicationUser { AboutMe = "User Not Found"};
            }
            else
            {
                ContactPage personal = new() { ProImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", PreImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", UserId = user.Id, Name = user.FirstName, PhoneNumber = user.PhoneNumber, Email = user.Email, PageType = PageType.Personal, EmailPreview = true, PhonePreview = true, WebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite), SubConpreview = false, SubmitContact = false, SaveToContacts = true, SaveToContactsPreview = true, NoteToSelf = false, NotetoSelfPreview = true, VPhonePreview = true, VEmailPreview = true, VWebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite) };
                ContactPage business = new() { ProImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", PreImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg",  UserId = user.Id, Name = user.FirstName, PhoneNumber = user.PhoneNumber, Email = user.Email, PageType = PageType.Business, EmailPreview = true, PhonePreview = true, WebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite), SubConpreview = false, SubmitContact = false, SaveToContacts = true, SaveToContactsPreview = true, NoteToSelf = false, NotetoSelfPreview = true, VPhonePreview = true, VEmailPreview = true, VWebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite) };
                ContactPage professional = new() { ProImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg", PreImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg",  UserId = user.Id, Name = user.FirstName, PhoneNumber = user.PhoneNumber, Email = user.Email, PageType = PageType.Professional, EmailPreview = true, PhonePreview = true, WebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite), SubConpreview = false, SubmitContact = false, SaveToContacts = true, SaveToContactsPreview = true, NoteToSelf = false, NotetoSelfPreview = true, VPhonePreview = true, VEmailPreview = true, VWebsitePreview = true && !string.IsNullOrEmpty(user.MyWebsite) };
                ContactList contactList = new() { UserId = user.Id };
                Chat chat = new() { Id = Guid.NewGuid(), UserId = user.Id };
                if(ContactPages.FirstOrDefault(x=>x.PageType == PageType.Personal && x.UserId== user.Id) == null)
                    await SaveContactPage(personal);
                if (ContactPages.FirstOrDefault(x => x.PageType == PageType.Professional && x.UserId == user.Id) == null)
                    await SaveContactPage(professional);
                if (ContactPages.FirstOrDefault(x => x.PageType == PageType.Business && x.UserId == user.Id) == null)
                    await SaveContactPage(business);
                await SaveContactList(contactList);
                await SaveChat(chat);
                AtliceTap tap = new()
                {
                    TapType = TapType.Virtual,
                    Sku = SKU.Virtual,
                    UserId = user.Id,
                    ForwardUrl = "https://atlice.com/tap/" + business.Id,
                    Note = "New User; ",
                    ContactPage = business,
                    Hits = 0,
                    Bypass = false,
                    Locked = false
                };
                user.Taps = new List<AtliceTap>
                {
                    await SaveTap(tap)
                };
                foreach(var t in Taps.Where(x=>x.UserId == id).ToList())
                {
                    t.ContactPage = business;
                    await SaveTap(t);
                }
                return user;
            }
            
        }       
        public async Task DeleteContact(Contact contact)
        {
            _dbContext.Contact.Remove(contact);
            await _dbContext.SaveChangesAsync();
        }
    }
}
