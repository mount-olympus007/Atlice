namespace Atlice.Domain.Entities
{
    public class Contact
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Contact()
        {
            DateMeet = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
            Name = "New Contact";
        }
        public Guid? Id { get; set; }
        public DateTime DateMeet { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
        public string? Note { get; set; }
        public string? Name { get; set; }
        public string? Website { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public virtual ContactPage? LinkedPage { get; set; }
        public virtual Location? Location { get; set; }
        public string? ApplicationUserID { get; set; }
    }
}
