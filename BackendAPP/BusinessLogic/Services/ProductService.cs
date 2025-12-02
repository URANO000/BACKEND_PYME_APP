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

        //This is just get all but not paged
        public async Task<IEnumerable<ProductDTO>> GetAllNonPaged()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDTO
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

            });

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

            //After this, I will print the img URL
            Console.WriteLine("img value: ", productDTO.ImageFile);

            string imagePath = null;

            if (productDTO.ImageFile != null && productDTO.ImageFile.Length > 0)
            {
                try
                {
                    //Ensure uploads folder exists
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    //Generate unique file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(productDTO.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    //Save the image with proper file handling
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await productDTO.ImageFile.CopyToAsync(stream);
                        await stream.FlushAsync(); // Ensure data is written to disk
                    }

                    //After conversion, let's see the value of our image
                    Console.WriteLine("Converted image: ", imagePath);

                    // Save the relative path (to be served statically)
                    imagePath = $"/uploads/products/{fileName}";
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error saving image: {ex.Message}");
                    throw new Exception("Error al guardar la imagen del producto", ex);
                }
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

            // Validate if producto exists
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException("Producto no encontrado!");

            // Validate if category exists
            var category = await _productRepository.GetCategoryByIdAsync(updatedProduct.CategoryId);
            if (category == null)
                throw new KeyNotFoundException("Categoría no encontrada!");

            // Store old image path for cleanup
            string oldImagePath = product.Image;

            // Update fields
            product.Name = updatedProduct.Name;
            product.Price = updatedProduct.Price;
            product.TaxPercentage = updatedProduct.TaxPercentage;
            product.Stock = updatedProduct.Stock;

            // Handle image upload
            if (updatedProduct.ImageFile != null && updatedProduct.ImageFile.Length > 0)
            {
                try
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(updatedProduct.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Use async file operations and ensure proper disposal
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await updatedProduct.ImageFile.CopyToAsync(stream);
                        await stream.FlushAsync(); // Ensure data is written
                    }

                    // Only update the image path after successful save
                    product.Image = $"/uploads/products/{fileName}";

                    // Optional: Delete old image after new one is saved
                    if (!string.IsNullOrEmpty(oldImagePath) && oldImagePath != product.Image)
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", oldImagePath.TrimStart('/'));
                        if (File.Exists(oldFilePath))
                        {
                            try
                            {
                                File.Delete(oldFilePath);
                            }
                            catch (Exception ex)
                            {
                                // Log but don't fail the operation
                                Console.WriteLine($"Failed to delete old image: {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error saving image: {ex.Message}");
                    throw new Exception("Error al guardar la imagen del producto", ex);
                }
            }

            // Parse state
            if (bool.TryParse(updatedProduct.State, out bool parsedState))
            {
                product.State = parsedState;
            }
            else
            {
                throw new ArgumentException("Estado del producto inválido. Debe ser 'true' o 'false'.", nameof(updatedProduct.State));
            }

            product.CategoryId = updatedProduct.CategoryId;
            // Don't set product.Category directly - let EF handle the relationship

            // Save changes to db
            await _productRepository.UpdateAsync(product);

            // Mapping back to DTO
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
