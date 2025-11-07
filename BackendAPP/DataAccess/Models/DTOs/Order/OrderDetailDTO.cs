using DataAccess.Models.DTOs.Category;
using DataAccess.Models.DTOs.Product;

namespace DataAccess.Models.DTOs.Order
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Discount { get; set; }

        //Database relations
        public OrdersDTO Order { get; set; } = new OrdersDTO();
        public ProductDTO Product { get; set; } = new ProductDTO { Category = new CategoryDTO(), State = string.Empty };
    }
}
