using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class BackupData
    {
        public ApplicationUser? ApplicationUser { get; set; }
        public List<AtliceTap>? AtliceTaps { get; set; }
        public List<ContactPage>? ContactPages { get; set; }
        public List<ContactList>? ContactLists { get; set; }
        public YouLoveProfile? YouLoveProfile { get; set; }
        public List<Order>? Orders { get; set; }
        public RewardTracker? RewardTracker { get; set; }
        public Passport? Passport { get; set; }
    }
}
