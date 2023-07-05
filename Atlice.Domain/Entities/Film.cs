using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlice.Domain.Entities
{
    public class Film
    {
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        public Film()
        {
            FileType = "Video";
            FileName = "DefaultVideo";
            DateUploaded = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime();
        }
        [Key]
        public Guid Id { get; set; }
        public Guid CreatorID { get; set; }
        public string FileType { get; set; }
        public string? FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
