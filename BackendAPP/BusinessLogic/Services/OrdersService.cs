using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Category;
using DataAccess.Models.DTOs.Client;
using DataAccess.Models.DTOs.Order;
using DataAccess.Models.DTOs.Product;
using DataAccess.Models.DTOs.Role;
using DataAccess.Models.DTOs.User;
using DataAccess.Models.Entities;
using DataAccess.Repositories.Client;
using DataAccess.Repositories.Orders;
using DataAccess.Repositories.Product;

namespace BusinessLogic.Services
{
    public class OrdersService: IOrdersService
    {

        //I'm gonna define the IVA
        private readonly IOrdersRepository _ordersRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IClientRepository _clientRepository;
        public OrdersService(IOrdersRepository ordersRepository, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository, IClientRepository clientRepository)
        {
            _ordersRepository = ordersRepository;
            _productRepository = productRepository;
            _orderDetailRepository = orderDetailRepository;
            _clientRepository = clientRepository;
        }

        //Here I start with my logic methods
        public async Task<OrdersDTO?> GetOrderByIdAsync(int id)
        {
            var order = await _ordersRepository.GetByIdAsync(id);
            if (order == null)
            {
                return null;
            }

            //else, map to dto and return
            return new OrdersDTO
            {
                OrderId = order.OrderId,
                Date = order.Date,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                State = order.State,
                Client = new ClientDTO
                {
                    ClientId = order.Client.ClientId,
                    FirstName = order.Client.FirstName,
                    LastName = order.Client.LastName,
                    Cedula = order.Client.Cedula,
                    Email = order.Client.Email,
                    Phone = order.Client.Phone,
                    Address = order.Client.Address
                },
                User = new UsersDTO
                {
                    UserId = order.User.UserId,
                    Username = order.User.Username,
                    Email = order.User.Email,
                    Role = new RoleDTO
                    {
                        RoleId = order.User.Role.RoleId,
                        RoleName = order.User.Role.RoleName
                    }
                },
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailId = od.OrderDetailId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Discount = od.Discount,
                    Product = new ProductDTO
                    {
                        ProductId = od.Product.ProductId,
                        Name = od.Product.Name,
                        Price = od.Product.Price,
                        Stock = od.Product.Stock,
                        State = od.Product.State.ToString(),
                        Category = new CategoryDTO
                        {
                            CategoryId = od.Product.Category.CategoryId,
                            Name = od.Product.Category.Name
                        }
                    }
                }).ToList()
            };
        }

        public async Task<IEnumerable<OrdersDTO>> GetAllOrdersAsync()
        {
            var orders = await _ordersRepository.GetAllAsync();
            return orders.Select(o => new OrdersDTO
            {
                OrderId = o.OrderId,
                Date = o.Date,
                Subtotal = o.Subtotal,
                Tax = o.Tax,
                Total = o.Total,
                State = o.State,
                Client = new ClientDTO
                {
                    ClientId = o.Client.ClientId,
                    Cedula = o.Client.Cedula,
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
                    Role = new RoleDTO
                    {
                        RoleId = o.User.Role.RoleId,
                        RoleName = o.User.Role.RoleName
                    }
                }
            });
        }

        public async Task DeleteOrderAsync(int id)
        {
            //validate if order exists
            var existingOrder = await _ordersRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException("Order not found");
            }
            await _ordersRepository.DeleteAsync(id);
        }

        //Calculate total of my order
        public async Task<(decimal subtotal, decimal impuesto, decimal total)> CalculateTotalAsync(List<CreateOrderDetailDTO> orderDetails)
        {
            decimal subtotal = 0;
            decimal impuesto = 0;

            foreach (var item in orderDetails)
            {
                //First validate if the product exists
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new KeyNotFoundException($"Producto con ID {item.ProductId} no encontrado.");

                //Precio base sin descuento
                decimal precioSinDescuento = product.Price * item.Quantity;

                //I asume the discount is input as a whole number
                decimal montoDescuento = item.IsPercentage
                    ? precioSinDescuento * (item.Discount / 100)
                    : item.Discount;

                //Precio con descuento (base imponible)
                decimal precioConDescuento = Math.Max(0, precioSinDescuento - montoDescuento);

                //The price is calculated with the IVA over the discount
                decimal impuestoItem = precioConDescuento * (product.TaxPercentage / 100);

                subtotal += precioConDescuento;
                impuesto += impuestoItem;
            }

            // Total = Subtotal + IVA
            decimal total = subtotal + impuesto;

            return (Math.Round(subtotal, 2), Math.Round(impuesto, 2), Math.Round(total, 2));
        }

        //Place order method
        public async Task<OrdersDTO> PlaceOrder(CreateOrdersDTO dto, int userId)
        {
            if (dto.OrderDetails == null || dto.OrderDetails.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos un detalle.");

            //VALIDATE STOCK FIRST - before creating anything...yes...tyes
            var productValidations = new List<(ProductDA product, CreateOrderDetailDTO detail)>();

            foreach (var detail in dto.OrderDetails)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId)
                    ?? throw new KeyNotFoundException($"Producto con ID {detail.ProductId} no encontrado.");

                if (product.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto '{product.Name}'.");
                }

                productValidations.Add((product, detail));
            }

            //Now that we know all products have sufficient stock, create the order
            var (subtotal, tax, total) = await CalculateTotalAsync(dto.OrderDetails);
            var order = new OrdersDA
            {
                Date = DateTime.Now,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                State = "CONFIRMADA",
                ClientId = dto.ClientId,
                UserId = userId
            };
            await _ordersRepository.CreateAsync(order);

            var orderDetailsCreated = new List<OrderDetailDTO>();

            //Process the validated products
            foreach (var (product, detail) in productValidations)
            {
                product.Stock -= (int)detail.Quantity;
                await _productRepository.UpdateAsync(product);

                var orderDetail = new OrderDetailDA
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = product.Price,
                    Discount = detail.Discount
                };
                await _orderDetailRepository.CreateAsync(orderDetail);

                orderDetailsCreated.Add(new OrderDetailDTO
                {
                    OrderDetailId = orderDetail.OrderDetailId,
                    Product = new ProductDTO
                    {
                        ProductId = product.ProductId,
                        Name = product.Name
                    },
                    Quantity = orderDetail.Quantity,
                    UnitPrice = orderDetail.UnitPrice,
                    Discount = orderDetail.Discount
                });
            }

            return new OrdersDTO
            {
                OrderId = order.OrderId,
                Date = order.Date,
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                Total = order.Total,
                State = order.State,
                Client = new ClientDTO { ClientId = order.ClientId },
                User = new UsersDTO { UserId = order.UserId }
            };
        }

        public async Task<OrdersDTO> UpdateAsync(int orderId, int userID, UpdateOrdersDTO dto)
        {
            //First things first, validate if the order exists
            var existingOrder = await _ordersRepository.GetByIdAsync(orderId);
            if (existingOrder == null)
            {
                throw new KeyNotFoundException("Order not found");
            }

            //If order details don't exist or are 0, throw error
            if (dto.OrderDetails == null || dto.OrderDetails.Count == 0)
            {
                throw new ArgumentException("El pedido debe tener al menos un detalle.");
            }

            //Now we get the old details, storing the object in this variable
            var existingDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);

            //For each to restore the previous details
            foreach (var existingDetail in existingDetails)
            {
                var product = await _productRepository.GetByIdAsync(existingDetail.ProductId);
                if (product != null)
                {
                    product.Stock += (int)existingDetail.Quantity; //Restore stock
                    await _productRepository.UpdateAsync(product);
                }
            }

            //Now we remove the existing detail
            foreach (var existingDetail in existingDetails)
            {
                await _orderDetailRepository.DeleteAsync(existingDetail);
            }

            //Now we have to make sure we calculate the right subtotals
            var (subtotal, tax, total) = await CalculateTotalAsync(dto.OrderDetails);

            //And we update the order
            existingOrder.Subtotal = subtotal;
            existingOrder.Tax = tax;
            existingOrder.Total = total;
            existingOrder.ClientId = dto.ClientId;
            existingOrder.UserId = userID;
            existingOrder.State = dto.State;

            //I'll track last modified
            existingOrder.Date = DateTime.Now;

            //Now I need to add the new order details and update the stock if need be
            foreach (var detail in dto.OrderDetails)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId);

                //Validate the product even exists
                if (product == null)
                {
                    throw new KeyNotFoundException($"Producto con ID {detail.ProductId} no fue encontrado :(");
                }

                //If now the client wants more products than ther ARE, then send out an error
                if (product.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto que desea comprar! {product.Name}");
                }

                //Now if the stock is suficient, then lower all stock
                product.Stock -= (int)detail.Quantity;
                await _productRepository.UpdateAsync(product);

                //Now finally insert and update properly
                var orderDetail = new OrderDetailDA
                {
                    OrderId = existingOrder.OrderId,
                    ProductId = product.ProductId,
                    Quantity = detail.Quantity,
                    UnitPrice = product.Price,
                    Discount = detail.Discount
                };


                //Save in db
                await _orderDetailRepository.CreateAsync(orderDetail);

            }

            //Reload nv properties so it doesn't return null anymore
            var updatedClient = await _clientRepository.GetByIdAsync(dto.ClientId);
            if (updatedClient == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {dto.ClientId} no fue encontrado.");
            }
            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(existingOrder.OrderId);

            //Out of foreach, we need to return the DTO
            return new OrdersDTO
            {
                OrderId = existingOrder.OrderId,
                Date = existingOrder.Date,
                Subtotal = subtotal,
                Tax = existingOrder.Tax,
                Total = existingOrder.Total,
                State =  existingOrder.State,
                Client =  new ClientDTO {
                    ClientId = updatedClient.ClientId, //Use the newly loaded client
                    FirstName = updatedClient.FirstName,
                    Cedula = updatedClient.Cedula,
                    LastName = updatedClient.LastName
                },
                User = new UsersDTO { UserId = existingOrder.UserId }
            };
        }
    }
}
