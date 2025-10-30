

namespace DataAccess.Models.DTOs.Order
{
    public class CreateOrdersDTO
    {
        public DateTime Date { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string? State { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }
    }
}
