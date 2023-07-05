using Atlice.Domain.Abstract;
using Atlice.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Specialized;

namespace Atlice.WebUI.Hubs
{
    public class MessageDetail
    {
        public string? FromUserID { get; set; }
        public string? FromUserName { get; set; }
        public string? ToUserID { get; set; }
        public string? ToUserName { get; set; }
        public string? Message { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class UserDetail
    {
        public string? ConnectionId { get; set; }
        public string? UserID { get; set; }
        public string? UserName { get; set; }
        public bool Admin { get; set; }
        public bool Mobile { get; set; }
    }

    public class ChatHub : Hub
    {
        public static IHubContext<ChatHub>? Current { get; set; }
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDataRepository repository;
        private readonly IServices services;
        private static TimeZoneInfo Eastern_Standard_Time = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public ChatHub(UserManager<ApplicationUser> userManager, IDataRepository repo, IServices _services)
        {
            services = _services;
            repository = repo;
            _userManager = userManager;
        }
        #region---Data Members---
        static readonly List<UserDetail> ConnectedUsers = new();


        private static int _userCount = 0;

        #endregion

        #region---Methods---
        public int OnConnected()
        {
            _userCount++;
            return _userCount;
        }
        public int OnReconnected()
        {
            _userCount++;
            return _userCount;
        }
        public int OnDisconnected()
        {
            _userCount--;
            _userCount--;
            return _userCount;

        }
       
        public async Task<bool> Connect(string userId)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(userId);
            if(user is not null)
            {
                var id = Context.ConnectionId;
                if(!ConnectedUsers.Any(x=>x.ConnectionId == id))
                {
                    ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = user.FirstName + " " + user.LastName, UserID = user.Id.ToString(), Mobile = false });
                }

                
                UserDetail? CurrentUser = ConnectedUsers.Where(u => u.ConnectionId == id).FirstOrDefault();

                if (await _userManager.IsInRoleAsync(user, "Adminis") && CurrentUser is not null)
                {
                    CurrentUser.Admin = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> CheckConnection(string userId)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if(user is not null)
            {
                if (!ConnectedUsers.Any(x => x.UserID == user.Id.ToString()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;          
        }

        public async Task SendToUser(string message, string userid, string adminid)
        {
            ApplicationUser? appuser = await _userManager.FindByIdAsync(userid);
            ApplicationUser? admin = await _userManager.FindByNameAsync(adminid);
            if(appuser != null && admin != null)
            {
                string username = appuser.FirstName + " " + appuser.LastName;
                Chat? chat = repository.Chats.FirstOrDefault(x => x.UserId == appuser.Id);
                ChatMessage c = new()
                {
                    Message = message,
                    Sender = admin,
                    TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime()
                };
                c = await repository.SaveChatMessage(c);
                if (chat != null)
                {
                    await repository.SaveChat(chat);
                }

                await Clients.User(admin.Id.ToString()).SendAsync("ReceiveMessageHome", "Atlice", message);
                await Clients.User(appuser.Id.ToString()).SendAsync("ReceiveMessageAway", "Atlice", message);

                await Clients.User(appuser.Id.ToString()).SendAsync("UpdateChatMessages", "Atlice", admin.CoverUrl + ";;" + message + ";;" + c.TimeStamp.ToLocalTime());
            }
            

        }

        public async Task SendToSupport(string message, string userid)
        {
            ApplicationUser? appuser = await _userManager.FindByIdAsync(userid);
            if(appuser != null)
            {
                string username = appuser.FirstName + " " + appuser.LastName;
                List<ApplicationUser> admins = new();
                admins.AddRange(await _userManager.GetUsersInRoleAsync("Adminis"));
                Chat? chat = repository.Chats.FirstOrDefault(x => x.UserId == appuser.Id);
                chat ??= await repository.SaveChat(new Chat { Id = Guid.NewGuid(), UserId = appuser.Id, Messages = new List<ChatMessage>() });
                ChatMessage c = new()
                {
                    Message = message,
                    Sender = appuser,
                    TimeStamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime()
                };
                c = await repository.SaveChatMessage(c);
                chat.Messages.Add(c);
                await repository.SaveChat(chat);
                foreach (var admin in admins)
                {
                    if(admin.PhoneNumber != null)
                        await services.SendTextAsync(admin.PhoneNumber, "user: " + appuser.FirstName + " " + appuser.LastName + "; message: " + message);
                    if (ConnectedUsers.FirstOrDefault(x => x.UserID == admin.Id.ToString()) != null)
                    {
                        await Clients.User(appuser.Id.ToString()).SendAsync("ReceiveMessageHome", username, message);
                        await Clients.User(admin.Id.ToString()).SendAsync("ReceiveMessageAway", username, message);

                    }

                }
                if (!ConnectedUsers.Any(x => x.Admin))
                {
                    await Clients.User(appuser.Id.ToString()).SendAsync("ReceiveMessageAway", "Atlice", "Your message was recieved and will be responded to as fast as possible");

                }
            }
           

        }

        public async Task UpdateNotifications(NotificationType type, string userid)
        {
            ApplicationUser? appuser = await _userManager.FindByIdAsync(userid);
            if(appuser is not null)
            {
                string username = appuser.FirstName + " " + appuser.LastName;
                Badge? b = appuser.Badges.FirstOrDefault();
                Notification notification = new()
                {
                    Type = type,
                    Created = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToLocalTime(),
                };
                if (type == NotificationType.ContactSubmission)
                {
                    ContactList? contactList = repository.ContactLists.FirstOrDefault(x => x.UserId == appuser.Id);
                    if (contactList != null)
                    {
                        var lastContact = contactList.Contacts.OrderBy(x => x.DateMeet).LastOrDefault();
                        if (lastContact != null)
                        {
                            notification.Message = "You received a new contact submission from<span class='hint - bold'>" + lastContact.Name + ". </span>Go to Contacts for details. Stay activated!";
                        }
                    }

                }
                if (type == NotificationType.AccountUpdate)
                {
                    notification.Message = repository.AdminNotes.Last(x => x.UserId == appuser.Id).What;

                }
                if (type == NotificationType.Badge && b is not null)
                {
                    notification.Message = "You have received the" + b.Name;
                }

                await Clients.User(appuser.Id.ToString()).SendAsync("UpdateNotifications", "Atlice", notification.GetImage(notification.Type) + ";;" + notification.Message + ";;" + TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Eastern_Standard_Time).ToShortTimeString());

            }


        }

        public async Task UpdateNews(News news, string userid)
        {
            ApplicationUser? appuser = await _userManager.FindByIdAsync(userid);
            if (appuser != null)
            {
                string username = appuser.FirstName + " " + appuser.LastName;
                News? n = repository.News.OrderBy(x => x.Created).LastOrDefault();
                await Clients.User(appuser.Id.ToString()).SendAsync("UpdateNews", "Atlice", news.ImagePath + ";;" + news.Title + ";;" + news.Description + ";;" + news.Created.ToShortDateString());
            }
            
        }


    }




    
    #endregion

    #region---private Messages---
    //private void AddMessageinCache(MessageDetail _MessageDetail)
    //    {
    //        CurrentMessage.Add(_MessageDetail);
    //        if (CurrentMessage.Count > 100)
    //            CurrentMessage.RemoveAt(0);
    //    }
    #endregion
}
