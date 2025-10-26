using DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;



namespace DataAccess
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            //EMPTY
        }

        //DB TABLES
        public DbSet<ProductDA> Products { get; set; }
        public DbSet<CategoryDA> Categories { get; set; }
        public DbSet<OrdersDA> Orders { get; set; }
        public DbSet<ClientDA> Clients { get; set; }
        public DbSet<RoleDA> Roles { get; set; }
        public DbSet<UsersDA> Users { get; set; }

    }
}
