using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class TapLink
    {
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private static IWebHostEnvironment _env => (IWebHostEnvironment)_httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));

        
        public TapLink() { }
        [Key]
        public Guid? Id { get; set; }
        public LinkType LinkType { get; set; }
        [Url]
        public string? SocialProviderMainUrl { get; set; }
        public bool VCard { get; set; }
        public bool VPreview { get; set; }
        public bool ContactPage { get; set; }
        public bool ShowPreview { get; set; }
        public int LinkClicks { get; set; }
        public SocialProvider SocialProvider { get; set; }
        public string GetLogo(SocialProvider u)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, "/icons/" + u.ToString() + ".png");

            return u switch
            {
                SocialProvider.Patreon => path,
                SocialProvider.WhatsApp => path,
                SocialProvider.Discord => path,
                SocialProvider.Telegram => path,
                SocialProvider.LinkedIn => path,
                SocialProvider.Facebook => path,
                SocialProvider.Instagram => path,
                SocialProvider.Twitter => path,
                SocialProvider.TikTok => path,
                SocialProvider.SnapChat => path,
                SocialProvider.Twitch => path,
                SocialProvider.Vimeo => path,
                SocialProvider.YouTube => path,
                SocialProvider.Apple => path,
                SocialProvider.Spotify => path,
                SocialProvider.SoundCloud => path,
                SocialProvider.Calendly => path,
                SocialProvider.PayPal => path,
                SocialProvider.CashApp => path,
                SocialProvider.Venmo => path,
                SocialProvider.Clubhouse => path,
                SocialProvider.MixCloud => path,
                SocialProvider.Ethereum => path,
                SocialProvider.Solana => path,
                SocialProvider.Cardano => path,
                SocialProvider.Tezos => path,
                SocialProvider.File => path,
                SocialProvider.Announcement => path,
                SocialProvider.Calendar => path,
                SocialProvider.Shop => path,
                _ => throw new Exception(),// Handle bad URL, possibly throw
            };
        }

        //shop links
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? CustomImage { get; set; }

        //crypto links
        public string? WalletAddress { get; set; }
    }
    
    public enum SocialProvider
    {
        Patreon, WhatsApp, Discord, Telegram, LinkedIn, Facebook, Instagram, Twitter, TikTok, SnapChat, Twitch, Vimeo, YouTube, Apple, Spotify, SoundCloud, Calendly, PayPal, CashApp, Venmo, Clubhouse, MixCloud, Ethereum, Solana, Cardano, Tezos, File, Announcement, Calendar, Shop
    }

    public enum SocialProviderLogo
    {
        Patreon, WhatsApp, Discord, Telegram, LinkedIn, Facebook, Instagram, Twitter, TikTok, SnapChat, Twitch, Vimeo, YouTube, Apple, Spotify, SoundCloud, Calendly, PayPal, CashApp, Venmo, Clubhouse, MixCloud, Ethereum, Solana, Cardano, Tezos, File, Announcement, Calendar, Shop
    }
    public enum LinkType
    {
        Pay, Connect, Blockchain, Shop, Flex
    }

    public enum Payments
    {
        PayPal, CashApp, Venmo
    }
    public enum Connects
    {
        Patreon, WhatsApp, Discord, Telegram, LinkedIn, Facebook, Instagram, Twitter, TikTok, SnapChat, Twitch, Vimeo, YouTube, Apple, Spotify, SoundCloud, Calendly, Clubhouse, MixCloud
    }
    public enum Blockchains
    {
        Ethereum, Solana, Cardano, Tezos
    }
    
}
