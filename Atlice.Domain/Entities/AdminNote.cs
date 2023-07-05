using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class AdminNote
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public AdminNote(Guid UserId, string Who, string What)
        {
            this.Id = Guid.NewGuid();
            this.When = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
            this.UserId = UserId;
            this.Who = Who;
            this.What = What;
        }

        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Who { get; set; } = "Admin";
        public string What { get; set; } = "System Message Test";
        public DateTime When { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);

    }
}
