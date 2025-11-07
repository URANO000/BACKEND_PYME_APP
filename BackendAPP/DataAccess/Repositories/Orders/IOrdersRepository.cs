using DataAccess.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Orders
{
    public interface IOrdersRepository
    {
        //Here I'll define the methods for managing orders
        Task CreateAsync(OrdersDA order);
        Task<OrdersDA?> GetByIdAsync(int id);
        Task<IEnumerable<OrdersDA>> GetAllAsync();
        Task UpdateAsync(OrdersDA order);
        Task DeleteAsync(int id);

    }
}
