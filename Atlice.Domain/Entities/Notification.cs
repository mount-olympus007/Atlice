namespace Atlice.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string? Message { get; set; }
        public DateTime Created { get; set; }
        public string GetImage(NotificationType notificationType)
        {
            return notificationType switch
            {
                NotificationType.Birthday => "../../av3.4/images/Present.png",
                NotificationType.Badge => "../../av3.4/images/atlice-tap-nfc-tag-4.png",
                NotificationType.AccountUpdate => "../../av3.4/images/settings-service-cog-maintenance.png",
                NotificationType.ContactSubmission => "../../av3.4/images/Orion_men.svg",
                _ => throw new NotImplementedException()
            };
        }
        
    }

    public enum NotificationType
    {
        Birthday, Badge, ContactSubmission, AccountUpdate
    }

}
