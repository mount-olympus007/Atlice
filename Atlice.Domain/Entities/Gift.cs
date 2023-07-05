using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Gift
    {
        public Gift() { Id = Guid.NewGuid(); GiftType = GiftType.AtliceTap; }
        public Guid Id { get; set; }
        public Guid GiftId { get; set; }
        public Guid To { get; set; }
        public Guid From { get; set; }
        public GiftType GiftType { get; set; }
        
    }

    public enum GiftType
    {
        AtliceTap
    }
}
