using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DTOs.Order
{
    public class CreateOrderDetailDTO
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }  // fixed discount or percentage
        public bool IsPercentage { get; set; } // if true, discount = percentage
    }
}
