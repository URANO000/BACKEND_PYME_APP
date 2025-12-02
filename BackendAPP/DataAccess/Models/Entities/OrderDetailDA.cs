using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("OrderDetail")]
    public class OrderDetailDA
    {
        [Key]
        [Column("order_detail_id")]
        public int OrderDetailId { get; set; }
        [Column("quantity")]
        public decimal Quantity { get; set; }
        [Column("unit_price")]
        public decimal UnitPrice { get; set; }
        [Column("discount")]
        public decimal Discount { get; set; }

        [ForeignKey("order_id")]
        [Column("order_id")]
        public int OrderId { get; set; }
        [ForeignKey("product_id")]
        [Column("product_id")]
        public int ProductId { get; set; }

        //Navigation
        public OrdersDA Order { get; set; }
        public ProductDA Product { get; set; }
    }
}
