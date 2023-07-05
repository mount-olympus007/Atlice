using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class State
    {
        public State()
        {
            this.Legislators = new HashSet<Legislator>();
    
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "Alaska";
        public string Abbreviation { get; set; } = "AA";
        public string Lat { get; set; } = "0";
        public string Lng { get; set; } = "0";
        public string? FormsOfId { get; set; }
        public string? VotersWithoutId { get; set; }
        public virtual ICollection<Legislator>? Legislators { get; set; }
    }
}
