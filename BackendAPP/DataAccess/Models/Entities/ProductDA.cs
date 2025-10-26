using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("PRODUCT")]
    public class ProductDA
    {
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("price")]
        public decimal Price { get; set; }
        [Column("tax_percentage")]
        public decimal TaxPercentage { get; set; }
        [Column("image")]
        public string? Image { get; set; }
        public ProductState State { get; set; }
        //This is my foereign key
        [Column("category_id")]
        public int CategoryId { get; set; }

        //Here I am creating the navigation property  -MANY TO ONE
        public required CategoryDA Category { get; set; }
    }
}
