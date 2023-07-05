using Microsoft.AspNetCore.Identity;

namespace Atlice.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public ApplicationUser()
        {
            Taps = new HashSet<AtliceTap>();
            Badges = new HashSet<Badge>();
            Notification = new HashSet<Notification>();
            InviteCode = Guid.NewGuid();
            FirstName = "New";
            LastName = "User";
            CoverUrl = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg";
        }
        public bool SmsAlerts { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MemberQuote { get; set; }
        public string? AboutMe { get; set; }
        public string? MyWebsite { get; set; }
        public bool IsMarried { get; set; }
        public bool HaveKids { get; set; }
        public bool Bookmarked { get; set; }
        public string? Gender { get; set; }
        public string? AstroSign { get; set; }
        public virtual Location? Location { get; set; }
        public Guid? AffiliateId { get; set; }
        public Guid YouLoveProfileId { get; set; }
        public string? CoverUrl { get; set; }
        public virtual ICollection<AtliceTap> Taps { get; set; }
        public virtual ICollection<Badge> Badges { get; set; }
        public virtual ICollection<Notification> Notification { get; set; }
        public string? WebflowPaymentId { get; set; }
        public string? Secret { get; set; }
        public Guid InviteCode { get; set; }
        public bool TermsConfirmed { get; set; }
        public DateTime? Created { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
        public DateTime? DOB { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
        public string? Interests { get; set; }

    }


}
