using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class AtliceTap
    {
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        private static IWebHostEnvironment _env => (IWebHostEnvironment)_httpContext.RequestServices.GetService(typeof(IWebHostEnvironment));
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public AtliceTap()
        {
            Id = Guid.NewGuid();
            LastEdited = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time);
            Hits = 0;
            SNumber = Guid.NewGuid().ToString();
            Note = "Created On: " + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime().ToString()+"; ";
        }
        [Key]
        public Guid Id { get; set; }
        public TapType TapType { get; set; }
        public SKU Sku { get; set; }
        public string? CustomName { get; set; }
        public virtual Location? Location { get; set; }
        public string SNumber { get; set; }
        public Guid? UserId { get; set; }
        [Url]
        public string? ForwardUrl { get; set; }
        public string? Note { get; set; }
        public virtual ContactPage? ContactPage { get; set; }
        public DateTime LastEdited { get; set; }
        public int Hits { get; set; }
        public bool Bypass { get; set; }
        [Url]
        public string? BypassURL { get; set; }
        public string? BypassSocialProvider { get; set; }
        public string? BypassImage { get; set; }
        public bool Locked { get; set; }

        public string GetOfficialName(SKU sku)
        {
            return sku switch
            {
                SKU.LGYCSTLGCD1 => "Legacy Custom Tap Card w/ Logo",
                SKU.CLSTG25GRY1 => "Classic Tap Tag 25mm",
                SKU.CLSTG30GRY1 => "Classic Tap Tag 30mm",
                SKU.CLSCDWHT => "Classic Tap Card",
                SKU.CLSCDTGCMB => "Companion Set",
                SKU.Virtual => "Virtual Device",
                _ => throw new NotImplementedException()
            };
        }

        public string GetImage(SKU sku)
        {
            var webRoot = _env.WebRootPath;
            var path = Path.Combine(webRoot, "/icons/" + sku.ToString() + ".png");

            return sku switch
            {
                SKU.LGYCSTLGCD1 => "https://atlicemedia.blob.core.windows.net/atliceapp/LGYCSTLGCD1.png",
                SKU.CLSTG25GRY1 => "https://atlicemedia.blob.core.windows.net/atliceapp/CLSTG25GRY1.png",
                SKU.CLSTG30GRY1 => "https://atlicemedia.blob.core.windows.net/atliceapp/CLSTG30GRY1.png",
                SKU.CLSCDWHT => "https://atlicemedia.blob.core.windows.net/atliceapp/Classic.png",
                SKU.CLSCDTGCMB => "https://atlicemedia.blob.core.windows.net/atliceapp/Classic.png",
                SKU.Virtual => "https://atlicemedia.blob.core.windows.net/atliceapp/Virtual.png",
                _ => throw new NotImplementedException()
            };
        }
    }
    public enum TapType
    {
        Card, Tag, Virtual
    }
    public enum SKU
    {
        LGYCSTLGCD1, CLSTG25GRY1, CLSTG30GRY1, CLSCDWHT, Virtual, CLSCDTGCMB
    }
}
