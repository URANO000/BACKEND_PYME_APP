using DataAccess.Models.DTOs.Helper;
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

        //This is for filtering
        Task<PagedResult<ProductDA>> GetFilteredAsync
        (
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            bool? state,
            int pageNumber =1,
            int pageSize = 10
        );

    }
}
