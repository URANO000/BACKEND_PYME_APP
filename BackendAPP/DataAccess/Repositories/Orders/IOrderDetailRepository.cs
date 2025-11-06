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
    }
}
