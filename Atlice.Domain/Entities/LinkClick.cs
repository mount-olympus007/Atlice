using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class LinkClick
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public LinkClick(LinkClickType linkClickType) 
        {
            Id= Guid.NewGuid();
            TimeStamp= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            LinkClickType = linkClickType;
        }
        public LinkClick() { }
        [Key]
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public LinkClickType LinkClickType { get; set; }
        public int PageVisitId { get;set; }
    }

    public enum LinkClickType
    {
        Patreon, WhatsApp, Discord, Telegram, LinkedIn, Facebook, Instagram, Twitter, TikTok, SnapChat, Twitch, Vimeo, YouTube, Apple, Spotify, SoundCloud, Calendly, PayPal, CashApp, Venmo, Clubhouse, MixCloud, Ethereum, Solana, Cardano, Tezos, File, Announcement, Calendar, Shop, Website
    }
}
