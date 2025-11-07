using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Category;
using DataAccess.Models.DTOs.Helper;
using DataAccess.Models.DTOs.Product;
using DataAccess.Models.Entities;
using DataAccess.Repositories.Product;


namespace BusinessLogic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        //Constructor
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        //Here I implement the business logic methods by calling the repository methods
        public async Task<PagedResult<ProductDTO>> GetAllProductsAsync(ProductFilterDTO filters)
        {
            var pagedProducts = await _productRepository.GetFilteredAsync(
                filters.Search,
                filters.CategoryId,
                filters.MinPrice,
                filters.MaxPrice,
                filters.State,
                filters.PageNumber,
                filters.PageSize);

            return new PagedResult<ProductDTO>
            {
                Items = pagedProducts.Items.Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    TaxPercentage = p.TaxPercentage,
                    Stock = p.Stock,
                    Image = p.Image,
                    State = p.State.ToString(),
                    Category = new CategoryDTO
                    {
                        CategoryId = p.Category.CategoryId,
                        Name = p.Category.Name
                    }
                }),
                TotalCount = pagedProducts.TotalCount,
                PageNumber = pagedProducts.PageNumber,
                PageSize = pagedProducts.PageSize
            };
        }
        //GetById method logic 
        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            //I get the entity from the db using the repositoru
            var product = await _productRepository.GetByIdAsync(id);

            //Validation if product is null, not found
            if (product == null)
            {
                return null;
            }
            //If product is found, then map to dto and return
            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                TaxPercentage = product.TaxPercentage,
                Stock = product.Stock,
                Image = product.Image,
                State = product.State.ToString(),
                Category = new CategoryDTO {
                    CategoryId = product.Category.CategoryId,
                    Name = product.Category.Name
                }
            };
        }

        //Now with my create method
        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO productDTO)
        {
            if (productDTO == null)
                throw new ArgumentNullException(nameof(productDTO), "Datos del producto no pueden ser nulos");

            var category = await _productRepository.GetCategoryByIdAsync(productDTO.CategoryId);
            if (category == null)
                throw new KeyNotFoundException("Categoría no encontrada!");

            string imagePath = null;

            if (productDTO.ImageFile != null && productDTO.ImageFile.Length > 0)
            {
                // Ensure uploads folder exists
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Generate unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDTO.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await productDTO.ImageFile.CopyToAsync(stream);
                }

                // Save the relative path (to be served statically)
                imagePath = $"/uploads/products/{fileName}";
            }

            var product = new ProductDA
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                TaxPercentage = productDTO.TaxPercentage,
                Stock = productDTO.Stock,
                Image = imagePath, // save only the URL/path
                CategoryId = productDTO.CategoryId,
                State = true // Default state to Active
            };

            await _productRepository.CreateAsync(product);

            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                TaxPercentage = product.TaxPercentage,
                Stock = product.Stock,
                Image = product.Image, // the stored URL
                State = product.State.ToString(),
                Category = new CategoryDTO
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                }
            };
        }


        //Next we have the update method
        public async Task<ProductDTO> UpdateProductAsync(int id, CreateProductDTO updatedProduct)
        {
            if (updatedProduct == null)
                throw new ArgumentNullException(nameof(updatedProduct), "Datos del producto no pueden ser nulos!");

            //Validate if producto exists
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Producto no encontrado!");

            //Validate if category exists
            var category = await _productRepository.GetCategoryByIdAsync(updatedProduct.CategoryId);
            if (category == null)
                throw new KeyNotFoundException("Categoría no encontrada!");

            //Update every field
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.TaxPercentage = updatedProduct.TaxPercentage;
            product.Stock = updatedProduct.Stock;


            if (updatedProduct.ImageFile != null && updatedProduct.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updatedProduct.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await updatedProduct.ImageFile.CopyToAsync(stream);
                }

                product.Image = $"/uploads/products/{fileName}";
            }

            if (bool.TryParse(updatedProduct.State, out bool parsedState))
            {
                product.State = parsedState;
            }
            else
            {
                // Optionally, handle invalid state string (e.g., default to true or false, or throw an exception)
                throw new ArgumentException("Estado del producto inválido. Debe ser 'true' o 'false'.", nameof(updatedProduct.State));
            }

            product.Category = category;
            product.CategoryId = updatedProduct.CategoryId;

            //Save changes to db
            await _productRepository.UpdateAsync(product);

            //Mapping back to DTO to return this
            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                TaxPercentage = product.TaxPercentage,
                Stock = product.Stock,
                Image = product.Image,
                State = product.State.ToString(),
                Category = new CategoryDTO
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name
                }
            };
        }


        //Finally delete method
        public async Task DeleteProductAsync(int id)
        {
            //Validate if product exists
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Producto no encontrado!");
            //If found, delete
            await _productRepository.DeleteAsync(product);
        }


    }
}
