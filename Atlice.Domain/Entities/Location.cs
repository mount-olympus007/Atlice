using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class Location
    {
        public Location()
        {
            Latitude = "Required";
            Longitude = "Required";
            GoogleID = "Required";
            Name = "Required";
            City = "Required";
        }
        [Key]
        public Guid? Id { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string GoogleID { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

    }
}
