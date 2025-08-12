using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Project.Server.Data;
using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public class GroupChatService : IGroupChatService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public GroupChatService(ApplicationDbContext db, IUserService userService, IMessageService messageService)
        {
            _db = db;
            _userService = userService;
            _messageService = messageService;
        }

        public GroupChat GetGroupChatById(int id)
        {
            return _db.GroupChats
                .Include(gc => gc.Users)
                .ThenInclude(ugc => ugc.User)
                .Include(ugc => ugc.Messages)
                .ThenInclude(m => m.Attachments)
                .AsSplitQuery()
                .SingleOrDefault(gc => gc.Id == id);
        }
        public List<GroupChat> GetUserChats(User user)
        {
            return user.GroupChats.Select(ugc => GetGroupChatById(ugc.GroupChatId)).ToList();
        }


        public GroupChatDTO GetGroupChatDTO(GroupChat groupChat)
        {
            Message? latestMessage = groupChat.Messages.Count > 0 ? groupChat.Messages.Last() : null;
            if (latestMessage == null) return new GroupChatDTO(groupChat.Id, groupChat.Name, 
                                                               _userService.GetUserDTOsFromUserGroupChat((List<UserGroupChat>)groupChat.Users),
                                                                groupChat.LastMessageReceived);
           return new GroupChatDTO(groupChat.Id,
                                     groupChat.Name,
                                     _userService.GetUserDTOsFromUserGroupChat((List<UserGroupChat>)groupChat.Users),
                                     _messageService.GetMessageDTO(latestMessage),
                                     groupChat.LastMessageReceived);
        }

        public List<GroupChatDTO>GetGroupChatList(List<GroupChat> groupChats)
        {
            return groupChats.Select(gc => GetGroupChatDTO(gc)).ToList();
        }

        public List<GroupChatDTO> GetGroupChatListFromQuery(User user, string query)
        {
            string likeQuery = $"%{query}";
            List<GroupChat> matchedGroupChats = (List<GroupChat>)_db.GroupChats
                .Include(groupChat => groupChat.Users)
                .ThenInclude(gcu => gcu.User)
                .Where(groupChat => EF.Functions.Like(groupChat.Name, query) &&
                       groupChat.Users.Any(gcu => gcu.UserId == user.Id)).ToList();

            return GetGroupChatList(matchedGroupChats);
                
        }

        public async Task<GroupChatDTO> CreateGroupChat(GroupChatDTO groupChat, User user)
        {
            GroupChat newGC = new GroupChat(groupChat.Name);
            _db.GroupChats.Add(newGC);
            await _db.SaveChangesAsync();
            groupChat.Id = newGC.Id;
            var userGroupChats = groupChat.Users.Select(u =>
            {
                return new UserGroupChat
                {
                    GroupChatId = newGC.Id,
                    UserId = u.Id
                };
            }).ToList();
            userGroupChats.Insert(0, new UserGroupChat
            { 
                GroupChatId = newGC.Id,
                UserId = user.Id
            });

            _db.UserGroups.AddRange(userGroupChats);
            await _db.SaveChangesAsync();

            return groupChat;
        }
    }
}
