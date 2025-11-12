using BusinessLogic.Services;
using DataAccess;
using DataAccess.Models.DTOs;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.Order;
using DataAccess.Models.DTOs.User;
using DataAccess.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        [Authorize(Roles = "ADMINISTRADOR, VENTAS, OPERACIONES")]
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
        [Authorize(Roles = "ADMINISTRADOR, VENTAS, OPERACIONES")]
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
        [Authorize(Roles = "ADMINISTRADOR, VENTAS")]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrdersDTO order)
        {
            try
            {
                //I extract the user from JWT
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                {
                    _logger.LogWarning("No user ID found in token.");
                    return Unauthorized(new { message = "No se pudo identificar el usuario autenticado :(" });
                }

                int userId = int.Parse(userIdClaim);
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
        [Authorize(Roles = "ADMINISTRADOR")]
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

        [HttpPut("{orderId}")]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<ActionResult> UpdateOrder(int orderId, [FromBody] CreateOrdersDTO dto)
        {
            if (dto == null)
            {
                _logger.LogWarning("UpdateOrder called with null DTO");
                return BadRequest(new { message = "Datos de la orden inválidos." });
            }

            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "No se pudo identificar el usuario autenticado." });
                }

                int userId = int.Parse(userIdClaim);

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
                _logger.LogError($"Error updating order {orderId}: {ex.Message}");
                return StatusCode(500, new { message = "Error al actualizar la orden.", error = ex.Message });
            }
        }

        //An example usage for this one is  PUT /api/orders/5/user/2
        //calculate details for live calculation of details
        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR, VENTAS")]
        public async Task<ActionResult<object>> CalculateTotal([FromBody] List<CreateOrderDetailDTO> orderDetails)
        {
            if (orderDetails == null || !orderDetails.Any())
            {
                return BadRequest("Debe incluir al menos un detalle de pedido para calcular el total...");
            }

            try
            {
                var (subtotal, impuesto, total) = await _ordersService.CalculateTotalAsync(orderDetails);
                _logger.LogInformation("Detail calculated successfully");

                return Ok(new
                {
                    Subtotal = subtotal,
                    Impuesto = impuesto,
                    Total = total
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating");
                return StatusCode(500, $"Error al calcular el total: {ex.Message}");
            }
        }
    }
}
