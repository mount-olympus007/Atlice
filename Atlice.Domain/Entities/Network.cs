using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Network
    {
        public Network()
        {
            Id = Guid.NewGuid();
            Name = "New Network";
            Private = true;
            Members = new HashSet<ApplicationUser>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Private { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }
    }
}
