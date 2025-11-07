
using DataAccess.Models.DTOs.Role;

namespace DataAccess.Models.DTOs.User
{
    public class UsersDTO
    {
        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        //Navigation
        public RoleDTO Role { get; set; } = new RoleDTO();

    }
}
