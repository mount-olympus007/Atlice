using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _sManager;

        private readonly IServices _services;
        private readonly IDataRepository _dataRepository;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
      
        public IndexModel(UserManager<ApplicationUser> userManager, IServices services, IDataRepository dataRepository, SignInManager<ApplicationUser> sManager)
        {
            _dataRepository = dataRepository;
            _userManager = userManager;
            _services = services;
            _sManager = sManager;
        }

        [ViewData]
        public int NewOrders { get; set; }
        [ViewData]
        public int Errors { get; set; }
        [ViewData]
        public int Outstanding { get; set; }
        [ViewData]
        public List<ApplicationUser> Invitees { get; set; } = new List<ApplicationUser>();
        [ViewData]
        public List<ApplicationUser> Leads { get; set; } = new List<ApplicationUser>();
        [ViewData]
        public List<PageVisit> PageVisits { get; set; } = new List<PageVisit>();
        [ViewData]
        public List<Contact> Contacts { get; set; } = new List<Contact>();
        [ViewData]
        public List<int> PageVisitsByDay { get; set; } = new List<int>();
        [ViewData]
        public List<int> ContactSubmissionsByDay { get; set; } = new List<int>();
        [ViewData]
        public List<int> PageVisitsByYear { get; set; } = new List<int>();
        [ViewData]
        public List<int> ContactSubmissionsByYear { get; set; } = new List<int>();


        public async Task<IActionResult> OnGet()
        {
            //await _sManager.SignOutAsync();
            //ApplicationUser u = _userManager.Users.FirstOrDefault(x => x.UserName.Contains("lexdamas"));
            //await _sManager.SignInAsync(u, isPersistent: true);
            NewOrders = _dataRepository.Orders.Where(x=>x.Status == OrderStatus.NewOrder).Count();
            Errors = _dataRepository.ErrorFeedbacks.Count();
            Invitees = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Invited");
            Leads = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Lead");
            var admins = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Adminis");
            foreach (var tracker in _dataRepository.RewardsTrackers.ToList())
            {
                ApplicationUser? invitee = Invitees.FirstOrDefault(x => x.Id == tracker.UserId);
                ApplicationUser? lead = Leads.FirstOrDefault(x => x.Id == tracker.UserId);
                ApplicationUser? admin = admins.FirstOrDefault(x => x.Id == tracker.UserId);
                if(invitee is null && lead is null && admin is null)
                {
                    if (!tracker.Credentials || !tracker.EligibilityForm || !tracker.OnboardingStep2 || !tracker.VerifyStep || !tracker.Terms || !tracker.DeviceSelect || !tracker.SetupContactPage || !tracker.OnboardingStep7)
                    {
                        Outstanding++;
                    }
                }
                
            }
            
            PageVisits = _dataRepository.PageVisits.ToList();
            Contacts = _dataRepository.Contacts.ToList();
            var month = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).Month;
            var year = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).Year;
            var day = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).Day;
            for(int i = 1; i <= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).Day; i++)
            {
                var date = new DateTime(year, month, i);
                var visits = PageVisits.Count(x => x.TimeStamp.ToShortDateString() == date.ToShortDateString());
                PageVisitsByDay.Add(visits);

                var contacts = Contacts.Count(x => x.DateMeet.ToShortDateString() == date.ToShortDateString());
                ContactSubmissionsByDay.Add(contacts);
            }
            

            for(int i = 1; i <= 12; i++)
            {
                var visits = PageVisits.Count(x => x.TimeStamp.Month == i);
                PageVisitsByYear.Add(visits);
                var contacts = Contacts.Count(x => x.DateMeet.Month == i);
                ContactSubmissionsByYear.Add(contacts);
            }



            return Page();
        }
        public async Task<IActionResult> OnGetUpgrade()
        {
            var Invitees = (List<ApplicationUser>)await _userManager.GetUsersInRoleAsync("Invited");
            foreach(var user in Invitees) 
            { 
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                }
                await _userManager.AddToRoleAsync(user, "Prospect");
            }
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " upgraded all invitees to prospects", EventType.Admin, "Upgrade", false));
            return Page();
        }
        public async Task<IActionResult> OnGetBackUpDatabase()
        {
            List<BackupData> bd = new List<BackupData>();
            foreach (var user in _userManager.Users.ToList())
            {
                List<AtliceTap> taps = _dataRepository.Taps.Where(x => x.UserId == user.Id).ToList();
                List<ContactPage> pages = _dataRepository.ContactPages.Where(x => x.UserId == user.Id).ToList();
                List<ContactList> contactLists = _dataRepository.ContactLists.Where(x => x.UserId == user.Id).ToList();
                YouLoveProfile? ylp = _dataRepository.YouLoveProfiles.FirstOrDefault(x => x.Id == user.YouLoveProfileId);
                List<Order>? orders = _dataRepository.Orders.Where(x => x.UserId == user.Id).ToList();
                RewardTracker? rewardTracker = _dataRepository.RewardsTrackers.FirstOrDefault(x => x.UserId == user.Id);
                Passport? passport = _dataRepository.Passports.FirstOrDefault(x=>x.UserId == user.Id);
                BackupData bud = new()
                {
                    ApplicationUser = user,
                    AtliceTaps = taps,
                    ContactPages = pages,
                    ContactLists = contactLists,
                    YouLoveProfile = ylp,
                    Orders = orders,
                    RewardTracker = rewardTracker,
                    Passport = passport
                };
                bd.Add(bud);
            }
            string JsonString = JsonConvert.SerializeObject(bd);
            string result = await _services.BackUpToCloud(Encoding.ASCII.GetBytes(JsonString), "databaseBackup.json");
            await _dataRepository.SaveEvent(new Event(User.Identity.Name, User.Identity.Name + " backed up the database", EventType.Admin, "BackUpDatabase", false));
            return Content("Success");
        }

    }
}
