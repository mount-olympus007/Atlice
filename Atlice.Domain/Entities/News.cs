namespace Atlice.Domain.Entities
{
    public class News
    {
        public News()
        {
            this.Photos = new HashSet<Photo>();
            this.Films = new HashSet<Film>();
        }

        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public PostType PostType { get; set; }
        public Category Category { get; set; }
        public string? EmbedCode { get; set; }
        public string? SharedLink { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }
        public virtual ICollection<Film>? Films { get; set; }
        public DateTime Created { get; set; }
    }

    public enum PostType
    {
        Photo,AboutMe,Reel,Video,Embed,Quote,Link,Music
    }
    public enum Category
    {
        Art,Social
    }
}
