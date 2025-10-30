
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.User;

namespace DataAccess.Models.DTOs.Order
{
    public class OrdersDTO
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public required string State { get; set; } 

        //User and Client details
        public ClientDTO Client { get; set; } = new ClientDTO();
        public UsersDTO User { get; set; } = new UsersDTO();
    }
}
