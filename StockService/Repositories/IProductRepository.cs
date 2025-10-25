using StockService.Models;

namespace StockService.Repositories
{
    public interface IProductRepository
    {
        Task<Product> AddAsync(Product p);
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetBySellerIdAsync(int sellerId); 
        Task<bool> UpdateAsync(Product p, int sellerId);
        Task<bool> DeleteAsync(int id, int sellerId);
        Task<bool> ReduceAsync(int productId, int quantity);
    }
}