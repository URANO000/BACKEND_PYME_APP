using BusinessLogic.Interfaces;
using DataAccess.Models.DTOs.Category;
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
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                TaxPercentage = p.TaxPercentage,
                Image = p.Image,
                State = p.State.ToString(),
                Category = new CategoryDTO { CategoryId = p.Category.CategoryId, Name = p.Category.Name }
            });
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
          //I had all my logic implemented in my controller before but now I'm putting it here
          if(productDTO == null)
            {
                //I will make sure the HTML fields are required anyways
                throw new ArgumentNullException(nameof(productDTO), "Datos del producto no pueden ser nulos");
            }

          //Validate that the category exists
            var category = await _productRepository.GetCategoryByIdAsync(productDTO.CategoryId);
            if (category == null)
                {
                   throw new KeyNotFoundException("Categoría no encontrada!");
                }

            // Convert DTO to Entity
            var product = new ProductDA
            {
                Name = productDTO.Name,
                Price = productDTO.Price,
                TaxPercentage = productDTO.TaxPercentage,
                Image = productDTO.Image,
                CategoryId = productDTO.CategoryId,
                State = Enum.TryParse<ProductState>(productDTO.State, true, out var parsedState)
                    ? parsedState
                    : ProductState.ACTIVE
            };

            //Save to db
            await _productRepository.CreateAsync(product);

            // Map Entity to DTO for response
            return new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                TaxPercentage = product.TaxPercentage,
                Image = product.Image,
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
            product.Image = updatedProduct.Image;
            product.State = Enum.Parse<ProductState>(updatedProduct.State, true);
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
