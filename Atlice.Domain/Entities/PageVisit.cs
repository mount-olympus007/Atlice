using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class PageVisit
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public PageVisit()
        {
            Ip = "Ip not recorded";
            TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
            UserAgent= string.Empty;
            this.LinkClicks = new HashSet<LinkClick>();
        }
        [Key]
        public int Id { get; set; }
        public string Ip { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool ContactDownloaded { get; set; }
        public Guid? ContactPageId { get; set; }
        public string? UserAgent { get; set; }
        public int Counter { get; set; }
        public virtual ICollection<LinkClick> LinkClicks { get; set; }
    }
}
