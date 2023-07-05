using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class ErrorFeedback
    {
        [Key]
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Comment { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string? RequestId { get; set; }
    }
}
