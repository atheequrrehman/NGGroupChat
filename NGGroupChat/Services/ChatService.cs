using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NGGroupChat.Service.Api.DBModels;
using NGGroupChat.Service.Api.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly ChatDbContext chatDbContext;
        private readonly IMapper mapper;

        public ChatService(ChatDbContext chatDbContext, IMapper mapper)
        {
            this.chatDbContext = chatDbContext;
            this.mapper = mapper;
        }
        public async Task<(bool IsSucess, DBModels.Message Message, Task GroupConnection, string ErrorMessage)> CreateMessage(DBModels.Message message)
        {
            try
            {
                var newMessage = new DBModels.Message { TextMessage = message.TextMessage, AddedBy = message.AddedBy, GroupID = message.GroupID };
                chatDbContext.Messages.Add(newMessage);
                await chatDbContext.SaveChangesAsync();
                return (true, newMessage, null, null);

            }

            catch (Exception)
            {
                return (false, null, null, "Message could not be sent");
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Message> Message, string ErrorMessage)> GetGroupMessages(int GroupId)
        {
            try
            {
                var messages = await chatDbContext.Messages.Where(x => x.GroupID == GroupId).ToListAsync();
                if (messages != null && messages.Any())
                {
                    var result = mapper.Map<IEnumerable<DBModels.Message>, IEnumerable<Models.Message>>(messages);
                    return (true, result, null);
                }
                return (false, null, "No Messages found");

            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
