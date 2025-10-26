
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Entities
{
    [Table("Orders")]
    public class OrdersDA
    {
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("subtotal")]
        public decimal Subtotal { get; set; }
        [Column("tax")]
        public decimal Tax { get; set; }
        [Column("total")]
        public decimal Total { get; set; }
        public OrderState state { get; set; }
        public int ClientId { get; set; }
        public int UserId { get; set; }

    }
}
