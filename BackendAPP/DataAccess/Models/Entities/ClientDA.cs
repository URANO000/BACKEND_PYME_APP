using System.ComponentModel.DataAnnotations.Schema;


namespace DataAccess.Models.Entities
{
    [Table("CLIENT")]
    public class ClientDA
    {
        [Column("client_id")]
        public int ClientId { get; set; }
        [Column("first_name")]
        public string? FirstName { get; set; }
        [Column("last_name")]
        public string? LastName { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("Phone")]
        public string? Phone { get; set; }
        [Column("address")]
        public string? Address { get; set; }

        //Collection 
        public ICollection<OrdersDA>? Orders { get; set; }



    }
}
