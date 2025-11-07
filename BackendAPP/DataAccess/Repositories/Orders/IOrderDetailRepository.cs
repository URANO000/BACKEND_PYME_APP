using DataAccess.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Orders
{
    public interface IOrderDetailRepository
    {
        //CREATE 
        Task CreateAsync(OrderDetailDA orderDetail);

        //Get by ID
        Task<OrderDetailDA?> GetByIdAsync(int id);

        //Update by id
        Task UpdateAsync(int id, OrderDetailDA orderDetail);

        //Delete by id
        Task DeleteAsync(OrderDetailDA orderDetail);

        //Get by OrderId
        Task<List<OrderDetailDA>> GetByOrderIdAsync(int orderId);
    }
}
