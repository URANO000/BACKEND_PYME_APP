

using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.DTOs.Login
{
    public class LoginDTO
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
