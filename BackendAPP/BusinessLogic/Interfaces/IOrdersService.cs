

using DataAccess.Models.DTOs.Order;

namespace BusinessLogic.Interfaces
{
    public interface IOrdersService
    {
        //SOLID -> Single Responsibility Principle 
        Task<IEnumerable<OrdersDTO>> GetAllOrdersAsync();
        Task<OrdersDTO?> GetOrderByIdAsync(int id);
        Task<OrdersDTO> PlaceOrder(CreateOrdersDTO dto, int userId);
        Task<OrdersDTO> UpdateAsync(int orderId,int userId, UpdateOrdersDTO dto);
        Task DeleteOrderAsync(int id);

        //Calculate totals
        Task<(decimal subtotal, decimal impuesto, decimal total)> CalculateTotalAsync(List<CreateOrderDetailDTO> orderDetails);
    }
}
