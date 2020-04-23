using Microsoft.AspNetCore.SignalR;
using NGGroupChat.Service.Api.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Services
{
    public class MessageHub : Hub
    {
        private readonly ChatDbContext chatDbContext;

        public MessageHub(ChatDbContext chatDbContext)
        {
            this.chatDbContext = chatDbContext;
        }
        public Task SendMessageToGroup(Message newMessage)
        {
            string groupName = chatDbContext.Groups.SingleOrDefault(x => x.ID == newMessage.GroupID)?.GroupName?.ToString();
            newMessage.ConnectionId = Context.ConnectionId;
            return Clients.Group(groupName).SendAsync("MessageReceived", newMessage);
        }

        public Task AddToGroup(int groupId)
        {
            string groupName = chatDbContext.Groups.SingleOrDefault(x => x.ID == groupId)?.GroupName?.ToString();
            Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            return Clients.Group(groupName).SendAsync("MessageReceived", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        public Task RemoveFromGroup(int groupId)
        {
            string groupName = chatDbContext.Groups.SingleOrDefault(x => x.ID == groupId)?.GroupName?.ToString();
            Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            return Clients.Group(groupName).SendAsync("MessageReceived", $"{Context.ConnectionId} has left the group {groupName}.");
        }
    }
}
