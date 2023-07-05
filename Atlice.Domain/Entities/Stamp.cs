using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Atlice.Domain.Entities
{
    public class Stamp
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private static IWebHostEnvironment _env => (IWebHostEnvironment)_httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));

        public Stamp(string name, Platform platform, Guid passportId)
        {
            Id = Guid.NewGuid();
            PassportId = passportId;
            Name = name;
            Description = GetDescription(name,platform);
            Platform = platform;
            TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            ImageUrl = GetImage(name,platform);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Platform Platform { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid PassportId { get; set; }
        public string? ImageUrl { get; set; }

        public string GetDescription(string Name, Platform platform)
        {
            switch (platform)
            {
                case Platform.Atlice:
                    return Name switch
                    {
                        "Founder's Stamp" => "You have been invited by the founders of the platform.  This gives you lifetime access.",
                        "Citizen's Stamp" => "You have been invited by a citizen of the platform.  This gives you premium access.",
                        "Tourist Stamp" => "You have been granted access to the Atlice Tap platform.",
                        "Lead Stamp" => "You have submitted your contact to a Atlice Tap Citizen.",
                        "Addmin Stamp" => "You have been invited by the Creators as a Special Invite.",
                        _ => throw new NotImplementedException()
                    };
                case Platform.AtliceTap:
                    return Name switch
                    {
                        "Founder's Stamp" => "You have been invited by the founders of the platform.  This gives you lifetime access.",
                        "Citizen's Stamp" => "You have been invited by a citizen of the platform.  This gives you premium access.",
                        "Tourist Stamp" => "You have been granted access to the Atlice Tap platform.",
                        "Lead Stamp" => "You have submitted your contact to a Atlice Tap Citizen.",
                        "Addmin Stamp" => "You have been invited by the Creators as a Special Invite.",
                        _ => throw new NotImplementedException()
                    };
                case Platform.PBTP:
                    return Name switch
                    {
                        "Founder's Stamp" => "You have been invited by the founders of the platform.  This gives you lifetime access.",
                        _ => throw new NotImplementedException()
                    };
                case Platform.Mastergrind:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MastergrindLife:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MastergrindNetwork:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MHSGreatness:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.KeysTonight:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.YouLove:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.ModelEntrepreneur:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.ValleyOfTheKings:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                    default: throw new NotImplementedException();
            }









            
        }
        public string GetImage(string Name, Platform platform)
        {
            var webRoot = _env.WebRootPath;

            switch (platform)
            {
                case Platform.Atlice:
                    return Name switch
                    {
                        "Founder's Stamp" => Path.Combine(webRoot, "/stamps/FoundersStamp.png"),
                        "Citizen's Stamp" => Path.Combine(webRoot, "/stamps/CitizensStamp.png"),
                        "Tourist Stamp" => Path.Combine(webRoot, "/stamps/TouristStamp.ico"),
                        "Device Stamp" => Path.Combine(webRoot, "/stamps/DeviceStamp.png"),
                        "Lead Stamp" => Path.Combine(webRoot, "/stamps/LeadStamp.svg"),
                        _ => throw new NotImplementedException()
                    };
                case Platform.AtliceTap:
                    return Name switch
                    {
                        "Founder's Stamp" => "You have been invited by the founders of the platform.  This gives you lifetime access.",
                        "Citizen's Stamp" => "You have been invited by a citizen of the platform.  This gives you premium access.",
                        "Tourist Stamp" => "You have been granted access to the Atlice Tap platform.",
                        "Device Stamp" => "You have purchased a device. You have been granted access to the Atlice Tap platform.",
                        "Lead Stamp" => "You have submitted your contact to an Atlice Citizen.",
                        _ => throw new NotImplementedException()
                    };
                case Platform.PBTP:
                    return Name switch
                    {
                        "Founder's Stamp" => "You have been invited by the founders of the platform.  This gives you lifetime access.",
                        _ => throw new NotImplementedException()
                    };
                case Platform.Mastergrind:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MastergrindLife:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MastergrindNetwork:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.MHSGreatness:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.KeysTonight:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.YouLove:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.ModelEntrepreneur:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                case Platform.ValleyOfTheKings:
                    return Name switch
                    {
                        _ => throw new NotImplementedException()
                    };
                default: throw new NotImplementedException();
            }










        }

    }

    public enum Platform
    {
        Atlice, AtliceTap, PBTP, Mastergrind, MastergrindLife, MastergrindNetwork, MHSGreatness, KeysTonight, YouLove, ModelEntrepreneur, ValleyOfTheKings
    }
}
