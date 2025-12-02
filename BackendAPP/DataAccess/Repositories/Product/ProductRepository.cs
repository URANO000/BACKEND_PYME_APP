using DataAccess.Models.DTOs.Helper;
using DataAccess.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Product
{
    public class ProductRepository :IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        //Method implementation will go here
        public async Task<IEnumerable<ProductDA>> GetAllAsync()
            => await _context.Products.Include(p => p.Category).ToListAsync();

        public async Task<ProductDA?> GetByIdAsync(int id)
            => await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

        public async Task CreateAsync(ProductDA product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductDA product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductDA product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<CategoryDA?> GetCategoryByIdAsync(int categoryId)
            => await _context.Categories.FindAsync(categoryId);

        public async Task<PagedResult<ProductDA>> GetFilteredAsync(
            string? search,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            bool? state,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();  

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Name.Contains(search));  //Searc

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (state.HasValue)
                query = query.Where(p => p.State == state);

            //We'll count how many items match the filters
            var totalCount = await query.CountAsync();

            //Then, we use this to apply pagination properly
            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDA>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
