using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class Chat
    {
        public Chat()
        {
            this.Messages = new HashSet<ChatMessage>();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; }

    }
}
