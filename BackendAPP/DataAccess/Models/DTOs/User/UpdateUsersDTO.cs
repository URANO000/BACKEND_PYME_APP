using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DTOs.User
{
    public class UpdateUsersDTO
    {
        [Required]
        [MinLength(3)]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}
