using Microsoft.AspNetCore.SignalR;
using Project.Server.DTOs;
using Project.Server.Models;
using Project.Server.Service;

namespace Project.Server.SocketHub
{
    public class SocketHub: Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IGroupChatService _groupChatService;
        private static Dictionary<string, string> _userConnections = new Dictionary<string, string>();


        public SocketHub(IMessageService messageService, IConversationService conversationService, IGroupChatService groupChatService)
        {
            _messageService = messageService;
            _conversationService = conversationService;
            _groupChatService = groupChatService;
        }

        public async Task RegisterUser(string userId)
        {
            _userConnections[userId] = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, $"notifications_{userId}");
            Console.WriteLine($"User {userId} registered with ConnectionId {Context.ConnectionId}");
        }

        public async Task JoinChat(int chatId, bool isConversation)
        {
            string chatGroup = !isConversation ? $"group_{chatId}" : $"conversation_{chatId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, chatGroup);
        }

        public async Task LeaveChat(int chatId, bool isConversation)
        {
            string chatGroup = !isConversation ? $"group_{chatId}" : $"conversation_{chatId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatGroup);
        }

        public async Task SendMessageToConversation(int conversationId, MessageDTO message)
        {
            
            message = await _messageService.CreateMessageForConversation(message);
            string conversation = $"conversation_{conversationId}";

            Conversation convo = _conversationService.GetUserConversationById(conversationId);
            

            foreach (UserConversation user in convo.Users){
                string userId = user.UserId.ToString();
                if(_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("UpdateChatList", message);
            }

            await Clients.Group(conversation).SendAsync("ReceiveMessage", message);
        }

        public async Task SendMessageToGroupChat(int groupId, MessageDTO message)
        {
            message = await _messageService.CreateMessageForGroupChat(message);
            string groupChat = $"group_{groupId}";

            GroupChat gc = _groupChatService.GetGroupChatById(groupId);
            foreach (var user in gc.Users) {
                string userId = user.UserId.ToString();
                if (_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("UpdateChatList", message);
            }
            await Clients.Group(groupChat).SendAsync("ReceiveMessage", message);

        }

        public async Task UpdateMessageContent(int chatId, bool isConversation, MessageDTO updateMessage)
        {
            string chatGroup = isConversation ? $"conversation_{chatId}" : $"group_{chatId}";

            var message = _messageService.GetMessageById(updateMessage.Id);
            if (message == null)
            {
                throw new KeyNotFoundException($"Message with ID {updateMessage.Id} not found.");
            }

            updateMessage = await _messageService.UpdateMessageContent(message, updateMessage.Content);
            await Clients.Group(chatGroup).SendAsync("UpdateMessage", updateMessage);

            if (isConversation)
            {
                Conversation chat = _conversationService.GetUserConversationById(chatId);
                foreach (var user in chat.Users)
                {
                    string userId = user.UserId.ToString();
                    if (_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("UpdateMessage", updateMessage);

                }
            }
            else { 
                GroupChat chat = _groupChatService.GetGroupChatById(chatId);
                foreach (var user in chat.Users)
                {
                    string userId = user.UserId.ToString();
                    if (_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("UpdateMessage", updateMessage);
                }
            }
        }

        public async Task DeleteMessage(int chatId, bool isConversation,MessageDTO messageDTO) {
            string chatGroup = isConversation ? $"conversation_{chatId}" : $"group_{chatId}";

            await _messageService.DeleteMessage(messageDTO.Id);
            await Clients.Group(chatGroup).SendAsync("DeleteMessage", messageDTO);

            if (isConversation)
            {
                Conversation chat = _conversationService.GetUserConversationById(chatId);
                foreach (var user in chat.Users)
                {
                    string userId = user.UserId.ToString();
                    if (_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("DeleteMessage", messageDTO);

                }
            }
            else
            {
                GroupChat chat = _groupChatService.GetGroupChatById(chatId);
                foreach (var user in chat.Users)
                {
                    string userId = user.UserId.ToString();
                    if (_userConnections.ContainsKey(userId)) await Clients.Client(_userConnections[userId]).SendAsync("DeleteMessage", messageDTO);
                }
            }

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = _userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                _userConnections.Remove(userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

    }

}
