
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("ROLE")]
    public class RoleDA
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("role")]
        public string? RoleName { get; set; }

        //Collection
        public ICollection<UsersDA> Users { get; set; }

    }
}
