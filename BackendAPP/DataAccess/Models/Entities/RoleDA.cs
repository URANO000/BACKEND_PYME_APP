using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Entities
{
    [Table("ROLE")]
    public class RoleDA
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("role")]
        public RoleName RoleName { get; set; }

        //Collection
        public ICollection<UsersDA> Users { get; set; }

    }
}
