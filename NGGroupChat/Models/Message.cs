using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.Models
{
    public class Message
    {
        public int ID { get; set; }
        public string AddedBy { get; set; }
        public string TextMessage { get; set; }
        public int GroupID { get; set; }
    }
}
