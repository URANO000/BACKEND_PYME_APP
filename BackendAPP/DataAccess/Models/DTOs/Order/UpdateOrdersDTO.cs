using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DTOs.Order
{
    public class UpdateOrdersDTO
    {
        public int ClientId { get; set; }
        public String? State { get; set; }
        public List<CreateOrderDetailDTO> OrderDetails { get; set; } = new();
    }
}
