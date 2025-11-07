using DataAccess;
using DataAccess.Models.DTOs.Order;
using DataAccess.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.User;
using DataAccess.Models.Entities;
using BusinessLogic.Services;

namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //First we need to call our service
        private readonly OrdersService _ordersService;
        //Then reference our logger!
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(OrdersService ordersService, ILogger<OrdersController> logger)
        {
            _ordersService = ordersService;
            _logger = logger;
        }

        //Now we can start with our methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTO>>> GetAllOrders()
        {
            try
            {
                var orders = await _ordersService.GetAllOrdersAsync();
                return Ok(orders);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error retrieving data :( something went wrong, check the exception");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }

        //Get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTO>> GetOrderById(int id)
        {
            try
            {
                var order = await _ordersService.GetOrderByIdAsync(id);
                //Little validation
                if (order == null)
                {
                    _logger.LogWarning("Order was not found");
                    return NotFound(new { message = "Orden no encontrada" });
                }
                _logger.LogInformation("Retrieved order by id succesfully!");
                return Ok(order);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order by id :(");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }

        //Now we create an order
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrdersDTO order)
        {
            try
            {
                int userId = 1; //In real scenario, get from auth context
                OrdersDTO createdOrder = await _ordersService.PlaceOrder(order, userId);
                _logger.LogInformation("Order was created succesfully :)");
                return Ok(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order :(");
                return StatusCode(500,new { message = "Error al crear la orden", error = ex.Message });
            }
        }


        //Delete...finally...finally
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            //First verify that the id matches an order
            try
            {
                await _ordersService.DeleteOrderAsync(id);
                _logger.LogInformation($"deleted {id}");
                return Ok(new { message = "Orden eliminada!" });
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error {ex.Message} --Couldn't delete... oh");
                return StatusCode(500,new { message = "Error al eliminar la orden", error = ex.Message });

            }
        }

        [HttpPut("{orderId}/user/{userId}")]
        public async Task<ActionResult> UpdateOrder(int orderId, int userId, [FromBody] CreateOrdersDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("UpdateOrder called with null DTO");
                return BadRequest(new { message = "Datos de la orden inválidos." });
            }

            try
            {
                var updatedOrder = await _ordersService.UpdateAsync(orderId, userId, dto);

                if (updatedOrder == null)
                {
                    _logger.LogWarning($"No order found with ID {orderId}.");
                    return NotFound(new { message = $"No se encontró la orden con ID {orderId}." });
                }

                _logger.LogInformation($"Order {orderId} updated by user {userId}.");
                return Ok(updatedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order {orderId} by user {userId}: {ex.Message}");
                return StatusCode(500, new { message = "Error al actualizar la orden.", error = ex.Message });
            }
        }

        //An example usage for this one is  PUT /api/orders/5/user/2

    }
}
