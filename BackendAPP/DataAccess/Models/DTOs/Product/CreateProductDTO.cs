using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.DTOs.Product
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal TaxPercentage { get; set; }
        public string? Image { get; set; }
        public string State { get; set; } = "ACTIVE"; //Por defecto
        public int CategoryId { get; set; }
    }
}
