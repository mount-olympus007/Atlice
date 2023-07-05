using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Event
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Event(string who, string what, EventType eventType, string method, bool error)
        {
            Id= Guid.NewGuid();
            Who = who;
            What = what;
            EventType = eventType;
            MethodName= method;
            HasError= error;
            CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
        }
        public Event() { }
        public Guid Id { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public EventType EventType { get; set; }
        public string MethodName { get; set; }
        public bool HasError { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public enum EventType
    {
        Admin,User,Anonymous
    }
}
