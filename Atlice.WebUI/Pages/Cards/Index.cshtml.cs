using Atlice.Domain.Abstract;
using Atlice.Domain.Concrete;
using Atlice.Domain.Entities;
using Atlice.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wangkanai.Detection.Services;

namespace Atlice.Pages.Cards
{
    public class IndexModel : PageModel
    {
        private readonly IDataRepository repository;
        public readonly IServices services;
        public IndexModel(IDataRepository repo, IServices _services)
        {
            repository = repo;
            services = _services;
        }

        public async Task<IActionResult> OnGet(string email, string? contactid, string? id)
        {
            if (repository.Users.FirstOrDefault(x => x.Email == email) == null && email != "atlicetap@atlice.com")
            {
                return Page();
            }
            if (contactid != null)
            {
                if(contactid == "0" && email == "atlicetap@atlice.com"){
                    Contact at = new Contact
                    {
                        Name = "Atlice Tap"
                    };
                    HttpClient client = new HttpClient();
                    VCard v = new VCard
                    {
                        FirstName = "Atlice",
                        LastName = "Tap",
                        Phone = "2152615226",
                        Website = "https://atlice.com/identity/account/login",
                        Links = new List<TapLink>() { new TapLink { SocialProvider = SocialProvider.Instagram, ShowPreview = true, ContactPage = true, VPreview = true, VCard = true, SocialProviderMainUrl = "https://instagram.com/atlicetap" } },
                        Image = await client.GetByteArrayAsync("https://atlice.com/images/Atlice-Tap-NFC-Tag-3.png")
                    };
                    return new vCardActionResult(v);
                }
                Contact? c = repository.Contacts.FirstOrDefault(x => x.Id.ToString() == contactid);
                if (c is not null)
                {
                    c.Name ??= "New Contact";
                    string[] ns = c.Name.Split(' ');
                    string fn = ns[0];
                    string ln = "";
                    if (ns.Length > 2)
                    {
                        foreach (string d in ns.Skip(1))
                        {
                            ln = ln + " " + d;
                        }
                    }
                    else
                    {
                        if (ns.Length < 2)
                        {
                            ln = "";
                        }
                        else
                        {
                            ln = ns[1];

                        }
                    }
                    var contactcard = new VCard
                    {
                        FirstName = fn,
                        LastName = ln,
                        Lead = c.Note
                    };
                    if (!string.IsNullOrEmpty(c.Email))
                        contactcard.Email = c.Email;
                    contactcard.Phone = c.Phone;
                    return new vCardActionResult(contactcard);
                }
                else 
                {
                    return RedirectToPage("/Index");
                }

            }
            else
            {
                
                ContactPage? meetYou = repository.ContactPages.FirstOrDefault(x => x.Id.ToString() == id);
                if (meetYou is not null)
                {
                    meetYou.Name ??= "New Contact";
                    string[] names = meetYou.Name.Split(' ');
                    string fname = names[0];
                    string lname = "";
                    if (names.Count() > 2)
                    {
                        foreach (string d in names.Skip(1))
                        {
                            lname = lname + " " + d;
                        }
                    }
                    else
                    {
                        if (names.Count() < 2)
                        {
                            lname = "";
                        }
                        else
                        {
                            lname = names[1];

                        }
                    }

                    var vcard = new VCard();
                    vcard.Links = new List<TapLink>();
                    vcard.FirstName = fname;
                    vcard.LastName = lname;
                    if (!string.IsNullOrEmpty(meetYou.ProImage))
                    {
                        HttpClient httpClient = new HttpClient();
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
                    return new vCardActionResult(vcard);
                }
                
            }
            return RedirectToPage("/Index");



        }
    }
}
