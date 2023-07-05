using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Subscription
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public Subscription(Guid UserId, SubscriptionType type)
        {
            Id= Guid.NewGuid();
            this.UserId = UserId;
            this.SubscriptionType = type;
            this.Price = GetPrice(type);
            this.Created= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            this.ExpireDate= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime().AddYears(1);
            this.LastModified = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            this.MembershipPaid = false;
            this.InitiationFee = 1;
            this.InitiationFeePaid = false;
            this.PayHistory = this.PayHistory + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime() + ": " + this.Price+"; ";
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public decimal Price { get; set; }
        public DateTime Created { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool MembershipPaid { get; set; }
        public decimal InitiationFee { get; set; }
        public bool InitiationFeePaid { get; set; }
        public DateTime InitiationDate { get; set; }
        public string PayHistory { get; set; }

        public decimal GetPrice(SubscriptionType type)
        {
            return type switch
            {
                SubscriptionType.Standard => 0,
                SubscriptionType.Premium => 120,
                SubscriptionType.Founder => 0,
                _ => throw new Exception()
            };
        }
    }
    public enum SubscriptionType
    {
        Standard,Premium,Founder
    }

    
}
