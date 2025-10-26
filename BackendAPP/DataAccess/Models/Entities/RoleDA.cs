using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Entities
{
    [Table("ROLE")]
    public class RoleDA
    {
        [Column("role_id")]
        public int roleId { get; set; }
        [Column("role")]
        public RoleName roleName { get; set; }

        //Collection
        public ICollection<UsersDA> Users { get; set; }

    }
}
