using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Atlice.Domain.Concrete
{
    public class EFDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public EFDbContext(DbContextOptions<EFDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ContactPage>().HasMany(c => c.TapLinks);
            builder.Entity<TapLink>().Property(c => c.SocialProvider).HasConversion(v => v.ToString(), v => (SocialProvider)Enum.Parse(typeof(SocialProvider), v));
            builder.Entity<Order>().Property(c => c.Status).HasConversion(v => v.ToString(), v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v));
            builder.Entity<Order>().Property(c => c.WebflowStatus).HasConversion(v => v.ToString(), v => (WebflowStatus)Enum.Parse(typeof(WebflowStatus), v));
        }
        public DbSet<Badge> Badges => Set<Badge>();
        public DbSet<News> News => Set<News>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<VCard> VCards => Set<VCard>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<TapLink> TapLink => Set<TapLink>();
        public DbSet<Contact> Contact =>Set<Contact>();
        public DbSet<ContactList> ContactList => Set<ContactList>();
        public DbSet<AtliceTap> AtliceTap => Set<AtliceTap>();
        public DbSet<Chat> Chat => Set<Chat>();
        public DbSet<ChatMessage> ChatMessage => Set<ChatMessage>();
        public DbSet<ContactPage> ContactPage => Set<ContactPage>();
        public DbSet<ErrorFeedback> ErrorFeedback => Set<ErrorFeedback>();
        public DbSet<Location> Location => Set<Location>();
        public DbSet<AdminNote> AdminNote => Set<AdminNote>();
        public DbSet<PageVisit> PageVisit => Set<PageVisit>();
        public DbSet<Photo>  Photo => Set<Photo>();
        public DbSet<YouLoveProfile> YouLoveProfile => Set<YouLoveProfile>();
        public DbSet<Legislator> Legislator => Set<Legislator>();
        public DbSet<State> State => Set<State>();
        public DbSet<ConnectedUser> ConnectedUsers => Set<ConnectedUser>();
        public DbSet<RewardTracker> RewardTracker => Set<RewardTracker>();
        public DbSet<Passport> Passport => Set<Passport>();
        public DbSet<Stamp> Stamp => Set<Stamp>();
        public DbSet<LinkClick> LinkClick => Set<LinkClick>();
        public DbSet<Film> Film => Set<Film>();
        public DbSet<Organization> Organization => Set<Organization>();
        public DbSet<Gift> Gift => Set<Gift>();
        public DbSet<Event> Event => Set<Event>();
    }

}
