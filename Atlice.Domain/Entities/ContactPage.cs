using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class ContactPage
    {
       
        public ContactPage()
        {
            Id = Guid.NewGuid();
            this.Visits = new HashSet<PageVisit>();
            this.TapLinks = new HashSet<TapLink>();
            ProImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg";
            PreImage = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg";
        }
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public bool Grid { get; set; }
        public bool GridPreview { get; set; }
        public string? Name { get; set; }
        public string? ProfileLead { get; set; }
        public string? BusinessName { get; set; }
        public PageType PageType { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public bool PhonePreview { get; set; }
        public bool PhonePublished { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public bool EmailPreview { get; set; }
        public bool EmailPublished { get; set; }
        [Url]
        public string? Website { get; set; }
        public bool WebsitePreview { get; set; }
        public bool WebsitePublished { get; set; }

        public bool SmsAlerts { get; set; }
        public bool SmsPreview { get; set; }
        

        public bool SubmitContact { get; set; }
        public bool SubConpreview { get; set; }
        public bool NoteToSelf { get; set; }
        public bool NotetoSelfPreview { get; set; }
        //vcard
        public bool SaveToContacts { get; set; }
        public bool SaveToContactsPreview { get; set; }
        public bool VName { get; set; }
        public bool VLead { get; set; }
        public bool VPhone { get; set; }
        public bool VPhonePreview { get; set; }
        public bool VEmail { get; set; }
        public bool VEmailPreview { get; set; }
        public bool VWebsite { get; set; }
        public bool VWebsitePreview { get; set; }

        public string ProImage { get; set; }
        public string PreImage { get; set; }
        
        public virtual ICollection<TapLink> TapLinks { get; set; }
        public bool LocationPreview { get; set; }
        public string? Location { get; set; }
        public bool LocationPublished { get; set; }
        public virtual ICollection<PageVisit> Visits { get; set; }


    }

    public enum PageType 
    { 
        Personal, Business, Professional
    }
}
