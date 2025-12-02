using Microsoft.AspNetCore.Http;
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
        public decimal Stock { get; set; }
        //I'm doing this to take image input from a form
        public IFormFile? ImageFile { get; set; }
        public string State { get; set; } = "ACTIVE"; //Por defecto
        public int CategoryId { get; set; }
    }
}
