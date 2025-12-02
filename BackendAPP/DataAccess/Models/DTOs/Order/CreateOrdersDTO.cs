

namespace DataAccess.Models.DTOs.Order
{
    public class CreateOrdersDTO
    {
        public int ClientId { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; } = new();
    }
}
