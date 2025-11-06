using DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace DataAccess.Repositories.Orders
{
    public class OrdersRepository : IOrdersRepository
    {
        //App db context
        private readonly ApplicationDbContext _context;
        public OrdersRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(OrdersDA order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrdersDA>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.Client)
                .Include(o => o.User)
                    .ThenInclude(u => u.Role)
                .ToListAsync();

        }

        public async Task<OrdersDA?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails!)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Client)
                .Include(o => o.User)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task UpdateAsync(OrdersDA order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

    }
}
