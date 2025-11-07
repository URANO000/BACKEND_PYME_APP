using DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Orders
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        //CREATE
        public async Task CreateAsync(OrderDetailDA orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }

        //Get by ID
        public async Task<OrderDetailDA?> GetByIdAsync(int id)
          => await _context.OrderDetails
                .FindAsync(id);

        public async Task UpdateAsync(int id, OrderDetailDA orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
            await _context.SaveChangesAsync();
        }
        //Dlete
        public async Task DeleteAsync(OrderDetailDA orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }

        //To return a collection
        public async Task<List<OrderDetailDA>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }
    }
}
