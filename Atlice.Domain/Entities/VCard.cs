using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Atlice.Domain.Entities
{
    public class VCard
    {
        public VCard()
        {
            Id = "0";
            FirstName = "Atlice";
            Email = "AtliceTap@atlice.com";
        }
        [Key]
        public string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } 
        public string? Website { get; set; }
        public string? Business { get; set; }
        public string? Linklist { get; set; }
        public Location? Location { get; set; }
        [NotMapped]
        public List<TapLink>? Links { get; set; }
        public byte[] Image { get; set; }
        public string? Lead { get; set; }
        const string OrganizationName = "ORG:";
        const string NewLine = "\r\n";

        public string GetFullName()
        {
            return FirstName + LastName;
        }
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("BEGIN:VCARD");
            builder.AppendLine("VERSION:3.0");
            var n = "N:" + LastName + ";" + FirstName+";";
            builder.AppendLine(n);
            var fn = "FN:" + FirstName + " " + LastName;
            builder.AppendLine(fn);
            if (!string.IsNullOrEmpty(Business))
            {
                var org = OrganizationName + Business;
                builder.AppendLine(org);
            }
            if (Image != null)
            {
                builder.AppendLine("PHOTO;ENCODING=BASE64;TYPE=JPEG:"+ Convert.ToBase64String(Image));
            }
            if (Location != null)
            {
                builder.AppendLine("ADR;TYPE=home:;;" + Location.Name);
            }
            if (Phone != null)
            {
                builder.Append("TEL:").AppendLine(Phone);
            }
            if (Email != null && !Email.Contains("atlicetap"))
            {
                var email = "EMAIL:" + Email;
                builder.AppendLine(email);
            }
            if (Website != null)
            {
                var url = "URL:" + Website;
                builder.AppendLine(url);
            }
            if (Links != null)
            {
                foreach (var link in Links)
                {
                    if (link.SocialProvider == SocialProvider.Announcement)
                    {
                        builder.AppendLine("URL;" + link.Title + ":" + link.SocialProviderMainUrl);
                        continue;
                    }
                    builder.AppendLine("URL;" + link.SocialProvider + ":"+ link.SocialProviderMainUrl);
                }
            }
            if (Lead != null)
            {
                builder.AppendLine("NOTE:"+Lead);
            }
            builder.AppendLine("END:VCARD");

            return builder.ToString();
        }
    }
}
