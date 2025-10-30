

using DataAccess.Models.DTOs.Order;

namespace BusinessLogic.Interfaces
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrdersDTO>> GetAllAsync();
        Task<OrdersDTO?> GetByIdAsync(int id);
        Task CreateAsync(CreateOrdersDTO dto);
        Task UpdateAsync(int id, CreateOrdersDTO dto);
        Task DeleteAsync(int id);
    }
}
