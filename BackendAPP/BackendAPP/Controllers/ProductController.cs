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
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                _logger.LogInformation("All products hace been retrieved successfully");
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error, something went wrong :(");
                return StatusCode(500, new { message = "Error del servidor X_X" });
            }
        }

        [HttpGet("{id}")]
        public async  Task<ActionResult <ProductDTO>> GetProductById(int id)
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
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Internal error, couldn't get product by id = {id}");
                return StatusCode(500, new { message = "Error interno del servidor X_X"});

            }
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> CreateProduct([FromForm] CreateProductDTO productDTO)
        {
            try
            {
                var result = await _productService.CreateProductAsync(productDTO);
                return CreatedAtAction(nameof(GetProductById), new { id = result.ProductId }, result);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPut("{id}")] //Need the id to know where to update
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] CreateProductDTO updatedProduct)
        {
            //Using try and catch to deal with the argument exceptions, instead returning HTTP response
            try
            {
                var updated = await _productService.UpdateProductAsync(id, updatedProduct);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Delete method, my favorite... it's simple
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Producto eliminado!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}


/*Little notes for myself
 * Given a controller with [Route("api/[controller]")] and an action
 * [HttpGet("details")], the route will be api/Product/details
 * if the controller is named ProductController
*/
