using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("PRODUCT")]
    public class ProductDA
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("name")]
        public string Name { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
        [Column("tax_percentage")]
        public decimal TaxPercentage { get; set; }
        [Column("stock")]
        public decimal Stock {  get; set; }
        [Column("image")]
        public string? Image { get; set; }
        [Column("state")]
        public ProductState State { get; set; }
        //This is my foereign key
        [Column("category_id")]
        public int CategoryId { get; set; }

        //Here I am creating the navigation property  -MANY TO ONE
        public  CategoryDA Category { get; set; }

        //Collection navigation property linked with order details - ONE TO MANY
        public ICollection<OrderDetailDA>? OrderDetails { get; set; }
    }
}
