using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Models
{
    public class CreateGroup
    {
        public string GroupName { get; set; }
        public List<string> UserNames { get; set; }
    }
}
