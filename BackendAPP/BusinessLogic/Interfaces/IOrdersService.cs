

using DataAccess.Models.DTOs.Order;

namespace BusinessLogic.Interfaces
{
    public interface IOrdersService
    {
        Task<IEnumerable<OrdersDTO>> GetAllOrdersAsync();
        Task<OrdersDTO?> GetOrderByIdAsync(int id);
        Task<OrdersDTO> PlaceOrder(CreateOrdersDTO dto, int userId);
        Task<OrdersDTO> UpdateAsync(int id, CreateOrdersDTO dto);
        Task DeleteOrderAsync(int id);
    }
}
