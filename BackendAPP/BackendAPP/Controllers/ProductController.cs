using BusinessLogic.Interfaces;
using DataAccess;
using DataAccess.Models.DTOs.Category;
using DataAccess.Models.DTOs.Product;
using DataAccess.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BackendAPP.Controllers
{
    //To fix ambiguity issue, added [action] to route
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        //  First thing to do is to call my service
        private readonly IProductService _productService;
        //Call logger for basic logging!
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }


        //Now I can create my API methods
        /* Note: Using ASYNC because database operations can be time-consuming. A task is a type of promise 
         * Note 2: I will be using Dtos as not to return nested object that can create circular references
         */

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? state,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filters = new ProductFilterDTO
                {
                    Search = search,
                    CategoryId = categoryId,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice,
                    State = state,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var products = await _productService.GetAllProductsAsync(filters);
                _logger.LogInformation("Products retrieved succesfully :)");
                return Ok(products);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error X_X");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            try
            {
                //Call the service method to get the product by id
                var product = await _productService.GetProductByIdAsync(id);

                //Validation if product is null
                if (product == null)
                {
                    _logger.LogWarning("Product not found...");
                    return NotFound(new { message = "Producto no encontrado..." });
                }
                return Ok(product);  //Returns my DTO 

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal error, couldn't get product by id = {id}");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });

            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] CreateProductDTO productDTO)
        {
            try
            {
                var result = await _productService.CreateProductAsync(productDTO);
                _logger.LogInformation("Product created successfully!!");
                return CreatedAtAction(nameof(GetProductById), new { id = result.ProductId }, result);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("Bad request while creating product: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Related entity not found while creating product: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while creating product");
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }
        [HttpPut("{id}")] //Need the id to know where to update
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateProductDTO updatedProduct)
        {
            //Using try and catch to deal with the argument exceptions, instead returning HTTP response
            try
            {
                var updated = await _productService.UpdateProductAsync(id, updatedProduct);
                _logger.LogInformation("Product with id {Id} updated successfully!!!", id);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Product not found while updating: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("Bad request while updating product: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while updating product with id {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }

        //Delete method, my favorite... it's simple
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                _logger.LogInformation("Product with id {Id} deleted successfully!!!", id);
                return Ok(new { message = "Producto eliminado!" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Product not found while deleting: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error while deleting product with id {Id}", id);
                return StatusCode(500, new { message = "Error interno del servidor X_X" });
            }
        }
    }
}


/*Little notes for myself
 * Given a controller with [Route("api/[controller]")] and an action
 * [HttpGet("details")], the route will be api/Product/details
 * if the controller is named ProductController
*/
