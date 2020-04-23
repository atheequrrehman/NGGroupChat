using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NGGroupChat.Service.Api.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Providers
{
    public interface IChatService
    {
        Task<(bool IsSucess, DBModels.Message Message, Task GroupConnection, string ErrorMessage)> CreateMessage(DBModels.Message newMessage);
        Task<(bool IsSuccess, IEnumerable<Models.Message> Message,string ErrorMessage)> GetGroupMessages(int GroupId);
    }
}
