using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class Order
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Order(string CustomerName,string ShippingName, string Address, string City, string State, string Zip, string? phoneNumber, string email, Guid userId, string? customerId)
        {
            Name = CustomerName ?? "New Order";
            OrderNumber = Guid.NewGuid().ToString();
            WebflowStatus = WebflowStatus.pending;
            ShipAddressLine1 = Address ?? "Required";
            ShipName = ShippingName;
            ShipCity = City ?? "Required";
            ShipState = State ?? "Required";
            ShipCode = Zip ?? "Required";
            Phone = phoneNumber ?? "Required";
            Email = email ?? "Required";
            UserId = userId;
            CustomerId = customerId;
            Status = OrderStatus.NewOrder;
            OrderRecieved = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            Taps = new List<AtliceTap>();
        }
        public Order() { Taps = new List<AtliceTap>(); OrderNumber = Guid.NewGuid().ToString(); }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string OrderNumber { get; set; }
        public WebflowStatus WebflowStatus { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddressLine1 { get; set; }
        public string? ShipAddressLine2 { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipState { get; set; }
        public string? ShipCode { get; set; }

        public string? Last4 { get; set; }
        public string? Brand { get; set; }
        public string? ChargeId { get; set; }
        public string? CustomerId { get; set; }
        public string? OwnerName { get; set; }
        public string? NameOnCard { get; set; }
        public string? Company { get; set; }
        public string? Role { get; set; }
        public string? LogoUrl { get; set; }
        public string? CustomerPaid { get; set; }
        public string? Comments { get; set; }
        public string? Tracking { get; set; }
        [Phone]
        public string? Phone { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderRecieved { get; set; }
        public DateTime OrderShipped { get; set; }
        public DateTime OrderDelivered { get; set; }
        public virtual ICollection<AtliceTap> Taps { get; set; }
        public bool BookMarked { get; set; }

    }

    public enum OrderStatus
    {
        NewOrder, RecuritmentOrder, ManualOrder, PartiallyComplete, PendingShipment, Shipped, Activated, PartiallyActivated,Incomplete, HandDeliver
    }

    public enum WebflowStatus
    {
        pending, unfulfilled, fulfilled, disputed, disputelost, refunded

    }
}
