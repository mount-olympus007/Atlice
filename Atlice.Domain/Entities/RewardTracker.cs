using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class RewardTracker
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool Credentials { get; set; }
        public bool EligibilityForm { get; set; }
        public bool PlacedOrder { get; set; }
        public bool OnboardingStep2 { get; set; }
        public bool VerifyStep { get; set; }
        public bool Terms { get; set; }
        public bool DeviceSelect { get; set; }
        public bool SetupContactPage { get; set; }
        public bool OnboardingStep7 { get; set; }
    }
}
