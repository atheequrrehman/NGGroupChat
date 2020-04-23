using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NGGroupChat.Service.Api.DBModels
{
    [Table("User", Schema = "dbo")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string FirstName { get; set; }

        [Column(TypeName = "varchar(20)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public string UserName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Password { get; set; }

    }
}
