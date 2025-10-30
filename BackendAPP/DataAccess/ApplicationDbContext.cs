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

        //This is to fix my state issue
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Logic to alter state into string
            modelBuilder.Entity<ProductDA>()
                .Property(p => p.State)
                .HasConversion<string>();
            base.OnModelCreating(modelBuilder);
        }

    }
}
