using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class Legislator
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Jon Doe"; 
        public string Party { get; set; } = "Democrat" ?? "Republican";
        public int StateId { get; set; }
        public string Title { get; set; } = "Senator" ?? "Representative";
        public string? District { get; set; }
        public string? Tenure { get; set; }
        public string? Image { get; set; }
        public string? Lat { get; set; }
        public string? Long { get; set; }
    }
}
