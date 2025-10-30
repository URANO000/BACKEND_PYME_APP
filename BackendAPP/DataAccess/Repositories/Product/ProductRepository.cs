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

    }
}
