using LPRStoresAPI.Models;
using LPRStoresAPI.Repositories;
using System; // For ArgumentException
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _productRepository.GetAllAsync();
        public async Task<Product?> GetProductByIdAsync(int id) => await _productRepository.GetByIdAsync(id);

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (await _productRepository.ExistsAsync(p => p.Name == product.Name))
            {
                throw new ArgumentException("Product with this name already exists.");
            }
            await _productRepository.AddAsync(product); // Assumes repo calls SaveChangesAsync
            return product;
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.ProductId);
            if (existingProduct == null) return false;

            if (existingProduct.Name != product.Name && await _productRepository.ExistsAsync(p => p.Name == product.Name && p.ProductId != product.ProductId))
            {
                 throw new ArgumentException("Another product with this name already exists.");
            }
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.ReorderLevel = product.ReorderLevel;
            await _productRepository.UpdateAsync(existingProduct); // Changed to await UpdateAsync
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return false;
            await _productRepository.RemoveAsync(product); // Changed to await RemoveAsync
            return true;
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            // Uses the updated GetLowStockProductsAsync in IProductRepository
            return await _productRepository.GetLowStockProductsAsync();
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantityChange)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;
            if (product.StockQuantity + quantityChange < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");
            product.StockQuantity += quantityChange;
            await _productRepository.UpdateAsync(product); // Changed to await UpdateAsync
            return true;
        }
    }
}
