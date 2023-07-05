using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BusinessType { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string LogoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdated { get; set;}
        public virtual ApplicationUser Manager { get; set; }

    }
}
