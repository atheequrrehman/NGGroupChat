using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Models
{
    public class UserGroup
    {
        public string UserName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
}
