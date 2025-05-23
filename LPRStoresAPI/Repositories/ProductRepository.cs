using LPRStoresAPI.Data;
using LPRStoresAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LPRStoresAPI.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _dbSet.Where(p => p.StockQuantity < p.ReorderLevel).ToListAsync();
        }
    }
}
