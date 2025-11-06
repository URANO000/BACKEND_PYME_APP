
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models.Entities
{
    [Table("ORDERS")]
    public class OrdersDA
    {
        [Key]
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
        [Column("state")]
        public String State { get; set; }
        [ForeignKey("client_id")]
        [Column("client_id")]
        public int ClientId { get; set; }

        [ForeignKey("user_id")]
        [Column("user_id")]
        public int UserId { get; set; }

        //Navigation
        public  ClientDA Client { get; set; }
        public  UsersDA User { get; set; }

        //Collection navigation property linked with order details  - ONE TO MANY
        public ICollection<OrderDetailDA>? OrderDetails { get; set; }

    }
}
