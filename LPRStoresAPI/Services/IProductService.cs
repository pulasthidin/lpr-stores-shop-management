using LPRStoresAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LPRStoresAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<bool> UpdateStockAsync(int productId, int quantityChange);
    }
}
