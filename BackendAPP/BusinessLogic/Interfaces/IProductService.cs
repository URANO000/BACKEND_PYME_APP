using DataAccess.Models.DTOs.Product;


namespace BusinessLogic.Interfaces
{
    public interface IProductService
    {
        //SOLID -> Single Responsibility Principle 
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO?> CreateProductAsync(CreateProductDTO dto);
        Task<ProductDTO?> UpdateProductAsync(int id, CreateProductDTO dto);
        Task DeleteProductAsync(int id);
    }
}
