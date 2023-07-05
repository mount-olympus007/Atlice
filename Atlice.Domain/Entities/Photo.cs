using System.ComponentModel.DataAnnotations;

namespace Atlice.Domain.Entities
{
    public class Photo
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Photo()
        {
            FileType = "Photo";
            FilePath = "https://atlicemedia.blob.core.windows.net/atliceapp/Orion_men.svg";
            FileName = "ProfileImage";
            DateUploaded = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
            Height = "50";
            Width = "50";
        }
        [Key]
        public Guid Id { get; set; }
        public Guid CreatorID { get; set; }
        public string FileType { get; set; } 
        public string FilePath { get; set; }
        public string FileName { get; set; } 
        public DateTime DateUploaded { get; set; } 
        public string Height { get; set; } 
        public string Width { get; set; }
    }
}
