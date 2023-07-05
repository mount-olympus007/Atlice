using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Passport
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Passport(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            LastUpdated= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            this.Stamps = new HashSet<Stamp>();
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual ICollection<Stamp> Stamps { get; set; }
    }
}
