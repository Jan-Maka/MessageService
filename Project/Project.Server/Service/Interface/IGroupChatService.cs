using Project.Server.DTOs;
using Project.Server.Models;

namespace Project.Server.Service
{
    public interface IGroupChatService
    {
        List<GroupChat> GetUserChats(User user);
        GroupChat GetGroupChatById(int id);
        GroupChatDTO GetGroupChatDTO(GroupChat groupChat);

        List<GroupChatDTO> GetGroupChatList(List<GroupChat> groupChats);

        List<GroupChatDTO> GetGroupChatListFromQuery(User user, string query);

        Task<GroupChatDTO> CreateGroupChat(GroupChatDTO groupChat, User user);
    }
}
