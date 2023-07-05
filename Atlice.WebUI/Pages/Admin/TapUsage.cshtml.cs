using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlice.WebUI.Pages.Admin
{
    [Authorize(Roles = "Adminis")]
    public class TapUsageModel : PageModel
    {
        private readonly IDataRepository _repository;
        public TapUsageModel(IDataRepository repository)
        {
            _repository = repository;
        }
        public class BypassDataModel
        {
            public AtliceTap AtliceTap { get; set; } = new AtliceTap();
            public ContactPage ContactPage { get; set; } = new ContactPage();
            public ApplicationUser ApplicationUser { get; set; } = new ApplicationUser();
        }
        [ViewData]
        public List<BypassDataModel> Taps { get; set; } = new List<BypassDataModel>();
        public IActionResult OnGet()
        {
            
            var taps = _repository.Taps.Where(x => x.Bypass).ToList();
            foreach(var t in taps)
            {
                if(t.ContactPage != null)
                {
                    ContactPage? p = _repository.ContactPages.FirstOrDefault(x => x.Id == t.ContactPage.Id);
                    ApplicationUser? u = _repository.Users.FirstOrDefault(x => x.Id == t.UserId);
                    if(p is not null && u is not null)
                    {
                        BypassDataModel d = new()
                        {
                            AtliceTap = t,
                            ContactPage = p,
                            ApplicationUser = u
                        };
                        Taps.Add(d);
                    }
                    
                    
                }
                
            }
            
            return Page();
        }
    }
}
