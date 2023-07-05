using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Atlice.Domain.Abstract
{
    public interface IDataRepository
    {
        IEnumerable<ConnectedUser> ConnectedUsers { get; }
        IEnumerable<ApplicationUser> Users { get; }
        IEnumerable<Badge> Badges { get; }
        IEnumerable<News> News { get; }
        IEnumerable<Notification> Notifications { get; }
        IEnumerable<VCard> VCards { get; }
        IEnumerable<Order> Orders { get; }
        IEnumerable<TapLink> TapLinks { get; }
        IEnumerable<Contact> Contacts { get; }
        IEnumerable<ContactList> ContactLists { get; }
        IEnumerable<AtliceTap> Taps { get; }
        IEnumerable<Chat> Chats { get; }
        IEnumerable<ChatMessage> ChatMessages { get; }
        IEnumerable<ContactPage> ContactPages { get; }
        IEnumerable<Location> Locations { get; }
        IEnumerable<AdminNote> AdminNotes { get; }
        IEnumerable<PageVisit> PageVisits { get; }
        IEnumerable<Photo> Photos { get; }
        IEnumerable<YouLoveProfile> YouLoveProfiles { get; }
        IEnumerable<State> States { get; }
        IEnumerable<Legislator> Legislators { get; }
        IEnumerable<RewardTracker> RewardsTrackers { get; }
        IEnumerable<ErrorFeedback> ErrorFeedbacks { get; }
        IEnumerable<Passport> Passports { get; }
        IEnumerable<Stamp> Stamps { get; }
        IEnumerable<LinkClick> LinkClicks { get; }
        IEnumerable<IdentityUserRole<Guid>> UserRoles { get; }
        IEnumerable<Gift> Gifts { get; }
        IEnumerable<Event> Events { get; }
        Task<Organization> SaveOrganization(Organization organization);
        Task<Notification> SaveNotification(Notification notification);
        Task<Stamp> SaveStamp(Stamp stamp);
        Task<Passport> SavePassport(Passport passport);
        //Task DeleteStamp(Guid stampId);
        //Task DeletePassport(Guid passportId);
        Task<Photo> SavePhoto(Photo photo);
        Task<LinkClick> SaveLinkClick(LinkClick linkClick);
        Task<PageVisit> SavePageVisit(PageVisit pageVisit);
        Task<Contact> SaveContact(Contact contact);
        Task DeleteContactPage(ContactPage contactPage);
        Task DeleteContactList(ContactList contactList);
        Task DeleteErrorFeedback(Guid id);
        Task DeleteOrder(Guid orderid);
        Task<ConnectedUser> AddConnectedUser(ConnectedUser connectedUser);
        Task<ConnectedUser> RemoveConnectedUser(string connectedUserId);
        Task<ErrorFeedback> SaveErrorFeedback(ErrorFeedback errorFeedback);
        Task<AtliceTap> SaveTap(AtliceTap atliceTap);
        Task<AdminNote> SaveAdminNote(AdminNote adminNote);
        Task<Order> SaveOrder(Order order);
        Task<YouLoveProfile> SaveYouLoveProfile(YouLoveProfile youLoveProfile);
        Task<ContactPage> SaveContactPage(ContactPage contactPage);
        Task<ContactList> SaveContactList(ContactList contactList);
        Task<Location> SaveLocation(Location location);
        Task<State> SaveState(State state);
        Task<Legislator> SaveLegislator(Legislator legislator);
        Task<ApplicationUser> CreateProfile(Guid id);
        Task<TapLink> SaveTapLink(TapLink tapLink);
        Task DeleteTapLink(TapLink tapLink);
        Task<ChatMessage> SaveChatMessage(ChatMessage chatMessage);
        Task<Chat> SaveChat(Chat chat);
        Task DeleteContact(Contact contact);
        Task DeleteTap(Guid tapid);
        Task<RewardTracker> SaveRewardTracker(RewardTracker rewardTracker);
        Task<Event> SaveEvent(Event e);
    }
}
