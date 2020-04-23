using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Profiles
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile()
        {
            CreateMap<DBModels.User, Models.User>();
            CreateMap<DBModels.UserGroup, Models.UserGroup>();
            CreateMap<DBModels.Message, Models.Message>();
            CreateMap<DBModels.Group, Models.Group>();
        }
    }
}
