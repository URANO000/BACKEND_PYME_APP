using DataAccess.Models.DTOs.Helper;
using DataAccess.Models.DTOs.Product;


namespace BusinessLogic.Interfaces
{
    public interface IProductService
    {
        //SOLID -> Single Responsibility Principle 
        Task<PagedResult<ProductDTO>> GetAllProductsAsync(ProductFilterDTO dto);
        Task<IEnumerable<ProductDTO>> GetAllNonPaged();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO?> CreateProductAsync(CreateProductDTO dto);
        Task<ProductDTO?> UpdateProductAsync(int id, CreateProductDTO dto);
        Task DeleteProductAsync(int id);
    }
}
