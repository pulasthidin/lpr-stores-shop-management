using LPRStoresAPI.Models;

namespace LPRStoresAPI.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        // Add other product-specific methods if needed
    }
}
