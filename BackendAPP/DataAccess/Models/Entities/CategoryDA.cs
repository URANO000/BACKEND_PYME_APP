using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("CATEGORY")]
    public class CategoryDA
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }
        [Column("name")]
        public string Name { get; set; } = string.Empty;
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        //Collection navigation property linked with products  - ONE TO MANY
        public ICollection<ProductDA>? Products { get; set; }
    }
}
