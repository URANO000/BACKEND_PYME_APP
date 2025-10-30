

namespace DataAccess.Models.DTOs.Client
{
    public class CreateClientDTO
    {
        public string Cedula { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}
