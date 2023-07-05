namespace Atlice.Domain.Entities
{
    public class ContactList
    {
        public ContactList()
        {
            Id = Guid.NewGuid();
            Contacts = new HashSet<Contact>();
        }
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }

    }
}
