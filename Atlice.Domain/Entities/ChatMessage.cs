using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class ChatMessage
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public ChatMessage()
        {
            Id = Guid.NewGuid();
            TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
        }
        [Key]
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public virtual ApplicationUser? Sender { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
