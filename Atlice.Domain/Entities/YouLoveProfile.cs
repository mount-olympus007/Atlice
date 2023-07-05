using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class YouLoveProfile
    {
        
        [Key]
        public Guid Id { get; set; }
        public string StateofMind { get; set; } = string.Empty;
        public bool Entrepreneur { get; set; }
        public bool Creator { get; set; }
        public bool BrandOwner { get; set; }
        public bool TeamLeader { get; set; }
        public bool IndependentContractor { get; set; }
        public bool Employee { get; set; }
        public bool Cards { get; set; }
        public bool Payments { get; set; }
        public bool Paypal { get; set; }
        public bool Cashapp { get; set; }
        public bool Venmo { get; set; }
        public bool Stripe { get; set; }
        public bool Apple { get; set; }
        public bool Google { get; set; }
        public bool Other { get; set; }
        public bool SocialMedia { get; set; }
        public string SocialMediaUse { get; set; } = string.Empty;
        public bool BusinessPage { get; set; }
        public bool Zoom { get; set; }
        public bool ContentCreator { get; set; }
        public bool Write { get; set; }
        public bool Video { get; set; }
        public bool Photographer { get; set; }
        public bool Design { get; set; }
        public bool Artwork { get; set; }
        public bool Music { get; set; }
        public bool Otherjob { get; set; }
        public bool ChainAssets { get; set; }
        public bool StoreAssets { get; set; }
    }
}
