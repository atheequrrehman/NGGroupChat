using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Providers
{
    public interface IHubClient
    {
        Task BroadCastMessage();
    }
}
