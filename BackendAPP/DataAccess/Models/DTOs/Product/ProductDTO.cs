using DataAccess.Models.DTOs.Category;

namespace DataAccess.Models.DTOs.Product
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal Stock { get; set; }
        public string? Image { get; set; }
        public string State { get; set; }

        //Basic category reference
        public CategoryDTO Category { get; set; } = new CategoryDTO();
    }
}
