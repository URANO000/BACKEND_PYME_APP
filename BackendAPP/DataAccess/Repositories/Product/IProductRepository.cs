using DataAccess.Models.Entities;


namespace DataAccess.Repositories.Product
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDA>> GetAllAsync();
        Task<ProductDA?> GetByIdAsync(int id);
        Task CreateAsync(ProductDA product);
        Task UpdateAsync(ProductDA product);
        Task DeleteAsync(ProductDA product);
        Task<CategoryDA?> GetCategoryByIdAsync(int categoryId);
    }
}
