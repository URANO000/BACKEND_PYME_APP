using DataAccess;
using DataAccess.Models.DTOs.Order;
using DataAccess.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.User;
using DataAccess.Models.Entities;

namespace BackendAPP.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //First we need to call our context
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Now we can start with our methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersDTO>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.User)
                .Select(o => new OrdersDTO
                {
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Subtotal = o.Subtotal,
                    Tax = o.Tax,
                    Total = o.Total,
                    State = o.State.ToString(),
                    Client = new ClientDTO
                    {
                        ClientId = o.Client.ClientId,
                        FirstName = o.Client.FirstName,
                        LastName = o.Client.LastName,
                        Email = o.Client.Email,
                        Phone = o.Client.Phone,
                        Address = o.Client.Address
                    },
                    User = new UsersDTO
                    {
                        UserId = o.User.UserId,
                        Username = o.User.Username,
                        Email = o.User.Email,
                        //No password for security reasons
                    }
                })
                .ToListAsync();
            return Ok(orders);
        }

        //Get by id
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdersDTO>> GetOrderById(int id)
        {
            //Store inside order, the order that matches by id
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.User)
                .Where(o => o.OrderId == id)
                .Select(o => new OrdersDTO
                {
                    OrderId = o.OrderId,
                    Date = o.Date,
                    Subtotal = o.Subtotal,
                    Tax = o.Tax,
                    Total = o.Total,
                    State = o.State.ToString(),
                    Client = new ClientDTO
                    {
                        ClientId = o.Client.ClientId,
                        FirstName = o.Client.FirstName,
                        LastName = o.Client.LastName,
                        Email = o.Client.Email,
                        Phone = o.Client.Phone,
                        Address = o.Client.Address
                    },
                    User = new UsersDTO
                    {
                        UserId = o.User.UserId,
                        Username = o.User.Username,
                        Email = o.User.Email,
                        //No password for security reasons
                    }
                })
                .FirstOrDefaultAsync();
            //Let's check if it exists!
            if(order == null)
            {
                return NotFound(new { message = "La orden no existe!" });
            }

            return Ok(order);
        }

        //Now we create an order
        [HttpPost]
        public async Task<ActionResult<OrdersDTO>> CreateOrder([FromBody] CreateOrdersDTO ordersDTO)
        {
            //First we verify that the object is not empty
            if(ordersDTO == null)
            { 
                return BadRequest(new { message = "Datos de orden inválidos" });
            }

            //Now we add all of the items to a new OrdersDA
            var order = new OrdersDA
            {
                Date = ordersDTO.Date,
                Subtotal = ordersDTO.Subtotal,
                Tax = ordersDTO.Tax,
                Total = ordersDTO.Total,
                ClientId = ordersDTO.ClientId,
                UserId = ordersDTO.UserId,

                //Do the whole enum spiel
                State = Enum.TryParse<OrderState>(ordersDTO.State, true, out var parsedState)
                ? parsedState
                : OrderState.PENDING //Default value if parsing fails

            };

            //Validate client and user
            var client = await _context.Clients
                .FindAsync(ordersDTO.ClientId);

            if (client == null)
            {
                return NotFound(new { message = "Cliente no existe..." });
            }

            var user = await _context.Users
                .FindAsync(ordersDTO.UserId);

            if(user == null)
            {
                return NotFound(new { message = "Usuario no existe..." });
            }

            //Now we link those up
            order.Client = client;
            order.User = user;

            //We create a response DTO
            var orderResponse = new OrdersDTO
            {
                OrderId = order.OrderId,
                Date = order.Date,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                State = order.State.ToString(),
                Client = new ClientDTO
                {
                    ClientId = client.ClientId,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email

                },
                User = new UsersDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username
                }
            };


            //Finally we create and save
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new {id = order.OrderId}, orderResponse);
        }

        //Update
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] OrdersDTO updatedOrder)
        {
            //First we find the order by id
            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            //Validate that it exists
            if (order == null)
            {
                return NotFound(new { message = "Orden no encontrada" });
            }

            //Validate client and user if needed
            var client = await _context.Clients.FindAsync(order.ClientId);
            if(client == null)
            {
                return NotFound(new { message = "Cliente no encontrado" });
            }
            var user = await _context.Users.FindAsync(order.UserId);
            if(user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            //If it does exist, then pdate the database with all the data
            order.Date = order.Date;
            order.Subtotal = order.Subtotal;
            order.Tax = order.Tax;
            order.Total = order.Total;
            order.State = Enum.Parse<OrderState>(updatedOrder.State, true);
            order.Client = client;
            order.User = user;

            //Once everything has been updated, then save to the database
            await _context.SaveChangesAsync();

            //Now we can convert it to a DTO to return it
            var orderResponse = new OrdersDTO
            {
                OrderId = order.OrderId,
                Date = order.Date,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                State = order.State.ToString(),
                Client = new ClientDTO
                {
                    ClientId = client.ClientId,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Email = client.Email

                },
                User = new UsersDTO
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Username = user.Username
                }

            };

            return Ok(orderResponse);
        }

        //Delete...finally...finally
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            //First verify that the id matches an order
            var order = await _context.Orders
                .FindAsync(id);

            //Validate that it isn't null
            if(order == null)
            {
                return NotFound(new { message = "Orden no encontrada" });
            }

            //If it's not null, then delete and save
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Orden elimiada con éxito" });
        }
    }
}
