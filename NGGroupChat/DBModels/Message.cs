using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.DBModels
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string AddedBy { get; set; }
        public string TextMessage { get; set; }
        public int GroupID { get; set; }
        public string ConnectionId { get; set; }
    }
}
