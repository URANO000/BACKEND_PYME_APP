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

        //Filtering methods
        Task<ProductDA?> GetByNameAsync(string name);

        //Filter by price range (minPrice to maxPrice)
        Task<IEnumerable<ProductDA>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        //Order by price (ascending or descending)
        Task<IEnumerable<ProductDA>> GetAllOrderedByPriceAsync(bool ascending = true);

    }
}
