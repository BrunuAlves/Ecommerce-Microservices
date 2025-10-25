using SalesService.Models;

namespace SalesService.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order);
        Task<List<Order>> GetByUserIdAsync(int userId);
        Task<Order> GetByIdAsync(int id, int userId);
        Task<bool> UpdateStatusAsync(int orderId, string status);
    }
}