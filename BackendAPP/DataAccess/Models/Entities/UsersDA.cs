using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Entities
{
    [Table("USERS")]
    public class UsersDA
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("username")]
        public string? Username { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("password")]
        public string? Password { get; set; }
        public int roleId { get; set; }
        
        //Navigation
        public required RoleDA Role { get; set; }
    }
}
